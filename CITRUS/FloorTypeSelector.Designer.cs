namespace CITRUS
{
    partial class FloorTypeSelector
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
            this.comboBox_FloorTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1_Ok = new System.Windows.Forms.Button();
            this.button2_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_FloorTypes
            // 
            this.comboBox_FloorTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_FloorTypes.FormattingEnabled = true;
            this.comboBox_FloorTypes.Location = new System.Drawing.Point(15, 35);
            this.comboBox_FloorTypes.Name = "comboBox_FloorTypes";
            this.comboBox_FloorTypes.Size = new System.Drawing.Size(357, 21);
            this.comboBox_FloorTypes.Sorted = true;
            this.comboBox_FloorTypes.TabIndex = 0;
            this.comboBox_FloorTypes.SelectedIndexChanged += new System.EventHandler(this.comboBox_FloorTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите тип пола из списка:";
            // 
            // button1_Ok
            // 
            this.button1_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1_Ok.Location = new System.Drawing.Point(50, 120);
            this.button1_Ok.Name = "button1_Ok";
            this.button1_Ok.Size = new System.Drawing.Size(100, 25);
            this.button1_Ok.TabIndex = 2;
            this.button1_Ok.Text = "Ок";
            this.button1_Ok.UseVisualStyleBackColor = true;
            this.button1_Ok.Click += new System.EventHandler(this.button1_Ok_Click);
            // 
            // button2_Cancel
            // 
            this.button2_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2_Cancel.Location = new System.Drawing.Point(250, 120);
            this.button2_Cancel.Name = "button2_Cancel";
            this.button2_Cancel.Size = new System.Drawing.Size(100, 25);
            this.button2_Cancel.TabIndex = 3;
            this.button2_Cancel.Text = "Отмена";
            this.button2_Cancel.UseVisualStyleBackColor = true;
            this.button2_Cancel.Click += new System.EventHandler(this.button2_Cancel_Click);
            // 
            // FloorTypeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.button2_Cancel);
            this.Controls.Add(this.button1_Ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_FloorTypes);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FloorTypeSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Пол";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_FloorTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1_Ok;
        private System.Windows.Forms.Button button2_Cancel;
    }
}