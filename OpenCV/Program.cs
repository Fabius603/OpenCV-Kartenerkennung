using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.XFeatures2D;
using System.Diagnostics;
using System.Drawing.Printing;

namespace OpenCV
{
    internal class Program
    {

        const double REAL_DISTANCE_MM = 10.0; // Real distance between Chessboard corners

        const int MIN_MATCH_COUNT = 10; // Minimal count of matching Keypoints
        const double GENAUIGKEIT = 0.7;
        const double BILDGRÖßE = 0.7; // Up-/Downscale the Image
        static string DIRECTORY = "C:\\Users\\schlieper\\source\\repos\\OpenCV Kartenerkennung - Chessboard\\OpenCV\\images\\";

        struct TrainerValues
        {
            public KeyPoint[] KP_TRAIN;
            public Mat DES_TRAIN;
            public Mat IMG;
        }

        struct QueryValues
        {
            public Rect ROI;
        }


        static void Main(string[] args)
        {

            // Method selection: true — Chessboard, false — 4 points (SIFT)
            bool useChessboard = true; // МChange to false to use SIFT

            if (useChessboard)
            {
                DIRECTORY += "Chessboard\\";
                ProcessChessboard(); // Method for Chessboard
            }
            else
            {
                DIRECTORY += "Chessboard\\";
                bool repeat = true;
                TrainerValues TV = TrainerLaden();
                QueryValues QV = QueryLaden();
                while (repeat == true)
                {
                    repeat = Vergleichen(TV.KP_TRAIN, TV.DES_TRAIN, TV.IMG, QV.ROI);
                }
            }
        }

        static bool Vergleichen(KeyPoint[] kp_train, Mat des_train, Mat trainImage, Rect ROI)
        {
            Timer.Start(out Stopwatch stopwatch);

            string queryImagePath = $"{DIRECTORY}queryImage/{new System.IO.DirectoryInfo($"{DIRECTORY}queryImage").GetFiles().First().Name}";


            // Images get Loaded in
            using (Mat queryImage = new Mat(queryImagePath, ImreadModes.Grayscale).SubMat(ROI))

            // calculation keypoints with the sift function
            using (SIFT sift = SIFT.Create())
            {
                KeyPoint[] kp_query;
                Mat des_query = new Mat();
                sift.DetectAndCompute(queryImage, null, out kp_query, des_query);

                // comparing the kepoints of both images and save them in matches
                using (FlannBasedMatcher matcher = new FlannBasedMatcher())
                {

                    if (des_train.Rows == 0 || des_query.Rows == 0)
                    {
                        Console.WriteLine("Keine Matches gefunden");
                        return false;
                    }
                    DMatch[][] matches = matcher.KnnMatch(des_train, des_query, 2);

                    //var goodMatches = matches.Where(m => m[0].Distance < GENAUIGKEIT * m[1].Distance).Select(m => m[0]).ToArray(); ---> wirkt negativ auf Ergebnis ?!?
                    var goodMatches = matches.Select(m => m[0]).ToArray();

                    if (goodMatches.Length >= MIN_MATCH_COUNT)
                    {
                        // Points of the matches get calculated
                        Point2f[] queryPoints = goodMatches.Select(match => kp_query[match.TrainIdx].Pt).ToArray();
                        Point2f[] trainPoints = goodMatches.Select(match => kp_train[match.QueryIdx].Pt).ToArray();

                        // Homography gets created -> its a matrix with conversion factors
                        Mat homography = Cv2.FindHomography(InputArray.Create(trainPoints), InputArray.Create(queryPoints), HomographyMethods.Ransac, 5.0);

                        // corners get calculated
                        Point2f[] points = { new Point2f(0, 0), new Point2f(0, trainImage.Rows - 1), new Point2f(trainImage.Cols - 1, trainImage.Rows - 1), new Point2f(trainImage.Cols - 1, 0) };
                        Point2f[] destination = Cv2.PerspectiveTransform(points, homography);

                        // Bereich des trainImage in das queryImage einzeichnen
                        // ---> nicht notwendig
                        IEnumerable<OpenCvSharp.KeyPoint> keyPointEnumerable = ConvertToEnumerable(destination);
                        Cv2.DrawKeypoints(queryImage, keyPointEnumerable, queryImage, Scalar.Lime, DrawMatchesFlags.Default);
                        DrawRectangle(destination, queryImage);

                        // rotation matrix is read out of the homography(matrix)
                        double rotationAngleDegrees = CalcRotation(homography);

                        double[,] realPosition = RealPosition(ROI, destination);
                        Point centerPoint = GetCenterPoint(realPosition);
                        Cv2.DrawMarker(queryImage, centerPoint, Scalar.Red, markerSize: 5);
                        double accordance = GetAccordance(goodMatches, kp_train);
                        Console.WriteLine(accordance);
                        // Timer
                        string time = Timer.Stop(stopwatch);

                        // Konsole gibt werte aus
                        // ---> nicht notwendig
                        ConsoleOutput(time, goodMatches, rotationAngleDegrees, destination, realPosition);

                        // Größe des Bildes wird angepasst
                        // ---> nicht notwendig
                        Mat newImage = ResizeImage(queryImage);

                        // ---> nicht notwendig
                        Cv2.ImShow("Matches", newImage);
                        Cv2.WaitKey(2);

                        string eingabe = Console.ReadLine();
                        Console.Clear();
                        if (eingabe == "q")
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Nicht genügend Matches - {goodMatches.Length}/{MIN_MATCH_COUNT}");
                        return false;
                    }
                }
            }
        }

