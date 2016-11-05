namespace iiiwave.MatManLib
{
    partial class DropDownRefControl
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
            this.uxRefEditButton = new System.Windows.Forms.Button();
            this.uxAddressTextBox = new System.Windows.Forms.TextBox();
            this.uxValueDropDownList = new System.Windows.Forms.ComboBox();
            this.uxParameterLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uxIsAddressCheckBox
            // 
            this.uxIsAddressCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxIsAddressCheckBox.Location = new System.Drawing.Point(4, 61);
            this.uxIsAddressCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxIsAddressCheckBox.Name = "uxIsAddressCheckBox";
            this.uxIsAddressCheckBox.Size = new System.Drawing.Size(150, 31);
            this.uxIsAddressCheckBox.TabIndex = 18;
            this.uxIsAddressCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxIsAddressCheckBox.UseVisualStyleBackColor = true;
            // 
            // uxRefEditButton
            // 
            this.uxRefEditButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.uxRefEditButton.Location = new System.Drawing.Point(370, 60);
            this.uxRefEditButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxRefEditButton.Name = "uxRefEditButton";
            this.uxRefEditButton.Size = new System.Drawing.Size(30, 31);
            this.uxRefEditButton.TabIndex = 17;
            this.uxRefEditButton.UseVisualStyleBackColor = false;
            // 
            // uxAddressTextBox
            // 
            this.uxAddressTextBox.Enabled = false;
            this.uxAddressTextBox.Location = new System.Drawing.Point(160, 62);
            this.uxAddressTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxAddressTextBox.Name = "uxAddressTextBox";
            this.uxAddressTextBox.Size = new System.Drawing.Size(202, 26);
            this.uxAddressTextBox.TabIndex = 16;
            // 
            // uxValueDropDownList
            // 
            this.uxValueDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxValueDropDownList.FormattingEnabled = true;
            this.uxValueDropDownList.Location = new System.Drawing.Point(160, 5);
            this.uxValueDropDownList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxValueDropDownList.Name = "uxValueDropDownList";
            this.uxValueDropDownList.Size = new System.Drawing.Size(238, 28);
            this.uxValueDropDownList.TabIndex = 15;
            // 
            // uxParameterLabel
            // 
            this.uxParameterLabel.Location = new System.Drawing.Point(6, 8);
            this.uxParameterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxParameterLabel.Name = "uxParameterLabel";
            this.uxParameterLabel.Size = new System.Drawing.Size(150, 49);
            this.uxParameterLabel.TabIndex = 14;
            this.uxParameterLabel.Text = "label1";
            this.uxParameterLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DropDownRefControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxIsAddressCheckBox);
            this.Controls.Add(this.uxRefEditButton);
            this.Controls.Add(this.uxAddressTextBox);
            this.Controls.Add(this.uxValueDropDownList);
            this.Controls.Add(this.uxParameterLabel);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "DropDownRefControl";
            this.Size = new System.Drawing.Size(405, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox uxIsAddressCheckBox;
        private System.Windows.Forms.Button uxRefEditButton;
        private System.Windows.Forms.TextBox uxAddressTextBox;
        private System.Windows.Forms.ComboBox uxValueDropDownList;
        private System.Windows.Forms.Label uxParameterLabel;
    }
}
