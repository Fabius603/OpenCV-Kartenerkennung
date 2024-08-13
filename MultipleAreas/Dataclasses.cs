using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

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
            this.TemplateImage = fullImage.SubMat(templateArea);
            this.GrayTemplateImage = this.TemplateImage.CvtColor(ColorConversionCodes.BGR2GRAY);
            int centerX = (templateArea.X + templateArea.Width / 2);
            int centerY = (templateArea.Y + templateArea.Height / 2);
            this.ExpectedCenter = new Point2f(centerX, centerY); 
            this.TemplateArea = templateArea;
        }
    }

    public class QueryValues
    {
        public Mat QueryImage { get; set; }
        public Rect ROI { get; set; }

        public QueryValues(Mat fullImage, Rect roi)
        {
            QueryImage = fullImage.SubMat(roi);
            ROI = roi;
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
