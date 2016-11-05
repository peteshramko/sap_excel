using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using iiiwave.MatManLib.Properties;

namespace iiiwave.MatManLib
{
    public  delegate void OnUpdateValuesEventHandler();
    public  delegate void OnDataFormClosingEventHandler();

    public partial class ReturnProgressDataForm:Form
    {
        public  DateTime               ProcessStartTime;
        public  TimeSpan               ElapsedTime;
        private BackgroundWorker       myBackgroundWorker; 
        private static volatile bool   m_cancelOperation   =  false;
        private volatile int           m_Progress          =  0;
        private volatile bool          m_isRunning         =  false;
        private volatile int           m_connectDataCount  =  0;
        private static volatile bool   m_executeFunctions  =  false;
        
        public  event         OnUpdateValuesEventHandler     OnUpdateValues;
        public  event         OnDataFormClosingEventHandler  OnDataFormClosing;
        

        private object       ThreadLockObj = new object();
        
        public ReturnProgressDataForm()
        {
            InitializeComponent();

            // Initialize components
		    myBackgroundWorker = new BackgroundWorker();
		    ProcessStartTime   = new DateTime();
		    ElapsedTime        = new TimeSpan();

		    System.Threading.ThreadPool.SetMaxThreads(5, 5);

		    // Assign properties to BackgroundWorker
		    this.myBackgroundWorker.WorkerReportsProgress        = true;
		    this.myBackgroundWorker.WorkerSupportsCancellation   = true;

            this.myBackgroundWorker.DoWork                      +=  myBackgroundWorker_DoWork;
            this.myBackgroundWorker.ProgressChanged             +=  myBackgroundWorker_ProgressChanged;
            this.myBackgroundWorker.RunWorkerCompleted          +=  myBackgroundWorker_RunWorkerCompleted;
        }

        /// <summary>
	    /// OnLoad is overidden to provide localization strings for the form
	    /// </summary>
	    /// <param name="e"></param>
	    protected override void OnLoad(EventArgs e)
	    {           

		    // FunctionAdded & Function Removed EventHandlers
            MatManFunctionCollection.GetObject().OnFunctionAdded    +=  functionCollection_OnFunctionAdded;
            MatManFunctionCollection.GetObject().OnFunctionRemoved  +=  functionCollection_OnFunctionRemoved;

            //// FunctionProcessed and BatchCompleted EventHandlers
            SAPRequest.GetObject().FunctionProcessedBySAP       +=  sapRequest_FunctionProcessedBySAP;		    

            m_connectDataCount  =  0;        

		    this.uxCalculationTimeLabel.Text                     =  iiiwave.MatManLib.Localization.Localize.ReturnProgressDataForm_uxCalculationTimeLabel_Text;
		    this.uxBalanceRecordsSummedLabel.Text                =  iiiwave.MatManLib.Localization.Localize.ReturnProgressDataForm_uxBalanceRecordsSummedLabel_Text;
		    this.uxCalculationCountLabel.Text                    =  iiiwave.MatManLib.Localization.Localize.ReturnProgressDataForm_uxCalculationCountLabel_Text;
		    this.uxCancelButton.Text                             =  iiiwave.MatManLib.Localization.Localize.ReturnProgressDataForm_uxCancelButton_Text;
		    
		    if (Settings.Default.FunctionExecutionType == (int)FunctionExecutionType.RetrievingData) 
            {
			    this.Text = "Retrieving Data";
		    } 
            else if (Settings.Default.FunctionExecutionType == (int)FunctionExecutionType.ValidateData)
            {
			    this.Text = "Validating Data";
		    }
            else
            {
                this.Text = "Posting Data";
            }

		    this.uxCancelButton.Enabled = false;
		    m_cancelOperation = false;

		    // Initialize Form
		    this.uxProgressBar.Value = 0;
		    this.m_Progress = 0;

		    // Initialize counters and booleans
		    MatManFunctionCollection.GetObject().Clear();
		    
		    // Start the background worker
		    myBackgroundWorker.WorkerReportsProgress       =  true;
		    myBackgroundWorker.WorkerSupportsCancellation  =  true;

		    myBackgroundWorker.RunWorkerAsync();
	    }

