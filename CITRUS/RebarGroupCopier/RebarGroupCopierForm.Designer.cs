namespace CITRUS.RebarGroupCopier
{
    partial class RebarGroupCopierForm
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
            this.groupBox_GroupTypes = new System.Windows.Forms.GroupBox();
            this.groupBox_ColumnArrangement = new System.Windows.Forms.GroupBox();
            this.radioButton_Project = new System.Windows.Forms.RadioButton();
            this.radioButton_Link = new System.Windows.Forms.RadioButton();
            this.radioButton_OutletsGroups = new System.Windows.Forms.RadioButton();
            this.radioButton_ColumnGroups = new System.Windows.Forms.RadioButton();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox_GroupTypes.SuspendLayout();
            this.groupBox_ColumnArrangement.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_GroupTypes
            // 
            this.groupBox_GroupTypes.Controls.Add(this.groupBox_ColumnArrangement);
            this.groupBox_GroupTypes.Controls.Add(this.radioButton_OutletsGroups);
            this.groupBox_GroupTypes.Controls.Add(this.radioButton_ColumnGroups);
            this.groupBox_GroupTypes.Location = new System.Drawing.Point(13, 13);
            this.groupBox_GroupTypes.Name = "groupBox_GroupTypes";
            this.groupBox_GroupTypes.Size = new System.Drawing.Size(259, 146);
            this.groupBox_GroupTypes.TabIndex = 0;
            this.groupBox_GroupTypes.TabStop = false;
            this.groupBox_GroupTypes.Text = "Выберите тип:";
            // 
            // groupBox_ColumnArrangement
            // 
            this.groupBox_ColumnArrangement.Controls.Add(this.radioButton_Project);
            this.groupBox_ColumnArrangement.Controls.Add(this.radioButton_Link);
            this.groupBox_ColumnArrangement.Location = new System.Drawing.Point(13, 69);
            this.groupBox_ColumnArrangement.Name = "groupBox_ColumnArrangement";
            this.groupBox_ColumnArrangement.Size = new System.Drawing.Size(233, 68);
            this.groupBox_ColumnArrangement.TabIndex = 11;
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
            // radioButton_OutletsGroups
            // 
            this.radioButton_OutletsGroups.AutoSize = true;
            this.radioButton_OutletsGroups.Location = new System.Drawing.Point(7, 46);
            this.radioButton_OutletsGroups.Name = "radioButton_OutletsGroups";
            this.radioButton_OutletsGroups.Size = new System.Drawing.Size(178, 17);
            this.radioButton_OutletsGroups.TabIndex = 1;
            this.radioButton_OutletsGroups.TabStop = true;
            this.radioButton_OutletsGroups.Text = "Группы арматурных выпусков";
            this.radioButton_OutletsGroups.UseVisualStyleBackColor = true;
            // 
            // radioButton_ColumnGroups
            // 
            this.radioButton_ColumnGroups.AutoSize = true;
            this.radioButton_ColumnGroups.Location = new System.Drawing.Point(7, 19);
            this.radioButton_ColumnGroups.Name = "radioButton_ColumnGroups";
            this.radioButton_ColumnGroups.Size = new System.Drawing.Size(172, 17);
            this.radioButton_ColumnGroups.TabIndex = 0;
            this.radioButton_ColumnGroups.TabStop = true;
            this.radioButton_ColumnGroups.Text = "Группы армирования колонн";
            this.radioButton_ColumnGroups.UseVisualStyleBackColor = true;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(13, 169);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 1;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(172, 169);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // RebarGroupCopierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 203);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.groupBox_GroupTypes);
            this.Name = "RebarGroupCopierForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Копирователь групп";
            this.groupBox_GroupTypes.ResumeLayout(false);
            this.groupBox_GroupTypes.PerformLayout();
            this.groupBox_ColumnArrangement.ResumeLayout(false);
            this.groupBox_ColumnArrangement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_GroupTypes;
        private System.Windows.Forms.RadioButton radioButton_OutletsGroups;
        private System.Windows.Forms.RadioButton radioButton_ColumnGroups;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.GroupBox groupBox_ColumnArrangement;
        private System.Windows.Forms.RadioButton radioButton_Project;
        private System.Windows.Forms.RadioButton radioButton_Link;
    }
}