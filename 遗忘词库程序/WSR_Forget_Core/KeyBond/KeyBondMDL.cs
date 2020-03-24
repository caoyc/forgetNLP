using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
namespace WSR_Forget_Core.KeyBond
{
    [Serializable]
    public class KeyBondColl<T, L> : KeyedCollection<T, KeyBondMDL<T, L>>
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
        protected override T GetKeyForItem(KeyBondMDL<T, L> item)
        {
            return item.KeyItem.Key;
        }
    }

    [Serializable]
    public class KeyBondMDL<T, L>
    {
        private KeyItemMDL<T> _KeyItem = new KeyItemMDL<T>();
        private KeyItemColl<L> _LinkColl = new KeyItemColl<L>();

        public KeyItemMDL<T> KeyItem
        {
            get
            {
                return _KeyItem;
            }

            set
            {
                _KeyItem = value;
            }
        }

        public KeyItemColl<L> LinkColl
        {
            get
            {
                return _LinkColl;
            }

            set
            {
                _LinkColl = value;
            }
        }
    }
}
