namespace CITRUS
{
    partial class FloorTypeSelectorForCapitalMaker
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_FloorTypesForCapitalMaker = new System.Windows.Forms.ComboBox();
            this.button1_Ok = new System.Windows.Forms.Button();
            this.button2_Cancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1_Offset = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2_Offset = new System.Windows.Forms.TextBox();
            this.textBoxY2_Offset = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxX2_Offset = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Выберите тип перекрытия из списка для создания капители:";
            // 
            // comboBox_FloorTypesForCapitalMaker
            // 
            this.comboBox_FloorTypesForCapitalMaker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_FloorTypesForCapitalMaker.FormattingEnabled = true;
            this.comboBox_FloorTypesForCapitalMaker.Location = new System.Drawing.Point(15, 35);
            this.comboBox_FloorTypesForCapitalMaker.Name = "comboBox_FloorTypesForCapitalMaker";
            this.comboBox_FloorTypesForCapitalMaker.Size = new System.Drawing.Size(407, 21);
            this.comboBox_FloorTypesForCapitalMaker.TabIndex = 3;
            this.comboBox_FloorTypesForCapitalMaker.SelectedIndexChanged += new System.EventHandler(this.comboBox_FloorTypesForCapitalMaker_SelectedIndexChanged);
            // 
            // button1_Ok
            // 
            this.button1_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1_Ok.Location = new System.Drawing.Point(50, 320);
            this.button1_Ok.Name = "button1_Ok";
            this.button1_Ok.Size = new System.Drawing.Size(100, 25);
            this.button1_Ok.TabIndex = 4;
            this.button1_Ok.Text = "Ок";
            this.button1_Ok.UseVisualStyleBackColor = true;
            this.button1_Ok.Click += new System.EventHandler(this.button1_Ok_Click);
            // 
            // button2_Cancel
            // 
            this.button2_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2_Cancel.Location = new System.Drawing.Point(300, 320);
            this.button2_Cancel.Name = "button2_Cancel";
            this.button2_Cancel.Size = new System.Drawing.Size(100, 25);
            this.button2_Cancel.TabIndex = 5;
            this.button2_Cancel.Text = "Отмена";
            this.button2_Cancel.UseVisualStyleBackColor = true;
            this.button2_Cancel.Click += new System.EventHandler(this.button2_Cancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Укажите значение смещения границы капители от центра колонны, мм:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::CITRUS.Properties.Resources.CapitalMaker_X;
            this.pictureBox1.Location = new System.Drawing.Point(240, 100);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(180, 180);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(34, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "X1=";
            // 
            // textBox1_Offset
            // 
            this.textBox1_Offset.Location = new System.Drawing.Point(77, 110);
            this.textBox1_Offset.Name = "textBox1_Offset";
            this.textBox1_Offset.Size = new System.Drawing.Size(100, 20);
            this.textBox1_Offset.TabIndex = 9;
            this.textBox1_Offset.TextChanged += new System.EventHandler(this.textBox1_Offset_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(34, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Y1=";
            // 
            // textBox2_Offset
            // 
            this.textBox2_Offset.Location = new System.Drawing.Point(77, 198);
            this.textBox2_Offset.Name = "textBox2_Offset";
            this.textBox2_Offset.Size = new System.Drawing.Size(100, 20);
            this.textBox2_Offset.TabIndex = 11;
            this.textBox2_Offset.TextChanged += new System.EventHandler(this.textBox2_Offset_TextChanged);
            // 
            // textBoxY2_Offset
            // 
            this.textBoxY2_Offset.Location = new System.Drawing.Point(77, 244);
            this.textBoxY2_Offset.Name = "textBoxY2_Offset";
            this.textBoxY2_Offset.Size = new System.Drawing.Size(100, 20);
            this.textBoxY2_Offset.TabIndex = 15;
            this.textBoxY2_Offset.TextChanged += new System.EventHandler(this.textBoxY2_Offset_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(34, 244);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "Y2=";
            // 
            // textBoxX2_Offset
            // 
            this.textBoxX2_Offset.Location = new System.Drawing.Point(77, 153);
            this.textBoxX2_Offset.Name = "textBoxX2_Offset";
            this.textBoxX2_Offset.Size = new System.Drawing.Size(100, 20);
            this.textBoxX2_Offset.TabIndex = 13;
            this.textBoxX2_Offset.TextChanged += new System.EventHandler(this.textBoxX2_Offset_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(34, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "X2=";
            // 
            // FloorTypeSelectorForCapitalMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 361);
            this.Controls.Add(this.textBoxY2_Offset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxX2_Offset);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2_Offset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1_Offset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2_Cancel);
            this.Controls.Add(this.button1_Ok);
            this.Controls.Add(this.comboBox_FloorTypesForCapitalMaker);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(450, 400);
            this.MinimumSize = new System.Drawing.Size(450, 400);
            this.Name = "FloorTypeSelectorForCapitalMaker";
            this.Text = "Выбератор типа капители";
            this.Load += new System.EventHandler(this.FloorTypeSelectorForCapitalMaker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_FloorTypesForCapitalMaker;
        private System.Windows.Forms.Button button1_Ok;
        private System.Windows.Forms.Button button2_Cancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1_Offset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2_Offset;
        private System.Windows.Forms.TextBox textBoxY2_Offset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxX2_Offset;
        private System.Windows.Forms.Label label6;
    }
}