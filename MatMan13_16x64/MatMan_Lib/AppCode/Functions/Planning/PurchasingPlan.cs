using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    class PurchasingPlan : IPlanningFunction, IDisposable
    {
        public PurchasingPlan(int topicId, ref System.Array inputStrings, ref string validationString)
	    {

        }

        public void Dispose()
	    {
		    this.Dispose();
	    }
    }
}
