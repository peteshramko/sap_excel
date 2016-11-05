using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iiiwave.MatManLib
{
    public  delegate void OnLogonCompleteEventHandler();

    public partial class SapLogonForm:Form
    {
        public  event         OnLogonCompleteEventHandler     OnLogonComplete;

        public SapLogonForm()
        {
            InitializeComponent();

            this.uxSystemHostTextBox.Text    =  DataCache.GetObject().SapHost;
            this.uxSystemIDTextBox.Text      =  DataCache.GetObject().SapSystemID;
            this.uxSystemNumberTextBox.Text  =  DataCache.GetObject().SapSystemNumber;
            this.uxClientTextBox.Text        =  DataCache.GetObject().SapClient;
            this.uxLanguageTextBox.Text      =  DataCache.GetObject().SapLanguage;
        }

        private void uxLogonButton_Click(object sender,EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.uxUserNameTextBox.Text))
                DataCache.GetObject().SapUserName       =  this.uxUserNameTextBox.Text;
            else { MessageBox.Show("Please enter a valid User Name"); return; }
      
            if (!String.IsNullOrEmpty(this.uxPasswordTextBox.Text))
                DataCache.GetObject().SapLogonPassword  =  this.uxPasswordTextBox.Text;
            else {  MessageBox.Show("Please enter a valid Password"); return; }

            if (!String.IsNullOrEmpty(this.uxSystemHostTextBox.Text))
                DataCache.GetObject().SapHost  =  this.uxSystemHostTextBox.Text;
            else {  MessageBox.Show("Please enter an SAP Host (Server)"); return; }

            if (!String.IsNullOrEmpty(this.uxSystemIDTextBox.Text))
                DataCache.GetObject().SapSystemID  =  this.uxSystemIDTextBox.Text;
            else {  MessageBox.Show("Please enter an SAP System ID"); return; }

            if (!String.IsNullOrEmpty(this.uxSystemNumberTextBox.Text))
                DataCache.GetObject().SapSystemNumber  =  this.uxSystemNumberTextBox.Text;
            else {  MessageBox.Show("Please enter an SAP System Number"); return; }

            if (!String.IsNullOrEmpty(this.uxClientTextBox.Text))
                DataCache.GetObject().SapClient        =  this.uxClientTextBox.Text;
            else {  MessageBox.Show("Please enter an SAP Client"); return; }

            if (!String.IsNullOrEmpty(this.uxLanguageTextBox.Text))
                DataCache.GetObject().SapLanguage        =  this.uxLanguageTextBox.Text;
            else {  MessageBox.Show("Please enter an SAP Language"); return; }

            DataCache.GetObject().SapLanguage  =  this.uxLanguageTextBox.Text;

            if(SapConnection.GetObject().IsConnected)
                SapConnection.GetObject().ResetSAPConnection();
            
            try
            {
                SapConnection.GetObject().connectSAPserver(this.uxUserNameTextBox.Text,      this.uxPasswordTextBox.Text,     this.uxClientTextBox.Text,     this.uxLanguageTextBox.Text,  
                                                           this.uxSystemNumberTextBox.Text,  this.uxSystemHostTextBox.Text,   this.uxSystemIDTextBox.Text,   true);

                if (SapConnection.GetObject().connectSAPserver())
                    MessageBox.Show("SAP Connected", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("SAP Connection Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception exp)
            {
                MessageBox.Show("SAP Connection Error: \r\n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.OnLogonComplete?.Invoke();
                this.Close();
            }                        
        }

        private void uxCancelButton_Click(object sender,EventArgs e)
        {
            this.Close();
        }
    }
}
