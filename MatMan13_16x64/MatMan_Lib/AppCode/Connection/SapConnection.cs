using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Web;
using SAP.Middleware.Connector;
using System.Threading;

namespace iiiwave.MatManLib
{
    public delegate void LogonCompleteHandler();

    public class SapConnection
	{
        public event    LogonCompleteHandler   OnLogonComplete;

        internal const string  SAP_CLIENT           =  "sap_client";
        internal const string  SAP_LANGUAGE         =  "sap_language";
        internal const string  SAP_SYS_NUMBER       =  "sap_system_number";
        internal const string  SAP_HOST             =  "sap_host";
        internal const string  USER_NAME            =  "sap_user_name";
        internal const string  PASSWORD             =  "sap_password";

        private SAP.Middleware.Connector.RfcDestination   m_rfcDestination; 
        private SapLogonConfig                            m_provider;
	    private String         errorMsg; 
	    private SapSessionProvider                        m_session;
        private Dictionary<string, Thread>                m_liveSessions;

	    
		/// <summary>
		/// Temp storage for compay list
		/// </summary>
		/// 
		//public List<string> m_companyList;

	    private static SapConnection  m_sapConnection;
	    private static object         syncRoot = new object();
	
        public static  SapConnection GetObject()
        {
	        if (SapConnection.m_sapConnection == null) 
            {
		        lock(syncRoot) 
                {
			        if (SapConnection.m_sapConnection == null) 
                    {
			    	    SapConnection.m_sapConnection = new SapConnection();
			        }
		        }
	        }
	        return SapConnection.m_sapConnection;
        }

        private SapConnection()
        {
            m_liveSessions  =  new Dictionary<string, Thread>();
            m_session       =  new SapSessionProvider();
            if (!RfcSessionManager.IsSessionProviderRegistered())
                    RfcSessionManager.RegisterSessionProvider(m_session);

        }

        public bool connectSAPserver()
        {
            m_rfcDestination  =  null;
            try 
			{ 
			    m_rfcDestination = RfcDestinationManager.GetDestination(DataCache.GetObject().SapDestinationName);

                OnLogonComplete?.Invoke();

                return true;
			} 
			catch (Exception exception) 
			{ 
			    throw exception; 
			}

            return false; 
        }
	
	    public bool connectSAPserver(string _sapUserName,               string _sapPassword,                      string _sapClient    =  null,  
                                     string _sapLanguage      =  null,  string _sapSystemNumber         =  null,  string _sapHost      =  null, 
                                     string _sapSystemID      =  null,  bool   _replaceExistingInCache  =  false)
	    { 
		    registerSAPServerDetails(_sapUserName,      _sapPassword,  _sapClient,     _sapLanguage,  
                                     _sapSystemNumber,  _sapHost,      _sapSystemID,   _replaceExistingInCache);
		    try 
			{ 
			    m_rfcDestination = RfcDestinationManager.GetDestination(DataCache.GetObject().SapDestinationName);
                
                    //attempt actual function call
                    IRfcFunction function  =  m_rfcDestination.Repository.CreateFunction("BAPI_COMPANYCODE_GETLIST");

                    //RfcSessionManager.BeginContext(m_rfcDestination);

                    //    function.Invoke(m_rfcDestination);

                    //RfcSessionManager.EndContext(m_rfcDestination);

                    //IRfcTable             returnTable        =  function.GetTable("COMPANYCODE_LIST");

                    //for (int y = 0; y <= (returnTable.RowCount - 1); y += 1) 
                    //{
                    //    for (int z = 0; z <= (returnTable[y].ElementCount - 1); z += 1) 
                    //    {
                    //        string par = returnTable[y][z].Metadata.Name;
                    //        string val = returnTable[y].GetString(z);

                    //        string messageLine = par + " : " + val;
                    //    }
                    //}

                //foreach (var r in returnStructure.) 
                    //{
                    //    foreach (var t in r) 
                    //    {
                    //        //string par = returnTable[y][z].Metadata.Name;
                    //        //string val = returnTable[y].GetString(z);

                    //        //string messageLine = par + " : " + val;
                    //        //writer.WriteLine(messageLine);

                    //        string a = r.Metadata.Name;
                    //        string b = r.GetString(a);
                    //    }
                    //    //writer.WriteLine(" ");
                    //}

                OnLogonComplete?.Invoke();

                return true;
			} 
			catch (Exception exception) 
			{ 
			    throw exception; 
			}

            return false; 
	    } 	
		 
