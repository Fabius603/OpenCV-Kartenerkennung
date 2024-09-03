using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Features2D;
using System.Diagnostics;
using System.Drawing;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Runtime.InteropServices;
using uEye;

namespace VisionMultiArea
{
    public class ImageCalc
    {
        public static ResultValues CalculateImage(Mat template, Mat fullImage, Rect[] rects)
        {
            Timer.Start(out Stopwatch stopwatch);

            TemplateValues[] TValues = GetTemplateValues(template, rects, fullImage);
            QueryValues QValues = GetQueryValues(fullImage, TValues);

            ResultValues RValues = CalculateRotation(TValues, QValues);

            RValues.Time = Timer.Stop(stopwatch);

            Cv2.Rectangle(fullImage, QValues.ROI, Scalar.Red, 2);
            RValues.ResultImage = fullImage;

            //Mat allTemplates = TemplatesZusammenfassen(TValues);
            //Cv2.ImShow("AllTemplates", allTemplates);
            Cv2.WaitKey();
            return RValues;
        }

        private static QueryValues GetQueryValues(Mat fullImage, TemplateValues[] TValues)
        {
            Rect roi = GetROI(TValues);
            roi = AdjustRoiToFit(fullImage, roi);

            QueryValues QValues = new QueryValues(fullImage, roi, TValues);
            return QValues;
        }

        public static Rect GetROI(TemplateValues[] TValues)
        {
            int minX = TValues[0].TemplateArea.X;
            int minY = TValues[0].TemplateArea.Y;
            int maxX = TValues[0].TemplateArea.X + TValues[0].TemplateArea.Width;
            int maxY = TValues[0].TemplateArea.Y + TValues[0].TemplateArea.Height;

            foreach (var template in TValues)
            {
                if (template.TemplateArea.X < minX)
                    minX = template.TemplateArea.X;

                if (template.TemplateArea.Y < minY)
                    minY = template.TemplateArea.Y;

                if (template.TemplateArea.X + template.TemplateArea.Width > maxX)
                    maxX = template.TemplateArea.X + template.TemplateArea.Width;

                if (template.TemplateArea.Y + template.TemplateArea.Height > maxY)
                    maxY = template.TemplateArea.Y + template.TemplateArea.Height;
            }

            int width = maxX - minX;
            int height = maxY - minY;

            return new Rect(minX - 70, minY - 70, width + 140, height + 140);
        }

        private static TemplateValues[] GetTemplateValues(Mat template, Rect[] Rects, Mat fullImage)
        {
            TemplateValues[] templateValues = new TemplateValues[Rects.Length];
            for(int i = 0; i < Rects.Length; i++)
            {
                templateValues[i] = new TemplateValues(template, AdjustRoiToFit(fullImage, Rects[i]));
            }
            return templateValues;
        }

