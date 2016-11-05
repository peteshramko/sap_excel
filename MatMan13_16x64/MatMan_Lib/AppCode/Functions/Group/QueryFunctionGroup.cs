using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    class QueryFunctionGroup : IFunctionGroup
    {
        private QueryFunctionType       _queryFunctionType;

        public QueryFunctionGroup(QueryFunctionType functionType)
	    {
		    this._functionList         =  new List<IMatManFunction>();
		    this._queryFunctionType    =  functionType;
	    }

        public QueryFunctionType FunctionType 
        {
		    get 
            { 
                return this._queryFunctionType; 
            }
		    set 
            { 
                this._queryFunctionType = value; 
            }
	    }
    }
}
