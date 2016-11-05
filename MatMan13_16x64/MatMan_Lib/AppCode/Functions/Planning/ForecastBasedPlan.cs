using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatManRTDServer
{
    class ForecastBasedPlan : IMatManFunction, IPlanningFunction, IDisposable
    {
        public ForecastBasedPlan(int topicId, ref System.Array inputStrings, ref string validationString)
	    {

        }

        public void Dispose()
	    {
		    this.Dispose();
	    }
    }
}
