using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using SAP.Middleware.Connector;


namespace iiiwave.MatManLib
{
    class ActivityPlan : IPlanningFunction, IDisposable
    {
        private const int m_totalNumberOfParameters  =  16;

        private string  m_quantity             =  String.Empty;  // 2
        private string  m_price                =  String.Empty;  // 3
        private string  m_controllingArea      =  String.Empty;  // 4
        private string  m_fiscalYear           =  String.Empty;  // 5
        private string  m_perFrom              =  String.Empty;  // 6
        private string  m_perTo                =  String.Empty;  // 7
        private string  m_distKey              =  String.Empty;  // 8
        private string  m_version              =  String.Empty;  // 9
        private string  m_documentTxt          =  String.Empty;  // 10
        private string  m_currencyType         =  String.Empty;  // 11
        private string  m_delta                =  String.Empty;  // 12
        private string  m_costCenter           =  String.Empty;  // 13
        private string  m_activityType         =  String.Empty;  // 14
        private string  m_transactionCurrency  =  String.Empty;  // 15

    // increments with Activity Center in group
        private int    m_objectIndex;
    // increments with Activity Element in group
        private int    m_valueIndex;
    // increments with Activity Element in group
        private int    m_attributeIndex;

        private static object m_syncObject = new object();

	    public ActivityPlan(int topicId, ref System.Array inputStrings, ref string validationString)
	    {
		    // Set to new request
		    this.Updated = false;
		    //

		    string[] requestStrings = inputStrings.Cast<string>().ToArray();

		    // add empty string values to fill out array (let SAP do validation)
		    if (requestStrings.Length < m_totalNumberOfParameters) 
            {
			    List<string> ls = new List<string>(requestStrings);
			    while (ls.Count < 15) 
                {
				    ls.Add(String.Empty);
			    }
			    requestStrings = ls.ToArray();
		    }


		    this.TopicID                    =  topicId;
		    this.Hash                       =  inputStrings.Concatenate();
            //this.FunctionType               =  FunctionType.;

            this.FunctionType               =  (PlanningFunctionType)Enum.Parse(typeof(PlanningFunctionType), requestStrings[0]);
            this.m_quantity                 =  requestStrings[1].ToUpper();       // [TOT_VALUE][ACTVTY_QTY]
            this.m_price                    =  requestStrings[2].ToUpper();       // [TOT_VALUE][PRICE_FIX]
            this.m_controllingArea          =  requestStrings[3].ToUpper();       // [HEADER_INFO][CO_AREA]
            this.m_fiscalYear               =  requestStrings[4].ToUpper();       // [HEADER_INFO][FISC_YEAR]
            this.m_perFrom                  =  requestStrings[5].ToUpper();       // [HEADER_INFO][PERIOD_FROM]
            this.m_perTo                    =  requestStrings[6].ToUpper();       // [HEADER_INFO][PERIOD_TO]
            this.m_distKey                  =  requestStrings[7].ToUpper();       // [TOT_VALUE][DIST_KEY_PRICE_FIX]
            this.m_version                  =  requestStrings[8].ToUpper();       // [HEADER_INFO][VERSION]
            this.m_documentTxt              =  requestStrings[9].ToUpper();       // [HEADER_INFO][DOC_HDR_TX]
            this.m_currencyType             =  requestStrings[10].ToUpper();      // [HEADER_INFO][PLAN_CURRTYPE]
            this.m_delta                    =  requestStrings[11].ToUpper();      // [DELTA][DELTA]
            this.m_costCenter               =  requestStrings[12].ToUpper();      // [OBJECT][COSTCENTER]
            this.m_activityType             =  requestStrings[13].ToUpper();      // [OBJECT][ACTTYPE]
            this.m_transactionCurrency      =  requestStrings[14].ToUpper();      // [TOT_VALUE][CURRENCY]

		    this.Signature                  =  requestStrings[requestStrings.Length - 1].ToUpper();

            IPlanningFunction     thisFunc  =  (IPlanningFunction)this;
		    MatManErrorDictionary.GetObject().ValidatePlanningFunction(ref thisFunc, ref validationString);
        }

#region validate function

