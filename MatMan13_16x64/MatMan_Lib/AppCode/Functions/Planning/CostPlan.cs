using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAP.Middleware.Connector;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace iiiwave.MatManLib
{
    class CostPlan : IPlanningFunction, IDisposable
    {
        private const int m_totalNumberOfParameters  =  22; // index 21 and 22 not used
            
	    private string m_fixedInputValue             =  String.Empty;  // 1		    
	    private string m_controllingArea             =  String.Empty;  // 2		    
	    private string m_fiscalYear                  =  String.Empty;  // 3		   
	    private string m_periodFrom                  =  String.Empty;  // 4		   
	    private string m_periodTo                    =  String.Empty;  // 5		    
	    private string m_distributionKey             =  String.Empty;  // 6		    
	    private string m_version                     =  String.Empty;  // 7		    
	    private string m_documentHeaderText          =  String.Empty;  // 8		   
	    private string m_planningCurrency            =  String.Empty;  // 9		    
	    private string m_delta                       =  String.Empty;  // 10		    
	    private string m_costCenter                  =  String.Empty;  // 11		    
	    private string m_costElement                 =  String.Empty;  // 12		    
	    private string m_activityType                =  String.Empty;  // 13		    
	    private string m_orderID                     =  String.Empty;  // 14		    
	    private string m_wbsElement                  =  String.Empty;  // 15		    
	    private string m_functionalArea              =  String.Empty;  // 16		    
	    private string m_fund                        =  String.Empty;  // 17		    
	    private string m_grant                       =  String.Empty;  // 18		    
	    private string m_transactionCurrency         =  String.Empty;  // 19

    // increments with Cost Center in group
        private int    m_objectIndex;
    // increments with Cost Element in group
        private int    m_valueIndex;

	    private static object m_syncObject = new object();


        public CostPlan(int topicId, ref System.Array inputStrings, ref string validationString)
	    {

		    // Set to new request
		    this.Updated = false;
		    //

		    string[] requestStrings = inputStrings.Cast<string>().ToArray();

		    // add empty string values to fill out array (let SAP do validation)
		    if (requestStrings.Length < m_totalNumberOfParameters) 
            {
			    List<string> ls = new List<string>(requestStrings);
			    while (ls.Count < m_totalNumberOfParameters) 
                {
				    ls.Add(String.Empty);
			    }
			    requestStrings = ls.ToArray();
		    }


		    this.TopicID                    =  topicId;
		    this.Hash                       =  inputStrings.Concatenate();
            this.FunctionType               =  PlanningFunctionType.CostPlan;

            this.m_fixedInputValue          =  requestStrings[1].ToUpper();        // [TOTVALUE][FIX_VALUE]
		    this.m_controllingArea          =  requestStrings[2].ToUpper();        // [HEADERINFO][CO_AREA]
		    this.m_fiscalYear               =  requestStrings[3].ToUpper();        // [HEADERINFO][FISC_YEAR]
		    this.m_periodFrom               =  requestStrings[4].ToUpper();        // [HEADERINFO][PERIOD_FROM]
		    this.m_periodTo                 =  requestStrings[5].ToUpper();        // [HEADERINFO][PERIOD_TO]
		    this.m_distributionKey          =  requestStrings[6].ToUpper();        // [TOTVALUE][DIST_KEY_FIX_VAL]
		    this.m_version                  =  requestStrings[7].ToUpper();        // [HEADERINFO][VERSION]
		    this.m_documentHeaderText       =  requestStrings[8].ToUpper();        // [HEADERINFO][DOC_HDR_TXT]
		    this.m_planningCurrency         =  requestStrings[9].ToUpper();       // [HEADERINFO][PLAN_CURRTYPE]
		    this.m_delta                    =  requestStrings[10].ToUpper();       // [DELTA][DELTA]
		    this.m_costCenter               =  requestStrings[11].ToUpper();       // [COOBJECT][COSTCENTER]
		    this.m_costElement              =  requestStrings[12].ToUpper();       // [TOTVALUE][COST_ELEM]
		    this.m_activityType             =  requestStrings[13].ToUpper();       // [COOBJECT][ACTTYPE]
		    this.m_orderID                  =  requestStrings[14].ToUpper();       // [COOBJECT][ORDERID]
		    this.m_wbsElement               =  requestStrings[15].ToUpper();       // [COOBJECT][WBS_ELEMENT]
		    this.m_functionalArea           =  requestStrings[16].ToUpper();       // [TOTVALUE][FUNCTION]
		    this.m_fund                     =  requestStrings[17].ToUpper();       // [TOTVALUE][FUND]
		    this.m_grant                    =  requestStrings[18].ToUpper();       // [TOTVALUE][GRANT_NBR]
		    this.m_transactionCurrency      =  requestStrings[19].ToUpper();       // [TOTVALUE][TRANS_CURR]

		    this.Signature                  =  requestStrings[requestStrings.Length - 1].ToUpper();

            if (this.m_functionalArea != string.Empty) 
            {
	            this.m_functionalArea = this.m_functionalArea.TrimStart('0');
	            this.m_functionalArea = "000" + this.m_functionalArea;
            }

            if (this.m_wbsElement != string.Empty)
            {
                this.m_wbsElement     =  this.m_wbsElement.TrimStart(' ');
                if( this.m_wbsElement.ToUpper().StartsWith("WBS") )
                    this.m_wbsElement  =  this.m_wbsElement.Remove(0, 3);
                this.m_wbsElement     =  this.m_wbsElement.TrimStart(' ');
            }

            IPlanningFunction thisFunc = (IPlanningFunction)this;

		    MatManErrorDictionary.GetObject().ValidatePlanningFunction(ref thisFunc, ref validationString);
	    }


        #region old validate function

        public static void ValidateSAPData(PlanningFunctionGroup costPlanGroup,int functionCount)
        {
            lock (m_syncObject)
            {
                if (costPlanGroup.FunctionList.Count > 0)
                {

                    IRfcFunction sapValidateCostPlanFunction = SapConnection.GetObject().CurrentDestination.Repository.CreateFunction("BAPI_COSTACTPLN_CHECKPRIMCOST");
                    IRfcTable returnTable = null;

                    try
                    {
                        ///*** --- IMPORT (SAP TAB) -------------------------------***//
                        IRfcStructure headerInfoStructure = sapValidateCostPlanFunction.GetStructure("HEADERINFO"); // HEADER_INFO


                        headerInfoStructure.SetValue("CO_AREA",        ((CostPlan)costPlanGroup.FunctionList[0]).ControllingArea);    // 2				    
                        headerInfoStructure.SetValue("FISC_YEAR",      ((CostPlan)costPlanGroup.FunctionList[0]).FiscalYear);         // 3					    
                        headerInfoStructure.SetValue("PERIOD_FROM",    ((CostPlan)costPlanGroup.FunctionList[0]).PeriodFrom);         // 4					     
                        headerInfoStructure.SetValue("PERIOD_TO",      ((CostPlan)costPlanGroup.FunctionList[0]).PeriodTo);           // 5					    
                        headerInfoStructure.SetValue("VERSION",        ((CostPlan)costPlanGroup.FunctionList[0]).Version);            // 7					    
                        headerInfoStructure.SetValue("DOC_HDR_TX",     ((CostPlan)costPlanGroup.FunctionList[0]).DocumentHeaderText); // 8					    
                        headerInfoStructure.SetValue("PLAN_CURRTYPE",  ((CostPlan)costPlanGroup.FunctionList[0]).PlanningCurrency);   // 9					    
                        sapValidateCostPlanFunction.SetValue("DELTA",  ((CostPlan)costPlanGroup.FunctionList[0]).Delta);              // 10					    

                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                        {
                            try
                            {
                                IRfcTable coObjectTable = sapValidateCostPlanFunction.GetTable("COOBJECT");
                                // OBJECT

                                string objectIndex  =  ((CostPlan)myFunction).ObjectIndex.ToString("000000");
                                string valueIndex   =  ((CostPlan)myFunction).ValueIndex.ToString("000000");

                                coObjectTable.Append();

                                try
                                {
                                    if (coObjectTable.GetValue("OBJECT_INDEX") != null)
                                    {
                                        if (coObjectTable.GetValue("OBJECT_INDEX").ToString() != objectIndex)
                                        {
                                            coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                            // Calculated
                                        }
                                    }
                                    else
                                    {
                                        coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                        // Calculated
                                    }
                                }
                                catch (Exception ex)
                                {
                                    coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                }

                                coObjectTable.SetValue("COSTCENTER",   ((CostPlan)myFunction).CostCenter);                  // 11						    
                                coObjectTable.SetValue("ACTTYPE",      ((CostPlan)myFunction).ActivityType);                // 13						    
                                coObjectTable.SetValue("ORDERID",      ((CostPlan)myFunction).OrderID);                     // 14						           
                                coObjectTable.SetValue("WBS_ELEMENT",  ((CostPlan)myFunction).WBSElement);                  // 15						    

                                IRfcTable totValueTable = sapValidateCostPlanFunction.GetTable("TOTVALUE");                 // TOT_VALUE						    

                                totValueTable.Append();

                                totValueTable.SetValue("VALUE_INDEX",   ((CostPlan)myFunction).ValueIndex.ToString("000000"));
                                // Calculated

                                if (((CostPlan)myFunction).FixedInputValue != string.Empty)
                                {
                                    totValueTable.SetValue("FIX_VALUE",   ((CostPlan)myFunction).FixedInputValue);          // 1
                                }
                                else
                                {
                                    totValueTable.SetValue("FIX_VALUE",   "0");
                                }

                                totValueTable.SetValue("DIST_KEY_FIX_VAL",  ((CostPlan)myFunction).DistributionKey);        // 6
                                totValueTable.SetValue("COST_ELEM",         ((CostPlan)myFunction).CostElement);            // 12
                                totValueTable.SetValue("FUNCTION",          ((CostPlan)myFunction).FunctionalArea);         // 16
                                totValueTable.SetValue("FUND",              ((CostPlan)myFunction).Fund);                   // 17
                                totValueTable.SetValue("GRANT_NBR",         ((CostPlan)myFunction).Grant);                  // 18
                                totValueTable.SetValue("TRANS_CURR",        ((CostPlan)myFunction).TransactionCurrency);    // 19
                                
                                IRfcTable indexTable = sapValidateCostPlanFunction.GetTable("INDEXSTRUCTURE");

                                indexTable.Append();
                                indexTable.SetValue("OBJECT_INDEX",         ((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                indexTable.SetValue("VALUE_INDEX",          ((CostPlan)myFunction).ValueIndex.ToString("000000"));
                                indexTable.SetValue("ATTRIB_INDEX",         "000000");

                                myFunction.Updated = true;
                            }
                            catch (Exception ex)
                            {
                                myFunction.ValidationResult = ex.Message;
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
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


                    foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                    {
                        if (!SAPRequest.ReturnValuesList.ContainsKey(myFunction.Signature))
                        {
                            SAPRequest.ReturnValuesList.Add(myFunction.Signature,myFunction);
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
                                    for (int y = 0;y <= (returnTable.RowCount - 1);y += 1)
                                    {
                                        for (int z = 0;z <= (returnTable[y].ElementCount - 1);z += 1)
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

                            rType     = returnTable[j].GetString("TYPE");
                            messageV1 = returnTable[j].GetString("MESSAGE_V1");
                            messageV2 = returnTable[j].GetString("MESSAGE_V2");

                            for (int i = 0;i <= (costPlanGroup.FunctionList.Count - 1);i++)
                            {
                                int elementLocation = SAPRequest.GetObject().TotalProcessedBySAP + i;
                                if (elementLocation < 0)
                                {
                                    elementLocation = 0;
                                }

                                string costElement = ((CostPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostElement;
                                string costCenter  = ((CostPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostCenter;

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
                                            if (messageV1 == costElement || messageV2 == costElement)
                                            {
                                                // account for incrementing batch number 
                                                SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                            }
                                            else if (messageV1 != costElement && messageV1 != costCenter && row == 0)
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
                                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
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
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                        {
                            if (SAPRequest.ReturnValuesList[myFunction.Signature].Result == null | SAPRequest.ReturnValuesList[myFunction.Signature].Result == string.Empty)
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
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
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

        #region old post function

        public static void PostSAPData(PlanningFunctionGroup costPlanGroup,int functionCount)
        {
            lock (m_syncObject)
            {
                if (costPlanGroup.FunctionList.Count > 0)
                {
                    IRfcFunction sapPostCostPlanFunction  =  SapConnection.GetObject().CurrentDestination.Repository.CreateFunction("BAPI_COSTACTPLN_POSTPRIMCOST");
                    IRfcFunction sapCommitWorkFunction    =  SapConnection.GetObject().CurrentDestination.Repository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                    IRfcTable    returnTable              =  null;
                    
                    try
                    {
                        IRfcStructure headerInfoStructure = sapPostCostPlanFunction.GetStructure("HEADERINFO");                                 // HEADER_INFO
                        
                        headerInfoStructure.SetValue("CO_AREA",        ((CostPlan)costPlanGroup.FunctionList[0]).ControllingArea);    // 3				    
                        headerInfoStructure.SetValue("FISC_YEAR",      ((CostPlan)costPlanGroup.FunctionList[0]).FiscalYear);         // 4					    
                        headerInfoStructure.SetValue("PERIOD_FROM",    ((CostPlan)costPlanGroup.FunctionList[0]).PeriodFrom);         // 5					     
                        headerInfoStructure.SetValue("PERIOD_TO",      ((CostPlan)costPlanGroup.FunctionList[0]).PeriodTo);           // 6					    
                        headerInfoStructure.SetValue("VERSION",        ((CostPlan)costPlanGroup.FunctionList[0]).Version);            // 8					    
                        headerInfoStructure.SetValue("DOC_HDR_TX",     ((CostPlan)costPlanGroup.FunctionList[0]).DocumentHeaderText); // 9					    
                        headerInfoStructure.SetValue("PLAN_CURRTYPE",  ((CostPlan)costPlanGroup.FunctionList[0]).PlanningCurrency);   // 10					    
                        sapPostCostPlanFunction.SetValue("DELTA",      ((CostPlan)costPlanGroup.FunctionList[0]).Delta);              // 11					

                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                        {
                            try
                            {

                                IRfcTable coObjectTable = sapPostCostPlanFunction.GetTable("COOBJECT");                                   // OBJECT

                                string objectIndex  =  ((CostPlan)myFunction).ObjectIndex.ToString("000000");
                                string valueIndex   =  ((CostPlan)myFunction).ValueIndex.ToString("000000");

                                coObjectTable.Append();

                                try
                                {
                                    if (coObjectTable.GetValue("OBJECT_INDEX") != null)
                                    {
                                        if (((string)coObjectTable.GetValue("OBJECT_INDEX")) != objectIndex)
                                        {
                                            coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));  // Calculated
                                        }
                                    }
                                    else
                                    {
                                        coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));     // Calculated
                                    }
                                }
                                catch (Exception ex)
                                {
                                    coObjectTable.SetValue("OBJECT_INDEX",((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                }

                                coObjectTable.SetValue("COSTCENTER",   ((CostPlan)myFunction).CostCenter);                  // 11						    
                                coObjectTable.SetValue("ACTTYPE",      ((CostPlan)myFunction).ActivityType);                // 13						    
                                coObjectTable.SetValue("ORDERID",      ((CostPlan)myFunction).OrderID);                     // 14						           
                                coObjectTable.SetValue("WBS_ELEMENT",  ((CostPlan)myFunction).WBSElement);                  // 15	

                                IRfcTable totValueTable = sapPostCostPlanFunction.GetTable("TOTVALUE");                         // TOT_VALUE

                                totValueTable.Append();
                                totValueTable.SetValue("VALUE_INDEX",((CostPlan)myFunction).ValueIndex.ToString("000000"));     // Calculated

                                if (((CostPlan)myFunction).FixedInputValue != string.Empty)
                                {
                                    totValueTable.SetValue("FIX_VALUE",  ((CostPlan)myFunction).FixedInputValue);               // 2
                                }
                                else
                                {
                                    totValueTable.SetValue("FIX_VALUE","0");
                                }

                                totValueTable.SetValue("DIST_KEY_FIX_VAL",  ((CostPlan)myFunction).DistributionKey);        // 6
                                totValueTable.SetValue("COST_ELEM",         ((CostPlan)myFunction).CostElement);            // 12
                                totValueTable.SetValue("FUNCTION",          ((CostPlan)myFunction).FunctionalArea);         // 16
                                totValueTable.SetValue("FUND",              ((CostPlan)myFunction).Fund);                   // 17
                                totValueTable.SetValue("GRANT_NBR",         ((CostPlan)myFunction).Grant);                  // 18
                                totValueTable.SetValue("TRANS_CURR",        ((CostPlan)myFunction).TransactionCurrency);    // 19
                                

                                IRfcTable indexTable = sapPostCostPlanFunction.GetTable("INDEXSTRUCTURE");
                                // IDX_STRUCTURE

                                indexTable.Append();
                                indexTable.SetValue("OBJECT_INDEX",          ((CostPlan)myFunction).ObjectIndex.ToString("000000"));
                                indexTable.SetValue("VALUE_INDEX",           ((CostPlan)myFunction).ValueIndex.ToString("000000"));
                                indexTable.SetValue("ATTRIB_INDEX",          "000000");

                                myFunction.Updated = true;
                            }
                            catch (Exception ex)
                            {
                                myFunction.ValidationResult = ex.Message;
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                        {
                            myFunction.ValidationResult = exp.Message;
                        }
                    }

                    try
                    {
                        RfcSessionManager.BeginContext(SapConnection.GetObject().CurrentDestination);

                            sapPostCostPlanFunction.Invoke(SapConnection.GetObject().CurrentDestination);
                            sapCommitWorkFunction.Invoke(SapConnection.GetObject().CurrentDestination);

                        RfcSessionManager.EndContext(SapConnection.GetObject().CurrentDestination);
                    }
                    catch (Exception ex)
                    {
                        DialogResult r = MessageBox.Show("SAP Authorization Error: " + ex.Message, "Error",
                                                          System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                          MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (r == DialogResult.OK)
                        {

                        }

                        ReturnProgressDataForm.CancelProcess();

                        return;
                    }


                    returnTable = sapPostCostPlanFunction.GetTable("RETURN");


                    foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                    {
                        if (!SAPRequest.ReturnValuesList.ContainsKey(myFunction.Signature))
                        {
                            SAPRequest.ReturnValuesList.Add(myFunction.Signature,myFunction);
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
                                    writer.WriteLine("VALIDATION AND POST: " + DateTime.Now.ToString("(dd-MMM-yyyy-HH-mm-ss-f)"));
                                    writer.WriteLine(" ");
                                    for (int y = 0;y <= (returnTable.RowCount - 1);y += 1)
                                    {
                                        for (int z = 0;z <= (returnTable[y].ElementCount - 1);z += 1)
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

                            for (int i = 0;i <= (costPlanGroup.FunctionList.Count - 1);i++)
                            {
                                int elementLocation = SAPRequest.GetObject().TotalProcessedBySAP + i;
                                if (elementLocation < 0)
                                {
                                    elementLocation = 0;
                                }

                                string costElement = ((CostPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostElement;
                                string costCenter  = ((CostPlan)SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation)).CostCenter;

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
                                            if (messageV1 == costElement || messageV2 == costElement)
                                            {
                                                // account for incrementing batch number 
                                                SAPRequest.ReturnValuesList.Values.ElementAt(elementLocation).Result = message;
                                            }
                                            else if (messageV1 != costElement && messageV1 != costCenter && row == 0)
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
                                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
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
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
                        {
                            if (SAPRequest.ReturnValuesList[myFunction.Signature].Result == null | SAPRequest.ReturnValuesList[myFunction.Signature].Result == string.Empty)
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
                        foreach (IPlanningFunction myFunction in costPlanGroup.FunctionList)
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

        public string FixedInputValue 
        {
		    get 
            { 
                return this.m_fixedInputValue; 
            }
		    set 
            { 
                this.m_fixedInputValue = value; 
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
                this.m_controllingArea = value;
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
                this.m_fiscalYear = value; 
            }
	    }

	    public string PeriodFrom 
        {
		    get 
            { 
                return this.m_periodFrom; 
            }
		    set 
            { 
                this.m_periodFrom = value; 
            }
	    }

	    public string PeriodTo 
        {
		    get 
            { 
                return this.m_periodTo; 
            }
		    set 
            { 
                this.m_periodTo = value; 
            }
	    }

	    public string DistributionKey 
        {
		    get 
            { 
                return this.m_distributionKey; 
            }
		    set 
            { 
                this.m_distributionKey = value; 
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
                this.m_version = value; 
            }
	    }

	    public string DocumentHeaderText 
        {
		    get 
            { 
                return this.m_documentHeaderText; 
            }
		    set 
            { 
                this.m_documentHeaderText = value; 
            }
	    }

	    public string PlanningCurrency 
        {
		    get 
            { 
                return this.m_planningCurrency; 
            }
		    set 
            { 
                this.m_planningCurrency = value; 
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
                this.m_delta = value; 
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
                this.m_costCenter = value; 
            }
	    }

	    public string CostElement 
        {
		    get 
            { 
                return this.m_costElement; 
            }
		    set 
            { 
                this.m_costElement = value; 
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
                this.m_activityType = value; 
            }
	    }

	    public string OrderID 
        {
		    get 
            { 
                return this.m_orderID; 
            }
		    set 
            { 
                this.m_orderID = value; 
            }
	    }

	    public string WBSElement 
        {
		    get 
            { 
                return this.m_wbsElement; 
            }
		    set 
            { 
                this.m_wbsElement = value; 
            }
	    }

	    public string FunctionalArea 
        {
		    get 
            { 
                return this.m_functionalArea; 
            }
		    set 
            { 
                this.m_functionalArea = value; 
            }
	    }

	    public string Fund 
        {
		    get 
            { 
                return this.m_fund; 
            }
		    set 
            { 
                this.m_fund = value; 
            }
	    }

	    public string Grant 
        {
		    get 
            { 
                return this.m_grant; 
            }
		    set 
            { 
                this.m_grant = value; 
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
                this.m_transactionCurrency = value; 
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

        public void Dispose()
	    {
		    this.Dispose();
	    }
    }
}
