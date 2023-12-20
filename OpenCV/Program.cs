using OpenCvSharp;
using OpenCvSharp.Features2D;
using System.Diagnostics;

namespace OpenCV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Variablen Festlegung
            const int MIN_MATCH_COUNT = 10;
            const double GENAUIGKEIT = 0.7;
            const string DIRECTORY = "C:/Users/schlieper/source/repos/OpenCV Kartenerkennung/OpenCV/";

            // Bilder werden aus den beiden Ordnern geladen (die jeweils ersten Dateien im Ordner)
            string trainImagePath = $"{DIRECTORY}trainImage/{new System.IO.DirectoryInfo($"{DIRECTORY}trainImage").GetFiles().First().Name}";
            string queryImagePath = $"{DIRECTORY}queryImage/{new System.IO.DirectoryInfo($"{DIRECTORY}queryImage").GetFiles().First().Name}";

            using (Mat trainImage = new Mat(trainImagePath, ImreadModes.Grayscale),
                       queryImage = new Mat(queryImagePath, ImreadModes.Grayscale))

            // keyPoints und destination wird miut sift berechnet
            using (SIFT sift = SIFT.Create())
            {
                KeyPoint[] kp_train, kp_query;
                Mat des_train = new Mat(), des_query = new Mat();
                sift.DetectAndCompute(trainImage, null, out kp_train, des_train);
                sift.DetectAndCompute(queryImage, null, out kp_query, des_query);

                // KeyPoints der Bilder werden verglichen und alle mit einer Abweichung unter GENAUIGKEIT werden in goodMatches gespeichert
                using (FlannBasedMatcher matcher = new FlannBasedMatcher())
                {
                    DMatch[][] matches = matcher.KnnMatch(des_train, des_query, 2);

                    var goodMatches = matches.Where(m => m[0].Distance < GENAUIGKEIT * m[1].Distance).Select(m => m[0]).ToArray();

                    if (goodMatches.Length >= MIN_MATCH_COUNT ) 
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
                        Cv2.DrawKeypoints(queryImage, keyPointEnumerable, queryImage ,Scalar.Lime, DrawMatchesFlags.Default);
                        drawRectangle(destination, queryImage);

                        // rotations Matrix wird aus homography entnommen
                        double rotationAngleDegrees = CalcRotation(homography);
                        
                        // Timer
                        stopwatch.Stop();
                        string time = stopwatch.Elapsed.ToString().Substring(7, 6);

                        // Konsole gibt werte aus
                        // ---> nicht notwendig
                        ConsoleOutput(time, goodMatches, rotationAngleDegrees, destination);

                        // ---> nicht notwendig
                        Cv2.ImShow("Matches", queryImage);

                        Cv2.WaitKey();
                        }
                    else
                    {
                        Console.WriteLine($"Nicht genügend Matches - {goodMatches.Length}/{MIN_MATCH_COUNT}");
                    }
                }
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
            double rotationAngleDegrees;
            return rotationAngleDegrees = rotationAngle * (180.0 / Math.PI);
        }

        static void drawRectangle(Point2f[] eckpunkte, Mat img)
        {
            Point[] pointArray = ConvertToPointArray(eckpunkte);
            Cv2.Line(img, pointArray[0], pointArray[1], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[1], pointArray[2], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[2], pointArray[3], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
            Cv2.Line(img, pointArray[3], pointArray[0], Scalar.AliceBlue, thickness: 1, lineType: LineTypes.Link8);
        }

        static void ConsoleOutput(string time, DMatch[] goodMatches, double rotationAngleDegrees, Point2f[] destination)
        {
            Console.WriteLine($"Zeit: {time} Sekunden");
            Console.WriteLine($"Matches: {goodMatches.Length}");
            Console.WriteLine($"Drehung: {rotationAngleDegrees:F4} Grad");
            Console.WriteLine($"Position im Bild:\n A:{destination[0]}\n B:{destination[1]}\n C:{destination[2]}\n D:{destination[3]}");
        }
    }
}