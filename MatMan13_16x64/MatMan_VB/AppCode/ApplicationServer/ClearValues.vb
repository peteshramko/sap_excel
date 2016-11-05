

Public Class ClearValues
    Public Shared Sub ClearPreviousRun()
	    'MatManFunctionCollection.GetObject().Clear()
	    'MatManFunctionCollection.GetObject().TotalFunctionsAddedToQueue = 0

	    'SAPRequest.GetObject().TotalProcessedBySAP = 0
	    'SAPRequest.ReturnValuesList.Clear()

	    MatManCalcEngine.CurrentFunctionsByCellAddress.Clear()

	    MatManCalcEngine.ConnectDataCount = 0
	    MatManCalcEngine.TopicCount = 0
	    MatManCalcEngine.UserInitiatedCalc = True
    End Sub
End Class
