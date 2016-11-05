using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using iiiwave.MatManLib.Properties;

namespace iiiwave.MatManLib
{
    public static class FunctionGroups
    {
	    /// <summary>
	    /// Extension member for PWFunctionCollection
	    /// This member adds a series of grouped functions, based on sorting criteria, to the List
	    /// A group represents a grid of tabular values to be passed to SAP in a single network function call
	    /// </summary>
	    /// <param name="functionCollection"></param>
	    /// <returns></returns>
        public static List<IFunctionGroup> GetFunctionGroups(this MatManFunctionCollection functionCollection)
	    {
		    List<IFunctionGroup>    groupList  =  new List<IFunctionGroup>();
		    MatManCompareFunction   myCompare  =  new MatManCompareFunction();

		    SortedList<string, CostPlan>       loadCostPlanList      =  new SortedList<string, CostPlan>(myCompare);
            SortedList<string, ActivityPlan>   loadActivityPlanList  =  new SortedList<string, ActivityPlan>(myCompare);
            // Future function Lists

		    //#Region Build function lists from the function collection (PWFunctionCollection) using dequeue

		    while (functionCollection.Count > 0) 
            {
			    IMatManFunction myFunction = functionCollection.Dequeue();

                Type myType  =  myFunction.GetType().BaseType;

                if (myFunction.GetType().BaseType == typeof(IPlanningFunction))
                {
                    switch ( ((IPlanningFunction)myFunction).FunctionType) 
                    {
				        case PlanningFunctionType.CostPlan:
                        {
                            loadCostPlanList.Add(myFunction.Signature,  (CostPlan)myFunction);
                            break;
                        }					    
                        case PlanningFunctionType.ActivityPlan:
                        {
                            loadActivityPlanList.Add(myFunction.Signature, (ActivityPlan)myFunction);
                            break;
                        }  
                                              
				        default:
					        break;
                    }
			    }
		    }

		    //#End Region

            // index for all elements
            int functionIndex   =  0;

#region CostPlan grouping	

            {            
		        var costCenterElements  =  from     function  in loadCostPlanList.Values 
                                            group    function  by new { ControllingArea     =   function.ControllingArea,
										                                FiscalYear          =   function.FiscalYear, 
													                    PeriodFrom          =   function.PeriodFrom, 
													                    PeriodTo            =   function.PeriodTo, 
													                    Version             =   function.Version,
													                    DocumentHeaderText  =   function.DocumentHeaderText, 
													                    PlanningCurrency    =   function.PlanningCurrency,
													                    Delta               =   function.Delta }
                                            into    functionGroup   select   functionGroup;

		        int costGroupSize    =  0;
                int costObjectIndex  =  0;
                int costValueIndex   =  0;
                int count            =  costCenterElements.AsEnumerable().Count();
                            			    
                //int i = 0;
		        for(int i = 0; i < count; i++) 
                {
			        var g           =  costCenterElements.ElementAt(i);
                    int groupCount  =  g.Count();
                    
                    //int j = 0;
                    for(int j = 0; j < groupCount; )
                    {
					    
				        costGroupSize = Math.Min((Settings.Default.MaximumBatchSize - 1), (groupCount - 1));
                        // Build a FunctionGroup to add to the list, if greater than max group size - begin new group			
				        PlanningFunctionGroup myGroup = new PlanningFunctionGroup(PlanningFunctionType.CostPlan);
				        // increment Object Index (Index Structure)

					        for (int k = 0; k <= costGroupSize; k++) 
                            {
						        if (j < groupCount) 
                                {

							        var item = costCenterElements.ElementAt(i).ElementAt(j);
                                    
                                    costObjectIndex++;
                                    costValueIndex++;
                                    functionIndex++;

							        // increment Value Index (Index Structure)
                                    item.ObjectIndex  =  costObjectIndex;
                                    item.ValueIndex   =  costValueIndex;
                                    item.Index        =  functionIndex;

							        myGroup.FunctionList.Add(item);

							        j++;

							        if (j >= groupCount)
								        break; 
						        } 
                                else 
                                    break;
					        }

					        groupList.Add(myGroup);

			        }
		        }
            }

#endregion  


#region ActivityPlan grouping

            {            
                var activityCenterElements  =  from     function  in loadActivityPlanList.Values 
                                               group    function  by new { CompanyCode         =   function.ControllingArea,
                                                                           FiscalYear          =   function.FiscalYear, 
                                                                           PeriodFrom          =   function.PeriodFrom, 
                                                                           PeriodTo            =   function.PeriodTo,
                                                                           Version             =   function.Version,
                                                                           DocumentHeaderText  =   function.DocumentHeaderText, 
                                                                           PlanningCurrency    =   function.PlanningCurrency,
                                                                           Delta               =   function.Delta }
                                               into    functionGroup   select   functionGroup;

                int activityGroupSize       =  0;		    
                int activityObjectIndex     =  0;
                int activityValueIndex      =  0;
                int activityAttributeIndex  =  0;
                int count                   =  activityCenterElements.AsEnumerable().Count();
                            			    
                //int i = 0;
                for(int i = 0; i < count; i++) 
                {
                    var g           =  activityCenterElements.ElementAt(i);
                    int groupCount  =  g.Count();
                    
                    //int j = 0;
                    for(int j = 0; j < groupCount; )
                    {
					    
                        activityGroupSize  =  Math.Min((Settings.Default.MaximumBatchSize - 1), (groupCount - 1));
                        // Build a FunctionGroup to add to the list, if greater than max group size - begin new group			
                        PlanningFunctionGroup myGroup = new PlanningFunctionGroup(PlanningFunctionType.ActivityPlan);
                        // increment Object Index (Index Structure)

                            for (int k = 0; k <= activityGroupSize; k++) 
                            {
                                if (j < groupCount) 
                                {

                                    var item = activityCenterElements.ElementAt(i).ElementAt(j);
                                    
                                        activityObjectIndex++;
                                        activityValueIndex++;
                                        activityAttributeIndex++;
                                        functionIndex++;
                                    
                                        item.ObjectIndex     =  activityObjectIndex;
                                        item.ValueIndex      =  activityValueIndex;
                                        item.AttributeIndex  =  activityAttributeIndex;
                                        item.Index           =  functionIndex;

                                        myGroup.FunctionList.Add(item);

                                    j++;

                                    if (j >= groupCount)
                                        break; 
                                } 
                                else 
                                    break;
                            }

                            groupList.Add(myGroup);
                    }
                }
            }

#endregion


		    return groupList;
	    }
    }
}