using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace iiiwave.MatManLib
{
    class ProjectTable : ISAPTable
    {
        public const string  MANDT    =  "MANDT";
        public const string  PSPNR    =  "PSPNR";
        public const string  PSPID    =  "PSPID";
        public const string  POST1    =  "POST1";
        public const string  OBJNR    =  "OBJNR";
        public const string  ERNAM    =  "ERNAM";
        public const string  ERDAT    =  "ERDAT";
        public const string  AENAM    =  "AENAM";
        public const string  AEDAT    =  "AEDAT";
        public const string  KIMSK    =  "KIMSK";
        public const string  AUTOD    =  "AUTOD";
        public const string  STSPD    =  "STSPD";
        public const string  STSPR    =  "STSPR";
        public const string  VERNR    =  "VERNR";
        public const string  VERNA    =  "VERNA";
        public const string  ASTNR    =  "ASTNR";
        public const string  ASTNA    =  "ASTNA";
        public const string  VBUKR    =  "VBUKR";
        public const string  VGSBR    =  "VGSBR";
        public const string  VKOKR    =  "VKOKR";
        
        //private SAPQuery                           m_sapQuery;
            
        private string                             m_tableName;
        private Dictionary<string, SAPField>       m_fieldList;
        private Dictionary<string, ISAPStructure>  m_structureList;
        private Dictionary<string, ISAPTable>      m_tableList;

        private static ProjectTable                m_proj;
        private static object                      syncRoot = new object();

        public static ProjectTable GetObject()
        {
            if (ProjectTable.m_proj == null)
            {
                lock (syncRoot)
                {
                    if (ProjectTable.m_proj == null)
                    {
                        ProjectTable.m_proj = new ProjectTable();
                    }
                }
            }
            return ProjectTable.m_proj;
        }

        public ProjectTable()
        {
            m_tableName       =  "PROJECT";
            m_fieldList       =  new Dictionary<string, SAPField>();
            m_structureList   =  new Dictionary<string, ISAPStructure>();
            m_tableList       =  new Dictionary<string, ISAPTable>();

                BuildFieldList();                
        }

        private void BuildFieldList()
        {
            m_fieldList.Add(ProjectTable.MANDT,      new SAPField(ProjectTable.MANDT,     true, true, "MANDT",       SAPDataType.CLNT, 3, 0, "Client"));
            m_fieldList.Add(ProjectTable.PSPNR,      new SAPField(ProjectTable.PSPNR,     true, true, "PS_INTNR",    SAPDataType.NUMC, 8, 0, "Project definition (internal)"));
            //m_fieldList.Add(new SAPField(ProjectTable.))
        }

        public string QueryProjectByObjectNumber(string objectNumber)
        {
            return "";
        }

        public void BuildTableFromReturnData(ref SAP.Middleware.Connector.IRfcTable  returnTable)
        {

        }

        public string TableName
        {
            get 
            {
                return m_tableName;
            }
        }

        public Dictionary<string, SAPField> FieldList
        {
            get
            {
                return m_fieldList;
            }
        }

        public Dictionary<string, ISAPStructure>  StructureList
        {
            get
            {
                return m_structureList;
            }
        }

        public Dictionary<string, ISAPTable> TableList
        {
            get
            {
                return m_tableList;
            }
        }

        //SAPQuery ISAPQueryTable.SAPQuery
        //{
        //    get
        //    {
        //        return m_sapQuery;
        //    }
        //}
    }
}
