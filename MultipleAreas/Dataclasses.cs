﻿using OpenCvSharp;

namespace MultipleAreas
{
    public class TemplateValues
    {
        public Mat TemplateImage { get; set; }
        public Mat GrayTemplateImage { get; set; }
        public Point2f ExpectedCenter { get; set; }
        public Rect TemplateArea { get; set; }

        public TemplateValues(Mat fullImage, Rect templateArea)
        {
            TemplateImage = fullImage.SubMat(templateArea);
            GrayTemplateImage = TemplateImage.CvtColor(ColorConversionCodes.BGR2GRAY);
            int centerX = (templateArea.X + templateArea.Width / 2);
            int centerY = (templateArea.Y + templateArea.Height / 2);
            ExpectedCenter = new Point2f(centerX, centerY); 
            TemplateArea = templateArea;
        }
    }

    public class QueryValues
    {
        public Mat QueryImage { get; set; }
        public Rect ROI { get; set; }
        public Rect[] MaskROIs { get; set; }
        public Mat Mask { get; set; }

        public QueryValues(Mat fullImage, Rect roi, Rect[] MaskROIs)
        {
            QueryImage = fullImage.SubMat(roi);
            ROI = roi;
            Mask = createMask(roi, MaskROIs);
        }

        public Mat createMask(Rect roi, Rect[] rects)
        {
            Mat mask = new Mat(roi.Height, roi.Width, MatType.CV_8UC1, Scalar.All(0));

            foreach (var rect in rects)
            {
                Cv2.Rectangle(mask, new Rect(rect.X - roi.X, rect.Y - roi.Y, rect.Width, rect.Height), Scalar.All(255), -1);
            }

            Cv2.ImShow("test", mask);
            return mask;
        }
    }

    public class ResultValues
    {
        public double Rotation { get; set; }
        public double[] OffsetX { get; set; }
        public double[] OffsetY { get; set; }
        public Point2f[] CenterPoints { get; set; }

        public ResultValues(double rotation, Point2f[] centerPoints, double[] offsetX, double[] offsetY)
        {
            Rotation = rotation;
            CenterPoints = centerPoints;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
