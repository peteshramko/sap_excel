Public Enum RefreshType As Integer
	Range = 0
	Sheet = 1
	Workbook = 3
End Enum

Public Enum PlanningFunctionType As Integer
	CostPlan = 1000
	ActivityPlan = 1001
	PurchasingPlan = 1002
End Enum

Public Enum QueryFunctionType As Integer
	GetRequisitionList = 2001
	' associates with WBS item
End Enum

Public Enum FunctionExecutionType As Integer
	RetrievingData = 0
	ValidateData = 1
	ValidateAndPostData = 2
End Enum