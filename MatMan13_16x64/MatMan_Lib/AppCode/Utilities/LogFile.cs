using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace iiiwave.MatManLib
{
    public class LogFile
    {
        private static object  fileLock   =  new object();
            
	    public static void DeleteLogFile()
	    {
		    PWEventLogging.GetObject().MyEvents.WriteEntry(MethodBase.GetCurrentMethod().Name + " " + DateTime.Now.ToString("dd-YYYY-mm HH:MM:ss"), EventLogEntryType.Information);

		    try 
            {
			    string[] fileArray = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Properties.Resources.TraceFolder + "PlanningWand");


			    if (fileArray.Length > 0) 
                {
				    if (MessageBox.Show("Please confirm deletion of " + fileArray.Length.ToString() + " files in the " + 
                                                              Environment.SpecialFolder.LocalApplicationData.ToString()  + Properties.Resources.TraceFolder.ToString() + 
                                                              " folder?", "Delete trace files", MessageBoxButtons.OK) == DialogResult.OK) 
                    {
					    foreach (string aFileName in fileArray) 
                        {
						    File.Delete(aFileName);
					    }

				    }
			    } 
                else 
                {
				    MessageBox.Show("No files to delete in the " + Environment.SpecialFolder.LocalApplicationData.ToString() + 
                                     Properties.Resources.TraceFolder.ToString(), "Trace file deletion", MessageBoxButtons.OK, MessageBoxIcon.Information);
			    }

		    } 
            catch (Exception ex) 
            {
			    PWEventLogging.GetObject().MyEvents.WriteEntry(ex.Message + " Clearing trace files", EventLogEntryType.Error);
			    MessageBox.Show("Unable to clear trace files. " + ex.Message, "Trace file deletion error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		    }

		    PWEventLogging.GetObject().MyEvents.WriteEntry(MethodBase.GetCurrentMethod().Name + " " + DateTime.Now, EventLogEntryType.SuccessAudit);
	    }
	
        static internal string CheckCreateLogFolder()
	    {
		    string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Properties.Resources.TraceFolder + "PlanningWand";
		    try 
            {
			    if (!Directory.Exists(directoryPath)) 
                {
				    Directory.CreateDirectory(directoryPath);
			    }
		    } 
            catch (Exception ex) 
            {
			    directoryPath = string.Empty;
		    }

		    return directoryPath;
	    }

        public static void WriteFileEntry(string message)
        {
            string appDirectory      =  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Properties.Resources.TraceFolder + "PlanningWand";               
            string logFilePath       =  appDirectory + "\\Planning_Wand_Trace.log";

                                   
            lock(fileLock)
            {
                if(File.Exists(logFilePath))
                {
                    try
                    {
                        using (TextWriter writer = (TextWriter)File.AppendText(logFilePath))
                        {
                            writer.WriteLine(" Source: Planning Wand: " + "  Message: " + message + "  at  " + DateTime.Now.ToString("HH:mm:ss dd-MMM-yyyy") );
                        }
                    }
                    catch(Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
                else
                {
                    try
                    {
                        using (TextWriter writer = (TextWriter)File.CreateText(logFilePath))
                        {
                            writer.WriteLine(" Source: Planning Wand: " + message + "  at  " + DateTime.Now.ToString("HH:mm:ss dd-MMM-yyyy") );
                        }
                    }
                    catch(Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }
    }
}
