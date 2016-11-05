using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public abstract class IMatManFunction
    {
        protected string             m_signature;
	    protected string             m_hash;
	    protected int                m_topicID;
        protected System.Array       m_paramStrings;
        protected string             m_Result;        
        
        public string Signature 
        {
		    get 
            { 
                return m_signature; 
            }
		    set 
            { 
                m_signature = value; 
            }
	    }
	
        public string Hash 
        {
		    get 
            { 
                return m_hash; 
            }
		    set 
            { 
                m_hash = value; 
            }
	    }
	
        public int TopicID 
        {
		    get 
            { 
                return m_topicID; 
            }
		    set 
            { 
                m_topicID = value; 
            }
	    }
	
	    public System.Array ParamStrings 
        {
		    get 
            { 
                return m_paramStrings; 
            }
		    set 
            { 
                m_paramStrings = value; 
            }
	    }

        public string Result 
        {
		    get 
            { 
                return m_Result; 
            }
		    set 
            { 
                m_Result = value; 
            }
	    }	    
    }
}
