using Autodesk.Revit.DB;

namespace CITRUS
{
    partial class FormRebarOutletsCreator
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
            this.group_OutletSizes = new System.Windows.Forms.GroupBox();
            this.radioButton_SetValue = new System.Windows.Forms.RadioButton();
            this.radioButton_Auto = new System.Windows.Forms.RadioButton();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox_ForceType = new System.Windows.Forms.GroupBox();
            this.radioButton_Compression = new System.Windows.Forms.RadioButton();
            this.radioButton_Stretching = new System.Windows.Forms.RadioButton();
            this.textBox_ManualOverlapLength = new System.Windows.Forms.TextBox();
            this.textBox_ManualAnchorageLength = new System.Windows.Forms.TextBox();
            this.textBox_OffsetFromSlabBottom = new System.Windows.Forms.TextBox();
            this.label_RequiredField = new System.Windows.Forms.Label();
            this.comboBox_StirrupBarTapes = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox_ColumnArrangement = new System.Windows.Forms.GroupBox();
            this.radioButton_Project = new System.Windows.Forms.RadioButton();
            this.radioButton_Link = new System.Windows.Forms.RadioButton();
            this.group_OutletSizes.SuspendLayout();
            this.groupBox_ForceType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox_ColumnArrangement.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_OutletSizes
            // 
            this.group_OutletSizes.Controls.Add(this.radioButton_SetValue);
            this.group_OutletSizes.Controls.Add(this.radioButton_Auto);
            this.group_OutletSizes.Location = new System.Drawing.Point(15, 15);
            this.group_OutletSizes.Name = "group_OutletSizes";
            this.group_OutletSizes.Size = new System.Drawing.Size(140, 70);
            this.group_OutletSizes.TabIndex = 0;
            this.group_OutletSizes.TabStop = false;
            this.group_OutletSizes.Text = "Раазмеры выпусков";
            // 
            // radioButton_SetValue
            // 
            this.radioButton_SetValue.AutoSize = true;
            this.radioButton_SetValue.Location = new System.Drawing.Point(9, 42);
            this.radioButton_SetValue.Name = "radioButton_SetValue";
            this.radioButton_SetValue.Size = new System.Drawing.Size(61, 17);
            this.radioButton_SetValue.TabIndex = 1;
            this.radioButton_SetValue.Text = "Задать";
            this.radioButton_SetValue.UseVisualStyleBackColor = true;
            // 
            // radioButton_Auto
            // 
            this.radioButton_Auto.AutoSize = true;
            this.radioButton_Auto.Checked = true;
            this.radioButton_Auto.Location = new System.Drawing.Point(9, 19);
            this.radioButton_Auto.Name = "radioButton_Auto";
            this.radioButton_Auto.Size = new System.Drawing.Size(49, 17);
            this.radioButton_Auto.TabIndex = 0;
            this.radioButton_Auto.TabStop = true;
            this.radioButton_Auto.Text = "Авто";
            this.radioButton_Auto.UseVisualStyleBackColor = true;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(435, 335);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 1;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(564, 335);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // groupBox_ForceType
            // 
            this.groupBox_ForceType.Controls.Add(this.radioButton_Compression);
            this.groupBox_ForceType.Controls.Add(this.radioButton_Stretching);
            this.groupBox_ForceType.Location = new System.Drawing.Point(15, 91);
            this.groupBox_ForceType.Name = "groupBox_ForceType";
            this.groupBox_ForceType.Size = new System.Drawing.Size(140, 70);
            this.groupBox_ForceType.TabIndex = 3;
            this.groupBox_ForceType.TabStop = false;
            this.groupBox_ForceType.Text = "Тип усилия";
            // 
            // radioButton_Compression
            // 
            this.radioButton_Compression.AutoSize = true;
            this.radioButton_Compression.Location = new System.Drawing.Point(9, 43);
            this.radioButton_Compression.Name = "radioButton_Compression";
            this.radioButton_Compression.Size = new System.Drawing.Size(63, 17);
            this.radioButton_Compression.TabIndex = 1;
            this.radioButton_Compression.Text = "Сжатие";
            this.radioButton_Compression.UseVisualStyleBackColor = true;
            // 
            // radioButton_Stretching
            // 
            this.radioButton_Stretching.AutoSize = true;
            this.radioButton_Stretching.Checked = true;
            this.radioButton_Stretching.Location = new System.Drawing.Point(9, 19);
            this.radioButton_Stretching.Name = "radioButton_Stretching";
            this.radioButton_Stretching.Size = new System.Drawing.Size(87, 17);
            this.radioButton_Stretching.TabIndex = 0;
            this.radioButton_Stretching.TabStop = true;
            this.radioButton_Stretching.Text = "Растяжение";
            this.radioButton_Stretching.UseVisualStyleBackColor = true;
            // 
            // textBox_ManualOverlapLength
            // 
            this.textBox_ManualOverlapLength.Location = new System.Drawing.Point(558, 98);
            this.textBox_ManualOverlapLength.Name = "textBox_ManualOverlapLength";
            this.textBox_ManualOverlapLength.Size = new System.Drawing.Size(80, 20);
            this.textBox_ManualOverlapLength.TabIndex = 4;
            this.textBox_ManualOverlapLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_ManualOverlapLength.TextChanged += new System.EventHandler(this.textBox_ManualOverlapLength_TextChanged);
            // 
            // textBox_ManualAnchorageLength
            // 
            this.textBox_ManualAnchorageLength.Location = new System.Drawing.Point(558, 228);
            this.textBox_ManualAnchorageLength.Name = "textBox_ManualAnchorageLength";
            this.textBox_ManualAnchorageLength.Size = new System.Drawing.Size(80, 20);
            this.textBox_ManualAnchorageLength.TabIndex = 5;
            this.textBox_ManualAnchorageLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_ManualAnchorageLength.TextChanged += new System.EventHandler(this.textBox_ManualAnchorageLength_TextChanged);
            // 
            // textBox_OffsetFromSlabBottom
            // 
            this.textBox_OffsetFromSlabBottom.Location = new System.Drawing.Point(195, 261);
            this.textBox_OffsetFromSlabBottom.Name = "textBox_OffsetFromSlabBottom";
            this.textBox_OffsetFromSlabBottom.Size = new System.Drawing.Size(50, 20);
            this.textBox_OffsetFromSlabBottom.TabIndex = 6;
            this.textBox_OffsetFromSlabBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_OffsetFromSlabBottom.TextChanged += new System.EventHandler(this.textBox_OffsetFromSlabBottom_TextChanged);
            // 
            // label_RequiredField
            // 
            this.label_RequiredField.AutoSize = true;
            this.label_RequiredField.BackColor = System.Drawing.SystemColors.Window;
            this.label_RequiredField.ForeColor = System.Drawing.Color.Red;
            this.label_RequiredField.Location = new System.Drawing.Point(172, 299);
            this.label_RequiredField.Name = "label_RequiredField";
            this.label_RequiredField.Size = new System.Drawing.Size(193, 13);
            this.label_RequiredField.TabIndex = 7;
            this.label_RequiredField.Text = "Укажите смещение для растяжения";
            // 
            // comboBox_StirrupBarTapes
            // 
            this.comboBox_StirrupBarTapes.FormattingEnabled = true;
            this.comboBox_StirrupBarTapes.Location = new System.Drawing.Point(431, 132);
            this.comboBox_StirrupBarTapes.Name = "comboBox_StirrupBarTapes";
            this.comboBox_StirrupBarTapes.Size = new System.Drawing.Size(100, 21);
            this.comboBox_StirrupBarTapes.Sorted = true;
            this.comboBox_StirrupBarTapes.TabIndex = 8;
            this.comboBox_StirrupBarTapes.SelectedIndexChanged += new System.EventHandler(this.comboBox_StirrupBarTapes_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CITRUS.Properties.Resources.RebarOutletsCreatorSketch;
            this.pictureBox1.Location = new System.Drawing.Point(168, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 301);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox_ColumnArrangement
            // 
            this.groupBox_ColumnArrangement.Controls.Add(this.radioButton_Project);
            this.groupBox_ColumnArrangement.Controls.Add(this.radioButton_Link);
            this.groupBox_ColumnArrangement.Location = new System.Drawing.Point(15, 241);
            this.groupBox_ColumnArrangement.Name = "groupBox_ColumnArrangement";
            this.groupBox_ColumnArrangement.Size = new System.Drawing.Size(140, 75);
            this.groupBox_ColumnArrangement.TabIndex = 10;
            this.groupBox_ColumnArrangement.TabStop = false;
            this.groupBox_ColumnArrangement.Text = "Расположение колонн:";
            // 
            // radioButton_Project
            // 
            this.radioButton_Project.AutoSize = true;
            this.radioButton_Project.Location = new System.Drawing.Point(9, 44);
            this.radioButton_Project.Name = "radioButton_Project";
            this.radioButton_Project.Size = new System.Drawing.Size(62, 17);
            this.radioButton_Project.TabIndex = 1;
            this.radioButton_Project.Text = "Проект";
            this.radioButton_Project.UseVisualStyleBackColor = true;
            // 
            // radioButton_Link
            // 
            this.radioButton_Link.AutoSize = true;
            this.radioButton_Link.Checked = true;
            this.radioButton_Link.Location = new System.Drawing.Point(9, 20);
            this.radioButton_Link.Name = "radioButton_Link";
            this.radioButton_Link.Size = new System.Drawing.Size(56, 17);
            this.radioButton_Link.TabIndex = 0;
            this.radioButton_Link.TabStop = true;
            this.radioButton_Link.Text = "Связь";
            this.radioButton_Link.UseVisualStyleBackColor = true;
            // 
            // FormRebarOutletsCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 376);
            this.Controls.Add(this.groupBox_ColumnArrangement);
            this.Controls.Add(this.comboBox_StirrupBarTapes);
            this.Controls.Add(this.label_RequiredField);
            this.Controls.Add(this.textBox_OffsetFromSlabBottom);
            this.Controls.Add(this.textBox_ManualAnchorageLength);
            this.Controls.Add(this.textBox_ManualOverlapLength);
            this.Controls.Add(this.groupBox_ForceType);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.group_OutletSizes);
            this.Controls.Add(this.pictureBox1);
            this.MaximumSize = new System.Drawing.Size(700, 415);
            this.MinimumSize = new System.Drawing.Size(700, 415);
            this.Name = "FormRebarOutletsCreator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выпуски";
            this.group_OutletSizes.ResumeLayout(false);
            this.group_OutletSizes.PerformLayout();
            this.groupBox_ForceType.ResumeLayout(false);
            this.groupBox_ForceType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox_ColumnArrangement.ResumeLayout(false);
            this.groupBox_ColumnArrangement.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox group_OutletSizes;
        private System.Windows.Forms.RadioButton radioButton_SetValue;
        private System.Windows.Forms.RadioButton radioButton_Auto;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.GroupBox groupBox_ForceType;
        private System.Windows.Forms.RadioButton radioButton_Compression;
        private System.Windows.Forms.RadioButton radioButton_Stretching;
        private System.Windows.Forms.TextBox textBox_ManualOverlapLength;
        private System.Windows.Forms.TextBox textBox_ManualAnchorageLength;
        private System.Windows.Forms.TextBox textBox_OffsetFromSlabBottom;
        private System.Windows.Forms.Label label_RequiredField;
        private System.Windows.Forms.ComboBox comboBox_StirrupBarTapes;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox_ColumnArrangement;
        private System.Windows.Forms.RadioButton radioButton_Project;
        private System.Windows.Forms.RadioButton radioButton_Link;
    }
}