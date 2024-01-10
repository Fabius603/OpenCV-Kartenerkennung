using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.XFeatures2D;
using System.Diagnostics;
using System.Drawing.Printing;

namespace OpenCV
{
    internal class Program
    {
        const int MIN_MATCH_COUNT = 10; // Mindestanzahl der übereinstimmenden KeyPoints
        const double GENAUIGKEIT = 0.7; // Wie genau soll die Übereinstimmung sein
        const double BILDGRÖßE = 0.7; // Faktor zur Vergrößerung/Verkleinerung des Bildes
        const string DIRECTORY = "C:/Users/schlieper/source/repos/OpenCV Kartenerkennung/OpenCV/";

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
            bool repeat = true;
            TrainerValues TV = TrainerLaden();
            QueryValues QV = QueryLaden();
            while(repeat == true)
            {
                repeat = Vergleichen(TV.KP_TRAIN, TV.DES_TRAIN, TV.IMG, QV.ROI);
            }
        }

        static bool Vergleichen(KeyPoint[] kp_train, Mat des_train, Mat trainImage, Rect ROI)
        {
            Timer.Start(out Stopwatch stopwatch);

            string queryImagePath = $"{DIRECTORY}queryImage/{new System.IO.DirectoryInfo($"{DIRECTORY}queryImage").GetFiles().First().Name}";


            // Bilder werden aus den beiden Ordnern geladen (die jeweils ersten Dateien im Ordner)
            using (Mat queryImage = new Mat(queryImagePath, ImreadModes.Grayscale).SubMat(ROI))

            // keyPoints und destination wird mit sift berechnet
            using (SIFT sift = SIFT.Create())
            {
                KeyPoint[] kp_query;
                Mat des_query = new Mat();
                sift.DetectAndCompute(queryImage, null, out kp_query, des_query);

                // KeyPoints der Bilder werden verglichen und alle mit einer Abweichung unter GENAUIGKEIT werden in goodMatches gespeichert
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
                        // Punkte der guten Matches werden berechnet
                        Point2f[] queryPoints = goodMatches.Select(match => kp_query[match.TrainIdx].Pt).ToArray();
                        Point2f[] trainPoints = goodMatches.Select(match => kp_train[match.QueryIdx].Pt).ToArray();

                        // Homography wird erstellt -> Matrix mit verschiedenen Umrechnungsfaktoren
                        Mat homography = Cv2.FindHomography(InputArray.Create(trainPoints), InputArray.Create(queryPoints), HomographyMethods.Ransac, 5.0);

                        // Eckpunkte des trainImage werden mit Umrechnungsfaktoren für queryImage berechnet
                        Point2f[] points = { new Point2f(0, 0), new Point2f(0, trainImage.Rows - 1), new Point2f(trainImage.Cols - 1, trainImage.Rows - 1), new Point2f(trainImage.Cols - 1, 0) };
                        Point2f[] destination = Cv2.PerspectiveTransform(points, homography);

                        // Bereich des trainImage in das queryImage einzeichnen
                        // ---> nicht notwendig
                        IEnumerable<OpenCvSharp.KeyPoint> keyPointEnumerable = ConvertToEnumerable(destination);
                        Cv2.DrawKeypoints(queryImage, keyPointEnumerable, queryImage, Scalar.Lime, DrawMatchesFlags.Default);
                        DrawRectangle(destination, queryImage);

                        // rotations Matrix wird aus homography entnommen
                        double rotationAngleDegrees = CalcRotation(homography);

                        double[,] realPosition = RealPosition(ROI, destination);

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
            // Position des Suchbereichs (x, y, width, height)
            QV.ROI = new Rect(1360, 100, 240, 660);
            return QV;
        }

        static TrainerValues TrainerLaden()
        {
            string trainImagePath = $"{DIRECTORY}trainImage/{new System.IO.DirectoryInfo($"{DIRECTORY}trainImage").GetFiles().First().Name}";
            Mat trainImage = new Mat(trainImagePath, ImreadModes.Grayscale);
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