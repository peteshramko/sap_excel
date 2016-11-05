using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    class ClearValues
    {
        public static void ClearPreviousRun()
	    {
		    MatManFunctionCollection.GetObject().Clear();
		    MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue = 0;

		    SAPRequest.GetObject().TotalProcessedBySAP                      = 0;
		    SAPRequest.ReturnValuesList.Clear();

		    //MatManCalcEngine.GetObject().CurrentFunctionsByCellAddress.Clear();

		    //MatManCalcEngine.GetObject().ConnectDataCount                   = 0;
		    //MatManCalcEngine.GetObject().TopicCount                         = 0;
		    //MatManCalcEngine.GetObject().UserInitiatedCalc                  = true;
	    }
    }
}