        private static ResultValues CalculateRotation(TemplateValues[] TValues, QueryValues QValues)
        {
            double ACCURACY = 0.7;
            SIFT sift = SIFT.Create();
            FlannBasedMatcher matcher = new FlannBasedMatcher();

            List<List<DMatch>> allMatches = new List<List<DMatch>>();
            List<List<KeyPoint>> allKeyPoints = new List<List<KeyPoint>>();
            List<Mat> allDescriptors = new List<Mat>();

            Point2f pointOfTemplateWithMostKeyPoints = new Point2f(0, 0);
            int mostKeyPoints = 0;

            foreach (TemplateValues TValue in TValues)
            {
                Mat templateDescriptors = new Mat();
                sift.DetectAndCompute(TValue.TemplateImage, null, out KeyPoint[] templateKeypoints, templateDescriptors);
                allKeyPoints.Add(templateKeypoints.ToList());
                allDescriptors.Add(templateDescriptors);

                if(templateKeypoints.Length > mostKeyPoints)
                {
                    mostKeyPoints = templateKeypoints.Length;
                    pointOfTemplateWithMostKeyPoints = new Point2f(TValue.TemplateArea.X, TValue.TemplateArea.Y);
                }
            }

            Mat queryDescriptors = new Mat();
            sift.DetectAndCompute(QValues.QueryImage, QValues.Mask, out KeyPoint[] queryKeypoints, queryDescriptors);

            foreach (Mat templateDescriptors in allDescriptors)
            {
                DMatch[][] matches = matcher.KnnMatch(queryDescriptors, templateDescriptors, 2);

                var goodMatches = matches.Where(m => m[0].Distance < ACCURACY * m[1].Distance).Select(m => m[0]).ToArray();
                allMatches.Add(goodMatches.ToList());
            }

            List<Point2f> srcPoints = new List<Point2f>();
            List<Point2f> dstPoints = new List<Point2f>();

            for (int i = 0; i < allMatches.Count; i++)
            {
                var goodMatches = allMatches[i];
                for (int j = 0; j < goodMatches.Count; j++)
                {
                    var match = goodMatches[j];

                    if (match.QueryIdx < queryKeypoints.Length && match.TrainIdx < allKeyPoints[i].Count)
                    {
                        srcPoints.Add(queryKeypoints[match.QueryIdx].Pt);
                        dstPoints.Add(allKeyPoints[i][match.TrainIdx].Pt);
                    }
                }
            }

            Mat homography = Cv2.FindHomography(InputArray.Create(srcPoints), InputArray.Create(dstPoints), HomographyMethods.Ransac, 5.0);
            Mat invertetHomography = InvertHomography(homography);

            Point2f[] allCenterpoints = new Point2f[TValues.Length];
            double[] allOffsetX = new double[TValues.Length];
            double[] allOffsetY = new double[TValues.Length];

            int index = 0;

            foreach (var template in TValues)
            {

                float xVerschiebung = template.TemplateArea.X - pointOfTemplateWithMostKeyPoints.X;
                float yVerschiebung = template.TemplateArea.Y - pointOfTemplateWithMostKeyPoints.Y;

                Point2f[] points = 
                { 
                    new Point2f(xVerschiebung, yVerschiebung), 
                    new Point2f(xVerschiebung, yVerschiebung + template.TemplateImage.Rows - 1), 
                    new Point2f(xVerschiebung + template.TemplateImage.Cols - 1, yVerschiebung + template.TemplateImage.Rows - 1), 
                    new Point2f(xVerschiebung + template.TemplateImage.Cols - 1, yVerschiebung) 
                };

                Point2f[] destination = Cv2.PerspectiveTransform(points, invertetHomography);
               
                DrawRectangle(destination, QValues.QueryImage);

                double[,] realPosition = RealPosition(QValues.ROI, destination);


                Point2f centerPoint = GetCenterPoint(realPosition);

                Cv2.Circle(QValues.QueryImage, new OpenCvSharp.Point((int)centerPoint.X - QValues.ROI.X, (int)centerPoint.Y - QValues.ROI.Y), 2, Scalar.Red, 2);

                double offsetX = template.ExpectedCenter.X - centerPoint.X;
                double offsetY = template.ExpectedCenter.Y - centerPoint.Y;
                allCenterpoints[index] = centerPoint;
                allOffsetX[index] = offsetX;
                allOffsetY[index] = offsetY;

                Point2f averageOffset = new Point2f(0, 0);
                for(var i = 0; i < allOffsetX.Length; i++)
                {
                    averageOffset.X += (float)allOffsetX[i];
                    averageOffset.Y += (float)allOffsetY[i];
                }
                index++;
            }

            double rotationAnglesDegrees = CalcRotation(invertetHomography);



            return new ResultValues(rotationAnglesDegrees, allCenterpoints, allOffsetX, allOffsetY);
        }

        private static void DrawPointsOnImage(double[,] points, Mat image)
        {
            if (points.GetLength(1) != 2)
            {
                throw new ArgumentException("Die Punkte müssen als Paare von X- und Y-Koordinaten vorliegen.");
            }

            for (int i = 0; i < points.GetLength(0); i++)
            {
                Point2f point = new Point2f((float)points[i, 0], (float)points[i, 1]);
                Cv2.Circle(image, new OpenCvSharp.Point((int)point.X, (int)point.Y), 5, Scalar.Blue, -1);
            }
        }

        public static Mat InvertHomography(Mat homography)
        {
            if (homography.Empty() || homography.Rows != 3 || homography.Cols != 3)
            {
                throw new ArgumentException("Die Homographie-Matrix muss eine 3x3-Matrix sein.");
            }

            Mat invertedHomography = new Mat();
            Cv2.Invert(homography, invertedHomography);

            return invertedHomography;
        }

