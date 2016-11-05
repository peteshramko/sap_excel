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
    public partial class UserOptionsForm:Form
    {
        public UserOptionsForm()
        {
            InitializeComponent();

            this.uxBatchSizeGroupBox.Text  =  iiiwave.MatManLib.Localization.Localize.UserOptionsForm_BatchSizeGroupBoxText;
        
        }


    }
}
