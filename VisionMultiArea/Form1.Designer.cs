namespace VisionMultiArea
{
    partial class Vision
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TemplatePictureBox = new PictureBox();
            StartCalculationButton = new Button();
            ResultValueLabel = new Label();
            LoadTemplateButton = new Button();
            LoadQuerryButton = new Button();
            QuerryPictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)TemplatePictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)QuerryPictureBox).BeginInit();
            SuspendLayout();
            // 
            // TemplatePictureBox
            // 
            TemplatePictureBox.Location = new Point(162, 6);
            TemplatePictureBox.Name = "TemplatePictureBox";
            TemplatePictureBox.Size = new Size(463, 466);
            TemplatePictureBox.TabIndex = 0;
            TemplatePictureBox.TabStop = false;
            // 
            // StartCalculationButton
            // 
            StartCalculationButton.Location = new Point(12, 64);
            StartCalculationButton.Name = "StartCalculationButton";
            StartCalculationButton.Size = new Size(130, 23);
            StartCalculationButton.TabIndex = 1;
            StartCalculationButton.Text = "Caclulate";
            StartCalculationButton.UseVisualStyleBackColor = true;
            StartCalculationButton.Click += StartCalculationButton_Click;
            // 
            // ResultValueLabel
            // 
            ResultValueLabel.AutoSize = true;
            ResultValueLabel.Location = new Point(733, 14);
            ResultValueLabel.Name = "ResultValueLabel";
            ResultValueLabel.Size = new Size(184, 15);
            ResultValueLabel.TabIndex = 2;
            ResultValueLabel.Text = "Ergebnisse werden hier Angezeigt";
            // 
            // LoadTemplateButton
            // 
            LoadTemplateButton.Location = new Point(12, 6);
            LoadTemplateButton.Name = "LoadTemplateButton";
            LoadTemplateButton.Size = new Size(130, 23);
            LoadTemplateButton.TabIndex = 3;
            LoadTemplateButton.Text = "Load Template";
            LoadTemplateButton.UseVisualStyleBackColor = true;
            LoadTemplateButton.Click += LoadTemplateButton_Click;
            // 
            // LoadQuerryButton
            // 
            LoadQuerryButton.Location = new Point(12, 35);
            LoadQuerryButton.Name = "LoadQuerryButton";
            LoadQuerryButton.Size = new Size(130, 23);
            LoadQuerryButton.TabIndex = 4;
            LoadQuerryButton.Text = "Load Querry";
            LoadQuerryButton.UseVisualStyleBackColor = true;
            LoadQuerryButton.Click += LoadQuerryButton_Click;
            // 
            // QuerryPictureBox
            // 
            QuerryPictureBox.Location = new Point(631, 152);
            QuerryPictureBox.Name = "QuerryPictureBox";
            QuerryPictureBox.Size = new Size(419, 320);
            QuerryPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            QuerryPictureBox.TabIndex = 5;
            QuerryPictureBox.TabStop = false;
            // 
            // Vision
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1141, 593);
            Controls.Add(StartCalculationButton);
            Controls.Add(QuerryPictureBox);
            Controls.Add(LoadQuerryButton);
            Controls.Add(LoadTemplateButton);
            Controls.Add(ResultValueLabel);
            Controls.Add(TemplatePictureBox);
            Name = "Vision";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)TemplatePictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)QuerryPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox TemplatePictureBox;
        private Button StartCalculationButton;
        private Label ResultValueLabel;
        private Button LoadTemplateButton;
        private Button LoadQuerryButton;
        private PictureBox QuerryPictureBox;
        private Panel panel1;
    }
}
