using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace iiiwave.MatManLib
{
    public class MatManCompareFunction : IComparer<string>
    {
	    public int Compare(string x, string y)
	    {
		    string s1 = string.Empty;
		    string s2 = string.Empty;
        
            for (int index = (x.Count() - 1); index >= 0; index--) 
            {
			    if(Regex.IsMatch(x[index].ToString(), "[0-9]"))
                {
                    s1 += x[index];
			    } 
                else
                {
				    continue;
			    }
		    }

		    for (int index = (y.Count() - 1); index >= 0; index--) 
            {
			    if(Regex.IsMatch(y[index].ToString(), "[0-9]"))
                {
				    s2 += y[index];
			    } 
                else 
                {
				    continue;
			    }
		    }

		    string firstString = new string(s1.Reverse().ToArray());
		    string nextString  = new string(s2.Reverse().ToArray());

		    if (Convert.ToInt32(firstString) < Convert.ToInt32(nextString)) 
            {
			    return -1;
		    } 
            else 
            {
			    return 1;
		    }

	    }
    }
}