	    private void registerSAPServerDetails(string _sapUserName,               string _sapPassword,                      string _sapClient    =  null,  
                                              string _sapLanguage      =  null,  string _sapSystemNumber         =  null,  string _sapHost      =  null, 
                                              string _sapSystemID      =  null,  bool   _replaceExistingInCache  =  false)
	    { 
		    m_rfcDestination  =  null;
            DataCache.GetObject().ClearCache();

            try
		    {
			    if(String.IsNullOrEmpty(DataCache.GetObject().SapClient)         ||  _replaceExistingInCache)
                {
                    DataCache.GetObject().SapClient = (!String.IsNullOrEmpty(_sapClient))   ?   _sapClient : string.Empty;
                }
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapClient))
                {
                    throw new Exception("Please provide valid client number");
                }

				if(String.IsNullOrEmpty(DataCache.GetObject().SapLanguage)       ||  _replaceExistingInCache)
                {
                    DataCache.GetObject().SapLanguage = (!String.IsNullOrEmpty(_sapLanguage))  ?   _sapLanguage : String.Empty;
                }
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapLanguage))
                {
                    throw new Exception("Please provide valid SAP Language");
                }

				if(String.IsNullOrEmpty(DataCache.GetObject().SapSystemNumber)  ||  _replaceExistingInCache)
                {   
                    DataCache.GetObject().SapSystemNumber = (!String.IsNullOrEmpty(_sapSystemNumber))  ?  _sapSystemNumber :  String.Empty;
                }
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapSystemNumber))
                {
                    throw new Exception("Please provide valid SAP System Number");
                }

				if(String.IsNullOrEmpty(DataCache.GetObject().SapHost)          ||  _replaceExistingInCache)
                {          
                   DataCache.GetObject().SapHost = (!String.IsNullOrEmpty(_sapHost))     ?       _sapHost       :  String.Empty;
                }
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapHost))
                {
                    throw new Exception("Please provide valid SAP Host");
                }

				if(String.IsNullOrEmpty(DataCache.GetObject().SapSystemID)      ||  _replaceExistingInCache)
                {         
                   DataCache.GetObject().SapSystemID  = (!String.IsNullOrEmpty(_sapSystemID)) ?     _sapSystemID   :  String.Empty;
                }
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapSystemID))
                {
                    throw new Exception("Please provide valid SAP ID");
                }

				if(String.IsNullOrEmpty(DataCache.GetObject().SapUserName)      ||  _replaceExistingInCache)
                {
                    DataCache.GetObject().SapUserName  = (!String.IsNullOrEmpty(_sapUserName))  ?   _sapUserName  : String.Empty;
                }       
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapUserName))
                {
                    throw new Exception("Please provide valid SAP User Name");
                }
                                
                if(String.IsNullOrEmpty(DataCache.GetObject().SapLogonPassword) ||  _replaceExistingInCache)
                {
                    DataCache.GetObject().SapLogonPassword = (!String.IsNullOrEmpty(_sapPassword))  ? _sapPassword  :  String.Empty;
                }    
                else if (String.IsNullOrEmpty(DataCache.GetObject().SapLogonPassword))
                {
                    throw new Exception("Please provide valid SAP User Password");
                }    
			
				if(m_rfcDestination == null)
				{
                    m_provider  =  new MatManLib.SapLogonConfig();

                    if(RfcDestinationManager.TryGetDestination(DataCache.GetObject().SapDestinationName) == null)
                        RfcDestinationManager.RegisterDestinationConfiguration(m_provider);
				}			
		    }
		    catch(Exception exp)
		    {
                throw exp;
		    }
	    }

		public RfcDestination CurrentDestination
		{
			get
			{
				if(DataCache.GetObject().SapDestinationName != null) return RfcDestinationManager.GetDestination(DataCache.GetObject().SapDestinationName);
				throw new Exception("SAP Login Information: Destination Name not found");
			}
			
		}
	
	    public bool IsConnected
	    {
		    get
            {
                try
		        {
			        return this.m_rfcDestination != null;
		        }
		        catch(Exception e)
		        {
			        return false;
		        }
            }
	    }

        public void ResetSAPConnection()
        {
            try
            {
                this.m_rfcDestination = null;
            }
            catch(Exception e)
            {

            }
        }

        public SAP.Middleware.Connector.ISessionProvider MySession
        {
            get
            {
                return m_session;
            }
            //set
            //{
            //    if (value is SapSessionProvider)
            //        m_session = (SapSessionProvider)value;
            //}
        }

        public Dictionary<string, Thread> LiveSessions
        {
            get
            {
                return m_liveSessions;
            }
        }
	}
}