        /// <summary>
	    /// Sends the Batch to SAP - This would be part of processing functions phase
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
	    private void myBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	    {

		    this.ProcessStartTime = DateTime.Now;

		    MatManFunctionCollection.GetObject().Clear();
		    MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue = 0;

		    //SAPRequest.GetObject().FunctionBatch.Clear()
		    SAPRequest.GetObject().TotalProcessedBySAP = 0;

		    m_cancelOperation = false;

		    Thread.Sleep(2000);

            while (true) 
            {
						
                if ((MatManFunctionCollection.GetObject().Count > 0 && m_executeFunctions && !m_isRunning)) 
                {
                    m_isRunning = true;
                    FunctionExecutionType functionType = (FunctionExecutionType)Properties.Settings.Default.FunctionExecutionType;
                    SAPRequest.GetObject().ProcessSAPRequests(functionType, Settings.Default.MaximumBatchSize);
                }

                if (m_cancelOperation) 
                {
                    MatManFunctionCollection.GetObject().StillAddingFunctions = true;
                    m_executeFunctions = false;
                    m_isRunning = true;
                    break; // TODO: might not be correct. Was : Exit While
                }

                System.Threading.Thread.Sleep(250);

                //' Breaks us out of while loop
                if ((SAPRequest.GetObject().TotalProcessedBySAP == MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue) && m_Progress == 100) 
                {
                    MatManFunctionCollection.GetObject().StillAddingFunctions = true;
                    m_executeFunctions = false;
                    break; 
                }

                if ((MatManFunctionCollection.GetObject().Count == 0)) 
                {
                    break; 
                }
            }            
		}
              

        public void UpdateStatus(string statusStripMessage, bool incrementProgress = true)
	    {

		    //SyncLock ThreadLockObj

		    bool stillAddingFunctions       =  MatManFunctionCollection.GetObject().StillAddingFunctions;
		    //int functionsRemainingInQueue  = PWFunctionCollection.GetObject().Count;
		    int totalFunctionsAddedToQueue  =  MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue + 1;
		    int totalProcessedBySAP         =  SAPRequest.GetObject().TotalProcessedBySAP;

		    this.ElapsedTime = DateTime.Now - this.ProcessStartTime;

		    if (totalProcessedBySAP < 1) 
            {
			    // report progress of RTD reading functions into batch

			    if (ElapsedTime.TotalSeconds > 0) 
                {
				    // Update the CalculationCount text box
				    this.uxCalculationCountTextBox.Text = totalFunctionsAddedToQueue.ToString() + " functions in processing queue...";

				    // Update the Balance Records Text Box
				    this.uxBalanceRecordsSummedTextBox.Text = totalFunctionsAddedToQueue.ToString() + "...pending";

				    // Update the CalculationTimeTextBox
				    decimal processRate = Convert.ToDecimal(totalFunctionsAddedToQueue / ElapsedTime.TotalSeconds);
				    uxCalculationTimeTextBox.Text = ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + " at " + processRate.ToString("0.00") + " cells/sec";

				    StatusStrip.Text = statusStripMessage;
			    }
		    } 
            else if (totalProcessedBySAP >= 1) 
            {
			    if (ElapsedTime.TotalSeconds > 0) 
                {
				    //Update the CalculationCount text box
				    this.uxCalculationCountTextBox.Text = totalFunctionsAddedToQueue.ToString() + " functions being processed by SAP...";

				    // Update the Balance Records Text Box
				    this.uxBalanceRecordsSummedTextBox.Text = totalProcessedBySAP.ToString();

				    // Update the CalculationTimeTextBox
				    decimal processRate = Convert.ToDecimal(totalProcessedBySAP / ElapsedTime.TotalSeconds);
				    uxCalculationTimeTextBox.Text = ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + " at " + processRate.ToString("0.00") + " cells/sec";

				    StatusStrip.Text = statusStripMessage;
			    }
		    }

		    this.Refresh();
	    }

