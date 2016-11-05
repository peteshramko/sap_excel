using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace iiiwave.MatManLib
{
    public  delegate void AddReturnEventHandler();

    //[ClassInterface(ClassInterfaceType.None)]
    public class MatManReturnValueDictionary : Dictionary<string, IMatManFunction>
    {
        public event         AddReturnEventHandler         OnAddReturnValues; 
        
        public MatManReturnValueDictionary()
        {

        }    
		
        public new void Add(string _key, IMatManFunction _function)
        {
            base.Add(_key, _function);

            OnAddReturnValues?.Invoke();
        }

        public new IMatManFunction this[string _key]
        {
            get
            {
                return base[_key];
            }
            set
            {
                base[_key] = value;

                OnAddReturnValues?.Invoke();
            }
        }
    }
}
