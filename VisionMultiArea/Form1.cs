using OpenCvSharp;

namespace VisionMultiArea
{
    public partial class Vision : Form
    {
        private System.Drawing.Point startPoint;
        private Rectangle selectionRectangle;
        private List<Rectangle> selectedAreas = new List<Rectangle>();

        public Vision()
        {
            InitializeComponent();
            pictureBox1.Image = Image.FromFile("C:\\Users\\schlieper\\source\\repos\\OpenCV Kartenerkennung\\VisionMultiArea\\Template.bmp");
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Focus();

            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Paint += pictureBox1_Paint;

            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;

            this.WindowState = FormWindowState.Maximized;
            PositionResultLabel();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (selectedAreas.Count > 0)
                {
                    selectedAreas.RemoveAt(selectedAreas.Count - 1);
                    pictureBox1.Invalidate();
                }
            }
        }

        private void PositionResultLabel()
        {
            if (pictureBox1 != null && ResultValueLabel != null)
            {
                ResultValueLabel.Location = new System.Drawing.Point(pictureBox1.Right + 10, pictureBox1.Top);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionResultLabel();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);
                selectionRectangle = new Rectangle(x, y, width, height);
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedAreas.Add(selectionRectangle);
            selectionRectangle = Rectangle.Empty;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectionRectangle != Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(Pens.Red, selectionRectangle);
            }

            // Zeichne alle gespeicherten Rechtecke
            foreach (var rect in selectedAreas)
            {
                e.Graphics.DrawRectangle(Pens.Blue, rect);
            }
        }

        private void StartCalculationButton_Click(object sender, EventArgs e)
        {
            if (selectedAreas.Count == 0)
            {
                MessageBox.Show("Mindestens ein Rechteck auswählen.");
                return;
            }

            try
            {
                Cv2.DestroyWindow("Result");
            } catch { }

            Mat template = new Mat("Template.bmp");
            Mat fullImage = new Mat("FullImage.bmp");

            ResultValues RValues = ImageCalc.CalculateImage(template, fullImage, ToOpenCvRect(selectedAreas));
            Ausgabe(RValues);
            LoadAndDisplayImage(RValues.ResultImage);
        }

        private Rect[] ToOpenCvRect(List<Rectangle> selectedAreas)
        {
            Rect[] rectArray = new Rect[selectedAreas.Count];

            for (int i = 0; i < selectedAreas.Count; i++)
            {
                Rectangle rect = selectedAreas[i];
                rectArray[i] = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
            }

            return rectArray;
        }

        private void LoadAndDisplayImage(Mat image)
        {

            if (image.Empty())
            {
                MessageBox.Show("Bild konnte nicht geladen werden.");
                return;
            }

            Cv2.ImShow("Result", image);

            Cv2.WaitKey(0); 
        }

        private void Ausgabe(ResultValues RValues)
        {
            var resultText = new System.Text.StringBuilder();

            resultText.AppendLine($"Zeit: {RValues.Time}\n");
            resultText.AppendLine($"Rotation: {RValues.Rotation}\n");

            int length = RValues.CenterPoints.Length;
            if (RValues.OffsetX.Length == length && RValues.OffsetY.Length == length)
            {
                for (int i = 0; i < length; i++)
                {
                    resultText.AppendLine($"Centerpoint {i}: {RValues.CenterPoints[i]}");
                    resultText.AppendLine($"OffsetX {i}: {RValues.OffsetX[i]}");
                    resultText.AppendLine($"OffsetY {i}: {RValues.OffsetY[i]}\n");
                }
            }
            else
            {
                resultText.AppendLine("Fehler: Die Arrays haben unterschiedliche Längen.");
            }

            ResultValueLabel.Text = resultText.ToString();
        }
    }
}
