using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatManRTDServer
{
    class GetSAPPlanningData
    {
        public static IPlanningFunction CreatePlanningData(int TopicID, ref Array requestArray, ref string validation)
	    {
            string[]             requestStrings = requestArray.Cast<string>().ToArray();
			MatManFunctionType   functionType   = (MatManFunctionType)Enum.Parse(typeof(MatManFunctionType), requestStrings[0]);

		    try 
            {			    
			    switch (functionType) 
                {
				    case MatManFunctionType.MatMan_ForecastBasedPlan:
                    {                        				    
					    IPlanningFunction function = new ForecastBasedPlan(TopicID, ref requestArray, ref validation);

                        return function;
                    }				    
                    case MatManFunctionType.MatMan_ReorderPointPlan:
                    {
                        IPlanningFunction function = new ReorderPointPlan(TopicID, ref requestArray, ref validation);
                    
                        return function;
                    }
                    case MatManFunctionType.MatMan_TimePhasedPlan:
                    {
                        IPlanningFunction function = new TimePhasedPlan(TopicID, ref requestArray, ref validation);
                        
                        return function;
                    }
				    default:
				    {
					    break; 
				    }
			    }
		    } 
            catch (Exception e) 
            {
			    
		    }

		    return null;
        }
    }

     public enum RefreshType : int
    {
	    Range                         =  0,
	    Sheet                         =  1,
	    Workbook                      =  3
    }

    public enum MatManFunctionType : int
    {
	    MatMan_ForecastBasedPlan      =  1000,
        MatMan_ReorderPointPlan       =  1001,
        MatMan_TimePhasedPlan         =  1002
    }

    public enum FunctionGroupingType : int
    {
	    NoGrouping                    =  0,
	    GroupByCostCenterCostElement  =  1
    }

    public enum FunctionExecutionType : int
    {
	    ValidateOnly                  =  0,
	    ValidateAndPostValues         =  1
    }
}