        static QueryValues QueryLaden()
        {
            QueryValues QV = new QueryValues();
            // region of interest
            QV.ROI = new Rect(1360, 100, 240, 660);
            return QV;
        }

        static TrainerValues TrainerLaden()
        {
            string trainImagePath = $"{DIRECTORY}trainImage/{new System.IO.DirectoryInfo($"{DIRECTORY}trainImage").GetFiles().First().Name}";
            Mat trainImage = new Mat(trainImagePath, ImreadModes.Grayscale);
            Cv2.ImShow("test", trainImage);
            using (SIFT sift = SIFT.Create())
            {
                TrainerValues TV = new TrainerValues();
                TV.IMG = trainImage;
                TV.DES_TRAIN = new Mat();
                sift.DetectAndCompute(trainImage, null, out TV.KP_TRAIN, TV.DES_TRAIN);
                return TV;
            }
        }

        static IEnumerable<KeyPoint> ConvertToEnumerable(Point2f[] array)
        {
            foreach (var point2f in array)
            {
                yield return new KeyPoint(point2f, 0);
            }
        }

        static Point[] ConvertToPointArray(Point2f[] array)
        {
            Point[] resultArray = new Point[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                resultArray[i] = new Point((int)array[i].X, (int)array[i].Y);
            }

            return resultArray;
        }

