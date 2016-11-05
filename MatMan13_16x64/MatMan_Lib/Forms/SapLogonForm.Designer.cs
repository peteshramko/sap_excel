namespace iiiwave.MatManLib
{
    partial class SapLogonForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uxPasswordTextBox = new System.Windows.Forms.TextBox();
            this.uxPasswordLabel = new System.Windows.Forms.Label();
            this.uxUserNameTextBox = new System.Windows.Forms.TextBox();
            this.uxUserNameLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.uxHostLabel = new System.Windows.Forms.Label();
            this.uxSystemHostTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.uxSystemIDTextBox = new System.Windows.Forms.TextBox();
            this.uxSystemNumberTextBox = new System.Windows.Forms.TextBox();
            this.uxSystemNumberLabel = new System.Windows.Forms.Label();
            this.uxClientLabel = new System.Windows.Forms.Label();
            this.uxClientTextBox = new System.Windows.Forms.TextBox();
            this.uxCancelButton = new System.Windows.Forms.Button();
            this.uxLogonButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.uxLanguageTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.uxPasswordTextBox);
            this.groupBox1.Controls.Add(this.uxPasswordLabel);
            this.groupBox1.Controls.Add(this.uxUserNameTextBox);
            this.groupBox1.Controls.Add(this.uxUserNameLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(436, 125);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "User Credentials";
            // 
            // uxPasswordTextBox
            // 
            this.uxPasswordTextBox.Location = new System.Drawing.Point(190, 71);
            this.uxPasswordTextBox.Name = "uxPasswordTextBox";
            this.uxPasswordTextBox.Size = new System.Drawing.Size(220, 26);
            this.uxPasswordTextBox.TabIndex = 3;
            // 
            // uxPasswordLabel
            // 
            this.uxPasswordLabel.Location = new System.Drawing.Point(10, 71);
            this.uxPasswordLabel.Name = "uxPasswordLabel";
            this.uxPasswordLabel.Size = new System.Drawing.Size(175, 26);
            this.uxPasswordLabel.TabIndex = 2;
            this.uxPasswordLabel.Text = "Password:";
            this.uxPasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxUserNameTextBox
            // 
            this.uxUserNameTextBox.Location = new System.Drawing.Point(190, 35);
            this.uxUserNameTextBox.Name = "uxUserNameTextBox";
            this.uxUserNameTextBox.Size = new System.Drawing.Size(220, 26);
            this.uxUserNameTextBox.TabIndex = 1;
            // 
            // uxUserNameLabel
            // 
            this.uxUserNameLabel.Location = new System.Drawing.Point(10, 35);
            this.uxUserNameLabel.Name = "uxUserNameLabel";
            this.uxUserNameLabel.Size = new System.Drawing.Size(175, 26);
            this.uxUserNameLabel.TabIndex = 0;
            this.uxUserNameLabel.Text = "User Name:";
            this.uxUserNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.uxLanguageTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.uxClientTextBox);
            this.groupBox2.Controls.Add(this.uxClientLabel);
            this.groupBox2.Controls.Add(this.uxSystemNumberLabel);
            this.groupBox2.Controls.Add(this.uxSystemNumberTextBox);
            this.groupBox2.Controls.Add(this.uxSystemIDTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.uxSystemHostTextBox);
            this.groupBox2.Controls.Add(this.uxHostLabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 233);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SAP Server";
            // 
            // uxHostLabel
            // 
            this.uxHostLabel.Location = new System.Drawing.Point(10, 33);
            this.uxHostLabel.Name = "uxHostLabel";
            this.uxHostLabel.Size = new System.Drawing.Size(175, 26);
            this.uxHostLabel.TabIndex = 0;
            this.uxHostLabel.Text = "SAP Host (Server):";
            this.uxHostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxSystemHostTextBox
            // 
            this.uxSystemHostTextBox.Location = new System.Drawing.Point(190, 33);
            this.uxSystemHostTextBox.Name = "uxSystemHostTextBox";
            this.uxSystemHostTextBox.Size = new System.Drawing.Size(220, 26);
            this.uxSystemHostTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "SAP System ID:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxSystemIDTextBox
            // 
            this.uxSystemIDTextBox.Location = new System.Drawing.Point(190, 69);
            this.uxSystemIDTextBox.Name = "uxSystemIDTextBox";
            this.uxSystemIDTextBox.Size = new System.Drawing.Size(100, 26);
            this.uxSystemIDTextBox.TabIndex = 3;
            // 
            // uxSystemNumberTextBox
            // 
            this.uxSystemNumberTextBox.Location = new System.Drawing.Point(190, 105);
            this.uxSystemNumberTextBox.Name = "uxSystemNumberTextBox";
            this.uxSystemNumberTextBox.Size = new System.Drawing.Size(100, 26);
            this.uxSystemNumberTextBox.TabIndex = 4;
            // 
            // uxSystemNumberLabel
            // 
            this.uxSystemNumberLabel.Location = new System.Drawing.Point(10, 105);
            this.uxSystemNumberLabel.Name = "uxSystemNumberLabel";
            this.uxSystemNumberLabel.Size = new System.Drawing.Size(175, 26);
            this.uxSystemNumberLabel.TabIndex = 5;
            this.uxSystemNumberLabel.Text = "SAP System Number:";
            this.uxSystemNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxClientLabel
            // 
            this.uxClientLabel.Location = new System.Drawing.Point(10, 141);
            this.uxClientLabel.Name = "uxClientLabel";
            this.uxClientLabel.Size = new System.Drawing.Size(175, 26);
            this.uxClientLabel.TabIndex = 6;
            this.uxClientLabel.Text = "SAP Client:";
            this.uxClientLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxClientTextBox
            // 
            this.uxClientTextBox.Location = new System.Drawing.Point(190, 140);
            this.uxClientTextBox.Name = "uxClientTextBox";
            this.uxClientTextBox.Size = new System.Drawing.Size(100, 26);
            this.uxClientTextBox.TabIndex = 7;
            // 
            // uxCancelButton
            // 
            this.uxCancelButton.Location = new System.Drawing.Point(338, 431);
            this.uxCancelButton.Name = "uxCancelButton";
            this.uxCancelButton.Size = new System.Drawing.Size(100, 33);
            this.uxCancelButton.TabIndex = 2;
            this.uxCancelButton.Text = "Cancel";
            this.uxCancelButton.UseVisualStyleBackColor = true;
            this.uxCancelButton.Click += new System.EventHandler(this.uxCancelButton_Click);
            // 
            // uxLogonButton
            // 
            this.uxLogonButton.Location = new System.Drawing.Point(227, 431);
            this.uxLogonButton.Name = "uxLogonButton";
            this.uxLogonButton.Size = new System.Drawing.Size(100, 33);
            this.uxLogonButton.TabIndex = 3;
            this.uxLogonButton.Text = "Logon";
            this.uxLogonButton.UseVisualStyleBackColor = true;
            this.uxLogonButton.Click += new System.EventHandler(this.uxLogonButton_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 26);
            this.label3.TabIndex = 8;
            this.label3.Text = "Language:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxLanguageTextBox
            // 
            this.uxLanguageTextBox.Location = new System.Drawing.Point(190, 177);
            this.uxLanguageTextBox.Name = "uxLanguageTextBox";
            this.uxLanguageTextBox.Size = new System.Drawing.Size(100, 26);
            this.uxLanguageTextBox.TabIndex = 9;
            // 
            // SapLogonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 486);
            this.Controls.Add(this.uxLogonButton);
            this.Controls.Add(this.uxCancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SapLogonForm";
            this.Text = "Logon to SAP";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label uxUserNameLabel;
        private System.Windows.Forms.Label uxPasswordLabel;
        private System.Windows.Forms.TextBox uxUserNameTextBox;
        private System.Windows.Forms.TextBox uxPasswordTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label uxHostLabel;
        private System.Windows.Forms.TextBox uxSystemHostTextBox;
        private System.Windows.Forms.TextBox uxSystemIDTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox uxSystemNumberTextBox;
        private System.Windows.Forms.Label uxSystemNumberLabel;
        private System.Windows.Forms.Label uxClientLabel;
        private System.Windows.Forms.TextBox uxClientTextBox;
        private System.Windows.Forms.Button uxCancelButton;
        private System.Windows.Forms.Button uxLogonButton;
        private System.Windows.Forms.TextBox uxLanguageTextBox;
        private System.Windows.Forms.Label label3;
    }
}