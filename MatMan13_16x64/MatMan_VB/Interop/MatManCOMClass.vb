Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports System.Runtime.InteropServices
Imports Excel = Microsoft.Office.Interop.Excel
Imports Office = Microsoft.Office.Interop
Imports System.ComponentModel
Imports System.Reflection
Imports SAP.Middleware.Connector
Imports System.Globalization
Imports System.Threading
Imports iiiwave.MatManLib

<Runtime.InteropServices.ComVisible(True)> _
<ComClass(MatManCOMClass.ClassId, MatManCOMClass.InterfaceId, MatManCOMClass.EventsId)> _
Public Class MatManCOMClass 
            Implements IDisposable

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "5CD3EF64-3C22-4CCD-AB93-C65B08E48B3A"
    Public Const InterfaceId As String = "AF8F52A4-95E4-4D5C-BF02-4327F2701B3F"
    Public Const EventsId As String = "5D88D8E2-E874-4E36-B5B1-5D710D6A9A48"
#End Region

    Private Shared    m_refreshCalcButtonsEnabled  As  Boolean                 =  False
	Private Shared    m_isConnected                As  Boolean                 =  False
	Private           m_funcionExecutionType       As  FunctionExecutionType   =  FunctionExecutionType.RetrievingData
	Private Shared    m_originalCultureName        As  String                  =  "en-US"

    Private                      m_userOptionsForm            As  UserOptionsForm
    Private Shared               m_nativeWindow               As  New NativeWindow()
    Private WithEvents           m_logonForm                  As  SapLogonForm

    Private Shared WithEvents    m_returnProgressDataForm     As  ReturnProgressDataForm
    Private Shared WithEvents    m_application                As  Excel.Application
	

    Private Shared m_lock As New Object()

	Public Sub New()
		MyBase.New()
	End Sub

	Private Sub MatManCOMClass_OnLogonComplete()
		m_refreshCalcButtonsEnabled = True
		InvalidateRibbonRefreshCalcButtons()
	End Sub

	Public Sub LogonToSAPDefault()
		Try
			SapConnection.GetObject().connectSAPserver("GPETERSON", "ronan1")
			MessageBox.Show("Connected!")
		Catch exp As Exception
			MessageBox.Show("Connection Error: " + exp.Message, "Error", MessageBoxButtons.OK)
		End Try
	End Sub

	<ComVisible(True)>
    Public Sub AssignApplicationObj(_app As Excel.Application)
	    m_application = _app
	End Sub

    <ComVisible(true)>
	Public Sub RefreshCalcs(refreshWhat As RefreshType)
		SyncLock m_lock
			ClearValues.ClearPreviousRun()
            If SapConnection.GetObject().IsConnected Then

                If refreshWhat.Equals(RefreshType.Range) Then

					Try
						MatManCalcEngine.RefreshWhat = refreshWhat

						MatManCalcEngine.RefreshCalculations(Nothing)
					Catch ex As Exception
						MessageBox.Show(ex.Message, "MatMan Error")
					End Try
				Else
					MatManCalcEngine.RefreshWhat = refreshWhat
					MatManCalcEngine.RefreshCalculations(Nothing)

				End If
            Else
                MessageBox.Show("User is not logged in", "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End If

            MatManCalcEngine.UserInitiatedCalc = False
			m_application.Interactive = True
		End SyncLock
	End Sub

	Public Sub SetFunctionExecutionType(executionType As Integer)
        Global.iiiwave.MatManLib.Properties.Settings.[Default].FunctionExecutionType = executionType
        Global.iiiwave.MatManLib.Properties.Settings.[Default].Save()
    End Sub

    <ComVisible(true)>
	Public Shared Function InvalidateRibbonRefreshCalcButtons() As Boolean

		If Not IsUSCulture() Then
			SetUSCulture()
		End If

		Dim retry As Boolean = True
		Dim count As Integer = 0

		Do
			'Inserted counter here as Excel close with cancel was putting this into loop on re-logon. GP.
			count += 1

			Try
				If Double.Parse(m_application.Version, System.Globalization.CultureInfo.InvariantCulture) > 11.0 Then
					m_application.Run("matman_InvalidateRibbonRefreshCalcButtons")
				End If
				retry = False
			Catch ex As Exception
				If count > 3 Then
					retry = False
				End If

			End Try
		Loop While retry

		SetOriginalCulture()

		Return True

	End Function

	Private Function ValidMatManServerVersion() As Boolean
		Return True
	End Function

	Public Shared Sub SetUSCulture()
		Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")
	End Sub

	Public Shared Sub SetOriginalCulture()
		Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(m_originalCultureName)
	End Sub

	Friend Shared Function IsUSCulture() As Boolean
		If System.Globalization.CultureInfo.CurrentCulture.Name <> "en-US" Then
			Return False
		End If
		Return True
	End Function

	Public Sub ShowSapLogonForm()
		If m_nativeWindow.Handle.ToInt32() <> 0 Then
			m_nativeWindow.ReleaseHandle()
		End If

		m_nativeWindow.AssignHandle(New IntPtr(DirectCast(MatManCOMClass.ExcelApplication, Excel.Application).Hwnd))

        If m_logonForm Is Nothing OrElse m_logonForm.IsDisposed Then
            m_logonForm = New SapLogonForm()
            AddHandler m_logonForm.OnLogonComplete,  AddressOf  MatManCOMClass_OnLogonComplete
            AddHandler m_logonForm.FormClosed,       AddressOf  m_logonForm_FormClosed
        End If
        If Not m_logonForm.IsHandleCreated Then
            m_logonForm.Show(m_nativeWindow)
        End If
    End Sub

	Private Sub m_logonForm_FormClosed(sender As Object, e As FormClosedEventArgs)
        RemoveHandler m_logonForm.OnLogonComplete,   AddressOf MatManCOMClass_OnLogonComplete
        RemoveHandler m_logonForm.FormClosed,        AddressOf m_logonForm_FormClosed
        m_logonForm.Dispose()
        m_nativeWindow.ReleaseHandle()
	End Sub

	' -> here


	'private void returnProgressDataForm_OnDataFormClosing()
	'{
	'    try
	'    {
	'        m_application.Interactive  =  true;
	'    }
	'    catch(Exception e) { }

	'    m_refreshCalcButtonsEnabled                               =  true;
	' InvalidateRibbonRefreshCalcButtons();
	' ((Excel.Application)m_application).Interactive            =  true;

	' MatManCalcEngine.GetObject().StopProcessing               =  true;
	' MatManCalcEngine.GetObject().CompletedCalculationProcess  =  true;

	' MatManCalcEngine.GetObject().CurrentFunctionsByCellAddress.Clear();

	' MatManCalcEngine.GetObject().ConnectDataCount             =  0;
	' MatManCalcEngine.GetObject().TopicCount                   =  0;
	' MatManCalcEngine.GetObject().StopProcessing               =  true;
	' MatManCalcEngine.GetObject().CompletedCalculationProcess  =  true;
	'}

    <ComVisible(true)>
    Public ReadOnly Property GetRefreshCalcButtonsEnabled As Boolean 
        Get
            Return m_refreshCalcButtonsEnabled
        End Get
    End Property

    Public Shared Property RefreshCalcButtonsEnabled() As Boolean
		Get
			SyncLock m_lock
				Return m_refreshCalcButtonsEnabled
			End SyncLock
		End Get
		Set
			SyncLock m_lock
				m_refreshCalcButtonsEnabled = value
			End SyncLock
		End Set
	End Property

    <ComVisible(true)>
	Public Function IsLoggedOnToSAP() As Boolean
        Return SapConnection.GetObject().IsConnected
        Return False
	End Function

	Friend Shared ReadOnly Property ExcelApplication() As Excel.Application
		Get
			Return m_application
		End Get
	End Property

    <ComVisible(true)>
	Public Shared Sub KillExcel()
        SapConnection.GetObject().ResetSAPConnection()

        Try
			DirectCast(m_application, Excel.Application).Run("matman_RefreshRibbon")
		Catch e As Exception
		End Try

		GC.Collect()
		GC.WaitForPendingFinalizers()

		Try
			System.Runtime.InteropServices.Marshal.FinalReleaseComObject(DirectCast(m_application, Excel.Application))
		Catch e As Exception
		End Try

		m_application = Nothing
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
        SapConnection.GetObject().ResetSAPConnection()

        Try
			DirectCast(m_application, Excel.Application).Run("matman_RefreshRibbon")
		Catch e As Exception
		End Try

		GC.Collect()
		GC.WaitForPendingFinalizers()

		Try
			System.Runtime.InteropServices.Marshal.FinalReleaseComObject(DirectCast(m_application, Excel.Application))
		Catch e As Exception
		End Try

		m_application = Nothing
	End Sub

End Class


