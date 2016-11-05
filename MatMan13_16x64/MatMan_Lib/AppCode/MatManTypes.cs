using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public enum RefreshType : int
    {
	    Range                         =  0,
	    Sheet                         =  1,
	    Workbook                      =  3
    }

    public enum PlanningFunctionType : int
    {
        CostPlan                      =  1000,
        ActivityPlan                  =  1001,
        PurchasingPlan                =  1002
    }

    public enum QueryFunctionType : int
    {
        GetRequisitionList            =  2001, // associates with WBS item

    }

    public enum FunctionExecutionType : int
    {
        RetrievingData                =  0,
        ValidateData                  =  1,
	    ValidateAndPostData           =  2
    }
}
