using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace iiiwave.MatManLib
{
    public  delegate void DeleteCacheEventHandler();    

    class DataCache
    {
        // Login Params
        private const string  SAP_LOGON_NAME  =  "SAP_LOGON_NAME";
        private const string  SAP_PASSWORD    =  "SAP_PASSWORD";
        
        public event   DeleteCacheEventHandler    OnDeleteCache;
        
        // holds data
        private        MemoryCache m_cache;

        /// <summary>
	    ///   Make accessor Thread-safe
	    /// </summary>	
	    private static DataCache m_sapDataCache;

	    private static object syncRoot = new object();
	    public static DataCache GetObject()
	    {
		    if (DataCache.m_sapDataCache == null) 
            {
			    lock (syncRoot) 
                {
				    if (DataCache.m_sapDataCache == null) 
                    {
					    DataCache.m_sapDataCache = new DataCache();
                                             
				    }
			    }
		    }
		    return DataCache.m_sapDataCache;
	    }

        private DataCache() 
        { 
            this.m_cache  = MemoryCache.Default; 
        }

        // Login Credentials
        internal string SapClient
        {
            get
            {
                if( String.IsNullOrEmpty(global::iiiwave.MatManLib.Properties.Settings.Default.SAPClient) ) return "800";
                else return global::iiiwave.MatManLib.Properties.Settings.Default.SAPClient;
            }
            set
            {
                global::iiiwave.MatManLib.Properties.Settings.Default.SAPClient  =  value;
                global::iiiwave.MatManLib.Properties.Settings.Default.Save();
            }
        }

        internal string SapLanguage
        {
            get
            {
                if( String.IsNullOrEmpty(global::iiiwave.MatManLib.Properties.Settings.Default.SAPLanguage) ) return "EN";
                else return global::iiiwave.MatManLib.Properties.Settings.Default.SAPLanguage;
            }
            set
            {
                global::iiiwave.MatManLib.Properties.Settings.Default.SAPLanguage  =  value;
                global::iiiwave.MatManLib.Properties.Settings.Default.Save();
            }
        }

        internal string SapSystemNumber
        {
            get
            {
                if( String.IsNullOrEmpty(global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemNumber) ) return "01";
                else return global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemNumber;
            }
            set
            {
                global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemNumber  =  value;
                global::iiiwave.MatManLib.Properties.Settings.Default.Save();
            }
        }

        internal string SapHost
        {
            get
            {
                if( String.IsNullOrEmpty(global::iiiwave.MatManLib.Properties.Settings.Default.SAPHost) ) return "192.168.1.125";
                else return global::iiiwave.MatManLib.Properties.Settings.Default.SAPHost;
            }
            set
            {
                global::iiiwave.MatManLib.Properties.Settings.Default.SAPHost  =  value;
                global::iiiwave.MatManLib.Properties.Settings.Default.Save();
            }
        }

        internal string SapSystemID
        {
            get
            {
                if( String.IsNullOrEmpty(global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemID) ) return "SYD";
                else return global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemID;
            }
            set
            {
                global::iiiwave.MatManLib.Properties.Settings.Default.SAPSystemID  =  value;
                global::iiiwave.MatManLib.Properties.Settings.Default.Save();
            }
        }
        
        internal string SapUserName
        {
            get
            {
                return (string)this.m_cache[SAP_LOGON_NAME];
            }
            set
            {
                this.m_cache[SAP_LOGON_NAME]  =  value;
            }
        }

        internal string SapLogonPassword
        {
            get
            {
                return (string)this.m_cache[SAP_PASSWORD];
            }
            set
            {
                this.m_cache[SAP_PASSWORD]  =  value;
            }
        }

        internal string SapDestinationName
        {
            get
            {
                return  SapClient     +  "|"  + SapLanguage + "|" + SapSystemNumber + "|" + SapSystemID + "|" + SapHost + "|" +
                        SapUserName   +  "|"  + SapLogonPassword;
            }
        }

        internal void ClearCache()
        {
            this.m_cache[SAP_LOGON_NAME]  =  (this.m_cache[SAP_LOGON_NAME] != null) ? String.Empty : null;
            this.m_cache[SAP_PASSWORD]    =  (this.m_cache[SAP_PASSWORD]   != null) ? String.Empty : null;
        }
    }
}
