using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgetNLP_WordLib_Demo.KeyWord;
 

namespace ForgetNLP_WordLib_Demo.Config
{
    public class DataConfig
    {
        private string _FileName = String.Empty;
        private KeyWord.DataConfig _KeyWord = new KeyWord.DataConfig();

        public string FileName
        {
            get
            {
                return _FileName;
            }

            set
            {
                _FileName = value;
            }
        }

        public KeyWord.DataConfig KeyWord
        {
            get
            {
                return _KeyWord;
            }

            set
            {
                _KeyWord = value;
            }
        }
    }
 
    public class OptionConfig
    {
    }
}
