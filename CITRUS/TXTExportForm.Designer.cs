namespace CITRUS
{
    partial class TXTExportForm
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
            this.listBox_ViewSchedules = new System.Windows.Forms.ListBox();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_FolderBrowserDialog = new System.Windows.Forms.Button();
            this.richTextBox_FilePach = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox_ViewSchedules
            // 
            this.listBox_ViewSchedules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_ViewSchedules.FormattingEnabled = true;
            this.listBox_ViewSchedules.Location = new System.Drawing.Point(12, 116);
            this.listBox_ViewSchedules.Name = "listBox_ViewSchedules";
            this.listBox_ViewSchedules.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_ViewSchedules.Size = new System.Drawing.Size(310, 186);
            this.listBox_ViewSchedules.Sorted = true;
            this.listBox_ViewSchedules.TabIndex = 1;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(93, 324);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 2;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(222, 324);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_FolderBrowserDialog
            // 
            this.btn_FolderBrowserDialog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_FolderBrowserDialog.Location = new System.Drawing.Point(12, 12);
            this.btn_FolderBrowserDialog.Name = "btn_FolderBrowserDialog";
            this.btn_FolderBrowserDialog.Size = new System.Drawing.Size(310, 23);
            this.btn_FolderBrowserDialog.TabIndex = 4;
            this.btn_FolderBrowserDialog.Text = "Выберите папку для сохранения txt";
            this.btn_FolderBrowserDialog.UseVisualStyleBackColor = true;
            this.btn_FolderBrowserDialog.Click += new System.EventHandler(this.btn_FolderBrowserDialog_Click);
            // 
            // richTextBox_FilePach
            // 
            this.richTextBox_FilePach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_FilePach.Location = new System.Drawing.Point(12, 41);
            this.richTextBox_FilePach.Name = "richTextBox_FilePach";
            this.richTextBox_FilePach.Size = new System.Drawing.Size(310, 40);
            this.richTextBox_FilePach.TabIndex = 5;
            this.richTextBox_FilePach.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Выберите одну или несколько спецификаций:";
            // 
            // TXTExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 361);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox_FilePach);
            this.Controls.Add(this.btn_FolderBrowserDialog);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.listBox_ViewSchedules);
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "TXTExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор спецификаций";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBox_ViewSchedules;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btn_FolderBrowserDialog;
        private System.Windows.Forms.RichTextBox richTextBox_FilePach;
        private System.Windows.Forms.Label label1;
    }
}