using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSR_Forget_Core.KeyBond;
using WSR_Forget_Core.KeyItem;

namespace ForgetNLP_WordLib_Demo.KeyWord
{
    public class DataConfig
    {
        private WSR_Forget_Core.KeyBond.KeyBondColl<string, string> _BasicCharBondColl = new WSR_Forget_Core.KeyBond.KeyBondColl<string, string>();
        private WSR_Forget_Core.KeyItem.KeyItemColl<string> _BasicKeyWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

       
        private WSR_Forget_Core.KeyBond.KeyBondColl<string, string> _CombinCharBondColl = new WSR_Forget_Core.KeyBond.KeyBondColl<string, string>();
        private WSR_Forget_Core.KeyItem.KeyItemColl<string> _CombinKeyWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

        private WSR_Forget_Core.KeyItem.KeyItemColl<string> _StandardWordColl = new KeyItemColl<string>();
        private WSR_Forget_Core.KeyItem.KeyItemColl<string> _TopicWordColl = new KeyItemColl<string>();
        private WSR_Forget_Core.KeyItem.KeyItemColl<string> _IncrementWordColl = new KeyItemColl<string>();

        public KeyBondColl<string, string> BasicCharBondColl
        {
            get
            {
                return _BasicCharBondColl;
            }

            set
            {
                _BasicCharBondColl = value;
            }
        }

        public KeyItemColl<string> BasicKeyWordColl
        {
            get
            {
                return _BasicKeyWordColl;
            }

            set
            {
                _BasicKeyWordColl = value;
            }
        }
             
        public KeyBondColl<string, string> CombinCharBondColl
        {
            get
            {
                return _CombinCharBondColl;
            }

            set
            {
                _CombinCharBondColl = value;
            }
        }

        public KeyItemColl<string> CombinKeyWordColl
        {
            get
            {
                return _CombinKeyWordColl;
            }

            set
            {
                _CombinKeyWordColl = value;
            }
        }

        public KeyItemColl<string> StandardWordColl
        {
            get
            {
                return _StandardWordColl;
            }

            set
            {
                _StandardWordColl = value;
            }
        }

        public KeyItemColl<string> TopicWordColl
        {
            get
            {
                return _TopicWordColl;
            }

            set
            {
                _TopicWordColl = value;
            }
        }

        public KeyItemColl<string> IncrementWordColl
        {
            get
            {
                return _IncrementWordColl;
            }

            set
            {
                _IncrementWordColl = value;
            }
        }
    }
 
    public class OptionConfig
    {
    }
}
