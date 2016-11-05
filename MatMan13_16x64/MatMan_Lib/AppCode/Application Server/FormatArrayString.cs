using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

/// <summary>
/// PWExtensions:
/// A LINQ extension class that a "Jagged" List of function strings and converts
/// those strings into a DataTable with all possible value permutations. 
/// The list of function strings is intended to be from a client update 
/// of excel triggering an SAP update. Values passed from Excel are parsed here.
/// </summary>
/// 
namespace iiiwave.MatManLib
{
    public static class MatManExtentions
    {
	    private static string[] FormatTrimWhiteSpaces(string[] functionParameters)
	    {
		    List<string> revisedParameterList = new List<string>();

		    foreach (string s in functionParameters)
            {
			    List<string> stringList = new List<string>();

			    int index = 0;

			    if (s.Contains(','))
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
						    break;
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
				    i += 1;
			    }

			    revisedParameterList.Add(paramValues);
		    }

		    return revisedParameterList.ToArray();
	    }

	    public static DataTable GetDataTableWithPermutations(this string[] functionParameters)
	    {
		    DataTable sourceTable = new DataTable();

		    string[] requestStrings = FormatTrimWhiteSpaces(functionParameters);

		    int selectionSize = 0;
		    for (int i = 0; i <= requestStrings.Length - 1; i++)
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
				    columnCount += 1;
			    }

			    rowCount += 1;
		    }

		    return sourceTable;
	    }
    }
}