using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace WSR_Forget_SDK.KeyWing
{
    [Serializable]
    public class PosWingColl : KeyedCollection<string,PosWingMDL>
    {
        protected override string GetKeyForItem(PosWingMDL item)
        {
            return item.KeyIndex;
        }

        public string GetKeyIndex(HashSet<int> objPosSet,string splitChar="|")
        {
            SortedSet<int> set = new SortedSet<int>(objPosSet);
            StringBuilder sb = new StringBuilder();
            int prev = -1;
            foreach (int k in set)
            {
                if (prev >= 0) sb.Append(splitChar);
                sb.Append(String.Format("{0}",k));
                prev = k;
            }
            return sb.ToString();
        }
    }
    [Serializable]
    public class PosWingMDL
    {
        private string _KeyIndex = String.Empty;        
        private HashSet<int> _PosSet = new HashSet<int>();
        private double _Weight = 0;

        public string KeyIndex
        {
            get
            {
                return _KeyIndex;
            }

            set
            {
                _KeyIndex = value;
            }
        }

        public HashSet<int> PosSet
        {
            get
            {
                return _PosSet;
            }

            set
            {
                _PosSet = value;
            }
        }

        public double Weight
        {
            get
            {
                return _Weight;
            }

            set
            {
                _Weight = value;
            }
        }
    }
 
}
