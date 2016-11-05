using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;


namespace iiiwave.MatManLib
{
    public enum FormulaValueType : int
    {
	    CellAddress = 0,
	    UserValue   = 1
    }

    public class CellValue
    {
	    private FormulaValueType  m_Fvt;
	    private string            m_Str;
	    
        public CellValue(FormulaValueType fvt, string str)
	    {
		    m_Fvt = fvt;
		    m_Str = str;
	    }

        public FormulaValueType FormulaType
        {
            get
            {
                return m_Fvt;
            }
        }

        public string ParamValue
        {
            get
            {
                return m_Str;
            }
        }
    }
    
    public static class MatManExtensions
    {
	    public static bool IsLoadCostPlanFunction(this string functionString)
        {
            int endFunctionIndex    =  functionString.IndexOf('('); 
            if(endFunctionIndex > 0)
            {
                string functionName     =  functionString.Substring(1, (endFunctionIndex-1) );            
                if(functionName.ToUpper() == "PW_LOAD_COST_PLAN")
                    return true;
            }           
            
            return false;        
        }

        public static bool IsLoadNewGLPlanFunction(this string functionString)
        {
            int endFunctionIndex    =  functionString.IndexOf('(');            
            if(endFunctionIndex > 0)
            {
                string functionName  =  functionString.Substring(1, (endFunctionIndex-1) );            
                if(functionName.ToUpper() == "PW_LOAD_NEWGL_PLAN")
                    return true;
            }
            return false;
        }

        public static bool IsLoadActivityPlanFunction(this string functionString)
        {
            int endFunctionIndex    =  functionString.IndexOf('(');            
            if(endFunctionIndex > 0)
            {
                string functionName  =  functionString.Substring(1, (endFunctionIndex-1) );            
                if(functionName.ToUpper() == "PW_LOAD_ACTIVITY_PLAN")
                    return true;
            }
            return false;
        }

        public static bool IsLoadPCAPlanFunction(this string functionString)
        {
            int endFunctionIndex    =  functionString.IndexOf('(');            
            if(endFunctionIndex > 0)
            {
                string functionName  =  functionString.Substring(1, (endFunctionIndex-1) );            
                if(functionName.ToUpper() == "PW_LOAD_PCA_PLAN")
                    return true;
            }
            return false;
        }
        
        public static List<CellValue> GetFormulaParameters(this string functionString)
        {
            List<CellValue> cellValueList = new List<CellValue>();
            
            //// Set to US Culture
            
            //if (!PlanningWandCOMClass.IsUSCulture) 
            //{
            //    PlanningWandCOMClass.SetUSCulture();
            //}

            int        startString   =  functionString.IndexOf("(");
			int        endString     =  functionString.LastIndexOf(")");
			string     s             =  functionString.Substring((startString + 1), (endString - startString - 1));

			// ----- Array Implementation (Check for empty parameter values without quotations) ----- '				

			char[]     cArray        =  s.ToArray();
			int        index         =  0;
			string     aParam        =  string.Empty;
			CellValue  cellValue     =  null;

            try
            {            
			    while (index < cArray.Length) 
                {
				    char currentChar = cArray[index];

				    //If index < cArray.Length Then
				    // Check for zero-length parameter
				    // Comma - Empty Param
				    if (index == 0 && currentChar == ',') 
                    {
					    aParam     =  "\" \"";
					    // Assign Empty Space
					    cellValue  =  new CellValue(FormulaValueType.UserValue, aParam);
					    aParam     =  string.Empty;
					    cellValueList.Add(cellValue);
				    } 
                    else if (index == 0) 
                    {
					    aParam += currentChar;					
				    } 
                    // Check for parameter with length > 0 and containing quotations (USER VALUES)
                    else if (index > 0 && aParam.Contains("\"")) 
                    {
					    // Check to determine end of parameter with both parenthesis (Chr(34))
					    // End of Parameter
					    if (currentChar == '"' || index == (cArray.Length - 1)) 
                        {
						    cellValue = new CellValue(FormulaValueType.UserValue, aParam.Replace(("\""), string.Empty));
						    cellValueList.Add(cellValue);
						    aParam    = string.Empty;
						    index    += 1;
					    } 
                        else 
                        {
						    aParam   += currentChar;
					    }					
				    } 
                    // Check for parameter with length > 0 and NOT containing quotations (CELL REFERENCES)
                    else if (index > 0 && !aParam.Contains("\"")) 
                    {
					    // comma
					    if (currentChar == ',' || currentChar == ';') 
                        {
						    cellValue = new CellValue(FormulaValueType.CellAddress, aParam.Trim());
						    cellValueList.Add(cellValue);
						    aParam    = string.Empty;
					    }
                        else if(index == (cArray.Length - 1))
                        {
                            // Append current character
                            aParam   += currentChar;

                            // Add to list
                            cellValue = new CellValue(FormulaValueType.CellAddress, aParam.Trim());
						    cellValueList.Add(cellValue);
						    aParam    = string.Empty;
                        }
                        else 
                        {
						    aParam   += currentChar;
					    }
				    }
				    index += 1;
				    //End If	
			    }
            }
            catch(Exception exp)
            {
                throw new Exception("Please check format of parameter values in Formula");
            }

            //// Reset to original culture
            
            //PlanningWandCOMClass.SetOriginalCulture();
        
            return cellValueList;
        }
        
