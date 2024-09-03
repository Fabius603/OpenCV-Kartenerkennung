using OpenCvSharp;

namespace VisionMultiArea
{
    public class TemplateValues
    {
        public Mat TemplateImage { get; set; }
        public Mat GrayTemplateImage { get; set; }
        public Point2f ExpectedCenter { get; set; }
        public Rect TemplateArea { get; set; }

        public TemplateValues(Mat template, Rect templateArea)
        {
            TemplateImage = template.SubMat(templateArea);
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

        public QueryValues(Mat fullImage, Rect roi, TemplateValues[] TValues)
        {
            QueryImage = fullImage.SubMat(roi);
            ROI = roi;
            Mask = createMask(roi, TValues);
        }

        public Mat createMask(Rect roi, TemplateValues[] TValues)
        {
            Mat mask = new Mat(roi.Height, roi.Width, MatType.CV_8UC1, Scalar.All(0));

            foreach (var rect in TValues)
            {
                Cv2.Rectangle(mask, new Rect(
                    rect.TemplateArea.X - roi.X - 50,
                    rect.TemplateArea.Y - roi.Y - 50,
                    rect.TemplateArea.Width + 100,
                    rect.TemplateArea.Height + 100),
                    Scalar.All(255),
                    -1);
            }

            //Cv2.ImShow("test", mask);
            return mask;
        }
    }

    public class ResultValues
    {
        public double Rotation { get; set; }
        public double[] OffsetX { get; set; }
        public double[] OffsetY { get; set; }
        public Point2f[] CenterPoints { get; set; }
        public string? Time { get; set; }
        public Mat ResultImage { get; set; }
        public Point2f AverageOffset { get; set; }
        public ResultValues(double rotation, Point2f[] centerPoints, double[] offsetX, double[] offsetY, Point2f averageOffset)
        {
            Rotation = rotation;
            CenterPoints = centerPoints;
            OffsetX = offsetX;
            OffsetY = offsetY;
            AverageOffset = averageOffset;
        }
    }
}
