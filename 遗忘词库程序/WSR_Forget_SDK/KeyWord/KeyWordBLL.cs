using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
using WSR_Forget_Core.KeyBond;

namespace WSR_Forget_SDK.KeyWord
{
    public class KeyWordBLL
    {
        public static void UpdateKeyWordColl(string line, KeyItemColl<string> objKeyWordColl, int nMaxWordSize = 7)
        {
            KeyWordBLL.UpdateKeyWordCollByNGram(line, objKeyWordColl, nMaxWordSize);
        }
        
        #region N_Gram词库
        public static void UpdateKeyWordCollByNGram(string line, KeyItemColl<string> objKeyWordColl,int nMaxWordSize=7 )
        {
            string text = Regex.Replace(line, @"\p{C}", "");
            string[] parts =   Regex.Split(text, @"([\s\p{P}\p{C}])");
            foreach (string part in parts)
            {                
                if (String.IsNullOrEmpty(part)) continue;
                for (int k = 0; k < part.Length; k++)
                {
                    for (int s = 0; s < nMaxWordSize; s++)
                    {
                        int nStartPos = k - s;
                        if (nStartPos < 0) break;
                        string keyword = part.Substring(nStartPos, s+1);
                        WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objKeyWordColl, new OffsetWeightMDL(1, 1));
                    }
                }
            }
        }
        #endregion
    }
}
