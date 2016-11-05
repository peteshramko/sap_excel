using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iiiwave.MatManLib
{
    class Utils
    {
        internal readonly string OriginalCultureName;

        private Utils()
	    {
            OriginalCultureName  =  Thread.CurrentThread.CurrentCulture.Name;
	    }
                

	    // Create a Thread-safe Singleton instantiation
	    private static Utils m_utils;

	    private static object syncRoot = new object();
	    /// <summary>
	    ///   Make accessor Thread-safe
	    /// </summary>
	    /// <returns></returns>
	    public static Utils GetObject()
	    {
		    if (Utils.m_utils == null) 
            {
			    lock (syncRoot) 
                {
				    if (Utils.m_utils == null) 
                    {
					    Utils.m_utils = new Utils();
				    }
			    }
		    }
		    return Utils.m_utils;
	    }

        internal void SetUSCulture()
        {
	        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        internal void SetOriginalCulture()
        {
	        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(OriginalCultureName);
        }

        internal bool IsUSCulture()
        {
	        if (System.Globalization.CultureInfo.CurrentCulture.Name != "en-US") 
            {
		        return false;
	        }
	        return true;
        }
    }
}
