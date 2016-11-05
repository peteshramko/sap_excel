using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiiwave.MatManLib
{
    class ProjectDefStructure : ISAPStructure
    {
        private string                             m_structureName;
        private Dictionary<string, SAPField>       m_fieldList;

        private static ProjectDefStructure         m_projDef;
        private static object                      syncRoot = new object();

        public static ProjectDefStructure GetObject()
        {
            if (ProjectDefStructure.m_projDef == null)
            {
                lock (syncRoot)
                {
                    if (ProjectDefStructure.m_projDef == null)
                    {
                        ProjectDefStructure.m_projDef = new ProjectDefStructure();
                    }
                }
            }
            return ProjectDefStructure.m_projDef;
        }

        public ProjectDefStructure()
        {
            m_structureName   =  "PROJECT_DEFINITION";
            m_fieldList       =  new Dictionary<string, SAPField>();

                BuildFieldList();                
        }

        private void BuildFieldList()
        {
            
        }

        public string StructureName
        {
            get 
            {
                return m_structureName;
            }
        }

        public Dictionary<string, SAPField> FieldList
        {
            get
            {
                return m_fieldList;
            }
        }
    }
}
