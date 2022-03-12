namespace CITRUS
{
    partial class CIT_00_1_FillingParameterLevelProgressBarForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar_pb = new System.Windows.Forms.ProgressBar();
            this.label_LevelName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar_pb
            // 
            this.progressBar_pb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_pb.Location = new System.Drawing.Point(12, 12);
            this.progressBar_pb.Name = "progressBar_pb";
            this.progressBar_pb.Size = new System.Drawing.Size(410, 41);
            this.progressBar_pb.TabIndex = 0;
            // 
            // label_LevelName
            // 
            this.label_LevelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_LevelName.AutoSize = true;
            this.label_LevelName.Location = new System.Drawing.Point(12, 59);
            this.label_LevelName.Name = "label_LevelName";
            this.label_LevelName.Size = new System.Drawing.Size(61, 13);
            this.label_LevelName.TabIndex = 1;
            this.label_LevelName.Text = "LevelName";
            // 
            // CIT_00_1_FillingParameterLevelProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 81);
            this.Controls.Add(this.label_LevelName);
            this.Controls.Add(this.progressBar_pb);
            this.MaximumSize = new System.Drawing.Size(450, 120);
            this.MinimumSize = new System.Drawing.Size(450, 120);
            this.Name = "CIT_00_1_FillingParameterLevelProgressBarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Нужно немного подождать...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ProgressBar progressBar_pb;
        public System.Windows.Forms.Label label_LevelName;
    }
}