using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;


namespace iiiwave.MatManLib
{
    //[ClassInterface(ClassInterfaceType.None)]
    [ComVisible(false)]
    public class MatManFunctionCollection : Queue<IMatManFunction>, IDisposable
    {

	    private DateTime?             m_startProcessingTime   =  null;
	    private DateTime?             m_updateProcessTime     =  null;		
	    private volatile  int         m_totalAddedToQueue     =  0;     //Represents the TOTAL NUMBER of functions added during a run, ALL BATCHES SUMMED		
	    private volatile  bool        m_stillAddingFunctions  =  true;  //Checks to see if RTD is still adding function to the Queue
    
	    //private object    m_threadLock;
	    public  event     OnFunctionAddedEventHandler OnFunctionAdded;
	    public  delegate  void OnFunctionAddedEventHandler(object sender, FunctionAddedEventArgs e);

	    public  event     OnFunctionRemovedEventHandler OnFunctionRemoved;
	    public  delegate  void OnFunctionRemovedEventHandler(object sender, FunctionRemovedEventArgs e);

	    private MatManFunctionCollection()
	    {
        
	    }

	    // Create a Thread-safe Singleton instantiation
	    private static MatManFunctionCollection m_pwFunctionCollection;

	    private static object syncRoot = new object();
	    /// <summary>
	    ///   Make accessor Thread-safe
	    /// </summary>
	    /// <returns></returns>
	    public static MatManFunctionCollection GetObject()
	    {
		    if (MatManFunctionCollection.m_pwFunctionCollection == null) 
            {
			    lock (syncRoot) 
                {
				    if (MatManFunctionCollection.m_pwFunctionCollection == null) 
                    {
					    MatManFunctionCollection.m_pwFunctionCollection = new MatManFunctionCollection();
				    }
			    }
		    }
		    return MatManFunctionCollection.m_pwFunctionCollection;
	    }

	    public new void Enqueue(IMatManFunction _function)
	    {
		    lock (syncRoot) 
            {
			    if (_function != null) 
                {
				    if (m_startProcessingTime == null) 
                    {
					    m_startProcessingTime = DateTime.Now;
				    }

				    if (m_updateProcessTime == null) 
                    {
					    m_updateProcessTime = DateTime.Now;
				    }

				    m_totalAddedToQueue += 1;

				    base.Enqueue(_function);

				    // increment the total count of ALL FUNCTIONS added throughout the entire process
				    // Raise the OnFunctionAdded event
				    if (OnFunctionAdded != null) 
                    {
					    OnFunctionAdded(this, new FunctionAddedEventArgs(this.Count, this.TotalFunctionsAddedToQueue));
				    }
			    }
		    }
	    }

	    public new IMatManFunction Dequeue()
	    {
		    lock (this) 
            {
		        while (this.Count == 0)
                {
			        Monitor.Wait(this);
		        }

		        IMatManFunction myFunction = base.Dequeue();

		        Monitor.PulseAll(this);

		        // Raise the OnFunctionRemoved event
		        if (OnFunctionRemoved != null) 
                {
			        OnFunctionRemoved(this, new FunctionRemovedEventArgs(this.Count, this.TotalFunctionsAddedToQueue));
		        }

		        return myFunction;
	        }
	    }

	    public DateTime StartProcessTime 
        {
		    get 
            {
			    if (m_startProcessingTime != null) 
                {
				    return (DateTime)m_startProcessingTime;
			    } 
                else 
                {
				    return DateTime.Now;
			    }
		    }
	    }

	    public DateTime UpdateProcessTime 
        {
		    get 
            {
			    if (m_updateProcessTime != null) 
                {
				    return (DateTime)m_updateProcessTime;
			    } 
                else 
                {
				    return DateTime.Now;
			    }
		    }
	    }

	    public int TotalFunctionsAddedToQueue 
        {
		    get 
            { 
                return m_totalAddedToQueue; 
            }
		    set 
            { 
                m_totalAddedToQueue = value; 
            }
	    }

	    public bool StillAddingFunctions 
        {
		    get 
            { 
                return m_stillAddingFunctions; 
            }
		    set 
            { 
                m_stillAddingFunctions = value; 
            }
	    }

	    public void ResetSummedQueue()
	    {
		    if (this.Count == 0) 
            {
			    m_totalAddedToQueue = 0;
		    }
	    }

	

	    public void Dispose()
	    {
		    m_startProcessingTime  =  null;
		    m_updateProcessTime    =  null;
		    m_totalAddedToQueue    =  0;
		    m_stillAddingFunctions =  false;

		    this.Clear();
	    }

    }

    public class FunctionAddedEventArgs : EventArgs
    {
	    private int m_CurrentBatchSize;
        private int m_TotalAddedToQueue;
		
        public int CurrentBatchSize 
        {
		    get 
            { 
                return m_CurrentBatchSize; 
            }
		    private set 
            { 
                m_CurrentBatchSize = value; 
            }
	    }
	
        public int TotalAddedToQueue 
        {
		    get 
            { 
                return m_TotalAddedToQueue; 
            }
		    private set 
            { 
                m_TotalAddedToQueue = value; 
            }
	    }

	    public FunctionAddedEventArgs(int _batchSize, int _totalAddedToQueue)
	    {
		    CurrentBatchSize = _batchSize;
		    TotalAddedToQueue = _totalAddedToQueue;
	    }
    }

    public class FunctionRemovedEventArgs : EventArgs
    {
	    private int m_CurrentBatchSize;
	    private int m_TotalAddedToQueue;
	
        public int CurrentBatchSize 
        {
		    get 
            { 
                return m_CurrentBatchSize; 
            }
		    private set 
            { 
                m_CurrentBatchSize = value; 
            }
	    }
	    public int TotalAddedToQueue 
        {
		    get 
            { 
                return m_TotalAddedToQueue; 
            }
		    private set 
            { 
                m_TotalAddedToQueue = value; 
            }
	    }

	    public FunctionRemovedEventArgs(int _batchSize, int _totalAddedToQueue)
	    {
		    CurrentBatchSize = _batchSize;
		    TotalAddedToQueue = _totalAddedToQueue;
	    }
    }
}