        private static string[] FormatTrimWhiteSpaces(string[] functionParameters)
	    {
		    List<string> revisedParameterList = new List<string>();

		    foreach (string s in functionParameters) 
            {
			    List<string> stringList = new List<string>();

			    int index = 0;

			    if (s.Contains(",")) 
                {
				    while (index >= 0 && index < s.Length) 
                    {
					    int endString = s.IndexOf(',', index);
					    int length = endString - index;

					    if (endString > 0) 
                        {
						    string rS = s.Substring(index, length);
						    stringList.Add(rS.Trim());
					    } 
                        else 
                        {
						    string rS = s.Substring(index);
						    stringList.Add(rS.Trim());
						    break; // TODO: might not be correct. Was : Exit While
					    }

					    index = index + length + 1;
				    }
			    } 
                else 
                {
				    stringList.Add(s);
			    }

			    string paramValues = String.Empty;
			    int i = 1;
			    foreach (string myString in stringList) 
                {
				    if (i < stringList.Count) 
                    {
					    paramValues += myString + Convert.ToString(",");
				    } 
                    else 
                    {
					    paramValues += myString;
				    }
				    i++;
			    }

			    revisedParameterList.Add(paramValues);
		    }

		    return revisedParameterList.ToArray();
	    }

	    public static DataTable GetDataTableWithPermutations(this string[] functionParameters)
	    {
		    DataTable sourceTable = new DataTable();

		    List<string> requestStrings = new List<string>(FormatTrimWhiteSpaces(functionParameters));

		    int selectionSize = 0;
		    for (int i = 0; i <= requestStrings.Count - 1; i++) 
            {
			    int count = 0;
			    string[] requestParams = requestStrings[i].Split(new char[] { ',' });
			    for (int j = 0; j <= requestParams.Length - 1; j++) 
                {
				    count += 1;
			    }

			    if (selectionSize < count) 
                {
				    selectionSize = count;
			    }
		    }

		    var myList = requestStrings.Permutations(selectionSize, true);

		    int rowCount = 0;
		    int columnCount = 0;
		    foreach (string[] row in myList) 
            {
			    DataRow dRow = sourceTable.NewRow();
			    sourceTable.Rows.Add(dRow);

			    foreach (string value in row) 
                {
				    DataColumn dColumn = sourceTable.Columns.Add();
				    sourceTable.Rows[rowCount][columnCount] = value;
				    columnCount++;
			    }

			    rowCount++;
		    }

		    return sourceTable;
	    }

        public static string Concatenate(this System.Array functionStrings)
        {
	        string concatenatedString = string.Empty;

	        foreach (object aObject in functionStrings) 
            {
		        string aString = aObject.ToString().ToUpper();
		        if (aString != string.Empty) 
                {
			        concatenatedString += aString + "\t";
		        } 
                else 
                {
			        concatenatedString += "\t";
		        }
	        }

	        return concatenatedString;
        }
    }
}