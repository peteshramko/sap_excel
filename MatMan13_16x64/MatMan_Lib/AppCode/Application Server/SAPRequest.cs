
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using SAP.Middleware.Connector;
using System.Runtime.InteropServices;


namespace iiiwave.MatManLib
{
    public  delegate void FunctionProcessedBySAPHandler(object sender, FunctionProcessedBySAPEventArgs e);
    
    public class SAPRequest : IDisposable 
    {
		public event         FunctionProcessedBySAPHandler FunctionProcessedBySAP;	    
        
        private static       MatManReturnValueDictionary   m_returnValuesDictionary  =  new MatManReturnValueDictionary();
                		    
	    private DateTime? m_batchProcessTimeStart       =  null;  // Nullable DateTime of the update for process		    
	    private DateTime? m_batchProcessTimeComplete    =  null;  // Nullable DateTime of the last process		    
	    private TimeSpan m_currentBatchProcessTime;              // Processing time for current batch received		    
	    private volatile bool m_updateBatchComplete     =  false; // the CURRENT Batch of Functions has completed processing		    
	    private volatile int m_totalAddedToQueue        =  0;     // Number of functions processed in the current BATCH		    
	    //private volatile int m_currentBatchNumberProcessed = 0;  // Number of functions in the current BATCH processed by SAP		    
	    private volatile int m_totalProcessedBySAP      =  0;     // GRAND TOTAL of all functions from all batches processed by SAP
	    private volatile int m_batchNumber              =  1;
	    private volatile int m_functionCount            =  0;
        private volatile bool m_operationCancelled      =  false;
        private volatile int  m_maximumBatchSize        =  50;
        private volatile FunctionExecutionType  m_executionType  =  FunctionExecutionType.RetrievingData;
        
	    private List<IFunctionGroup>  m_functionBatchGroups;	    
        
	    /// <summary>
	    ///   Make accessor Thread-safe
	    /// </summary>	
	    private static SAPRequest m_sapRequest;

	    private static object syncRoot = new object();
	    public static SAPRequest GetObject()
	    {
		    if (SAPRequest.m_sapRequest == null) 
            {
			    lock (syncRoot) 
                {
				    if (SAPRequest.m_sapRequest == null) 
                    {
					    SAPRequest.m_sapRequest = new SAPRequest();
				    }
			    }
		    }
		    return SAPRequest.m_sapRequest;
	    }

        private SAPRequest()
	    {
            m_functionBatchGroups                        =  new List<IFunctionGroup>();
        }

	    #region "SAPRequest function handler"

