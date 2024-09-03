using OpenCvSharp;
using System.IO;
using System.Windows.Forms;

namespace VisionMultiArea
{
    public partial class Vision : Form
    {
        private System.Drawing.Point startPoint;
        private Rectangle selectionRectangle;
        private List<Rectangle> selectedAreas = new List<Rectangle>();
        private string TemplatePath = "";
        private string QuerryPath = "";
        private bool isMouseHoveringonTemplate = false;
        private bool isMouseHoveringonQuerry = false;

        public Vision()
        {
            InitializeComponent();
            TemplatePictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            QuerryPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Focus();

            TemplatePictureBox.MouseDown += TemplatePictureBox_MouseDown;
            TemplatePictureBox.MouseMove += TemplatePictureBox_MouseMove;
            TemplatePictureBox.MouseUp += TemplatePictureBox_MouseUp;
            TemplatePictureBox.Paint += TemplatePictureBox_Paint;

            TemplatePictureBox.MouseEnter += new EventHandler(TemplatePictureBox_MouseEnter);
            TemplatePictureBox.MouseLeave += new EventHandler(TemplatePictureBox_MouseLeave);

            QuerryPictureBox.Paint += QuerryPictureBox_Paint;

            QuerryPictureBox.MouseEnter += new EventHandler(QuerryPictureBox_MouseEnter);
            QuerryPictureBox.MouseLeave += new EventHandler(QuerryPictureBox_MouseLeave);

            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;

            this.WindowState = FormWindowState.Maximized;
            PositionToTemplatePictureBox();
            TryLoadPictures();
        }

        private void TemplatePictureBox_MouseEnter(object sender, EventArgs e)
        {
            isMouseHoveringonTemplate = true;
            TemplatePictureBox.Invalidate();
        }
        private void TemplatePictureBox_MouseLeave(object sender, EventArgs e)
        {
            isMouseHoveringonTemplate = false;
            TemplatePictureBox.Invalidate();
        }
        private void QuerryPictureBox_MouseEnter(object sender, EventArgs e)
        {
            isMouseHoveringonQuerry = true;
            QuerryPictureBox.Invalidate();
        }
        private void QuerryPictureBox_MouseLeave(object sender, EventArgs e)
        {
            isMouseHoveringonQuerry = false;
            QuerryPictureBox.Invalidate();
        }

        private void TryLoadPictures()
        {
            try
            {
                string templatePath = "C:\\Users\\schlieper\\OneDrive - Otto Künnecke GmbH\\Pictures\\LaserRotation\\Template.bmp";
                string querryPath = "C:\\Users\\schlieper\\OneDrive - Otto Künnecke GmbH\\Pictures\\LaserRotation\\QuerryImage.bmp";
                SetNewTemplatePicture(templatePath);
                SetNewQuerryPicture(querryPath);
            } catch { }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (selectedAreas.Count > 0)
                {
                    selectedAreas.RemoveAt(selectedAreas.Count - 1);
                    TemplatePictureBox.Invalidate();
                }
            }
        }

        private void PositionToTemplatePictureBox()
        {
            if (TemplatePictureBox != null && ResultValueLabel != null)
            {
                ResultValueLabel.Location = new System.Drawing.Point(TemplatePictureBox.Right + 10, TemplatePictureBox.Top);
            }
            if(QuerryPictureBox != null && TemplatePictureBox != null)
            {
                QuerryPictureBox.Location = new System.Drawing.Point(TemplatePictureBox.Right + 10, TemplatePictureBox.Bottom - QuerryPictureBox.Height);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionToTemplatePictureBox();
        }

        private void TemplatePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        private void TemplatePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);
                selectionRectangle = new Rectangle(x, y, width, height);
                TemplatePictureBox.Invalidate();
            }
        }

        private void TemplatePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            selectedAreas.Add(selectionRectangle);
            selectionRectangle = Rectangle.Empty;
            TemplatePictureBox.Invalidate();
        }

        private void TemplatePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (selectionRectangle != Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(Pens.Red, selectionRectangle);
            }

            foreach (var rect in selectedAreas)
            {
                e.Graphics.DrawRectangle(Pens.Blue, rect);
            }

            if (isMouseHoveringonTemplate)
            {
                string text = "Template";
                Font font = new Font("Arial", 12, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.White);
                e.Graphics.DrawString(text, font, brush, new PointF(5, 5));
            }
        }
        
        private void QuerryPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (isMouseHoveringonQuerry)
            {
                string text = "Querry";
                Font font = new Font("Arial", 12, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.White);
                e.Graphics.DrawString(text, font, brush, new PointF(5, 5));
            }
        }

        private void StartCalculationButton_Click(object sender, EventArgs e)
        {
            if (selectedAreas.Count == 0)
            {
                MessageBox.Show("Mindestens ein Rechteck auswählen.");
                return;
            }
            else if(TemplatePath == "" || QuerryPath == "")
            {
                MessageBox.Show("Template und Querry müssen gegeben sein!");
                return;
            }

            try
            {
                Cv2.DestroyWindow("Result");
            }
            catch { }
            Mat template = new Mat(TemplatePath);
            Mat fullImage = new Mat(QuerryPath);

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
            for (int i = 0; i < length; i++)
            {
                resultText.AppendLine($"Centerpoint {i}: {RValues.CenterPoints[i]}");
                resultText.AppendLine($"OffsetX {i}: {RValues.OffsetX[i]}");
                resultText.AppendLine($"OffsetY {i}: {RValues.OffsetY[i]}\n");
            }
            resultText.AppendLine($"AverageOffset: {RValues.AverageOffset}");
            ResultValueLabel.Text = resultText.ToString();
        }

        private string GetFilePath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string selectedFilePath = "";
                string picturesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                openFileDialog.InitialDirectory = picturesFolderPath;
                openFileDialog.Filter = "All files (*.*)|*.*|Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    return selectedFilePath;
                }
                return "";
            }
        }

        private void SetNewTemplatePicture(string path)
        {
            if (path == "")
            {
                return;
            }

            TemplatePath = path;

            using (var originalImage = Image.FromFile(path))
            {
                using (var memoryStream = new MemoryStream())
                {
                    originalImage.Save(memoryStream, originalImage.RawFormat);
                    memoryStream.Position = 0;
                    TemplatePictureBox.Image = Image.FromStream(memoryStream);
                }
            }

            PositionToTemplatePictureBox();
        }

        private void SetNewQuerryPicture(string path)
        {
            if (path == "")
            {
                return;
            }

            QuerryPath = path;

            using (var originalImage = Image.FromFile(path))
            {
                using (var memoryStream = new MemoryStream())
                {
                    originalImage.Save(memoryStream, originalImage.RawFormat);
                    memoryStream.Position = 0;
                    QuerryPictureBox.Image = Image.FromStream(memoryStream);
                }
            }

            PositionToTemplatePictureBox();
        }

        private void LoadTemplateButton_Click(object sender, EventArgs e)
        {
            string path = GetFilePath();
            SetNewTemplatePicture(path);
        }

        private void LoadQuerryButton_Click(object sender, EventArgs e)
        {
            string path = GetFilePath();
            SetNewQuerryPicture(path);
        }
    }
}