        private void myBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	    {

		    lock (ThreadLockObj) 
            {

			    bool stillAddingFunctions  =  MatManFunctionCollection.GetObject().StillAddingFunctions;
			    int remainingInQueue       =  MatManFunctionCollection.GetObject().Count;
			    int totalAddedToQueue      =  MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue;
			    int totalProcessedBySAP    =  SAPRequest.GetObject().TotalProcessedBySAP;

                decimal progressDec        =  ((decimal)totalProcessedBySAP / (decimal)totalAddedToQueue) * 100;
				m_Progress                 =  (int)progressDec;

                try 
                {
				    this.ElapsedTime = DateTime.Now - this.ProcessStartTime;

				    if (m_cancelOperation) 
                    {
					    this.uxProgressBar.Value = 100;
					    this.Refresh();
					    Application.DoEvents();
				    } 
                    else if (stillAddingFunctions) 
                    {
					    if (totalProcessedBySAP < totalAddedToQueue) 
                        {
						    //this.uxToolStripStatusLabel.Text         =  Convert.ToString(e.UserState);
						    this.uxCalculationCountTextBox.Text = "SAP is processing functions...";
						    this.uxBalanceRecordsSummedTextBox.Text = "Total Functions Queued: " + totalAddedToQueue.ToString() + " -- Processing: " + totalProcessedBySAP.ToString();


						    // Items being processed by SAP - Update the CalculationTimeTextBox
						    decimal processRate = Convert.ToDecimal(totalProcessedBySAP / ElapsedTime.TotalSeconds);
						    uxCalculationTimeTextBox.Text = totalProcessedBySAP.ToString() + " Functions processed by SAP at " + ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + "- Process Rate: " + processRate.ToString("0.00") + " cells/sec";

						    this.uxProgressBar.Value = m_Progress;

						    this.Refresh();
						    // (2) Done
					    } 
                        else if (totalProcessedBySAP == totalAddedToQueue) 
                        {
						    //this.uxToolStripStatusLabel.Text         =  Convert.ToString(e.UserState);
						    this.uxCalculationCountTextBox.Text = "SAP is processing functions...";
						    this.uxBalanceRecordsSummedTextBox.Text = "Total Functions Queued: " + totalAddedToQueue.ToString() + " -- Complete: " + totalProcessedBySAP.ToString();


						    // Items being processed by SAP - Update the CalculationTimeTextBox
						    decimal processRate = Convert.ToDecimal(totalProcessedBySAP / ElapsedTime.TotalSeconds);
						    uxCalculationTimeTextBox.Text = totalProcessedBySAP.ToString() + " processed at " + ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + "- Process Rate: " + processRate.ToString("0.00") + " cells/sec";

						    this.uxProgressBar.Value = m_Progress;
						    this.Refresh();

						    Application.DoEvents();

					    }
					    // RTD has COMPLETED adding functions to the queue. The background worker sends remaining functions to SAP to be processed
					    // Excel is done reading functions - process the remaining batch
				    } 
                    else if (!stillAddingFunctions) 
                    {
					    this.uxCancelButton.Enabled = true;

					    if (totalProcessedBySAP < totalAddedToQueue) 
                        {
						    //this.uxToolStripStatusLabel.Text         =  Convert.ToString(e.UserState);
						    this.uxCalculationCountTextBox.Text = "SAP is processing functions...";
						    this.uxBalanceRecordsSummedTextBox.Text = "Total Functions Queued: " + totalAddedToQueue.ToString() + " -- Complete: " + totalProcessedBySAP.ToString();


						    // Items being processed by SAP - Update the CalculationTimeTextBox
						    decimal processRate = Convert.ToDecimal(totalProcessedBySAP / ElapsedTime.TotalSeconds);
						    uxCalculationTimeTextBox.Text = totalProcessedBySAP.ToString() + " processed at " + ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + "- Process Rate: " + processRate.ToString("0.00") + " cells/sec";

						    if ((m_Progress < 90)) 
                            {
							    this.uxProgressBar.Value = m_Progress;
						    } 
                            else 
                            {
							    this.uxProgressBar.Value = 100;
						    }

						    this.Refresh();
                            //Application.DoEvents();
					    } 
                        else if (totalProcessedBySAP == totalAddedToQueue) 
                        {
						    //this.uxToolStripStatusLabel.Text         =  Convert.ToString(e.UserState);
						    this.uxCalculationCountTextBox.Text = "SAP is processing functions...";
						    this.uxBalanceRecordsSummedTextBox.Text = "Total Functions Queued: " + totalAddedToQueue.ToString() + " -- Complete: " + totalProcessedBySAP.ToString();


						    // Items being processed by SAP - Update the CalculationTimeTextBox
						    decimal processRate = Convert.ToDecimal(totalProcessedBySAP / ElapsedTime.TotalSeconds);
						    uxCalculationTimeTextBox.Text = totalProcessedBySAP.ToString() + " processed at " + ElapsedTime.Minutes.ToString() + " : " + ElapsedTime.Seconds.ToString() + "- Process Rate: " + processRate.ToString("0.00") + " cells/sec";


						    this.uxProgressBar.Value = 100;
						    this.Refresh();

						    Application.DoEvents();
					    }
				    }
			    } 
                catch (Exception ex) 
                {
				    //MessageBox.Show(ex.Message)
			    }

		    }

	    }

	    private void myBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	    {					            
		    this.Close();
	    }

	    private void uxCancelButton_Click(object sender, EventArgs e)
	    {
		    m_cancelOperation = true;
		    Thread.Sleep(500);
	    }

	    public static void CancelProcess()
	    {
		    m_cancelOperation = true;
		    Thread.Sleep(500);
	    }

