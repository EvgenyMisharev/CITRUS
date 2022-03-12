namespace CITRUS.CIT_03_3_Insolation
{
    partial class InsolationAtPointForm
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
            this.checkBox_CheckSelectedPanels = new System.Windows.Forms.CheckBox();
            this.groupBox_VerificationOptions = new System.Windows.Forms.GroupBox();
            this.checkBox_WallsAndFloorsGeometry = new System.Windows.Forms.CheckBox();
            this.checkBox_CheckSelectedPoints = new System.Windows.Forms.CheckBox();
            this.radioButton_PointInsolation = new System.Windows.Forms.RadioButton();
            this.radioButton_PanelInsolation = new System.Windows.Forms.RadioButton();
            this.groupBox_VerificationOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(302, 159);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 7;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(173, 159);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 6;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // checkBox_CheckSelectedPanels
            // 
            this.checkBox_CheckSelectedPanels.AutoSize = true;
            this.checkBox_CheckSelectedPanels.Checked = true;
            this.checkBox_CheckSelectedPanels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CheckSelectedPanels.Location = new System.Drawing.Point(24, 43);
            this.checkBox_CheckSelectedPanels.Name = "checkBox_CheckSelectedPanels";
            this.checkBox_CheckSelectedPanels.Size = new System.Drawing.Size(181, 17);
            this.checkBox_CheckSelectedPanels.TabIndex = 8;
            this.checkBox_CheckSelectedPanels.Text = "Проверить выбранные панели";
            this.checkBox_CheckSelectedPanels.UseVisualStyleBackColor = true;
            // 
            // groupBox_VerificationOptions
            // 
            this.groupBox_VerificationOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_VerificationOptions.Controls.Add(this.checkBox_WallsAndFloorsGeometry);
            this.groupBox_VerificationOptions.Controls.Add(this.checkBox_CheckSelectedPoints);
            this.groupBox_VerificationOptions.Controls.Add(this.radioButton_PointInsolation);
            this.groupBox_VerificationOptions.Controls.Add(this.radioButton_PanelInsolation);
            this.groupBox_VerificationOptions.Controls.Add(this.checkBox_CheckSelectedPanels);
            this.groupBox_VerificationOptions.Location = new System.Drawing.Point(12, 12);
            this.groupBox_VerificationOptions.Name = "groupBox_VerificationOptions";
            this.groupBox_VerificationOptions.Size = new System.Drawing.Size(390, 141);
            this.groupBox_VerificationOptions.TabIndex = 9;
            this.groupBox_VerificationOptions.TabStop = false;
            this.groupBox_VerificationOptions.Text = "Варианты проверки";
            // 
            // checkBox_WallsAndFloorsGeometry
            // 
            this.checkBox_WallsAndFloorsGeometry.AutoSize = true;
            this.checkBox_WallsAndFloorsGeometry.Location = new System.Drawing.Point(24, 112);
            this.checkBox_WallsAndFloorsGeometry.Name = "checkBox_WallsAndFloorsGeometry";
            this.checkBox_WallsAndFloorsGeometry.Size = new System.Drawing.Size(353, 17);
            this.checkBox_WallsAndFloorsGeometry.TabIndex = 12;
            this.checkBox_WallsAndFloorsGeometry.Text = "Учитывать геометрию стен и перекрытий из связанных файлов";
            this.checkBox_WallsAndFloorsGeometry.UseVisualStyleBackColor = true;
            // 
            // checkBox_CheckSelectedPoints
            // 
            this.checkBox_CheckSelectedPoints.AutoSize = true;
            this.checkBox_CheckSelectedPoints.Checked = true;
            this.checkBox_CheckSelectedPoints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CheckSelectedPoints.Location = new System.Drawing.Point(24, 89);
            this.checkBox_CheckSelectedPoints.Name = "checkBox_CheckSelectedPoints";
            this.checkBox_CheckSelectedPoints.Size = new System.Drawing.Size(173, 17);
            this.checkBox_CheckSelectedPoints.TabIndex = 11;
            this.checkBox_CheckSelectedPoints.Text = "Проверить выбранные точки";
            this.checkBox_CheckSelectedPoints.UseVisualStyleBackColor = true;
            // 
            // radioButton_PointInsolation
            // 
            this.radioButton_PointInsolation.AutoSize = true;
            this.radioButton_PointInsolation.Location = new System.Drawing.Point(7, 66);
            this.radioButton_PointInsolation.Name = "radioButton_PointInsolation";
            this.radioButton_PointInsolation.Size = new System.Drawing.Size(55, 17);
            this.radioButton_PointInsolation.TabIndex = 10;
            this.radioButton_PointInsolation.Text = "Точки";
            this.radioButton_PointInsolation.UseVisualStyleBackColor = true;
            // 
            // radioButton_PanelInsolation
            // 
            this.radioButton_PanelInsolation.AutoSize = true;
            this.radioButton_PanelInsolation.Checked = true;
            this.radioButton_PanelInsolation.Location = new System.Drawing.Point(7, 20);
            this.radioButton_PanelInsolation.Name = "radioButton_PanelInsolation";
            this.radioButton_PanelInsolation.Size = new System.Drawing.Size(63, 17);
            this.radioButton_PanelInsolation.TabIndex = 9;
            this.radioButton_PanelInsolation.TabStop = true;
            this.radioButton_PanelInsolation.Text = "Панели";
            this.radioButton_PanelInsolation.UseVisualStyleBackColor = true;
            // 
            // InsolationAtPointForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 196);
            this.Controls.Add(this.groupBox_VerificationOptions);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.MaximumSize = new System.Drawing.Size(430, 235);
            this.MinimumSize = new System.Drawing.Size(270, 130);
            this.Name = "InsolationAtPointForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Инсоляция";
            this.groupBox_VerificationOptions.ResumeLayout(false);
            this.groupBox_VerificationOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.CheckBox checkBox_CheckSelectedPanels;
        private System.Windows.Forms.GroupBox groupBox_VerificationOptions;
        private System.Windows.Forms.CheckBox checkBox_CheckSelectedPoints;
        private System.Windows.Forms.RadioButton radioButton_PointInsolation;
        private System.Windows.Forms.RadioButton radioButton_PanelInsolation;
        private System.Windows.Forms.CheckBox checkBox_WallsAndFloorsGeometry;
    }
}