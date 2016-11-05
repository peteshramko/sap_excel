using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    interface ISAPQueryTable
    {
        string                              TableName         { get; }
        string                              m_tableJoinName   { get; }
        string                              JoinCondition     { get; }
        List<string>                        FieldNames        {get; }
        //KeyValuePair <string, List<string>> m_orderByFields;
        //KeyValuePair <string, List<string>> m_conditions;
        //KeyValuePair <string, List<string>> m_columnNames;
        //KeyValuePair <string, List<string>> m_columnCaptions;
    }

    //class SAPQuery
    //{
    //    private string                              m_tableName;
    //    private string                              m_tableJoinName;
    //    private string                              m_joinCondition;
    //    private KeyValuePair <string, List<string>> m_filedNames;
    //    private KeyValuePair <string, List<string>> m_orderByFields;
    //    private KeyValuePair <string, List<string>> m_conditions;
    //    private KeyValuePair <string, List<string>> m_columnNames;
    //    private KeyValuePair <string, List<string>> m_columnCaptions;

    //    SAPQuery(string _tableName)
    //    {

    //    }
    //}
}
