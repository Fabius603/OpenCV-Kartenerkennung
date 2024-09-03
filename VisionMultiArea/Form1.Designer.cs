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
            pictureBox1 = new PictureBox();
            StartCalculationButton = new Button();
            ResultValueLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(124, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(463, 466);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // StartCalculationButton
            // 
            StartCalculationButton.Location = new Point(12, 14);
            StartCalculationButton.Name = "StartCalculationButton";
            StartCalculationButton.Size = new Size(75, 23);
            StartCalculationButton.TabIndex = 1;
            StartCalculationButton.Text = "Caclulate";
            StartCalculationButton.UseVisualStyleBackColor = true;
            StartCalculationButton.Click += StartCalculationButton_Click;
            // 
            // ResultValueLabel
            // 
            ResultValueLabel.AutoSize = true;
            ResultValueLabel.Location = new Point(611, 14);
            ResultValueLabel.Name = "ResultValueLabel";
            ResultValueLabel.Size = new Size(184, 15);
            ResultValueLabel.TabIndex = 2;
            ResultValueLabel.Text = "Ergebnisse werden hier Angezeigt";
            // 
            // Vision
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1141, 593);
            Controls.Add(ResultValueLabel);
            Controls.Add(StartCalculationButton);
            Controls.Add(pictureBox1);
            Name = "Vision";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button StartCalculationButton;
        private Label ResultValueLabel;
    }
}