        public static void ValidateSAPData(PlanningFunctionGroup activityPlanGroup, int functionCount)
        {
            lock (m_syncObject) 
            {
                if (activityPlanGroup.FunctionList.Count > 0) 
                {

                    IRfcFunction sapValidateCostPlanFunction = SapConnection.GetObject().CurrentDestination.Repository.CreateFunction("BAPI_ACT_PRICE_CHECK_AND_POST");
                    IRfcTable    returnTable = null;

                    try 
                    {                        
                        ///*** --- IMPORT (SAP TAB) -------------------------------***//

                        IRfcStructure headerInfoStructure = sapValidateCostPlanFunction.GetStructure("HEADER_INFO"); // HEADER_INFO


                        headerInfoStructure.SetValue("CO_AREA",        ((ActivityPlan)activityPlanGroup.FunctionList[0]).ControllingArea);    // 3				    
                        headerInfoStructure.SetValue("FISC_YEAR",      ((ActivityPlan)activityPlanGroup.FunctionList[0]).FiscalYear);         // 4					    
                        headerInfoStructure.SetValue("PERIOD_FROM",    ((ActivityPlan)activityPlanGroup.FunctionList[0]).PeriodFrom);         // 5					     
                        headerInfoStructure.SetValue("PERIOD_TO",      ((ActivityPlan)activityPlanGroup.FunctionList[0]).PeriodTo);           // 6					    
                        headerInfoStructure.SetValue("VERSION",        ((ActivityPlan)activityPlanGroup.FunctionList[0]).Version);            // 8					    
                        headerInfoStructure.SetValue("DOC_HDR_TX",     ((ActivityPlan)activityPlanGroup.FunctionList[0]).DocumentHeaderText); // 9					    
                        headerInfoStructure.SetValue("PLAN_CURRTYPE",  ((ActivityPlan)activityPlanGroup.FunctionList[0]).PlanningCurrency);   // 10	

                        sapValidateCostPlanFunction.SetValue("DELTA",  ((ActivityPlan)activityPlanGroup.FunctionList[0]).Delta);              // 11		
                        sapValidateCostPlanFunction.SetValue("TESTRUN", "X");                                                                 // "X" - Validate Only, " " - Post

                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            try
                            {
                                /*** --- IDX_STRUCTURE -- CO Planning: Plan Activity BAPIs -------------- ***/
                                                                                          
                                IRfcTable indexTable = sapValidateCostPlanFunction.GetTable("IDX_STRUCTURE");
                                indexTable.Append();
                                    
                                    indexTable.SetValue("OBJECT_INDEX",            ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));
                                    indexTable.SetValue("VALUE_INDEX",             ((ActivityPlan)myFunction).ValueIndex.ToString("000000"));
                                    indexTable.SetValue("ATTRIB_INDEX",            "000000");
                                

                                /*** --- OBJECT -- CO Planning: Objects for Plan Activity BAPIs --------- ***/

                                IRfcTable coObjectTable = sapValidateCostPlanFunction.GetTable("OBJECT");                                
                                coObjectTable.Append();
                                    
                                    string objectIndex  =  ((ActivityPlan)myFunction).ObjectIndex.ToString("000000");
                                    string valueIndex   =  ((ActivityPlan)myFunction).ValueIndex.ToString("000000");
                                
                                try 
                                {
                                    if (coObjectTable.GetValue("OBJECT_INDEX") != null) 
                                    {
                                        if (coObjectTable.GetValue("OBJECT_INDEX").ToString() != objectIndex) 
                                        {
                                            coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));  // Calculated
                                        }
                                    } 
                                    else 
                                    {
                                        coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));      // Calculated
                                    }
                                } 
                                catch (Exception ex) 
                                {
                                    coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));
                                }

                                    coObjectTable.SetValue("COSTCENTER",       ((ActivityPlan)myFunction).CostCenter);                          // 12						    
                                    coObjectTable.SetValue("ACTTYPE",          ((ActivityPlan)myFunction).ActivityType);                        // 13						           
                                    
                                
                                /*** --- ACCOUNT_PLAN_TOTVALUE -- CO Planning: Objects for Primary Cost BAPIs --------- ***/

                                IRfcTable totValueTable = sapValidateCostPlanFunction.GetTable("TOT_VALUE");  
                                totValueTable.Append();
                                    
                                    totValueTable.SetValue("VALUE_INDEX",       ((ActivityPlan)myFunction).ValueIndex.ToString("000000"));    // Calculated

                                if (((ActivityPlan)myFunction).Price != string.Empty) 
                                {
                                    totValueTable.SetValue("PRICE_FIX",          ((ActivityPlan)myFunction).Price);                   // 2
                                    totValueTable.SetValue("DIST_KEY_PRICE_FIX", ((ActivityPlan)myFunction).DistributionKey);
                                    totValueTable.SetValue("PRICE_UNIT",          "00001");
                                } 
                                else 
                                {
                                    totValueTable.SetValue("PRICE_FIX",      "0");
                                }

                                    totValueTable.SetValue("ACTVTY_QTY",     ((ActivityPlan)myFunction).Quantity);                    // 1
                                    totValueTable.SetValue("DIST_KEY_QUAN",  ((ActivityPlan)myFunction).DistributionKey);             // 7
                                    
                                    //// ToDo
                                    
                                    totValueTable.SetValue("CURRENCY",       ((ActivityPlan)myFunction).TransactionCurrency);         // 14 
                                    
                                
                                myFunction.Updated = true;
                            }
                            catch(Exception ex)
                            {
                                myFunction.ValidationResult = ex.Message;                  
                            }
                        }
                    } 
                    catch (Exception exp) 
                    {
                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            myFunction.ValidationResult = exp.Message;
                        }
                    }

                    try 
                    {
                        sapValidateCostPlanFunction.Invoke(SapConnection.GetObject().CurrentDestination);
                    } 
                    catch (Exception ex) 
                    {

                        DialogResult r = MessageBox.Show("SAP Authorization Error: " + ex.Message, "Error", 
                                                         System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (r == DialogResult.OK) 
                        {

                        }

                        ReturnProgressDataForm.CancelProcess();
                        
                        return;
                    }


                    returnTable = sapValidateCostPlanFunction.GetTable("RETURN");

				
                    foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                    {
                        if (!SAPRequest.ReturnValuesList.ContainsKey(myFunction.Signature)) 
                        {
                            SAPRequest.ReturnValuesList.Add(myFunction.Signature, myFunction);
                        }
                    }

                    if (returnTable.RowCount > 0) 
                    {
                        try 
                        {
                            string logPath = LogFile.CheckCreateLogFolder() + "\\PWLogValOnly" + DateTime.Now.ToString("(dd-MMM-yyyy-HH-mm-ss-f)") + ".txt";
                            if (!File.Exists(logPath)) 
                            {
                                using (TextWriter writer = File.CreateText(logPath)) 
                                {
                                    writer.WriteLine("VALIDATION ONLY: " + DateTime.Now.ToString("(dd-MMM-yyyy-HH-mm-ss-f)"));
                                    writer.WriteLine(" ");
                                    for (int y = 0; y <= (returnTable.RowCount - 1); y += 1) 
                                    {
                                        for (int z = 0; z <= (returnTable[y].ElementCount - 1); z += 1) 
                                        {
                                            string par = returnTable[y][z].Metadata.Name;
                                            string val = returnTable[y].GetString(z);

                                            string messageLine = par + " : " + val;
                                            writer.WriteLine(messageLine);
                                        }
                                        writer.WriteLine(" ");
                                    }
                                }
                            }

                        } 
                        catch (Exception ex) 
                        {
                            //MessageBox.Show(ex.Message)
                        }

                        for (int j = 0;j <= (returnTable.RowCount - 1);j++)
                        {
                            int     row      =  Convert.ToInt32(returnTable[j].GetString("ROW")) - 1;
                            string  message  =  returnTable[j].GetString("MESSAGE");

                            if (row < 0)
                            {
                                row = 0;
                            }

                            string rType      =  string.Empty;
                            string messageV1  =  string.Empty;
                            string messageV2  =  string.Empty;
                            string messageV3  =  string.Empty;
                            string messageV4  =  string.Empty;
                            string rNumber    =  string.Empty;

                            rType = returnTable[j].GetString("TYPE");
                            messageV1 = returnTable[j].GetString("MESSAGE_V1");
                            messageV2 = returnTable[j].GetString("MESSAGE_V2");

                            for (int i = 0;i <= (activityPlanGroup.FunctionList.Count - 1);i++)
                            {
                                int elementLocation = SAPRequest.GetObject().TotalProcessedBySAP + i;
                                if (elementLocation < 0)
                                {
                                    elementLocation = 0;
                                }

                                string activityType  = ((ActivityPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).ActivityType;
                                string costCenter    = ((ActivityPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostCenter;

                                try
                                {
                                    messageV1 = messageV1.TrimStart('0');
                                    messageV2 = messageV2.TrimStart('0');
                                }
                                catch (Exception ex)
                                {
                                }

                                try
                                {
                                    if (i == row && !string.IsNullOrEmpty(message))
                                    {
                                        SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                    }
                                    else if (i != row && rType == "E")
                                    {
                                        try
                                        {
                                            if (messageV1 == activityType || messageV2 == activityType)
                                            {
                                                // account for incrementing batch number 
                                                SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                            }
                                            else if (messageV1 != activityType && messageV1 != costCenter && row == 0)
                                            {
                                                rNumber = returnTable[j].GetString("NUMBER");
                                                if (rNumber != string.Empty)
                                                {
                                                    SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                                    if (ReturnProgressDataForm.OperationCancelled)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else if (rType == "I")
                                    {
                                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList)
                                        {
                                            SAPRequest.ReturnValuesList[myFunction.Signature].Result = message;
                                            if (ReturnProgressDataForm.OperationCancelled)
                                            {
                                                break;
                                            }
                                        }
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                if (ReturnProgressDataForm.OperationCancelled)
                                {
                                    break;
                                }
                            }
                            if (ReturnProgressDataForm.OperationCancelled)
                            {
                                break;
                            }
                        }


                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            if ( (SAPRequest.ReturnValuesList[myFunction.Signature].Result == null) || (SAPRequest.ReturnValuesList[myFunction.Signature].Result == string.Empty) ) 
                            {
                                SAPRequest.ReturnValuesList[myFunction.Signature].Result = "pwValidated";
                            }
                            if (ReturnProgressDataForm.OperationCancelled) 
                            {
                                break;
                            }
                        }
                    } 
                    else 
                    {
                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            SAPRequest.ReturnValuesList[myFunction.Signature].Result = "pwValidated";
                            if (ReturnProgressDataForm.OperationCancelled) 
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

#endregion

#region post function

        public static void PostSAPData(PlanningFunctionGroup activityPlanGroup, int functionCount)
        {
            lock (m_syncObject) 
            {
                if (activityPlanGroup.FunctionList.Count > 0) 
                {

                    IRfcFunction sapValidateCostPlanFunction = SapConnection.GetObject().CurrentDestination.Repository.CreateFunction("BAPI_ACT_PRICE_CHECK_AND_POST");
                    IRfcTable    returnTable = null;

                    try 
                    {                        
                        ///*** --- IMPORT (SAP TAB) ------------------------------- ***//

                        IRfcStructure headerInfoStructure = sapValidateCostPlanFunction.GetStructure("HEADER_INFO"); // HEADER_INFO


                        headerInfoStructure.SetValue("CO_AREA",        ((ActivityPlan)activityPlanGroup.FunctionList[0]).ControllingArea);    // 3				    
                        headerInfoStructure.SetValue("FISC_YEAR",      ((ActivityPlan)activityPlanGroup.FunctionList[0]).FiscalYear);         // 4					    
                        headerInfoStructure.SetValue("PERIOD_FROM",    ((ActivityPlan)activityPlanGroup.FunctionList[0]).PeriodFrom);         // 5					     
                        headerInfoStructure.SetValue("PERIOD_TO",      ((ActivityPlan)activityPlanGroup.FunctionList[0]).PeriodTo);           // 6					    
                        headerInfoStructure.SetValue("VERSION",        ((ActivityPlan)activityPlanGroup.FunctionList[0]).Version);            // 8					    
                        headerInfoStructure.SetValue("DOC_HDR_TX",     ((ActivityPlan)activityPlanGroup.FunctionList[0]).DocumentHeaderText); // 9					    
                        headerInfoStructure.SetValue("PLAN_CURRTYPE",  ((ActivityPlan)activityPlanGroup.FunctionList[0]).PlanningCurrency);   // 10	

                        sapValidateCostPlanFunction.SetValue("DELTA",  ((ActivityPlan)activityPlanGroup.FunctionList[0]).Delta);              // 11		
                        sapValidateCostPlanFunction.SetValue("TESTRUN", " ");                                                                 // "X" - Validate Only, " " - Post

                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            try
                            {
                                /*** --- IDX_STRUCTURE -- CO Planning: Plan Activity BAPIs -------------- ***/
                                                                                          
                                IRfcTable indexTable = sapValidateCostPlanFunction.GetTable("IDX_STRUCTURE");
                                indexTable.Append();
                                    
                                    indexTable.SetValue("OBJECT_INDEX",            ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));
                                    indexTable.SetValue("VALUE_INDEX",             ((ActivityPlan)myFunction).ValueIndex.ToString("000000"));
                                    indexTable.SetValue("ATTRIB_INDEX",            "000000");
                                

                                /*** --- OBJECT -- CO Planning: Objects for Plan Activity BAPIs --------- ***/

                                IRfcTable coObjectTable = sapValidateCostPlanFunction.GetTable("OBJECT");                                
                                coObjectTable.Append();
                                    
                                    string objectIndex  =  ((ActivityPlan)myFunction).ObjectIndex.ToString("000000");
                                    string valueIndex   =  ((ActivityPlan)myFunction).ValueIndex.ToString("000000");
                                
                                try 
                                {
                                    if (coObjectTable.GetValue("OBJECT_INDEX") != null) 
                                    {
                                        if (coObjectTable.GetValue("OBJECT_INDEX").ToString() != objectIndex) 
                                        {
                                            coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));  // Calculated
                                        }
                                    } 
                                    else 
                                    {
                                        coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));      // Calculated
                                    }
                                } 
                                catch (Exception ex) 
                                {
                                    coObjectTable.SetValue("OBJECT_INDEX", ((ActivityPlan)myFunction).ObjectIndex.ToString("000000"));
                                }

                                    coObjectTable.SetValue("COSTCENTER",       ((ActivityPlan)myFunction).CostCenter);                          // 12						    
                                    coObjectTable.SetValue("ACTTYPE",          ((ActivityPlan)myFunction).ActivityType);                        // 13						           
                                    
                                
                                /*** --- ACCOUNT_PLAN_TOTVALUE -- CO Planning: Objects for Primary Cost BAPIs --------- ***/

                                IRfcTable totValueTable = sapValidateCostPlanFunction.GetTable("TOT_VALUE");  
                                totValueTable.Append();
                                    
                                    totValueTable.SetValue("VALUE_INDEX",       ((ActivityPlan)myFunction).ValueIndex.ToString("000000"));    // Calculated

                                if (((ActivityPlan)myFunction).Price != string.Empty) 
                                {
                                    totValueTable.SetValue("PRICE_FIX",          ((ActivityPlan)myFunction).Price);                   // 2
                                    totValueTable.SetValue("DIST_KEY_PRICE_FIX", ((ActivityPlan)myFunction).DistributionKey);
                                    totValueTable.SetValue("PRICE_UNIT",          "00001");
                                } 
                                else 
                                {
                                    totValueTable.SetValue("PRICE_FIX",      "0");
                                }

                                    totValueTable.SetValue("ACTVTY_QTY",     ((ActivityPlan)myFunction).Quantity);                    // 1
                                    totValueTable.SetValue("DIST_KEY_QUAN",  ((ActivityPlan)myFunction).DistributionKey);             // 7
                                    
                                    //// ToDo
                                    
                                    totValueTable.SetValue("CURRENCY",       ((ActivityPlan)myFunction).TransactionCurrency);         // 14 
                                    
                                
                                myFunction.Updated = true;
                            }
                            catch(Exception ex)
                            {
                                myFunction.ValidationResult = ex.Message;                  
                            }
                        }
                    } 
                    catch (Exception exp) 
                    {
                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            myFunction.ValidationResult = exp.Message;
                        }
                    }

                    try 
                    {
                        sapValidateCostPlanFunction.Invoke(SapConnection.GetObject().CurrentDestination);
                    } 
                    catch (Exception ex) 
                    {

                        DialogResult r = MessageBox.Show("SAP Authorization Error: " + ex.Message, "Error", 
                                                         System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (r == DialogResult.OK) 
                        {

                        }

                        ReturnProgressDataForm.CancelProcess();
                        
                        return;
                    }


                    returnTable = sapValidateCostPlanFunction.GetTable("RETURN");

				
                    foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                    {
                        if (!SAPRequest.ReturnValuesList.ContainsKey(myFunction.Signature)) 
                        {
                            SAPRequest.ReturnValuesList.Add(myFunction.Signature, myFunction);
                        }
                    }

                    if (returnTable.RowCount > 0) 
                    {
                        try 
                        {
                            string logPath = LogFile.CheckCreateLogFolder() + "\\PWLogValOnly" + DateTime.Now.ToString("(dd-MMM-yyyy-HH-mm-ss-f)") + ".txt";
                            if (!File.Exists(logPath)) 
                            {
                                using (TextWriter writer = File.CreateText(logPath)) 
                                {
                                    writer.WriteLine("VALIDATION ONLY: " + DateTime.Now.ToString("(dd-MMM-yyyy-HH-mm-ss-f)"));
                                    writer.WriteLine(" ");
                                    for (int y = 0; y <= (returnTable.RowCount - 1); y += 1) 
                                    {
                                        for (int z = 0; z <= (returnTable[y].ElementCount - 1); z += 1) 
                                        {
                                            string par = returnTable[y][z].Metadata.Name;
                                            string val = returnTable[y].GetString(z);

                                            string messageLine = par + " : " + val;
                                            writer.WriteLine(messageLine);
                                        }
                                        writer.WriteLine(" ");
                                    }
                                }
                            }

                        } 
                        catch (Exception ex) 
                        {
                            //MessageBox.Show(ex.Message)
                        }

                        //REPLACE
                        ////////for (int j = 0; j <= (returnTable.RowCount - 1); j++) 
                        ////////{
                        ////////    int     xl4aKey  =  Convert.ToInt32(returnTable[j].GetString("XL4AKEY"));
                        ////////    string  message  =  returnTable[j].GetString("MESSAGE");

                        ////////    foreach (IPlanningWandFunction _function in activityPlanGroup.FunctionList) 
                        ////////    {
                        ////////        try
                        ////////        {
                        ////////            if( _function.Index == xl4aKey )
                        ////////            {
                        ////////                SAPRequest.ReturnValuesList.Values.Where((p, i) => p.Index == xl4aKey).First().Result  =  message;

                        ////////                /* -- Alt Method 1 -- */
                        ////////                //SAPRequest.ReturnValuesList.Values.ToList().ForEach(p => 
                        ////////                //{
                        ////////                //    if(p.Index == xl4aKey)
                        ////////                //    {
                        ////////                //        SAPRequest.ReturnValuesList[p.Signature].Result = message;
                        ////////                //    }
                        ////////                //});

                        ////////                /* -- Alt Method 2 -- */
                        ////////                //int k = SAPRequest.ReturnValuesList.Values.ToList().FindIndex(p => p.Index == xl4aKey);   
                        ////////                //SAPRequest.ReturnValuesList.Values.ElementAt(k).Result  =  message;
                        ////////                break;
                        ////////            }                                              
                        ////////        }
                        ////////        catch(Exception e)
                        ////////        {

                        ////////        }

                        ////////        if (GetPlanningDataForm.OperationCancelled) 
                        ////////        {
                        ////////            break;
                        ////////        }
                        ////////    }
                        ////////    if (GetPlanningDataForm.OperationCancelled) 
                        ////////    {
                        ////////        break;
                        ////////    }
                        ////////}


                        for (int j = 0;j <= (returnTable.RowCount - 1);j++)
                        {
                            int     row      =  Convert.ToInt32(returnTable[j].GetString("ROW")) - 1;
                            string  message  =  returnTable[j].GetString("MESSAGE");

                            if (row < 0)
                            {
                                row = 0;
                            }

                            string rType      =  string.Empty;
                            string messageV1  =  string.Empty;
                            string messageV2  =  string.Empty;
                            string messageV3  =  string.Empty;
                            string messageV4  =  string.Empty;
                            string rNumber    =  string.Empty;

                            rType = returnTable[j].GetString("TYPE");
                            messageV1 = returnTable[j].GetString("MESSAGE_V1");
                            messageV2 = returnTable[j].GetString("MESSAGE_V2");

                            for (int i = 0;i <= (activityPlanGroup.FunctionList.Count - 1);i++)
                            {
                                int elementLocation = SAPRequest.GetObject().TotalProcessedBySAP + i;
                                if (elementLocation < 0)
                                {
                                    elementLocation = 0;
                                }

                                string activityType =  ((ActivityPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).ActivityType;
                                string costCenter   =  ((ActivityPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostCenter;

                                try
                                {
                                    messageV1 = messageV1.TrimStart('0');
                                    messageV2 = messageV2.TrimStart('0');
                                }
                                catch (Exception ex)
                                {
                                }

                                try
                                {
                                    if (i == row && !string.IsNullOrEmpty(message))
                                    {
                                        SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                    }
                                    else if (i != row && rType == "E")
                                    {
                                        try
                                        {
                                            if (messageV1 == activityType || messageV2 == activityType)
                                            {
                                                // account for incrementing batch number 
                                                SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                            }
                                            else if (messageV1 != activityType && messageV1 != costCenter && row == 0)
                                            {
                                                rNumber = returnTable[j].GetString("NUMBER");
                                                if (rNumber != string.Empty)
                                                {
                                                    SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                                    if (ReturnProgressDataForm.OperationCancelled)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else if (rType == "I")
                                    {
                                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList)
                                        {
                                            SAPRequest.ReturnValuesList[myFunction.Signature].Result = message;
                                            if (ReturnProgressDataForm.OperationCancelled)
                                            {
                                                break;
                                            }
                                        }
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                if (ReturnProgressDataForm.OperationCancelled)
                                {
                                    break;
                                }
                            }
                            if (ReturnProgressDataForm.OperationCancelled)
                            {
                                break;
                            }
                        }


                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            if ( (SAPRequest.ReturnValuesList[myFunction.Signature].Result == null) || (SAPRequest.ReturnValuesList[myFunction.Signature].Result == string.Empty) ) 
                            {
                                SAPRequest.ReturnValuesList[myFunction.Signature].Result = "pwValidated";
                            }
                            if (ReturnProgressDataForm.OperationCancelled) 
                            {
                                break;
                            }
                        }
                    } 
                    else 
                    {
                        foreach (IPlanningFunction myFunction in activityPlanGroup.FunctionList) 
                        {
                            SAPRequest.ReturnValuesList[myFunction.Signature].Result = "pwValidated";
                            if (ReturnProgressDataForm.OperationCancelled) 
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

#endregion

#region properties

        public string Quantity
        {
            get
            {
                return this.m_quantity;
            }
            set
            {
                this.m_quantity  =  value;
            }
        }        
        
        public string Price
        {
            get
            {
                return this.m_price;
            }
            set
            {
                this.m_price  =  value;
            }
        }

        public string ControllingArea
        {
            get
            {
                return this.m_controllingArea;
            }
            set
            {
                this.m_controllingArea  =  value;
            }
        }

        public string FiscalYear
        {
            get
            {
                return this.m_fiscalYear;
            }
            set
            {
                this.m_fiscalYear  =  value;
            }
        }

        public string PeriodFrom
        {
            get
            {
                return this.m_perFrom;
            }
            set
            {
                this.m_perFrom  =  value;
            }
        }

        public string PeriodTo
        {
            get
            {
                return this.m_perTo;
            }
            set
            {
                this.m_perTo  =  value;
            }
        }

        public string DistributionKey
        {
            get
            {
                return this.m_distKey;
            }
            set
            {
                this.m_distKey  =  value;
            }
        }

        public string Version
        {
            get
            {
                return this.m_version;
            }
            set
            {
                this.m_version  =  value;
            }
        }

        public string DocumentHeaderText
        {
            get
            {
                return this.m_documentTxt;
            }
            set
            {
                this.m_documentTxt  =  value;
            }
        }

        public string PlanningCurrency
        {
            get
            {
                return this.m_currencyType;
            }
            set
            {
                this.m_currencyType  =  value;
            }
        }

        public string Delta
        {
            get
            {
                return this.m_delta;
            }
            set
            {
                this.m_delta  =  value;
            }
        }

        public string CostCenter
        {
            get
            {
                return this.m_costCenter;
            }
            set
            {
                this.m_costCenter  =  value;
            }
        }

        public string ActivityType
        {
            get
            {
                return this.m_activityType;
            }
            set
            {
                this.m_activityType  =  value;
            }
        }

        public string TransactionCurrency
        {
            get
            {
                return this.m_transactionCurrency;
            }
            set
            {
                this.m_transactionCurrency  =  value;
            }
        }

        public int ObjectIndex
        {
            get
            {
                return this.m_objectIndex;
            }
            set
            {
                this.m_objectIndex  =  value;
            }
        }

        public int ValueIndex
        {
            get
            {
                return this.m_valueIndex;
            }
            set
            {
                this.m_valueIndex  =  value;
            }
        }

        public int AttributeIndex
        {
            get
            {
                return this.m_attributeIndex;
            }
            set
            {
                this.m_attributeIndex  =  value;
            }
        }

#endregion

        public void Dispose()
	    {
		    this.Dispose();
	    }
    }
}
