namespace CITRUS.CIT_03_1_WallFinishCreator
{
    partial class CIT_03_1_WallFinishCreatorForm
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
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.comboBox_WallTypeFirst = new System.Windows.Forms.ComboBox();
            this.textBox_MainWallFinishHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(172, 104);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(43, 104);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 4;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // comboBox_WallTypeFirst
            // 
            this.comboBox_WallTypeFirst.FormattingEnabled = true;
            this.comboBox_WallTypeFirst.Location = new System.Drawing.Point(15, 27);
            this.comboBox_WallTypeFirst.Name = "comboBox_WallTypeFirst";
            this.comboBox_WallTypeFirst.Size = new System.Drawing.Size(260, 21);
            this.comboBox_WallTypeFirst.Sorted = true;
            this.comboBox_WallTypeFirst.TabIndex = 6;
            this.comboBox_WallTypeFirst.SelectedIndexChanged += new System.EventHandler(this.comboBox_WallTypeFirst_SelectedIndexChanged);
            // 
            // textBox_MainWallFinishHeight
            // 
            this.textBox_MainWallFinishHeight.Location = new System.Drawing.Point(175, 54);
            this.textBox_MainWallFinishHeight.Name = "textBox_MainWallFinishHeight";
            this.textBox_MainWallFinishHeight.Size = new System.Drawing.Size(100, 20);
            this.textBox_MainWallFinishHeight.TabIndex = 8;
            this.textBox_MainWallFinishHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_MainWallFinishHeight.TextChanged += new System.EventHandler(this.textBox_MainWallFinishHeight_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Высота отделки, мм:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Выберите тип отделки:";
            // 
            // CIT_03_1_WallFinishCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_MainWallFinishHeight);
            this.Controls.Add(this.comboBox_WallTypeFirst);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.MaximumSize = new System.Drawing.Size(300, 180);
            this.MinimumSize = new System.Drawing.Size(300, 180);
            this.Name = "CIT_03_1_WallFinishCreatorForm";
            this.Text = "Отделка стен";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.ComboBox comboBox_WallTypeFirst;
        private System.Windows.Forms.TextBox textBox_MainWallFinishHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}