	    public void ProcessSAPRequests(FunctionExecutionType _executionType, int _maxBatchSize)
        {
	        lock (syncRoot) 
            {
                //m_returnValuesDictionary.Clear();
                
                m_executionType           =  _executionType; 
                m_maximumBatchSize        =  _maxBatchSize;
                m_batchProcessTimeStart   =  DateTime.Now;

		        m_functionCount = 0;
		        //' reset to 0 for each batch
		        //m_currentBatchNumberProcessed = 0
		        // reset to 0 for each batch
		        m_updateBatchComplete = false;
		        // reset for each batch
		        m_batchNumber = 1;

		        if (MatManFunctionCollection.GetObject().Count > 0) 
                {
			        // ToDo: Log entry here

			        // Get all of the batched functions from the PWFunctionCollection
			        lock (syncRoot) 
                    {
				        m_functionBatchGroups = MatManFunctionCollection.GetObject().GetFunctionGroups();
			        }

			        //For Each _functionGroup As FunctionGroup In m_planningFunctionBatchGroups
			        for (int index = 0; index <= (m_functionBatchGroups.Count - 1); index++) 
                    {
				        IFunctionGroup _functionGroup = m_functionBatchGroups[index];
				
				        // Start of Batch Process
				        

				        if (!m_operationCancelled) 
                        {
                            if(_functionGroup.GetType() == typeof(PlanningFunctionGroup))
                            {
                                switch ( ((PlanningFunctionGroup)_functionGroup).FunctionType) 
                                {
						            case PlanningFunctionType.CostPlan:
                                        if (m_executionType == FunctionExecutionType.ValidateData) 
                                        {
								            CostPlan.ValidateSAPData( ((PlanningFunctionGroup)_functionGroup), m_functionCount );
							            } 
                                        else 
                                        {
								            CostPlan.PostSAPData( ((PlanningFunctionGroup)_functionGroup), m_functionCount );
							            }
                                        break;
                                    case PlanningFunctionType.ActivityPlan :
							            if (m_executionType == FunctionExecutionType.ValidateData) 
                                        {
								            ActivityPlan.ValidateSAPData( ((PlanningFunctionGroup)_functionGroup), m_functionCount );
							            } 
                                        else 
                                        {
								            ActivityPlan.PostSAPData( ((PlanningFunctionGroup)_functionGroup), m_functionCount );
							            }
							            break;
                                    default:
							            break;
                                }						        						
					        }
                            else if(_functionGroup.GetType() == typeof(QueryFunctionGroup))
                            {

                            }
				        } 
                        else 
                        {
					        //m_currentBatchNumberProcessed =  0;
					        m_totalProcessedBySAP         =  0;
					        m_batchNumber                 =  1;

					        MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue = 0;
					        MatManFunctionCollection.GetObject().Clear();

                            FunctionProcessedBySAP?.Invoke(this,new FunctionProcessedBySAPEventArgs(1,1));

                            break;
				        }

				        m_functionCount                =  m_functionCount               + _functionGroup.FunctionList.Count - 1;
				        //m_currentBatchNumberProcessed  =  m_currentBatchNumberProcessed + _functionGroup.FunctionList.Count;
				        m_totalProcessedBySAP          =  m_totalProcessedBySAP         + _functionGroup.FunctionList.Count;

				        m_totalAddedToQueue            =  MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue;

				        // Increment Batch Number
				        m_batchNumber++;

				        try 
                        {
                            //Invoke the FunctionProcessed event
                            FunctionProcessedBySAP?.Invoke(this,new FunctionProcessedBySAPEventArgs(m_totalAddedToQueue,m_totalProcessedBySAP));
                        } 
                        catch (Exception e1) 
                        {
				        }
			        }
			        // end foreach
			        // Re-assign the LAST BATCH UPDATE time to now.
			        m_batchProcessTimeComplete = DateTime.Now;

			        // Calculate the time to complete the batch process
			        m_currentBatchProcessTime = (DateTime)m_batchProcessTimeComplete - (DateTime)m_batchProcessTimeStart;

			        //Update the Batch to COMPLETE processing
			        m_updateBatchComplete = true;

		        } 
                else 
                {
			        //PWCalculationEngine.AcceptNewCalcs  =  True
		        }
	        }
        }


        #endregion

        public static MatManReturnValueDictionary ReturnValuesList
        {
            get
            {
                return m_returnValuesDictionary;
            }
        }

        public List<IFunctionGroup> FunctionBatchGroups 
        {
		    get 
            {
			    if (m_functionBatchGroups == null) 
                {
				    m_functionBatchGroups = new List<IFunctionGroup>();
			    }
			    return this.m_functionBatchGroups;
		    }
	    }

	    public bool UpdateBatchComplete 
        {
		    get 
            { 
                return m_updateBatchComplete; 
            }
	    }

	    public int TotalAddedToQueue 
        {
		    get 
            { 
                return m_totalAddedToQueue; 
            }
	    }

        //public int CurrentBatchNumberProcessed 
        //{
        //    get 
        //    { 
        //        return m_currentBatchNumberProcessed; 
        //    }
        //}

	    public int TotalProcessedBySAP 
        {
		    get 
            {
			    return m_totalProcessedBySAP;
		    }
		    set 
            {
			    m_totalProcessedBySAP = 0;
		    }
	    }

	    public TimeSpan CurrentBatchProcessTime 
        {
		    get 
            { 
                return m_currentBatchProcessTime; 
            }
	    }

