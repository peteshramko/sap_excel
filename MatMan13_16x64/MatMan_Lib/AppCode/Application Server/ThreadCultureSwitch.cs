using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public class ThreadCultureSwitch
    {
	    public Thread ThisThread                                { get; set; }
	    public System.Globalization.CultureInfo OriginalCulture { get; set; }

	    public ThreadCultureSwitch()
	    {
		    ThisThread      = Thread.CurrentThread;
		    OriginalCulture = ThisThread.CurrentCulture;
	    }

	    public void SetUSCulture()
	    {
		    ThisThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
	    }

	    public void SetOriginalCulture()
	    {
		    ThisThread.CurrentCulture = this.OriginalCulture;
	    }

    }
}
