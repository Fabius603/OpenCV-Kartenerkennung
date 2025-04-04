using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

class Program
{
    static void Main(string[] args)
    {
        // Load the image
        string imagePath = "C:\\Users\\schlieper\\source\\repos\\OpenCV Kartenerkennung - Chessboard\\OpenCV\\images\\Chessboard\\input\\Schachbrett3.PNG"; // Use the specified image path
        using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.Color))
        {
            if (image.IsEmpty)
            {
                Console.WriteLine("Could not load the image.");
                return;
            }

            // Convert to grayscale
            Mat gray = new Mat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            // Apply Gaussian blur to reduce noise
            CvInvoke.GaussianBlur(gray, gray, new Size(5, 5), 0);

            // Apply simple thresholding instead of adaptive thresholding
            Mat binary = new Mat();
            CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);

            // Apply morphological opening to remove small noise
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(binary, binary, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));

            // Show binary image for debugging
            //CvInvoke.Imshow("Binary Image", binary);
            //CvInvoke.WaitKey(0);

            // Find contours
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(binary, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                Console.WriteLine($"Total contours found: {contours.Size}");

                // Create a list to store corner points
                VectorOfPointF corners = new VectorOfPointF();

                // Process each contour to find corners
                for (int i = 0; i < contours.Size; i++)
                {
                    VectorOfPoint contour = contours[i];
                    double area = CvInvoke.ContourArea(contour);
                    if (area < 1000) continue; // Adjusted area threshold

                    double perimeter = CvInvoke.ArcLength(contour, true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contour, approx, 0.02 * perimeter, true);

                    // Check if the contour has 4 vertices and is a square
                    //if (approx.Size == 4 && IsSquare(approx))
                    //{
                        Console.WriteLine($"Square found with area: {area}");
                        for (int j = 0; j < approx.Size; j++)
                        {
                            PointF corner = new PointF(approx[j].X, approx[j].Y);
                            corners.Push(new PointF[] { corner });
                        }
                    //}
                }

                // Remove duplicate corners
                VectorOfPointF uniqueCorners = RemoveDuplicates(corners, 5.0);

                Console.WriteLine($"Total unique corners found: {uniqueCorners.Size}");

                // Draw detected corners on the original image
                for (int i = 0; i < uniqueCorners.Size; i++)
                {
                    CvInvoke.Circle(image, new Point((int)uniqueCorners[i].X, (int)uniqueCorners[i].Y), 5, new MCvScalar(0, 0, 255), -1); // Red circles
                }

                // Display the result
                CvInvoke.Imshow("Detected Corners", image);
                CvInvoke.WaitKey(0);
                CvInvoke.DestroyAllWindows();

                // Save the result image
                string outputPath = "C:\\Users\\schlieper\\source\\repos\\OpenCV Kartenerkennung - Chessboard\\OpenCV\\images\\Chessboard\\output\\";
                CvInvoke.Imwrite(outputPath + "binary.jpg", binary);
                CvInvoke.Imwrite(outputPath + "corners.jpg", image);
                Console.WriteLine($"Result saved to {outputPath}");
            }
        }
    }

    // Simplified function to check if a contour is a square
    static bool IsSquare(VectorOfPoint approx)
    {
        if (approx.Size != 4) return false;

        // Calculate bounding rectangle
        var rect = CvInvoke.BoundingRectangle(approx);
        double aspectRatio = (double)rect.Width / rect.Height;

        // Check if the aspect ratio is close to 1 (square)
        if (aspectRatio > 0.8 && aspectRatio < 1.2)
        {
            return true;
        }

        return false;
    }

    // Function to remove duplicate corners
    static VectorOfPointF RemoveDuplicates(VectorOfPointF corners, double threshold)
    {
        VectorOfPointF uniqueCorners = new VectorOfPointF();
        for (int i = 0; i < corners.Size; i++)
        {
            PointF p1 = corners[i];
            bool isDuplicate = false;

            for (int j = 0; j < uniqueCorners.Size; j++)
            {
                PointF p2 = uniqueCorners[j];
                double distance = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                if (distance < threshold)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                uniqueCorners.Push(new PointF[] { p1 });
            }
        }

        return uniqueCorners;
    }
}