namespace CITRUS.CIT_04_7_ElementsTransfer
{
    partial class CIT_04_7_ElementsTransferForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_FoundatioTransfer = new System.Windows.Forms.CheckBox();
            this.checkBox_WallTransfer = new System.Windows.Forms.CheckBox();
            this.checkBox_BeamTransfer = new System.Windows.Forms.CheckBox();
            this.checkBox_СolumnTransfer = new System.Windows.Forms.CheckBox();
            this.checkBox_FloorTransfer = new System.Windows.Forms.CheckBox();
            this.checkBox_ReplaceBeamType = new System.Windows.Forms.CheckBox();
            this.checkBox_ReplaceWallType = new System.Windows.Forms.CheckBox();
            this.checkBox_ReplaceСolumnType = new System.Windows.Forms.CheckBox();
            this.checkBox_ReplaceFloorType = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(192, 274);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(63, 274);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 8;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_FoundatioTransfer);
            this.groupBox1.Controls.Add(this.checkBox_WallTransfer);
            this.groupBox1.Controls.Add(this.checkBox_BeamTransfer);
            this.groupBox1.Controls.Add(this.checkBox_СolumnTransfer);
            this.groupBox1.Controls.Add(this.checkBox_FloorTransfer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 133);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Перенести элементы:";
            // 
            // checkBox_FoundatioTransfer
            // 
            this.checkBox_FoundatioTransfer.AutoSize = true;
            this.checkBox_FoundatioTransfer.Location = new System.Drawing.Point(6, 111);
            this.checkBox_FoundatioTransfer.Name = "checkBox_FoundatioTransfer";
            this.checkBox_FoundatioTransfer.Size = new System.Drawing.Size(93, 17);
            this.checkBox_FoundatioTransfer.TabIndex = 4;
            this.checkBox_FoundatioTransfer.Text = "Фундаменты";
            this.checkBox_FoundatioTransfer.UseVisualStyleBackColor = true;
            // 
            // checkBox_WallTransfer
            // 
            this.checkBox_WallTransfer.AutoSize = true;
            this.checkBox_WallTransfer.Location = new System.Drawing.Point(6, 65);
            this.checkBox_WallTransfer.Name = "checkBox_WallTransfer";
            this.checkBox_WallTransfer.Size = new System.Drawing.Size(58, 17);
            this.checkBox_WallTransfer.TabIndex = 3;
            this.checkBox_WallTransfer.Text = "Стены";
            this.checkBox_WallTransfer.UseVisualStyleBackColor = true;
            // 
            // checkBox_BeamTransfer
            // 
            this.checkBox_BeamTransfer.AutoSize = true;
            this.checkBox_BeamTransfer.Location = new System.Drawing.Point(6, 88);
            this.checkBox_BeamTransfer.Name = "checkBox_BeamTransfer";
            this.checkBox_BeamTransfer.Size = new System.Drawing.Size(57, 17);
            this.checkBox_BeamTransfer.TabIndex = 2;
            this.checkBox_BeamTransfer.Text = "Балки";
            this.checkBox_BeamTransfer.UseVisualStyleBackColor = true;
            // 
            // checkBox_СolumnTransfer
            // 
            this.checkBox_СolumnTransfer.AutoSize = true;
            this.checkBox_СolumnTransfer.Location = new System.Drawing.Point(6, 42);
            this.checkBox_СolumnTransfer.Name = "checkBox_СolumnTransfer";
            this.checkBox_СolumnTransfer.Size = new System.Drawing.Size(71, 17);
            this.checkBox_СolumnTransfer.TabIndex = 1;
            this.checkBox_СolumnTransfer.Text = "Колонны";
            this.checkBox_СolumnTransfer.UseVisualStyleBackColor = true;
            // 
            // checkBox_FloorTransfer
            // 
            this.checkBox_FloorTransfer.AutoSize = true;
            this.checkBox_FloorTransfer.Location = new System.Drawing.Point(6, 19);
            this.checkBox_FloorTransfer.Name = "checkBox_FloorTransfer";
            this.checkBox_FloorTransfer.Size = new System.Drawing.Size(89, 17);
            this.checkBox_FloorTransfer.TabIndex = 0;
            this.checkBox_FloorTransfer.Text = "Перекрытия";
            this.checkBox_FloorTransfer.UseVisualStyleBackColor = true;
            // 
            // checkBox_ReplaceBeamType
            // 
            this.checkBox_ReplaceBeamType.AutoSize = true;
            this.checkBox_ReplaceBeamType.Location = new System.Drawing.Point(6, 88);
            this.checkBox_ReplaceBeamType.Name = "checkBox_ReplaceBeamType";
            this.checkBox_ReplaceBeamType.Size = new System.Drawing.Size(57, 17);
            this.checkBox_ReplaceBeamType.TabIndex = 8;
            this.checkBox_ReplaceBeamType.Text = "Балки";
            this.checkBox_ReplaceBeamType.UseVisualStyleBackColor = true;
            // 
            // checkBox_ReplaceWallType
            // 
            this.checkBox_ReplaceWallType.AutoSize = true;
            this.checkBox_ReplaceWallType.Location = new System.Drawing.Point(6, 65);
            this.checkBox_ReplaceWallType.Name = "checkBox_ReplaceWallType";
            this.checkBox_ReplaceWallType.Size = new System.Drawing.Size(58, 17);
            this.checkBox_ReplaceWallType.TabIndex = 7;
            this.checkBox_ReplaceWallType.Text = "Стены";
            this.checkBox_ReplaceWallType.UseVisualStyleBackColor = true;
            // 
            // checkBox_ReplaceСolumnType
            // 
            this.checkBox_ReplaceСolumnType.AutoSize = true;
            this.checkBox_ReplaceСolumnType.Location = new System.Drawing.Point(6, 42);
            this.checkBox_ReplaceСolumnType.Name = "checkBox_ReplaceСolumnType";
            this.checkBox_ReplaceСolumnType.Size = new System.Drawing.Size(71, 17);
            this.checkBox_ReplaceСolumnType.TabIndex = 6;
            this.checkBox_ReplaceСolumnType.Text = "Колонны";
            this.checkBox_ReplaceСolumnType.UseVisualStyleBackColor = true;
            // 
            // checkBox_ReplaceFloorType
            // 
            this.checkBox_ReplaceFloorType.AutoSize = true;
            this.checkBox_ReplaceFloorType.Location = new System.Drawing.Point(6, 19);
            this.checkBox_ReplaceFloorType.Name = "checkBox_ReplaceFloorType";
            this.checkBox_ReplaceFloorType.Size = new System.Drawing.Size(89, 17);
            this.checkBox_ReplaceFloorType.TabIndex = 5;
            this.checkBox_ReplaceFloorType.Text = "Перекрытия";
            this.checkBox_ReplaceFloorType.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_ReplaceFloorType);
            this.groupBox2.Controls.Add(this.checkBox_ReplaceBeamType);
            this.groupBox2.Controls.Add(this.checkBox_ReplaceСolumnType);
            this.groupBox2.Controls.Add(this.checkBox_ReplaceWallType);
            this.groupBox2.Location = new System.Drawing.Point(12, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 110);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Заменить тип элемента:";
            // 
            // CIT_04_7_ElementsTransferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 311);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.MaximumSize = new System.Drawing.Size(320, 350);
            this.MinimumSize = new System.Drawing.Size(320, 350);
            this.Name = "CIT_04_7_ElementsTransferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Свойства";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_СolumnTransfer;
        private System.Windows.Forms.CheckBox checkBox_FloorTransfer;
        private System.Windows.Forms.CheckBox checkBox_BeamTransfer;
        private System.Windows.Forms.CheckBox checkBox_WallTransfer;
        private System.Windows.Forms.CheckBox checkBox_FoundatioTransfer;
        private System.Windows.Forms.CheckBox checkBox_ReplaceFloorType;
        private System.Windows.Forms.CheckBox checkBox_ReplaceСolumnType;
        private System.Windows.Forms.CheckBox checkBox_ReplaceWallType;
        private System.Windows.Forms.CheckBox checkBox_ReplaceBeamType;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}