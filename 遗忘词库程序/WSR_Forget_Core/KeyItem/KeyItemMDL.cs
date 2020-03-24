using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WSR_Forget_Core.Memory;

namespace WSR_Forget_Core.KeyItem
{
    [Serializable]
    public class KeyItemColl<T> : KeyedCollection<T, KeyItemMDL<T>>
    {
       
        private MemoryParameter _Parameter = new MemoryParameter();

        public MemoryParameter Parameter
        {
            get
            {
                return _Parameter;
            }

            set
            {
                _Parameter = value;
            }
        }

        protected override T GetKeyForItem(KeyItemMDL<T> item)
        {
            return item.Key;
        }
    }

    [Serializable]
    public class KeyItemMDL<T> : MemoryMDL
    {
        private T _Key = default(T);
       

        public T Key
        {
            get
            {
                return _Key;
            }

            set
            {
                _Key = value;
            }
        }

        
    }
}