        private static double CalcRotation(Mat homography)
        {
            double[,] rotationMatrix = new double[2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    rotationMatrix[i, j] = homography.At<double>(i, j);
                }
            }
            double rotationAngle = Math.Atan2(rotationMatrix[1, 0], rotationMatrix[0, 0]);
            return rotationAngle * (180.0 / Math.PI);
        }

        public static double[,] RealPosition(Rect roi, Point2f[] destination)
        {
            double[] P1 = { (double)roi.X + (double)destination[0].X, (double)roi.Y + (double)destination[0].Y };
            double[] P2 = { (double)roi.X + (double)destination[1].X, (double)roi.Y + (double)destination[1].Y };
            double[] P3 = { (double)roi.X + (double)destination[2].X, (double)roi.Y + (double)destination[2].Y };
            double[] P4 = { (double)roi.X + (double)destination[3].X, (double)roi.Y + (double)destination[3].Y };
            double[,] realPosition = { { P1[0], P1[1] }, { P2[0], P2[1] }, { P3[0], P3[1] }, { P4[0], P4[1] } };
            return realPosition;
        }

        public static Point2f GetCenterPoint(double[,] points)
        {
            int numPoints = points.GetLength(0);
            if (numPoints < 1 || points.GetLength(1) != 2)
            {
                throw new ArgumentException("Es müssen mindestens ein Punkt mit jeweils zwei Koordinaten (X, Y) übergeben werden.");
            }

            double centerX = 0;
            double centerY = 0;

            for (int i = 0; i < numPoints; i++)
            {
                centerX += points[i, 0];
                centerY += points[i, 1];
            }

            centerX /= numPoints;
            centerY /= numPoints;

            return new Point2f((float)centerX, (float)centerY);
        }

        public static OpenCvSharp.Point[] ConvertToPointArray(Point2f[] array)
        {
            OpenCvSharp.Point[] resultArray = new OpenCvSharp.Point[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                resultArray[i] = new OpenCvSharp.Point((int)array[i].X, (int)array[i].Y);
            }

            return resultArray;
        }

        public static void DrawRectangle(Point2f[] eckpunkte, Mat img)
        {
            OpenCvSharp.Point[] pointArray = ConvertToPointArray(eckpunkte);
            Cv2.Line(img, pointArray[0], pointArray[1], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[1], pointArray[2], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[2], pointArray[3], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[3], pointArray[0], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
        }

        public static Mat TemplatesZusammenfassen(TemplateValues[] bilder)
        {
            int breite = 0;
            int hoeheMax = 0;

            foreach (var bild in bilder)
            {
                breite += bild.TemplateImage.Cols;
                hoeheMax = Math.Max(hoeheMax, bild.TemplateImage.Rows);
            }

            Mat zusammengefasstesBild = new Mat(hoeheMax, breite, MatType.CV_8UC3);

            int xOffset = 0;
            foreach (var bild in bilder)
            {
                Mat bildMat = bild.TemplateImage;

                Mat roi = zusammengefasstesBild[new Rect(xOffset, 0, bildMat.Cols, bildMat.Rows)];

                bildMat.CopyTo(roi);

                xOffset += bildMat.Cols;
            }

            return zusammengefasstesBild;
        }

        private static Rect AdjustRoiToFit(Mat image, Rect roi)
        {
            int imgWidth = image.Width;
            int imgHeight = image.Height;

            int x = Math.Max(0, roi.X);
            int y = Math.Max(0, roi.Y);
            int width = Math.Min(roi.Width, imgWidth - x);
            int height = Math.Min(roi.Height, imgHeight - y);

            return new Rect(x, y, width, height);
        }

        public static Mat ToMat(Bitmap src)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("Non-Windows OS are not supported");
            }

            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            int width = src.Width;
            int height = src.Height;
            int ch;
            switch (src.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    ch = 3;
                    break;
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppArgb:
                    ch = 4;
                    break;
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    ch = 1;
                    break;
                default:
                    throw new NotImplementedException();
            }

            Mat mat = new Mat(height, width, MatType.CV_8UC(ch));
            src.ToMat(mat);
            return mat;
        }
    }
}