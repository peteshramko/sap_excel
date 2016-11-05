namespace iiiwave.MatManLib
{
    partial class UserOptionsForm
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
            this.uxBatchSizeGroupBox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // uxBatchSizeGroupBox
            // 
            this.uxBatchSizeGroupBox.Location = new System.Drawing.Point(13, 13);
            this.uxBatchSizeGroupBox.Name = "uxBatchSizeGroupBox";
            this.uxBatchSizeGroupBox.Size = new System.Drawing.Size(386, 80);
            this.uxBatchSizeGroupBox.TabIndex = 0;
            this.uxBatchSizeGroupBox.TabStop = false;
            // 
            // UserOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 363);
            this.Controls.Add(this.uxBatchSizeGroupBox);
            this.Name = "UserOptionsForm";
            this.Text = "UserOptionsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox uxBatchSizeGroupBox;
    }
}