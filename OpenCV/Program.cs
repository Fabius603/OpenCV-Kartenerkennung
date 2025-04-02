using OpenCvSharp;
using System;
using System.Diagnostics;

namespace OpenCV
{
    internal class Program
    {
        // Globale Variablen
        private static Mat originalImage;
        private const string windowName = "Interaktive Filter";

        // Trackbar-Parameter
        private static int blockSizeValue = 15;  // Lokale Umgebung für AdaptiveThreshold (muss ungerade sein!)
        private static int cValue = 5;           // Konstante, die vom Mittelwert abgezogen wird
        private static int blurKsizeValue = 3;   // Kernel-Größe für MedianBlur
        private static int morphSizeValue = 5;   // Kernel-Größe für Morph. Operationen
        private static int minAreaValue = 1000;  // Minimum-Flaeche (ConnectedComponents)

        static void Main()
        {
            ProcessImage(
                "C:\\Users\\schlieper\\source\\repos\\OpenCV Kartenerkennung - Chessboard\\OpenCV\\images\\Chessboard\\queryImage\\2.bmp"
            );
        }

        static void ProcessImage(string imagePath)
        {
            // Bild laden (Graustufen)
            originalImage = new Mat(imagePath, ImreadModes.Grayscale);

            // Fenster anlegen
            Cv2.NamedWindow(windowName);

            // Trackbar 1: blockSize
            Cv2.CreateTrackbar(
                "BlockSize",               // Name
                windowName,                // Fenster
                ref blockSizeValue,        // Anfangswert
                200,                        // Maximalwert (z.B. bis 51)
                OnBlockSizeChanged,        // Callback
                IntPtr.Zero
            );

            // Trackbar 2: C (Offset)
            Cv2.CreateTrackbar(
                "C-Offset",
                windowName,
                ref cValue,
                20,
                OnCChanged,
                IntPtr.Zero
            );

            // Trackbar 3: Blur-Kernel
            Cv2.CreateTrackbar(
                "Blur KSize",
                windowName,
                ref blurKsizeValue,
                40,
                OnBlurChanged,
                IntPtr.Zero
            );

            // Trackbar 4: Morph-Kernel
            Cv2.CreateTrackbar(
                "Morph Size",
                windowName,
                ref morphSizeValue,
                40,
                OnMorphChanged,
                IntPtr.Zero
            );

            // Trackbar 5: minArea
            Cv2.CreateTrackbar(
                "minArea",
                windowName,
                ref minAreaValue,
                30000,
                OnMinAreaChanged,
                IntPtr.Zero
            );

            // Anfangszustand einmal anzeigen
            ShowFilteredImage();

            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
        }

        // ——————————————————————————————————————————————————————————
        // Trackbar-Callbacks:

        private static void OnBlockSizeChanged(int pos, IntPtr userdata)
        {
            blockSizeValue = pos;
            // BlockSize sollte >= 3 sein und ungerade
            if (blockSizeValue < 3) blockSizeValue = 3;
            if (blockSizeValue % 2 == 0) blockSizeValue++;
            ShowFilteredImage();
        }

        private static void OnCChanged(int pos, IntPtr userdata)
        {
            cValue = pos;
            ShowFilteredImage();
        }

        private static void OnBlurChanged(int pos, IntPtr userdata)
        {
            blurKsizeValue = pos;
            ShowFilteredImage();
        }

        private static void OnMorphChanged(int pos, IntPtr userdata)
        {
            morphSizeValue = pos;
            ShowFilteredImage();
        }

        private static void OnMinAreaChanged(int pos, IntPtr userdata)
        {
            minAreaValue = pos;
            ShowFilteredImage();
        }

        // ——————————————————————————————————————————————————————————
        // Zeigt das Bild mit den aktuellen Parametern an
        private static void ShowFilteredImage()
        {
            // 1) Evtl. Weichzeichnen (MedianBlur), wenn blurKsizeValue >= 2
            Mat blurred = new Mat();
            int kSize = (blurKsizeValue < 3) ? 1 : blurKsizeValue;
            // MedianBlur braucht ungerade Kernel-Größe:
            if (kSize % 2 == 0) kSize++;
            if (kSize > 1)
            {
                Cv2.MedianBlur(originalImage, blurred, kSize);
            }
            else
            {
                blurred = originalImage.Clone();
            }

            // 2) Adaptives Threshold
            //    BlockSize: wie groß ist die lokale Umgebung?
            //    cValue   : wie viel wird vom lokalen Mittelwert abgezogen?
            Mat binary = new Mat();

            // Sicherheit: blockSizeValue muss >= 3 und ungerade sein
            int localBlockSize = (blockSizeValue < 3) ? 3 : blockSizeValue;
            if (localBlockSize % 2 == 0) localBlockSize++;

            Cv2.AdaptiveThreshold(
                blurred,
                binary,
                maxValue: 255,
                adaptiveMethod: AdaptiveThresholdTypes.MeanC, // oder GaussianC
                thresholdType: ThresholdTypes.Binary,
                blockSize: localBlockSize,
                c: cValue
            );

            // 3) Morphological Closing (um weiße Flächen zu verbinden)
            Mat morphClosed = binary.Clone();
            if (morphSizeValue > 0)
            {
                // Kernelgröße
                Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(morphSizeValue, morphSizeValue));
                Cv2.MorphologyEx(morphClosed, morphClosed, MorphTypes.Close, kernel);
            }

            // 4) Weiße Flecken entfernen
            Mat cleanedWhite = FilterSmallComponents(morphClosed, minAreaValue);

            // 5) Schwarze Flecken entfernen (Invertieren -> Filter -> Zurück-Invertieren)
            Mat inverted = new Mat();
            Cv2.BitwiseNot(cleanedWhite, inverted);
            Mat cleanedBlack = FilterSmallComponents(inverted, minAreaValue);
            Cv2.BitwiseNot(cleanedBlack, cleanedBlack);

            // 6) Anzeigen
            Cv2.ImShow(windowName, cleanedBlack);
        }

        // ——————————————————————————————————————————————————————————
        // Kleine Komponenten entfernen per ConnectedComponents
        private static Mat FilterSmallComponents(Mat binaryImage, int minArea)
        {
            if (minArea < 1)
            {
                // Keine Filterung, falls minArea = 0
                return binaryImage;
            }

            Mat labels = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();

            int numLabels = Cv2.ConnectedComponentsWithStats(
                binaryImage,
                labels,
                stats,
                centroids,
                PixelConnectivity.Connectivity8,
                MatType.CV_32S
            );

            Mat output = Mat.Zeros(binaryImage.Size(), MatType.CV_8UC1);

            // Wir beginnen bei Label 1 (Label 0 = Hintergrund)
            for (int i = 1; i < numLabels; i++)
            {
                int area = stats.Get<int>(i, (int)ConnectedComponentsTypes.Area);
                if (area >= minArea)
                {
                    Mat componentMask = new Mat();
                    Cv2.Compare(labels, i, componentMask, CmpType.EQ);
                    output.SetTo(255, componentMask);
                }
            }

            return output;
        }
    }

    // Optional: Wenn du Zeitmessung benötigst
    class Timer
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
