using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    public class GetPlanningData
    {
        public static IPlanningFunction CreatePlanningData(int TopicID, ref Array requestArray, ref string validation)
	    {
            string[]               requestStrings  =  requestArray.Cast<string>().ToArray();
			PlanningFunctionType   functionType    =  (PlanningFunctionType)Enum.Parse(typeof(PlanningFunctionType), requestStrings[0]);

		    try 
            {			    
			    switch (functionType) 
                {
				    case PlanningFunctionType.CostPlan:
                    {                        				    
					    return new CostPlan(TopicID, ref requestArray, ref validation);                        
                    }				    
                    case PlanningFunctionType.ActivityPlan:
                    {                        				    
					    return new ActivityPlan(TopicID, ref requestArray, ref validation);                        
                    }				    
                    //case PlanningFunctionType.PurchasingPlan:
                    //{
                    //    return new PurchasingPlan(TopicID, ref requestArray, ref validation);                    
                    //}
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

    
}


