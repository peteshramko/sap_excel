using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace iiiwave.MatManLib
{
    public class PWEventLogging
    {
        private  object    m_threadLock;
        private  EventLog  m_eventLog;

        private PWEventLogging()
        {
            if(!EventLog.SourceExists("Excel4AppsPW"))
            {
                EventLog.CreateEventSource("Excel4AppsPW", "Planning Wand Trace");        
            }        
        }

        // Create a Thread-safe Singleton instantiation
        private static PWEventLogging m_pwEventLogging;

        private static object syncRoot = new object();
        /// <summary>
        ///   Make accessor Thread-safe
        /// </summary>
        /// <returns></returns>
        public static PWEventLogging GetObject()
        {
            if (PWEventLogging.m_pwEventLogging == null) 
            {
                lock (syncRoot) 
                {
                    if (PWEventLogging.m_pwEventLogging == null) 
                    {
                        PWEventLogging.m_pwEventLogging = new PWEventLogging();
                    }
                }
            }
            return PWEventLogging.m_pwEventLogging;
        }

        public EventLog MyEvents
        {
            get
            {
                return this.m_eventLog;      
            }
        }
    }
}
