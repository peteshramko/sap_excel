using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;
using SAP.Middleware.Connector;


namespace iiiwave.MatManLib
{
    class SapSessionProvider : SAP.Middleware.Connector.ISessionProvider
    {
        private Dictionary<string, Thread> rfcThreads;

        public event RfcSessionManager.SessionChangeHandler SessionChanged;

        public SapSessionProvider()
	    {
		    rfcThreads = new Dictionary<string, Thread>();
	    }

	    public String AddCurrentSession()
	    {

		    // Note: this implementation is ok for RFC clients.
		    // For (stateful) servers, more work would be necessary. (E.g. use CPIC conversation ID of R/3 user session as session ID inside .NET.)
		    string sessionID = Thread.CurrentThread.ManagedThreadId.ToString();

		    if (!rfcThreads.ContainsKey(sessionID))
            {
			    rfcThreads[sessionID] = Thread.CurrentThread;
		    }

		    return sessionID;

	    }

	    String ISessionProvider.GetCurrentSession()
	    {
		    return AddCurrentSession();
	    }


	    public void DestroySession(String sessionID)
	    {
		    try
            {
			    lock (rfcThreads)
                {
				    if (rfcThreads.Remove(sessionID))
                    {
                        // Send the event to the RfcSessionManager:
                        SessionChanged?.Invoke(new RfcSessionEventArgs(RfcSessionManager.EventType.DESTROYED,sessionID));
                    }
			    }

		    }
            catch (Exception ex)
            {
			    //My.Application.Log.WriteException(ex);
		    }

	    }

	    public bool RemoveCurrentSession(String sessionID)
	    {
		    Thread thread = null;

		    lock (rfcThreads)
            {
			    if (rfcThreads.TryGetValue(sessionID, out thread) && thread != null)
                {
				    rfcThreads.Remove(sessionID);
				    return true;
			    }
		    }

		    return false;

	    }
	    bool ISessionProvider.IsAlive(String sessionID)
	    {
		    return RemoveCurrentSession(sessionID);
	    }

	    public bool ChangeEventsSupported()
	    {
		    return true;
	    }

	    // The following methods are needed only in RFC Server scenarios (stateful servers). Not implemented for now.
	    public void ContextStarted()
	    {
	    }

	    public void ContextFinished()
	    {
	    }

	    public String CreateSession()
	    {
		    throw new NotImplementedException();
	    }

	    public void PassivateSession(String sessionID)
	    {
	    }

	    public void ActivateSession(String sessionID)
	    {
	    }

    }

}
