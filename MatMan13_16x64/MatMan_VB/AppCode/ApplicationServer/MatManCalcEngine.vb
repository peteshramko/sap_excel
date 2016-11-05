Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Collections
Imports System.Data
Imports System.Diagnostics
Imports System.ComponentModel
Imports System.Threading
Imports System.Timers
Imports System.Windows.Forms
Imports System.Reflection
Imports System.IO
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports Microsoft.Office.Interop
Imports SAP.Middleware.Connector
Imports iiiwave.MatManLib


Public Class MatManCalcEngine
    Private Shared m_rtdFunctionsByTopicID                 As New Dictionary(Of Integer, System.Array)
    Private Shared m_rtdFunctionsByHash                    As New Dictionary(Of String, Integer)
    Private Shared m_currentFunctionsByCellAddress         As New Dictionary(Of String, Integer)
    Private Shared m_returnValuesDictionary                As New Dictionary(Of String, IMatManFunction)

    Private Shared WithEvents m_connectDataTimer           As New System.Timers.Timer
    Private Shared WithEvents m_disconnectDataTimer        As New System.Timers.Timer
    Private Shared m_lastConnectDataUpdate                 As New DateTime
    Private Shared m_lastDisconnectDataUpdate              As New DateTime
    Private Shared m_disconnectCounter                     As Integer = 0
    Private Shared m_completedCalculationProcess           As Boolean = False
    Private Shared m_completedProcessing                   As Boolean = True
    Private Shared m_stopProcessing                        As Boolean = False
    Private Shared m_userInitiatedCalc                     As Boolean = False
    Private Shared m_cancelMessageIssued                   As Boolean = False
    Private Shared m_userCancelledCalc                     As Boolean = False
    Private Shared m_isLoggingOn                           As Boolean = False
    Private Shared m_connectDataCount                      As Integer = 0
    Private Shared m_topicCount                            As Integer = 0
    Private Shared m_refreshWhat                           As RefreshType = RefreshType.Sheet

    Private Shared WithEvents m_progressForm               As ReturnProgressDataForm       =  Nothing
    Private Shared WithEvents m_sapReturn                  As MatManReturnValueDictionary  =  SAPRequest.ReturnValuesList
    Private Shared m_application                           As Excel.Application            =  MatManCOMClass.ExcelApplication

    Private Shared m_syncRoot                              As New Object()

    Public Shared Sub ResetCounters()
        MatManFunctionCollection.GetObject().Clear()
    End Sub

    Friend Shared Sub StartConnectDataTimer()
        m_connectDataTimer.Start()
        m_connectDataTimer.AutoReset                    = True

        MatManCOMClass.RefreshCalcButtonsEnabled        = False
        MatManCOMClass.InvalidateRibbonRefreshCalcButtons()

        MatManCalcEngine.CompletedCalculationProcess    = False
        MatManCalcEngine.StopProcessing                 = False
        MatManCalcEngine.CancelMessageIssued            = False
        MatManCalcEngine.UserCancelledCalc              = False

        If m_progressForm Is Nothing Then
            m_progressForm = New ReturnProgressDataForm()
            m_progressForm.StartPosition = FormStartPosition.CenterScreen
            m_progressForm.Show()
        Else
            m_progressForm.Close()
            m_progressForm = New ReturnProgressDataForm()
            m_progressForm.StartPosition = FormStartPosition.CenterScreen
            m_progressForm.Show()
        End If

        m_progressForm.TopMost = True
    End Sub

    Private Shared Sub _ConnectDataTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles m_connectDataTimer.Elapsed
        If DateTime.Now.Subtract(m_lastConnectDataUpdate).TotalMilliseconds > iiiwave.MatManLib.Properties.Settings.[Default].UserRefreshTimerInterval Then
            ' This call STOPS ALL THE FUNCTION ADDING PROCESS FROM EXCEL CLIENT TO TO MatManFunctionCollection
            MatManFunctionCollection.GetObject().StillAddingFunctions = False
            ReturnProgressDataForm.ExecuteFunctions = True
            m_connectDataTimer.[Stop]()
        End If
    End Sub

    Private Shared Sub _DisconnectDataTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles m_disconnectDataTimer.Elapsed
        If DateTime.Now.Subtract(m_lastDisconnectDataUpdate).TotalMilliseconds > iiiwave.MatManLib.Properties.Settings.[Default].DisconnectTimerInterval Then
            m_disconnectDataTimer.[Stop]()
        End If
    End Sub


    Public Shared Function ConnectData(topicID As Integer, ByRef inputStrings As System.Array, ByRef getNewValues As Boolean) As Object
        Dim lastIndex As Integer = inputStrings.Length - 1
        Dim cellAddress As String = inputStrings.GetValue(lastIndex).ToString()
        Dim hash As String = Concatenate(inputStrings)
        Dim validationResponse As String = [String].Empty
        Dim returnVal As String = String.Empty

        If Not m_userInitiatedCalc Then

            getNewValues = True
            If inputStrings.GetValue(2).ToString() <> String.Empty Then
                ' [0] is "server", [1] is function type, [2] is first param 
                m_rtdFunctionsByTopicID(topicID) = inputStrings
                m_rtdFunctionsByHash(hash) = topicID
            End If

            If SapConnection.GetObject().IsConnected Then
                Dim planningWandFunction As IPlanningFunction = GetPlanningData.CreatePlanningData(topicID, inputStrings, validationResponse)

                If validationResponse = [String].Empty Then
                    returnVal = "mm_Pending"
                Else
                    returnVal = validationResponse
                End If
            Else
                returnVal = "mm_Error (Not Logged On)"
            End If
                       

            Return returnVal
        End If

        If m_userInitiatedCalc Then
            If SapConnection.GetObject().IsConnected AndAlso Not IsLoggingOn Then
                Try
                    Dim existingTopic As Integer = m_rtdFunctionsByHash(hash)

                    If existingTopic <> topicID Then
                        Return Nothing
                    End If

                    m_topicCount = m_rtdFunctionsByTopicID.Count
                    m_connectDataCount += 1
                    m_disconnectCounter += 1
                    ' add function count
                    'FileLogger.WriteEntry("Connect Data: Connect topic id = " + TopicID.ToString() + " Disconnect counter = " + DisconnectCounter.ToString() + " -- Trace Type: Information ")

                    m_lastConnectDataUpdate = DateTime.Now

                    If True Then
                        ' set to true for testing only
                        ' Force new calculations if auto calc is on
                        If iiiwave.MatManLib.Properties.Settings.[Default].CalculateOn Then

                            ' if AutoCalc is *not* on return pending and abandon the refresh
                            getNewValues = True
                        ElseIf Not iiiwave.MatManLib.Properties.Settings.[Default].CalculateOn Then
                            Return "mmPending"
                        End If

                        ' Start Timer if necessary
                        If Not m_connectDataTimer.Enabled Then
                            StartConnectDataTimer()
                        End If

                        '---------- This process should be able to continue to ACCEPT new calculations even while batches are being sent to SAP -------
                        ' Refresh form based opn Batch Size - every 100 (TESTING, refresh form for all functions added)
                        System.Windows.Forms.Application.DoEvents()

                        'Call the UpdateStatus function to report on function adding progress
                        m_progressForm.UpdateStatus("Validating Requests...")

                        'MyProgressForm.uxCalculationCountTextBox.Text = Convert.ToString(PWFunctionCollection.GetObject().Count)
                        m_progressForm.uxToolStripStatusLabel.Text = "Validating Requests..."


                        SyncLock m_syncRoot
                            'This will start the Function Collection Timer and then add the first *valid* function to the Queue
                            ' 1 - Creates the PWFunctionCollection instance or references the existing instance (Singleton)
                            ' 2 - Creates the IPlanningWandFunction instance
                            ' 3 - Checks the validation string for errors
                            ' 4 - If not errors - then it adds the IPlanningWandFunction to the Queue						

                            ' An Empty validationResponse string indicates all is well and Function can be added to the Queue
                            ' Only those functions added to the Queue will be processed
                            'Dim minPair = m_rtdFunctionsByTopicID.Aggregate(Function(p1, p2) If((p1.Value < p2.Value), p1, p2))


                            Dim planningWandFunction As IPlanningFunction = GetPlanningData.CreatePlanningData(topicID, inputStrings, validationResponse)
                            If validationResponse = [String].Empty Then
                                If Not m_currentFunctionsByCellAddress.ContainsKey(planningWandFunction.Hash) Then

                                    m_currentFunctionsByCellAddress(planningWandFunction.Hash) = topicID
                                    MatManFunctionCollection.GetObject().Enqueue(planningWandFunction)
                                End If
                            End If
                        End SyncLock


                        ' An empty string represents a successful validation
                        If validationResponse = [String].Empty Then
                            ' return a "pending" notification while SAP processes the transaction
                            Return "mmPending"
                        Else
                            ' a NON-empty string represents a validation error
                            Return validationResponse
                        End If
                    Else
                        ' This only becomes useful when LICENSING is integrated, otherwise - this code is unreachable
                        getNewValues = True
                        Return "mmError (Not logged on)"

                    End If
                    'PWCalculationEngine.AcceptNewCalcs               =  True
                    'PlanningWandCOMClass.InvalidateRibbonRefreshCalcButtons()
                Catch ex As Exception
                End Try
            ElseIf m_isLoggingOn Then
                getNewValues = True

                Return "mmPending"
            Else
                getNewValues = True

                Return "mmError (Not logged on)"
            End If
        End If
        Return "mmError (Not logged on)"
    End Function

    Public Shared Sub DisconnectData(topicID As Integer)
        Dim sArray As System.Array = m_rtdFunctionsByTopicID(topicID)
        Dim cString As String = Concatenate(sArray)

        m_rtdFunctionsByTopicID.Remove(topicID)
        m_rtdFunctionsByHash.Remove(cString)

        m_disconnectCounter -= 1
        m_lastDisconnectDataUpdate = DateTime.Now
    End Sub

    Public Shared Function Heartbeat() As Integer
        Return 1
    End Function

    Public Shared Sub ReturnValuesEvent() Handles m_sapReturn.OnAddReturnValues
        SyncLock m_syncRoot
            Dim mKV As KeyValuePair(Of String, IMatManFunction) = m_sapReturn.Last()
            Try
                m_returnValuesDictionary(mKV.Key) = mKV.Value
            Catch ex As Exception
            End Try
        End SyncLock
    End Sub


    Private Shared Sub UpdateValuesEvent() Handles m_progressForm.OnUpdateValues
        MatManRTDServer.RTDUpdateValues()
    End Sub


    Private Shared Sub DataFormCloseEvent() Handles m_progressForm.OnDataFormClosing
        MatManCOMClass.RefreshCalcButtonsEnabled = True
        MatManCOMClass.InvalidateRibbonRefreshCalcButtons()
    End Sub

    Public Shared Function RefreshData(ByRef TopicCount As Integer) As System.Array
        m_completedCalculationProcess = True
        m_userInitiatedCalc = False

        If m_returnValuesDictionary.Count > 0 Then
            Try
                Dim rvSize As Integer = 0

                VolatileProperty.VolatileWrite(rvSize, (m_returnValuesDictionary.Keys.Count - 1))

                Dim ReturnValues As Object(,) = New Object(1, rvSize) {}

                ' convert return values to a System.Array
                SyncLock m_syncRoot
                    For index As Integer = 0 To rvSize

                        Dim TiD As Integer = m_returnValuesDictionary.Values.ElementAt(index).TopicID
                        Dim cellAddress As String = m_returnValuesDictionary.Values.ElementAt(index).Signature


                        ReturnValues(0, index) = m_returnValuesDictionary.Values.ElementAt(index).TopicID
                        ReturnValues(1, index) = m_returnValuesDictionary.Values.ElementAt(index).Result

                    Next
                End SyncLock

                TopicCount = ReturnValues.GetLength(1)
                
                Return ReturnValues

            Catch ex As Exception
                Return Nothing
            End Try
        Else
            Try
                m_progressForm.Close()
            Catch ex As Exception
                ' write error
            End Try
            Return Nothing
        End If
    End Function

    Public Shared Sub RefreshCalculations(stateObject As Object)
        Dim rememberCalc As Boolean = iiiwave.MatManLib.Properties.Settings.[Default].CalculateOn
        Dim cultureSwitch As New ThreadCultureSwitch()

        Try
            cultureSwitch.SetUSCulture()

            MatManCOMClass.ExcelApplication.Interactive = False

            ' Check for multi selected sheets. Causing crashes when multi sheets selected and then refreshing sheet, book or all
            Dim allSheets As Excel.Sheets = MatManCOMClass.ExcelApplication.ActiveWindow.SelectedSheets
            If MatManCOMClass.ExcelApplication.ActiveWindow.SelectedSheets.Count > 1 Then
                MatManCOMClass.ExcelApplication.ActiveSheet.[Select]()
            End If

            ' Reset Disconnect Timer
            m_disconnectCounter = 0
            ' Start the Disconnect Timer
            m_disconnectDataTimer.Start()

            iiiwave.MatManLib.Properties.Settings.[Default].CalculateOn = True
            iiiwave.MatManLib.Properties.Settings.[Default].Save()

            Select Case m_refreshWhat
                Case (RefreshType.Range)
                    If True Then
                        Dim myRange As Excel.Range = DirectCast(MatManCOMClass.ExcelApplication.Selection, Excel.Range)

                        CalculateRange(myRange)
                        ' This kicks off the work - refresh
                        If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                            While Not m_completedCalculationProcess
                                Thread.Sleep(iiiwave.MatManLib.Properties.Settings.[Default].UserRefreshTimerInterval)
                            End While

                            CalculateRange(myRange)
                        End If
                    End If
                    Exit Select
                Case (RefreshType.Sheet)
                    If True Then
                        CalculateSelectedSheet(allSheets)

                        ' If Excel is on manual then need to perform some post calculation steps
                        If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                            While Not m_completedCalculationProcess
                                Thread.Sleep(iiiwave.MatManLib.Properties.Settings.[Default].UserRefreshTimerInterval)
                            End While

                            ' Have RTD populate the value in Excel
                            For Each mySheet As Excel.Worksheet In allSheets
                                mySheet.[Select]()
                                If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                                    MatManCOMClass.ExcelApplication.ActiveSheet.Calculate()
                                End If
                            Next
                        End If
                    End If
                    Exit Select
                Case (RefreshType.Workbook)
                    If True Then
                        CalculateAllSheets()

                        ' If Excel is on manual then need to perform some post calculation steps
                        If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                            While Not m_completedCalculationProcess
                                Thread.Sleep(iiiwave.MatManLib.Properties.Settings.[Default].UserRefreshTimerInterval)
                            End While

                            ' Have RTD populate the value in Excel
                            For Each mySheet As Excel.Worksheet In MatManCOMClass.ExcelApplication.ActiveWorkbook.Sheets
                                mySheet.Calculate()
                            Next
                        End If
                    End If
                    Exit Select
            End Select
        Catch e As Exception
            m_userInitiatedCalc = False
        End Try

        ' Restore culture information
        cultureSwitch.SetOriginalCulture()

        ' Restore Calculation setting
        iiiwave.MatManLib.Properties.Settings.[Default].CalculateOn = rememberCalc
        iiiwave.MatManLib.Properties.Settings.[Default].Save()

        m_userInitiatedCalc = False

        MatManCOMClass.ExcelApplication.Interactive = True
    End Sub

    Private Shared Sub CalculateRange(aRange As Excel.Range)
        If CInt(DirectCast(MatManCOMClass.ExcelApplication, Excel.Application).Calculation) <> CInt(Excel.XlCalculation.xlCalculationManual) Then
            aRange.Dirty()
        ElseIf Convert.ToInt32(MatManCOMClass.ExcelApplication.Version) > 11 Then
            aRange.CalculateRowMajorOrder()
        Else
            aRange.Calculate()
        End If
    End Sub

    Private Shared Sub CalculateSelectedSheet(allSheets As Excel.Sheets)
        For Each aSheet As Excel.Worksheet In allSheets
            aSheet.[Select]()
            MatManCOMClass.ExcelApplication.ActiveSheet.EnableCalculation = False
        Next
        For Each aSheet As Excel.Worksheet In allSheets
            aSheet.[Select]()
            MatManCOMClass.ExcelApplication.ActiveSheet.EnableCalculation = True

            If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                MatManCOMClass.ExcelApplication.ActiveSheet.Calculate()
            End If
        Next
    End Sub

    Private Shared Sub CalculateAllSheets()
        For Each sheet As Excel.Worksheet In MatManCOMClass.ExcelApplication.ActiveWorkbook.Sheets
            sheet.EnableCalculation = False
        Next

        For Each sheet As Excel.Worksheet In MatManCOMClass.ExcelApplication.ActiveWorkbook.Sheets
            sheet.EnableCalculation = True

            If MatManCOMClass.ExcelApplication.Calculation = Excel.XlCalculation.xlCalculationManual Then
                sheet.Calculate()
            End If
        Next
    End Sub

    Public Shared Function Concatenate(functionStrings As System.Array) As String
        Dim concatenatedString As String = String.Empty

        For Each aObject As Object In functionStrings
            Dim aString As String = aObject.ToString().ToUpper()
            If aString <> String.Empty Then
                concatenatedString += aString & Convert.ToString(vbTab)
            Else
                concatenatedString += vbTab
            End If
        Next

        Return concatenatedString
    End Function

    Public Shared Property ConnectDataCount() As Integer
        Get
            Return VolatileProperty.VolatileRead(m_connectDataCount)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_connectDataCount, Value)
        End Set
    End Property

    Public Shared Property TopicCount() As Integer
        Get
            Return VolatileProperty.VolatileRead(m_topicCount)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_topicCount, Value)
        End Set
    End Property

    Public Shared ReadOnly Property ConnectDataTimer() As System.Timers.Timer
        Get
            Return VolatileProperty.VolatileRead(m_connectDataTimer)
        End Get
    End Property

    Public Shared ReadOnly Property DisconnectDataTimer() As System.Timers.Timer
        Get
            Return VolatileProperty.VolatileRead(m_disconnectDataTimer)
        End Get
    End Property

    Public Shared ReadOnly Property LastConnectDataUpdate() As DateTime
        Get
            Return VolatileProperty.VolatileRead(m_lastConnectDataUpdate)
        End Get
    End Property

    Public Shared ReadOnly Property LastDisconnectDataUpdate() As DateTime
        Get
            Return VolatileProperty.VolatileRead(m_lastDisconnectDataUpdate)
        End Get
    End Property

    Public Shared Property DisconnectCounter() As Integer
        Get
            Return VolatileProperty.VolatileRead(m_disconnectCounter)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_disconnectCounter, Value)
        End Set
    End Property

    Public Shared Property CompletedCalculationProcess() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_completedCalculationProcess)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_completedCalculationProcess, Value)
        End Set
    End Property

    Public Shared Property CompletedProcessing() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_completedProcessing)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_completedProcessing, Value)
        End Set
    End Property

    Public Shared Property StopProcessing() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_stopProcessing)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_stopProcessing, Value)
        End Set
    End Property

    Public Shared ReadOnly Property RTDFunctionsByHash() As Dictionary(Of String, Integer)
        Get
            Return VolatileProperty.VolatileRead(m_rtdFunctionsByHash)
        End Get
    End Property

    Public Shared ReadOnly Property CurrentFunctionsByCellAddress() As Dictionary(Of String, Integer)
        Get
            Return VolatileProperty.VolatileRead(m_currentFunctionsByCellAddress)
        End Get
    End Property

    Public Shared Property ReturnValuesList() As Dictionary(Of String, IMatManFunction)
        Get
            Return VolatileProperty.VolatileRead(m_returnValuesDictionary)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_returnValuesDictionary, Value)
        End Set
    End Property

    Public Shared Property RefreshWhat() As RefreshType
        Get
            Return VolatileProperty.VolatileRead(m_refreshWhat)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_refreshWhat, Value)
        End Set
    End Property

    Public Shared Property UserInitiatedCalc() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_userInitiatedCalc)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_userInitiatedCalc, Value)
        End Set
    End Property

    Public Shared Property IsLoggingOn() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_isLoggingOn)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_isLoggingOn, Value)
        End Set
    End Property

    Public Shared Property CancelMessageIssued() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_cancelMessageIssued)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_cancelMessageIssued, Value)
        End Set
    End Property

    Public Shared Property UserCancelledCalc() As Boolean
        Get
            Return VolatileProperty.VolatileRead(m_userCancelledCalc)
        End Get
        Set
            VolatileProperty.VolatileWrite(m_userCancelledCalc, Value)
        End Set
    End Property

    Public Shared ReadOnly Property CostPlanGrouping() As Integer
        Get
            Return iiiwave.MatManLib.Properties.Settings.[Default].PlanGrouping
        End Get
    End Property
End Class

Public Class VolatileProperty
    Public Shared Function VolatileRead(Of T)(ByRef Address As T) As T
        Dim functionReturnValue As T = Nothing
        functionReturnValue = Address
        Thread.MemoryBarrier()
        Return functionReturnValue
    End Function
    Public Shared Sub VolatileWrite(Of T)(ByRef Address As T, Value As T)
        Thread.MemoryBarrier()
        Address = Value
    End Sub
End Class
