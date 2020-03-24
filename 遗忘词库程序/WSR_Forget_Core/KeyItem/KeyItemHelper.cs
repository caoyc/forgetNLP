using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using WSR_Forget_Core.Memory;

namespace WSR_Forget_Core.KeyItem
{
    public class KeyItemHelper
    {
        #region 计算遗忘系数
        public static double CalcRemeberValue<T>(T key,KeyItemColl<T> objMemoryItemColl)
        {
            if (objMemoryItemColl.Contains(key))
            {
                return MemoryDAL.CalcRemeberValue(objMemoryItemColl.Parameter.TotalOffset - objMemoryItemColl[key].UpdateOffset,objMemoryItemColl.Parameter);
            }
            return 0;
        }

        public static double CalcRemeberValue<T>(KeyItemMDL<T> mdl,KeyItemColl<T> objMemoryBondColl)
        {
            return MemoryDAL.CalcRemeberValue(objMemoryBondColl.Parameter.TotalOffset - mdl.UpdateOffset,objMemoryBondColl.Parameter);
        }
        
        #endregion
    

        #region 计算有效值
        public static double CalcValidCount<T>(KeyItemMDL<T> mdl, KeyItemColl<T> objKeyItemColl)
        {

            return mdl.ValidCount *  CalcRemeberValue(mdl, objKeyItemColl );

        }
        public static double CalcValidCount<T>(T key, KeyItemColl<T> objKeyItemColl)
        {
            if (!objKeyItemColl.Contains(key)) return 0;

            KeyItemMDL<T> mdl = objKeyItemColl[key];
            return mdl.ValidCount *  CalcRemeberValue(mdl, objKeyItemColl );

        }

        #endregion

      
        #region 显示词库
        public static string ShowKeyItemColl(KeyItemColl<string> objKeyItemColl, int nTopCount, bool bIsOnlyWord = true, bool bIsOrderbyDesc = true, bool bIsShowTitle = true, string splitChar = "\t", string spaceChar = "\r")
        {
            StringBuilder sb = new StringBuilder();
            if (bIsShowTitle)
            {
                sb.AppendLine(String.Format("[{0}]{1}|{2}|{3}|{4}", "词项", "遗忘词频", "总词频", "词权重", "成熟度"));
            }

            IEnumerable<KeyItemMDL<string>> buffer = objKeyItemColl;
            if (bIsOrderbyDesc)
            {
                buffer = from x in objKeyItemColl
                         let dRemeberValue = x.ValidCount * CalcRemeberValue(x, objKeyItemColl )
                         where (!bIsOnlyWord || (bIsOnlyWord && x.Key.Length > 1 && !Regex.IsMatch(x.Key,@"^[\d\p{P}\p{C}a-zA-Z]+$")) ) 
                         orderby dRemeberValue descending
                         select x;
            }
            else
            {
                buffer = from x in objKeyItemColl
                         let dRemeberValue = x.ValidCount *  CalcRemeberValue(x, objKeyItemColl )
                         where (!bIsOnlyWord || (bIsOnlyWord && x.Key.Length > 1 && !Regex.IsMatch(x.Key, @"^[\d\p{P}\p{C}a-zA-Z]+$")))
                         orderby dRemeberValue ascending
                         select x;
            }
            sb.AppendLine(String.Format("=============={0}/{1}|{2}=============", buffer.Count(), Math.Round(buffer.Sum(x => x.ValidCount * CalcRemeberValue(x, objKeyItemColl ))),Math.Round(objKeyItemColl.Parameter.Entropy,4)));
            double dTotalVaildCount = 1.0 / (1.0 - MemoryDAL.CalcRemeberValue(1, objKeyItemColl.Parameter));
            int nRecordCount = 0;
            //buffer = buffer.OrderByDescending(x => x.ValidDegree);
            foreach (KeyItemMDL<string> mdl in buffer)
            {
                //if (!mdl.Key.Contains(splitChar)) continue;
                double dRemeberValue = mdl.ValidCount *  CalcRemeberValue(mdl, objKeyItemColl );
                //double dProbValue = dRemeberValue / dTotalVaildCount;
                if (dRemeberValue < objKeyItemColl.Parameter.Threshold) continue;
                //if (-dProbValue * Math.Log(dProbValue) < 0.0005) continue;//小于平均信息熵
                if (nRecordCount >= nTopCount) break;
                sb.AppendLine(String.Format("[{0}]{1}|{2}|{3}|{4}", mdl.Key.Replace(splitChar,"×").Replace(spaceChar,"$"), Math.Round(dRemeberValue, 4), Math.Round(mdl.TotalCount), Math.Round(dRemeberValue * Math.Log(dTotalVaildCount /dRemeberValue ), 4),""));
                nRecordCount += 1;
            }
            return sb.ToString();
        }
        #endregion
    }
}
