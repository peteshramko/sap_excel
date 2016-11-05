using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iiiwave.MatManLib
{
    public abstract class IFunctionGroup
    {
	    protected List<IMatManFunction>   _functionList;
	    
	    public List<IMatManFunction> FunctionList 
        {
		    get 
            { 
                return this._functionList; 
            }
	    }
    }
}