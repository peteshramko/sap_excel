Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports System.Reflection.MethodBase
Imports System.Diagnostics
Imports System.Threading

<Runtime.InteropServices.ComVisible(True)>
<ProgId("com.iiiwave.matman.rtd.1")>
<ComClass(MatManRTDServer.ClassId, MatManRTDServer.InterfaceId, MatManRTDServer.EventsId)>
Public Class MatManRTDServer
    Implements Excel.IRtdServer

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "6103a5aa-a0c8-4d38-b61d-1b1fd602a7b9"
    Public Const InterfaceId As String = "87818bb3-92c5-4f10-abe4-3579f95578c7"
    Public Const EventsId As String = "59165182-6cf7-4f55-971e-4e2aef023c44"
#End Region

    Private Shared m_synclock As New Object()

    Public Sub New()
        MyBase.New()
    End Sub

    Friend Shared Property ExcelRTD() As Excel.IRTDUpdateEvent
        Get
            Return m_ExcelRTD
        End Get
        Set
            m_ExcelRTD = Value
        End Set
    End Property
    Private Shared m_ExcelRTD As Excel.IRTDUpdateEvent

    Public Function ConnectData(TopicID As Integer, ByRef Strings As System.Array, ByRef GetNewValues As Boolean) As Object Implements Microsoft.Office.Interop.Excel.IRtdServer.ConnectData
        Return MatManCalcEngine.ConnectData(TopicID, Strings, GetNewValues)
    End Function

    Public Sub DisconnectData(TopicID As Integer) Implements Microsoft.Office.Interop.Excel.IRtdServer.DisconnectData
        MatManCalcEngine.DisconnectData(TopicID)
    End Sub

    Public Function Heartbeat() As Integer Implements Microsoft.Office.Interop.Excel.IRtdServer.Heartbeat
        Return 1
    End Function

    Public Function RefreshData(ByRef TopicCount As Integer) As System.Array Implements Microsoft.Office.Interop.Excel.IRtdServer.RefreshData
        Return MatManCalcEngine.RefreshData(TopicCount)
    End Function

    Public Function ServerStart(CallbackObject As Excel.IRTDUpdateEvent) As Integer Implements Microsoft.Office.Interop.Excel.IRtdServer.ServerStart
        Try
            ExcelRTD = CallbackObject
            Return 1
        Catch e As Exception
            Return 0
        End Try
    End Function

    Public Sub ServerTerminate() Implements Microsoft.Office.Interop.Excel.IRtdServer.ServerTerminate
        ExcelRTD = Nothing
    End Sub

    Public Shared Sub RTDUpdateValues()
        ExcelRTD.UpdateNotify()

        Thread.Sleep(500)

        ExcelRTD.UpdateNotify()
    End Sub

    Public Function IsUserInitiatedCalc() As Boolean
        SyncLock m_syncLock
            Return MatManCalcEngine.UserInitiatedCalc
        End SyncLock
    End Function

    Public Function UserInitiatedConnectData(topicStringsObj As Object) As Object
        
        Dim topicObjects As Object() = DirectCast(topicStringsObj, Object())
        Dim topicLength As Integer = topicObjects.Length - 1
        Dim topicStrings As String() = New String(topicLength) {}
        Dim errString As String = Nothing

        For i As Integer = 0 To topicLength
            topicStrings(i) = topicObjects(i).ToString()
        Next

        Dim myTopicID As Integer = -1

        Dim cellAddress As String = topicStrings(topicLength)
        Dim getNewValues As Boolean = True

        Dim hash As String = MatManCalcEngine.Concatenate(topicObjects)

        Dim tsarray As Array = DirectCast(topicStrings, Array)

        Try
            myTopicID = MatManCalcEngine.RTDFunctionsByHash(hash)

            If myTopicID <> -1 Then
                Return ConnectData(myTopicID, tsarray, getNewValues)
            Else
                errString = "mm_Error - (Topic not found)"
            End If
        Catch exp As Exception
            errString = "mm_Error - (" + exp.Message + ")"
        End Try

        If errString <> String.Empty Then
            Return errString
        End If

        Return Nothing
    End Function

    Friend Shared Function ExcelIsInteractive() As Boolean
        Dim tcs As New ThreadCultureSwitch()
        tcs.SetUSCulture()

        Try
            Return MatManCOMClass.ExcelApplication.Interactive
        Catch ex As Exception
            Return False
        Finally
            tcs.SetOriginalCulture()
        End Try

        Return True
    End Function

End Class