        static double CalcRotation(Mat homography)
        {
            // rotations Matrix wird aus homography entnommen
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

        static void DrawRectangle(Point2f[] eckpunkte, Mat img)
        {
            Point[] pointArray = ConvertToPointArray(eckpunkte);
            Cv2.Line(img, pointArray[0], pointArray[1], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[1], pointArray[2], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[2], pointArray[3], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[3], pointArray[0], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
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

        static double GetAccordance(DMatch[] goodMatches, KeyPoint[] trainPoints)
        {
            return Math.Round((double)(goodMatches.Length / trainPoints.Length), 0) * 100;
        }

        static Point GetCenterPoint(double[,] realPosition)
        {
            int midX = (int)(realPosition[0, 0] + realPosition[1, 0] + realPosition[2, 0] + realPosition[3, 0]) / 4;
            int midY = (int)(realPosition[0, 1] + realPosition[1, 1] + realPosition[2, 1] + realPosition[3, 1]) / 4;
            Point centerPoint = new Point(midX, midY);
            return centerPoint;
        }

        static void ConsoleOutput(string time, DMatch[] goodMatches, double rotationAngleDegrees, Point2f[] destination, double[,] realPosition)
        {
            Console.WriteLine($"Zeit: {time} Sekunden");
            Console.WriteLine($"Matches: {goodMatches.Length}");
            Console.WriteLine($"Drehung: {rotationAngleDegrees:F4} Grad");
            Console.WriteLine($"Position im Ausschnitt:\n A:{destination[0]}\n B:{destination[1]}\n C:{destination[2]}\n D:{destination[3]}");
            Console.WriteLine($"Position im Bild:\n A:(x:{realPosition[0, 0]:F5}, y:{realPosition[0, 1]:F5})\n B:(x:{realPosition[1, 0]:F5}, y:{realPosition[1, 1]:F5})\n C:(x:{realPosition[2, 0]:F5}, y:{realPosition[2, 1]:F5})\n D:(x:{realPosition[3, 0]:F5}, y:{realPosition[3, 1]:F5})");
        }

        static Mat ResizeImage(Mat image)
        {
            Size newSize = new Size(image.Cols * BILDGRÖßE, image.Rows * BILDGRÖßE);
            Mat newImage = new Mat();
            Cv2.Resize(image, newImage, newSize, 0, 0, InterpolationFlags.Linear);
            return newImage;
        }



        // Method for Chessboard
        static void ProcessChessboard()
        {
            // Load all images in the queryImage directory
            string[] queryImagePaths = System.IO.Directory.GetFiles($"{DIRECTORY}queryImage", "*.png");

            foreach (string queryImagePath in queryImagePaths)
            {
                Mat queryImage = new Mat(queryImagePath, ImreadModes.Grayscale);

                // Apply Blur filter for better detection
                Cv2.GaussianBlur(queryImage, queryImage, new Size(5, 5), 0);

                // Chessboard size (for example, 9x6 corners): Size patternSize = new Size(9, 6);
                // Test different pattern sizes from 3x3 to 7x7
                bool found = false;
                Size patternSize = new Size(3, 3);
                Point2f[] corners = [];
                for (int width = 3; width <= 7; width++)
                {
                    for (int height = 3; height <= 7; height++)
                    {
                        patternSize = new Size(width, height);

                        found = Cv2.FindChessboardCorners(queryImage, patternSize, out corners);
                        if (found)
                        {
                            Console.WriteLine($"Chessboard found in image: {queryImagePath} with pattern size: {width}x{height}");
                            // Draw corners for visualization


                            // Center calculation (X, Y)
                            double centerX = 0, centerY = 0;
                            foreach (var corner in corners)
                            {
                                centerX += corner.X;
                                centerY += corner.Y;
                            }
                            centerX /= corners.Length;
                            centerY /= corners.Length;

                            // Calculation of coefficient (mm/pixel) for X and Y
                            double pixelDistanceX = corners[1].X - corners[0].X; // distance in pixels on X
                            double pixelDistanceY = corners[patternSize.Width].Y - corners[0].Y; // distance in pixels on Y

                            double mmPerPixelX = REAL_DISTANCE_MM / pixelDistanceX; // Coefficient for X
                            double mmPerPixelY = REAL_DISTANCE_MM / pixelDistanceY; // Coefficient for Y

                            // Print results
                            Console.WriteLine($"Image: {queryImagePath}");
                            Console.WriteLine($"Center: X={centerX:F2} pixels, Y={centerY:F2} pixels");
                            Console.WriteLine($"Conversion factors: X={mmPerPixelX:F4} mm/pixel, Y={mmPerPixelY:F4} mm/pixel");


                            Cv2.DrawChessboardCorners(queryImage, patternSize, corners, found);
                            Cv2.ImShow("Chessboard Corners", queryImage);
                            Cv2.WaitKey(0);
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                if (!found)
                {
                    Console.WriteLine($"Chessboard not found in image: {queryImagePath}");
                }

            
                Cv2.DestroyAllWindows();
            }
        }


    }

    class Timer()
    {
        public static void Start(out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static string Stop(Stopwatch stopwatch)
        {
            stopwatch.Stop();
            string time = stopwatch.Elapsed.ToString().Substring(7, 6);
            return time;
        }






    }
}