	    public int BatchNumber 
        {
		    get 
            { 
                return m_batchNumber; 
            }
	    }
        
        public int MaximumBatchSize
        {
            get
            {
                return m_maximumBatchSize;
            }
        }

        public void Dispose()
        {
            m_totalAddedToQueue           =  0;
            // set current batch size to 0
            //m_currentBatchNumberProcessed = 0;
            // set the batch number processed to 0
            m_totalProcessedBySAP         =  0;
            // set the total run count to 0

            m_updateBatchComplete         =  false;
            m_batchProcessTimeStart       =  null;
            m_batchProcessTimeComplete    =  null;

            if(m_functionBatchGroups != null)
            {
                m_functionBatchGroups.Clear();
                m_functionBatchGroups         =  null;
            }
            

            //m_returnValuesDictionary.OnAddReturnValues  -=  MatManCalcEngine.GetObject()._SapReturn_OnAddReturnValues;

            //if(m_returnValuesDictionary != null)
            //{
            //    m_returnValuesDictionary.Clear();
            //}            
        }
    }


    public class FunctionProcessedBySAPEventArgs : EventArgs
    {
	    public int TotalSentToSAP 
        {
		    get 
            { 
                return m_totalSentToSAP; 
            }
		    private set 
            { 
                m_totalSentToSAP = value; 
            }
	    }
	    private int m_totalSentToSAP;
	    
        //public int CurrentBatchNumberProcessed
        //{
        //    get 
        //    { 
        //        return m_currentBatchNumberProcessed; 
        //    }
        //    private set 
        //    { 
        //        m_currentBatchNumberProcessed = value; 
        //    }
        //}
        //private int m_currentBatchNumberProcessed;

	    public int TotalFunctionsProcessedbySAP 
        {
		    get 
            { 
                return m_totalFunctionsProcessedbySAP; 
            }
		    private set 
            { 
                m_totalFunctionsProcessedbySAP = value; 
            }
	    }

	    private int m_totalFunctionsProcessedbySAP;

	    public FunctionProcessedBySAPEventArgs(int _totalSentToSAP, int _totalProcessed)
	    {
		    TotalSentToSAP = _totalSentToSAP;
		    //CurrentBatchNumberProcessed = _batchProcessed;
		    TotalFunctionsProcessedbySAP = _totalProcessed;
	    }
    }

    public class BatchCompleteProcessingEventArgs : EventArgs
    {
	    public int TotalSentToSAP 
        {
		    get 
            { 
                return m_totalSentToSAP; 
            }
		    private set 
            { 
                m_totalSentToSAP = value; 
            }
	    }
	    private int m_totalSentToSAP;

	    public int CurrentBatchNumberProcessed 
        {
		    get 
            { 
                return m_currentBatchNumberProcessed; 
            }
		    private set 
            { 
                m_currentBatchNumberProcessed = value; 
            }
	    }

	    private int m_currentBatchNumberProcessed;

	    public int TotalFunctionsProcessedbySAP 
        {
		    get 
            { 
                return m_totalFunctionsProcessedbySAP; 
            }
		    private set 
            { 
                m_totalFunctionsProcessedbySAP = value; 
            }
	    }
	    private int m_totalFunctionsProcessedbySAP;
	    
        public TimeSpan BatchProcessTime 
        {
		    get 
            { 
                return m_batchProcessTime; 
            }
		    private set 
            { 
                m_batchProcessTime = value; 
            }
	    }
	    private TimeSpan m_batchProcessTime;
	    
        public BatchCompleteProcessingEventArgs(int _totalSentToSap, int _batchProcessed, int _totalProcessed, TimeSpan _timeFromLastProcess)
	    {
		    TotalSentToSAP                =  _totalSentToSap;
		    CurrentBatchNumberProcessed   =  _batchProcessed;
		    TotalFunctionsProcessedbySAP  =  _totalProcessed;
		    BatchProcessTime              =  _timeFromLastProcess;
        
	    }
    }
}