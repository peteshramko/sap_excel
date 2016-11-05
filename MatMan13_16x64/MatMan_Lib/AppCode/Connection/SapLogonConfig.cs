using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;

namespace iiiwave.MatManLib
{
    public class SapLogonConfig : IDestinationConfiguration
    {
        private RfcConfigParameters ABAP_AS_PARAMETERS;        
        
        public SapLogonConfig()
        {
            this.ABAP_AS_PARAMETERS  =  new RfcConfigParameters();

                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.Client,         DataCache.GetObject().SapClient);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.Language,       DataCache.GetObject().SapLanguage);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.SystemNumber,   DataCache.GetObject().SapSystemNumber);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.SystemID,       DataCache.GetObject().SapSystemID);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.AppServerHost,  DataCache.GetObject().SapHost);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.User,           DataCache.GetObject().SapUserName);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.Password,       DataCache.GetObject().SapLogonPassword);
                ABAP_AS_PARAMETERS.Add(RfcConfigParameters.Name,           DataCache.GetObject().SapDestinationName);                
        }
        
        public bool ChangeEventsSupported()
        {
            return false;
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

        public RfcConfigParameters GetParameters(string destinationName)
        {
            return ABAP_AS_PARAMETERS;
        }

        internal string DestinationName { get { return DataCache.GetObject().SapDestinationName; } }

    }
}