	    protected override void OnFormClosing(FormClosingEventArgs e)
	    {
            if (m_cancelOperation) 
            {
			    myBackgroundWorker.CancelAsync();

			    while ((myBackgroundWorker.IsBusy)) 
                {
				    Thread.Sleep(500);
				    continue;
			    }

			    Thread.Sleep(500);

                OnDataFormClosing?.Invoke();

                // Remove event listeners
                MatManFunctionCollection.GetObject().OnFunctionAdded    -=  functionCollection_OnFunctionAdded;
			    MatManFunctionCollection.GetObject().OnFunctionRemoved  -=  functionCollection_OnFunctionRemoved;
			    SAPRequest.GetObject().FunctionProcessedBySAP           -=  sapRequest_FunctionProcessedBySAP;

			    Thread.Sleep(500);

			    MessageBox.Show(this, "Process has been cancelled");

			    m_cancelOperation = false;

			    // Clear out PWFunctionCollection completely
			    MatManFunctionCollection.GetObject().Dispose();

			    // Clear out SAPRequest completely
			    SAPRequest.GetObject().Dispose();
		    } 
            else 
            {
			    this.uxProgressBar.Value = 100;
			
			    this.Refresh();
			    Application.DoEvents();
			
                Thread.Sleep(1000);
            
                this.uxProgressBar.Value = 100;
			    this.uxToolStripStatusLabel.Text = "Done";
			    this.StatusStrip.BackColor = Color.FromArgb(80, 161, 216);

			    this.Refresh();
			    Application.DoEvents();

                OnDataFormClosing?.Invoke();

                // Remove event listeners
                MatManFunctionCollection.GetObject().OnFunctionAdded    -= functionCollection_OnFunctionAdded;
			    MatManFunctionCollection.GetObject().OnFunctionRemoved  -= functionCollection_OnFunctionRemoved;
			    SAPRequest.GetObject().FunctionProcessedBySAP           -= sapRequest_FunctionProcessedBySAP;

			    // Clear out PWFunctionCollection completely
			    MatManFunctionCollection.GetObject().Dispose();

			    // Clear out SAPRequest completely
			    SAPRequest.GetObject().Dispose();

			    Thread.Sleep(2000);

                OnUpdateValues?.Invoke();
            }

		    base.OnFormClosing(e);

		    //FileLogger.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + DateTime.Now.ToString("HH:mm:ss dd-MMM-yyyy") + " -- Trace Type: Stop ")
	    }

	    /// <summary>
	    ///  Function was ADDED to the Queue
	    ///  FormProcessState is --> AddingFunctions
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
	    private void functionCollection_OnFunctionAdded(object sender, FunctionAddedEventArgs e)
	    {
		    int totalAddedToQueue    =  e.TotalAddedToQueue;
		    int totalProcessedBySAP  =  SAPRequest.GetObject().TotalProcessedBySAP;
	    }

	    /// <summary>
	    ///  Function was REMOVED from Queue
	    ///  FormProcessState is --> ProcessingFunctions
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
	    private void functionCollection_OnFunctionRemoved(object sender, FunctionRemovedEventArgs e)
	    {

	    }

	    /// <summary>
	    ///  Function was PROCESSED by SAP
	    ///  FormProcessState --> ProcessingFunctions
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
	    private void sapRequest_FunctionProcessedBySAP(object sender, FunctionProcessedBySAPEventArgs e)
	    {
		    // Update all of the variables used to provide user feedback
		    int totalProcessedBySAP = e.TotalFunctionsProcessedbySAP;
		    int totalSentToQueue = e.TotalSentToSAP;

		    int Progress = 0;
		    if (totalProcessedBySAP > 0 && totalProcessedBySAP < totalSentToQueue) 
            {
			    Progress = Convert.ToInt32((totalProcessedBySAP / totalSentToQueue) * 100);
		    } 
            else if (totalProcessedBySAP > 0 && totalProcessedBySAP == totalSentToQueue) 
            {
			    Progress = 100;
		    }

		    this.myBackgroundWorker.ReportProgress(Progress);
	    }


    //Private Sub sapRequest_BatchCompletedProcessing(sender As Object, e As BatchCompleteProcessingEventArgs)
    //	' Update all of the variables used to provide user feedback        

    //End Sub

	    public static bool OperationCancelled 
        {
		    get 
            { 
                return m_cancelOperation; 
            }
	    }

        public static bool ExecuteFunctions
        {
            get
            {
                return m_executeFunctions;
            }
            set
            {
                m_executeFunctions = value;
            }
        }

	    private bool IsRunning 
        {
		    get 
            {
			    lock (ThreadLockObj) 
                {
				    return m_isRunning;
			    }
		    }
		    set 
            {
			    lock (ThreadLockObj) 
                {
				    m_isRunning = value;
			    }
		    }
	    }
    }
}
