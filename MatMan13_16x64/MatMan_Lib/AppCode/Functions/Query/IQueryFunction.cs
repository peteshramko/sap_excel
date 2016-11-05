using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public abstract class IQueryFunction : IMatManFunction
    {
        protected string              m_validationResult;
	    protected QueryFunctionType   m_functionType;
        protected bool                m_isError;
        protected string              m_errorText;
        protected int                 m_type;

        public string ValidationResult 
        {
		    get 
            { 
                return m_validationResult; 
            }
		    set 
            { 
                m_validationResult = value; 
            }
	    }

        public QueryFunctionType FunctionType
        {
            get
            {
                return m_functionType;
            }
            set
            {
                m_functionType  =  value;
            }
        }
	
        public bool IsError 
        {
		    get 
            { 
                return m_isError; 
            }
		    set 
            { 
                m_isError = value; 
            }
	    }
	
	    public string Error_Text 
        {
		    get 
            { 
                return m_errorText; 
            }
		    set 
            { 
                m_errorText = value; 
            }
	    }

        public int Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type  =  value;
            }
        }
    }
}
