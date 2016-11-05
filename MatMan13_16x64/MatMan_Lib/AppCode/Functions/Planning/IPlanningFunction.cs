using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public abstract class IPlanningFunction : IMatManFunction
    {        
	    protected string                 m_validationResult;
	    protected PlanningFunctionType   m_functionType;
        protected bool                   m_isError;
        protected string                 m_errorText;
        protected bool                   m_updated;
        protected int                    m_index;
        protected int                    m_type;
	
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

        public PlanningFunctionType FunctionType
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
	
	    public bool Updated 
        {
		    get 
            { 
                return m_updated; 
            }
		    set 
            { 
                m_updated = value; 
            }
	    }

        public int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
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
