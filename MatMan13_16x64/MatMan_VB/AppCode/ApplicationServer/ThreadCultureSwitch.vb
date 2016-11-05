Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Public Class ThreadCultureSwitch

    Public Property ThisThread() As Thread
	Get
		Return m_ThisThread
	End Get
	Set
		m_ThisThread = Value
	End Set
    End Property
    Private m_ThisThread As Thread

    Public Property OriginalCulture() As System.Globalization.CultureInfo
	    Get
		    Return m_OriginalCulture
	    End Get
	    Set
		    m_OriginalCulture = Value
	    End Set
    End Property
    Private m_OriginalCulture As System.Globalization.CultureInfo

    Public Sub New()
	    ThisThread = Thread.CurrentThread
	    OriginalCulture = ThisThread.CurrentCulture
    End Sub

    Public Sub SetUSCulture()
	    ThisThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")
    End Sub

    Public Sub SetOriginalCulture()
	    ThisThread.CurrentCulture = Me.OriginalCulture
    End Sub

End Class
