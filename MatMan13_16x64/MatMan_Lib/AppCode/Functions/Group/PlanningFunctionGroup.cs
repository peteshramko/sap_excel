using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    class PlanningFunctionGroup : IFunctionGroup
    {
        private PlanningFunctionType    _planningFunctionType;
        
        public PlanningFunctionGroup(PlanningFunctionType functionType)
	    {
		    this._functionList          =  new List<IMatManFunction>();
		    this._planningFunctionType  =  functionType;
	    }

        public PlanningFunctionType FunctionType 
        {
		    get 
            { 
                return this._planningFunctionType; 
            }
		    set 
            { 
                this._planningFunctionType = value; 
            }
	    }
    }
}
