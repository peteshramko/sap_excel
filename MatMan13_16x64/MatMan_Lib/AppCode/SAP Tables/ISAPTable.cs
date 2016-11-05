using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;

namespace iiiwave.MatManLib
{
    interface ISAPStructure
    {
        string                             StructureName         { get; }
        Dictionary<string, SAPField>       FieldList             { get; }
    }

	interface ISAPTable
    {
        string                             TableName             { get; }
        Dictionary<string, SAPField>       FieldList             { get; }
        Dictionary<string, ISAPTable>      TableList             { get; }
        Dictionary<string, ISAPStructure>  StructureList         { get; }
	}

    class SAPField
	{
		internal  string                   ShortDescription	     { get; }   
        internal  string                   FieldName             { get; }   
		internal  bool                     Key                   { get; }   
		internal  bool                     InitialValue          { get; }   
		internal  string                   DataElement           { get; }   
		internal  SAPDataType              DataType              { get; }   
		internal  int                      DataLength            { get; }   
		internal  int                      DecimalPlaces         { get; } 
        internal  object                   Data                  { get; set; }  

        internal SAPField(string       field_name,   bool  key,          bool  initial_value,   string data_element,   
                          SAPDataType  data_type,    int   data_length,  int   decimal_places,  string short_description)
        {
            FieldName         =  field_name;
            Key               =  key;
            InitialValue      =  initial_value;
            DataElement       =  data_element;
            DataType          =  data_type;
            DataLength        =  decimal_places;
            ShortDescription  =  short_description;            
        }
	}

    enum SAPDataType
	{
	    ACCP          =  1,
        CHAR          =  2,
        CLNT          =  3,
        CUKY          =  4,
        CURR          =  5,
        DATS          =  6,
        DEC           =  7,
        DF16_DEC      =  8,
        DF16_RAW      =  9,
        DF16_SCL      =  10,
        DF34_DEC      =  11,
        DF34_RAW      =  12,
        DF34_SCL      =  13,
        FLTP          =  14,
        INT1          =  15,
        INT2          =  16,
        INT4          =  17,
        LANG          =  18,
        LCHR          =  19,
        LRAW          =  20,
        NUMC          =  21,
        PREC          =  22,
        QUAN          =  23,
        RAW           =  24,
        RAW_STRING    =  25,
        XSTRING       =  26,
        STRING        =  27,
        STRU          =  28,
        TIMS          =  29,
        UNIT          =  30
	}
}
