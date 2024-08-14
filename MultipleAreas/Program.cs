using OpenCvSharp;
using OpenCvSharp.Features2D;
using System;
using System.Drawing;
using System.Drawing.Printing;

namespace MultipleAreas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Mat fullImage = new Mat("InitialImage.bmp");
            TemplateValues[] TValues = GetTemplateValues(fullImage);
            QueryValues QValues = GetQueryValues(fullImage);

            ResultValues RValues = CalculateRotation(TValues, QValues);

            Console.WriteLine("Rotation: " + RValues.Rotation);
            Console.WriteLine("");

            for (var i = 0; i < TValues.Length; i++)
            {
                Console.WriteLine("Centerpoint " + i + ": " + RValues.CenterPoints[i]);
                Console.WriteLine("OffsetX " + i + ": " + RValues.OffsetX[i]);
                Console.WriteLine("OffsetY " + i + ": " + RValues.OffsetY[i]);
                Console.WriteLine("");
            }


            Mat allTemplates = TemplatesZusammenfassen(TValues);
            Cv2.ImShow("AllTemplates", allTemplates);
            Cv2.ImShow("Result", fullImage);
            Cv2.WaitKey();
        }

        static ResultValues CalculateRotation(TemplateValues[] TValues, QueryValues QValues)
        {
            double ACCURACY = 0.7;
            SIFT sift = SIFT.Create();
            FlannBasedMatcher matcher = new FlannBasedMatcher();

            List<List<DMatch>> allMatches = new List<List<DMatch>>();
            List<List<KeyPoint>> allKeyPoints = new List<List<KeyPoint>>();
            List<Mat> allDescriptors = new List<Mat>();

            foreach (TemplateValues TValue in TValues)
            {
                Mat templateDescriptors = new Mat();
                sift.DetectAndCompute(TValue.TemplateImage, null, out KeyPoint[] templateKeypoints, templateDescriptors);
                allKeyPoints.Add(templateKeypoints.ToList());
                allDescriptors.Add(templateDescriptors);
            }

            Mat queryDescriptors = new Mat();
            sift.DetectAndCompute(QValues.QueryImage, null, out KeyPoint[] queryKeypoints, queryDescriptors);

            foreach(Mat templateDescriptors in allDescriptors)
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


            Point2f[] allCenterpoints = new Point2f[TValues.Length];
            double[] allOffsetX = new double[TValues.Length];
            double[] allOffsetY = new double[TValues.Length];

            int index = 0;
            foreach (var template in TValues)
            {
                Point2f[] points = { new Point2f(0, 0), new Point2f(0, template.TemplateImage.Rows - 1), new Point2f(template.TemplateImage.Cols - 1, template.TemplateImage.Rows - 1), new Point2f(template.TemplateImage.Cols - 1, 0) };

                Point2f centerPoint = GetCenterPoint(points);
                Cv2.Circle(template.TemplateImage, new OpenCvSharp.Point((int)centerPoint.X, (int)centerPoint.Y), 2, Scalar.Red, 2);

                DrawRectangle(points, template.TemplateImage);

                Point2f[] destination = Cv2.PerspectiveTransform(points, homography);
                double[,] realPosition = RealPosition(QValues.ROI, destination);

                double offsetX = centerPoint.X - template.ExpectedCenter.X + template.TemplateArea.X;
                double offsetY = centerPoint.Y - template.ExpectedCenter.Y + template.TemplateArea.Y;

                allCenterpoints[index] = centerPoint;
                allOffsetX[index] = offsetX;
                allOffsetY[index] = offsetY;
                index++;
            }

            double rotationAnglesDegrees = CalcRotation(homography);

            

            return new ResultValues(rotationAnglesDegrees, allCenterpoints, allOffsetX, allOffsetY);
        }

        static QueryValues GetQueryValues(Mat fullImage)
        {
            Rect roi = new Rect(130, 80, 1080, 180);
            QueryValues QValues = new QueryValues(fullImage, roi);
            return QValues;
        }

        static TemplateValues[] GetTemplateValues(Mat fullImage)
        {
            TemplateValues TV1 = new TemplateValues(fullImage, new Rect(1060, 100, 150, 160)); // Land
            TemplateValues TV2 = new TemplateValues(fullImage, new Rect(446, 120, 410, 37)); // Überschrift
            TemplateValues TV3 = new TemplateValues(fullImage, new Rect(168, 110, 160, 55)); // Passport
            return new TemplateValues[] { TV1, TV2, TV3 };
        }

        static double CalcRotation(Mat homography)
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

        static double[,] RealPosition(Rect roi, Point2f[] destination)
        {
            double[] P1 = { (double)roi.X + (double)destination[0].X, (double)roi.Y + (double)destination[0].Y };
            double[] P2 = { (double)roi.X + (double)destination[1].X, (double)roi.Y + (double)destination[1].Y };
            double[] P3 = { (double)roi.X + (double)destination[2].X, (double)roi.Y + (double)destination[2].Y };
            double[] P4 = { (double)roi.X + (double)destination[3].X, (double)roi.Y + (double)destination[3].Y };
            double[,] realPosition = { { P1[0], P1[1] }, { P2[0], P2[1] }, { P3[0], P3[1] }, { P4[0], P4[1] } }; ;
            return realPosition;
        }

        static Point2f GetCenterPoint(Point2f[] points)
        {
            if (points == null || points.Length == 0)
            {
                throw new ArgumentException("Array darf nicht null oder leer sein.");
            }

            float sumX = 0;
            float sumY = 0;
            int count = points.Length;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
            }

            float midX = sumX / count;
            float midY = sumY / count;

            return new Point2f(midX, midY);
        }

        static OpenCvSharp.Point[] ConvertToPointArray(Point2f[] array)
        {
            OpenCvSharp.Point[] resultArray = new OpenCvSharp.Point[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                resultArray[i] = new OpenCvSharp.Point((int)array[i].X, (int)array[i].Y);
            }

            return resultArray;
        }

        static void DrawRectangle(Point2f[] eckpunkte, Mat img)
        {
            OpenCvSharp.Point[] pointArray = ConvertToPointArray(eckpunkte);
            Cv2.Line(img, pointArray[0], pointArray[1], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[1], pointArray[2], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[2], pointArray[3], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[3], pointArray[0], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
        }

        static Mat TemplatesZusammenfassen(TemplateValues[] bilder)
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

    }
}