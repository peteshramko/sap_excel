using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace iiiwave.MatManLib
{
    public class MatManErrorDictionary
    {
    // Create a Thread-safe Singleton instantiation
	    private static MatManErrorDictionary  m_errorDictionary;

	    private static object syncRoot = new object();
	    /// <summary>
	    ///   Make accessor Thread-safe
	    /// </summary>
	    /// <returns></returns>
	    public static MatManErrorDictionary GetObject()
	    {
		    if (MatManErrorDictionary.m_errorDictionary == null) 
            {
			    lock (syncRoot) 
                {
				    if (MatManErrorDictionary.m_errorDictionary == null) 
                    {
					    MatManErrorDictionary.m_errorDictionary = new MatManErrorDictionary();
				    }
			    }
		    }
		    return MatManErrorDictionary.m_errorDictionary;
	    }

	    public void ValidatePlanningFunction(ref IPlanningFunction _function, ref string validationResponse)
	    {
		    switch (_function.FunctionType) 
            {

#region CostPlan
			    case PlanningFunctionType.CostPlan :
                {				
                    CostPlan costPlanFunction = (CostPlan)_function;
                
				    try 
                    {

        #region Check Period From, Period To Check
                            /// Check Period from greater than period to
					        int pFromInt  =  Convert.ToInt32(costPlanFunction.PeriodFrom);
					        int pToInt    =  Convert.ToInt32(costPlanFunction.PeriodTo);
					        int diff      =  pToInt - pFromInt;

					        if (diff < 0) 
                            {
						        throw new Exception("Period From must be less than Period To");
					        }
                            else if ( pFromInt < 1 || pFromInt > 12 || pToInt < 1 || pToInt > 12)
                            {
                                throw new Exception("Period From and Period To must be a numeric value between 1 and 12");
                            }
        #endregion

        #region Check Trial Version Restriction
    //                        /// Check trial version restriction
    //					    string productName = Excel4apps.Connector_SAP.Product.GetProductName(Excel4apps.Connector_SAP.Product.Code.PlanningWand);
    //                        if (Excel4apps.Connector_SAP.Product.GetProductLicenseType(productName) == Excel4apps.Connector_SAP.Product.LicenseType.Trial) 
    //                        {
    //	                        if ((pToInt < 1 || pToInt > 3) || (pFromInt < 1 || pFromInt > 3)) 
    //                            {
    //		                        throw new Exception("Trial version may only post from periods 1 to 3, please reduce difference between From and To periods");
    //	                        }
    //                        }
        #endregion

        #region Check Delta
                            /// Check Delta is empty or X
					        if (costPlanFunction.Delta != " " && costPlanFunction.Delta != "X") 
                            {
						        costPlanFunction.Delta = " ";
					        }
        #endregion

        #region Check Fiscal Year is Numeric		    
                            //// Check Fiscal Year is Numeric
                            try 
                            {
                                costPlanFunction.FiscalYear = costPlanFunction.FiscalYear.TrimStart(new char[] {'0', ' '});
                                int.Parse(costPlanFunction.FiscalYear);
                            } 
                            catch (Exception ex) 
                            {
                                throw new Exception("Fiscal Year must be a valid numeric year");
                            }
        #endregion

        #region Check Planning Currency Type
					        try 
                            {
						        if (costPlanFunction.PlanningCurrency.ToUpper() != "C" && costPlanFunction.PlanningCurrency.ToUpper() != "O" && costPlanFunction.PlanningCurrency.ToUpper() != "T") 
                                {
                                     throw new Exception("Planning Currency must be C, O or T");
						        }
					        } 
                            catch (Exception ex) 
                            {
						        throw ex;
					        }
        #endregion

        #region Check Fixed Input is Numeric

                            try
                            {
                                Decimal.Parse(costPlanFunction.FixedInputValue);
                            }
                            catch(Exception ex)
                            {
                                throw new Exception("Fixed Value must be a numeric");
                            }

        #endregion


				    } 
                    catch (Exception e) 
                    {
					        costPlanFunction.ValidationResult    =  "mmError (" + e.Message + ")";
					        validationResponse                   =  "mmError (" + e.Message + ")";
				    }
                }
				break;
#endregion


#region ActivityPlan
			    case PlanningFunctionType.ActivityPlan :
                {				
                    ActivityPlan activityPlanFunction = (ActivityPlan)_function;
                
				    try 
                    {

        #region Check Period From, Period To Check
                            /// Check Period from greater than period to
					        int pFromInt  =  Convert.ToInt32(activityPlanFunction.PeriodFrom);
					        int pToInt    =  Convert.ToInt32(activityPlanFunction.PeriodTo);
					        int diff      =  pToInt - pFromInt;

					        if (diff < 0) 
                            {
						        throw new Exception("Period From must be less than Period To");
					        }
                            else if ( pFromInt < 1 || pFromInt > 12 || pToInt < 1 || pToInt > 12)
                            {
                                throw new Exception("Period From and Period To must be a numeric value between 1 and 12");
                            }
        #endregion

        #region Check Trial Version Restriction
    //                        /// Check trial version restriction
    //					    string productName = Excel4apps.Connector_SAP.Product.GetProductName(Excel4apps.Connector_SAP.Product.Code.PlanningWand);
    //                        if (Excel4apps.Connector_SAP.Product.GetProductLicenseType(productName) == Excel4apps.Connector_SAP.Product.LicenseType.Trial) 
    //                        {
    //	                        if ((pToInt < 1 || pToInt > 3) || (pFromInt < 1 || pFromInt > 3)) 
    //                            {
    //		                        throw new Exception("Trial version may only post from periods 1 to 3, please reduce difference between From and To periods");
    //	                        }
    //                        }
        #endregion

        #region Check Delta
                            /// Check Delta is empty or X
					        if (activityPlanFunction.Delta != " " && activityPlanFunction.Delta != "X") 
                            {
						        activityPlanFunction.Delta = " ";
					        }
        #endregion

        #region Check Fiscal Year is Numeric		    
                            //// Check Fiscal Year is Numeric
                            try 
                            {
                                activityPlanFunction.FiscalYear = activityPlanFunction.FiscalYear.TrimStart(new char[] {'0', ' '});
                                int.Parse(activityPlanFunction.FiscalYear);
                            } 
                            catch (Exception ex) 
                            {
                                throw new Exception("Fiscal Year must be a valid numeric year");
                            }
        #endregion

        #region Check Planning Currency Type
					        try 
                            {
						        if (activityPlanFunction.PlanningCurrency.ToUpper() != "C" && activityPlanFunction.PlanningCurrency.ToUpper() != "O" && activityPlanFunction.PlanningCurrency.ToUpper() != "T") 
                                {
                                     throw new Exception("Planning Currency must be C, O or T");
						        }
					        } 
                            catch (Exception ex) 
                            {
						        throw ex;
					        }
        #endregion
       

				    } 
                    catch (Exception e) 
                    {
					        activityPlanFunction.ValidationResult  =  "mmError (" + e.Message + ")";
					        validationResponse                     =  "mmError (" + e.Message + ")";
				    }
                }
				break;
#endregion

                default:
				break;
		    }
	    }
    }
}