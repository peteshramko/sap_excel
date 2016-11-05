namespace iiiwave.MatManLib
{
    partial class RefEditControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxIsAddressCheckBox = new System.Windows.Forms.CheckBox();
            this.uxAddressTextBox = new System.Windows.Forms.TextBox();
            this.uxRefEditButton = new System.Windows.Forms.Button();
            this.uxLovButton = new System.Windows.Forms.Button();
            this.uxParameterLabel = new System.Windows.Forms.Label();
            this.uxValueTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // uxIsAddressCheckBox
            // 
            this.uxIsAddressCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxIsAddressCheckBox.Location = new System.Drawing.Point(8, 61);
            this.uxIsAddressCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxIsAddressCheckBox.Name = "uxIsAddressCheckBox";
            this.uxIsAddressCheckBox.Size = new System.Drawing.Size(150, 31);
            this.uxIsAddressCheckBox.TabIndex = 23;
            this.uxIsAddressCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxIsAddressCheckBox.UseVisualStyleBackColor = true;
            // 
            // uxAddressTextBox
            // 
            this.uxAddressTextBox.Enabled = false;
            this.uxAddressTextBox.Location = new System.Drawing.Point(164, 63);
            this.uxAddressTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxAddressTextBox.Name = "uxAddressTextBox";
            this.uxAddressTextBox.Size = new System.Drawing.Size(199, 26);
            this.uxAddressTextBox.TabIndex = 22;
            // 
            // uxRefEditButton
            // 
            this.uxRefEditButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.uxRefEditButton.Location = new System.Drawing.Point(372, 61);
            this.uxRefEditButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxRefEditButton.Name = "uxRefEditButton";
            this.uxRefEditButton.Size = new System.Drawing.Size(30, 30);
            this.uxRefEditButton.TabIndex = 20;
            this.uxRefEditButton.UseVisualStyleBackColor = true;
            // 
            // uxLovButton
            // 
            this.uxLovButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.uxLovButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uxLovButton.ForeColor = System.Drawing.Color.White;
            this.uxLovButton.Location = new System.Drawing.Point(377, 7);
            this.uxLovButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxLovButton.Name = "uxLovButton";
            this.uxLovButton.Size = new System.Drawing.Size(26, 26);
            this.uxLovButton.TabIndex = 19;
            this.uxLovButton.UseVisualStyleBackColor = true;
            // 
            // uxParameterLabel
            // 
            this.uxParameterLabel.Location = new System.Drawing.Point(10, 9);
            this.uxParameterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxParameterLabel.Name = "uxParameterLabel";
            this.uxParameterLabel.Size = new System.Drawing.Size(150, 49);
            this.uxParameterLabel.TabIndex = 18;
            this.uxParameterLabel.Text = "label1";
            this.uxParameterLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // uxValueTextBox
            // 
            this.uxValueTextBox.Location = new System.Drawing.Point(164, 7);
            this.uxValueTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxValueTextBox.Name = "uxValueTextBox";
            this.uxValueTextBox.Size = new System.Drawing.Size(238, 26);
            this.uxValueTextBox.TabIndex = 21;
            // 
            // RefEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxIsAddressCheckBox);
            this.Controls.Add(this.uxAddressTextBox);
            this.Controls.Add(this.uxRefEditButton);
            this.Controls.Add(this.uxLovButton);
            this.Controls.Add(this.uxParameterLabel);
            this.Controls.Add(this.uxValueTextBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "RefEditControl";
            this.Size = new System.Drawing.Size(410, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox uxIsAddressCheckBox;
        private System.Windows.Forms.TextBox uxAddressTextBox;
        private System.Windows.Forms.Button uxRefEditButton;
        private System.Windows.Forms.Button uxLovButton;
        private System.Windows.Forms.Label uxParameterLabel;
        private System.Windows.Forms.TextBox uxValueTextBox;
    }
}
