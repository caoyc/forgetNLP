using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
namespace WSR_Forget_Core.KeyBond
{
    public class KeyBondHelper
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
        public static double CalcRemeberValue<T, L>(T key,KeyBondColl<T,L> objMemoryBondColl)
        {
            if (objMemoryBondColl.Contains(key))
            {
                return MemoryDAL.CalcRemeberValue(objMemoryBondColl.Parameter.TotalOffset - objMemoryBondColl[key].KeyItem.UpdateOffset,objMemoryBondColl.Parameter);
            }
            return 0;
        }

        public static double CalcRemeberValue<T, L>(KeyBondMDL<T,L> mdl,KeyBondColl<T,L> objMemoryBondColl)
        {
            return MemoryDAL.CalcRemeberValue(objMemoryBondColl.Parameter.TotalOffset - mdl.KeyItem.UpdateOffset,objMemoryBondColl.Parameter);
        }


        public static double CalcTailValidCount<T, L>(T head,L tail,KeyBondColl<T,L> objMemoryBondColl)
        {
            if (!objMemoryBondColl.Contains(head)) return 0;
            if (!objMemoryBondColl[head].LinkColl.Contains(tail)) return 0;
            return KeyItemHelper.CalcValidCount(tail,objMemoryBondColl[head].LinkColl);
        }

        public static double CalcHeadValidCount<T, L>(T head, KeyBondColl<T, L> objMemoryBondColl)
        {
            if (!objMemoryBondColl.Contains(head)) return 0;
            return objMemoryBondColl[head].KeyItem.ValidCount * CalcRemeberValue(head, objMemoryBondColl);
        }
        #endregion
        #region 计算关联系数


        public static double GetRandomNumber(double min,double max)
        {
            return ( new Random(Guid.NewGuid().GetHashCode()) ).NextDouble() * ( max - min ) + min;
        }
        public static bool IsBondValid<T>(T keyHead,T keyTail,KeyBondColl<T,T> objMemoryBondColl)
        {
            double dRate= CalcBondRelateValue<T>(keyHead,keyTail,objMemoryBondColl) ;
            return GetRandomNumber(0, dRate) > GetRandomNumber(0, Math.E);
            
        }
        /// <summary>
        /// 计算关联系数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyHead"></param>
        /// <param name="keyTail"></param>
        /// <param name="objKeyBondColl"></param>
        /// <returns></returns>
        public static double CalcBondRelateValue<T>(T keyHead, T keyTail, KeyBondColl<T, T> objKeyBondColl)
        {
            return CalcBondRelateValueByPMI<T>(keyHead, keyTail, objKeyBondColl);
        }

        public static double CalcBondRelateValueWithLaplace<T>(T keyHead, T keyTail, KeyBondColl<T, T> objKeyBondColl)
        {

            //分别获得相邻单项的频次
            double dHeadValidCount = objKeyBondColl.Contains(keyHead) ? 1 + objKeyBondColl[keyHead].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyHead, objKeyBondColl) : 1;
            double dTailValidCount = objKeyBondColl.Contains(keyTail) ? 1 + objKeyBondColl[keyTail].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyTail, objKeyBondColl) : 1;
            double dTotalValidCount = 1 + 1.0 / (1 - MemoryDAL.CalcRemeberValue(1, objKeyBondColl.Parameter));
            //获得相邻项共现的频次
            KeyItemColl<T> objLinkColl = objKeyBondColl.Contains(keyHead) ? objKeyBondColl[keyHead].LinkColl : new KeyItemColl<T>();

            KeyItemMDL<T> mdl = objLinkColl.Contains(keyTail) ? objLinkColl[keyTail] : new KeyItemMDL<T>();
            double dShareValidCount = 1 + mdl.ValidCount * KeyItemHelper.CalcRemeberValue(mdl, objLinkColl);
            double dShareTotalCount = objKeyBondColl.Contains(keyHead) ? 1 + objKeyBondColl[keyHead].KeyItem.TotalCount : 1;


            //P(AB)=P(B|A)*P(A)
            //result=P(AB)/(P(A)*P(B))=P(B|A)/P(B)
            return (dShareValidCount / dShareTotalCount) / (dTailValidCount / dTotalValidCount);

        }
        /// <summary>
        /// 使用互信息计算关联系数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyHead"></param>
        /// <param name="keyTail"></param>
        /// <param name="objKeyBondColl"></param>
        /// <returns></returns>
        public static double CalcBondRelateValueByPMI<T>(T keyHead,T keyTail,KeyBondColl<T,T> objKeyBondColl)
        {

            ////分别获得相邻单项的频次
            //double dHeadValidCount = objKeyBondColl.Contains(keyHead) ? 1 + objKeyBondColl[keyHead].KeyItem.ValidCount * CalcRemeberValue<T,T>(keyHead,objKeyBondColl) : 1;
            //double dTailValidCount = objKeyBondColl.Contains(keyTail) ? 1 + objKeyBondColl[keyTail].KeyItem.ValidCount * CalcRemeberValue<T,T>(keyTail,objKeyBondColl) : 1;
            //double dTotalValidCount = 1 + 1.0 / ( 1 - MemoryDAL.CalcRemeberValue(1,objKeyBondColl.Parameter) );
            ////获得相邻项共现的频次
            //KeyItemColl<T>  objLinkColl = objKeyBondColl.Contains(keyHead) ? objKeyBondColl[keyHead].LinkColl : new KeyItemColl<T>();

            //KeyItemMDL<T> mdl = objLinkColl.Contains(keyTail) ? objLinkColl[keyTail] : new KeyItemMDL<T>();
            //double dShareValidCount = 1 + mdl.ValidCount * KeyItemHelper.CalcRemeberValue(mdl,objLinkColl);
            //double dShareTotalCount = objKeyBondColl.Contains(keyHead) ? 1 + objKeyBondColl[keyHead].KeyItem.TotalCount : 1;

            //if (!objKeyBondColl.Contains(keyHead) || !objKeyBondColl.Contains(keyTail)) return 0;
            //if (!objKeyBondColl[keyHead].LinkColl.Contains(keyTail)) return 0;

            if (!objKeyBondColl.Contains(keyHead) || !objKeyBondColl.Contains(keyTail)) return 0;
            if (!objKeyBondColl[keyHead].LinkColl.Contains(keyTail)) return 0;

            //分别获得相邻单项的频次
            double dHeadValidCount = objKeyBondColl[keyHead].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyHead, objKeyBondColl);
            double dTailValidCount = objKeyBondColl[keyTail].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyTail, objKeyBondColl);           
            if (dHeadValidCount < objKeyBondColl.Parameter.Threshold || dTailValidCount < objKeyBondColl.Parameter.Threshold) return 0;
            double dTotalValidCount = objKeyBondColl.Parameter.TotalValidCount;// 1.0 / (1 - MemoryDAL.CalcRemeberValue(1, objKeyBondColl.Parameter));

            //获得相邻项共现的频次
            KeyItemColl<T> objLinkColl = objKeyBondColl[keyHead].LinkColl;

            KeyItemMDL<T> mdl = objLinkColl[keyTail];
            double dShareValidCount = mdl.ValidCount * KeyItemHelper.CalcRemeberValue(mdl, objLinkColl);
            double dShareTotalCount = objLinkColl.Parameter.TotalValidCount;
            if (dShareTotalCount < objLinkColl.Parameter.Threshold || dShareValidCount < objLinkColl.Parameter.Threshold) return 0;

            //P(AB)=P(B|A)*P(A)
            //result=P(AB)/(P(A)*P(B))=P(B|A)/P(B)
            return ( dShareValidCount / dShareTotalCount ) / ( dTailValidCount / dTotalValidCount );
          
        }
        /// <summary>
        /// 使用平均信息熵计算关联
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyHead"></param>
        /// <param name="keyTail"></param>
        /// <param name="objKeyBondColl"></param>
        /// <returns></returns>
        public static double CalcBondRelateValueByAverageEntropy<T>(T keyHead, T keyTail, KeyBondColl<T, T> objKeyBondColl)
        {
            if (!objKeyBondColl.Contains(keyHead) || !objKeyBondColl.Contains(keyTail)) return 0;
            if (!objKeyBondColl[keyHead].LinkColl.Contains(keyTail)) return 0;

            

            //分别获得相邻单项的频次
            double dHeadValidCount = objKeyBondColl[keyHead].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyHead, objKeyBondColl);
            double dTailValidCount = objKeyBondColl[keyTail].KeyItem.ValidCount * CalcRemeberValue<T, T>(keyTail, objKeyBondColl);

            if (dHeadValidCount < objKeyBondColl.Parameter.Threshold || dTailValidCount < objKeyBondColl.Parameter.Threshold) return 0;

            //获得相邻项共现的频次
            KeyItemColl<T> objLinkColl = objKeyBondColl[keyHead].LinkColl;

            KeyItemMDL<T> mdl = objLinkColl[keyTail];
            double dShareValidCount =  mdl.ValidCount * KeyItemHelper.CalcRemeberValue(mdl, objLinkColl);
            double dShareTotalCount = objLinkColl.Parameter.TotalValidCount;
            if (dShareTotalCount < objLinkColl.Parameter.Threshold || dShareValidCount < objLinkColl.Parameter.Threshold) return 0;

            double dEuler = 0.5772156649;
            double dKeywordCount = objKeyBondColl[keyHead].LinkColl.Count;
            double dAverageEntropy = Math.Log(dKeywordCount) + dEuler -1;

            double dKeywordEntropy = (dKeywordCount * dShareValidCount / dShareTotalCount) * (Math.Log(dShareTotalCount) - Math.Log(dShareValidCount));
            return dKeywordEntropy- dAverageEntropy;
        }

        #endregion
        #region 联想
        /// <summary>
        /// 当关键项列表发生时，获取关联项一次都不发生时的概率对数，以及包含的关键项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <param name="objKeyList"></param>
        /// <param name="objMemoryBondColl"></param>
        /// <param name="objLinkProbDict">注：此处存放的是相关项一次都不发生的概率的对数</param>
        /// <param name="objLinkKeyDict"></param>
        public static void UpdateKeyLinkDict<T,L>(List<T> objKeyList,KeyBondColl<T,L> objMemoryBondColl,Dictionary<L,double> objLinkProbDict,Dictionary<L,List<T>> objLinkKeyDict,Dictionary<L,List<int>> objLinkPosDict)
        {
            for(int k=0;k<objKeyList.Count;k++)            
            {
                T key = objKeyList[k];
                if (!objMemoryBondColl.Contains(key)) continue;
                KeyItemMDL<T> objKeyMDL = objMemoryBondColl[key].KeyItem;
                KeyItemColl<L> objLinkColl = objMemoryBondColl[key].LinkColl;

                double dKeyValidCount =/* objKeyMDL.ValidCount * */ objKeyMDL.TotalCount * CalcRemeberValue<T,L>(objKeyMDL.Key,objMemoryBondColl);
                
                if (dKeyValidCount < objMemoryBondColl.Parameter.Threshold) continue;
                foreach (KeyItemMDL<L> link in objLinkColl)
                {
                    double dLinkValidCount = link.ValidCount * CalcRemeberValue<L>(link,objLinkColl);
                    if (!objLinkProbDict.ContainsKey(link.Key)) objLinkProbDict.Add(link.Key,0);
                    double dLinkProb = dLinkValidCount / dKeyValidCount;
                    objLinkProbDict[link.Key] += dLinkProb >= 1 ? 0 : Math.Log(1 - dLinkProb); //不发生的概率

                    if (!objLinkKeyDict.ContainsKey(link.Key)) objLinkKeyDict.Add(link.Key,new List<T>());
                    objLinkKeyDict[link.Key].Add(objKeyMDL.Key);

                    if (!objLinkPosDict.ContainsKey(link.Key)) objLinkPosDict.Add(link.Key,new List<int>());
                    objLinkPosDict[link.Key].Add(k);
                }
            }
        }


        #endregion
        #region 显示词库
        public static string ShowKeyBondCollEx(KeyBondColl<string, string> objKeyBondColl, List<string> objKeyWordList, int nLinkTopCount, bool bIsOrderbyDesc = true, string splitChar = "\t", string spaceChar = "\r")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("[{0}]{1}|{2}|{3}|{4}", "词项", "遗忘词频", "总词频", "词权重", "成熟度"));
            sb.AppendLine("=============================================");


            StringBuilder sbkey = new StringBuilder();
            HashSet<KeyItemMDL<string>> objBufferSet = new HashSet<KeyItemMDL<string>>();
            foreach (string keyword in objKeyWordList)
            {
                if (String.IsNullOrWhiteSpace(keyword)) continue;               
                if (!objKeyBondColl.Contains(keyword)) continue;
                if (sbkey.Length > 0) sbkey.Append("、");
                sbkey.Append(keyword);
                KeyBondMDL<string, string> bond = objKeyBondColl[keyword];
                if (objBufferSet.Count <= 0)
                {
                    objBufferSet.UnionWith(bond.LinkColl);
                }
                else
                {
                    HashSet<KeyItemMDL<string>> buffer = new HashSet<KeyItemMDL<string>>();
                    foreach (KeyItemMDL<string> mdl in objBufferSet)
                    {
                        if (bond.LinkColl.Contains(mdl.Key))
                        {
                            buffer.Add(mdl);
                        }
                    }
                    objBufferSet = new HashSet<KeyItemMDL<string>>(buffer);
                }
            }
            KeyItemColl<string> objBufferColl = new KeyItemColl<string>();
            foreach (KeyItemMDL<string> mdl in objBufferSet)
            {
                if (!objBufferColl.Contains(mdl.Key))
                {
                    objBufferColl.Add(mdl);
                }
            }

            sb.AppendLine();
            sb.AppendLine(String.Format("【{0}】", sbkey));
            sb.Append(KeyItemHelper.ShowKeyItemColl(objBufferColl, nLinkTopCount, false, bIsOrderbyDesc, false, splitChar, spaceChar));

            return sb.ToString();
        }
        public static string ShowKeyBondColl(KeyBondColl<string, string> objKeyBondColl, List<string> objKeyWordList, int nLinkTopCount, bool bIsOrderbyDesc = true, string splitChar = "\t", string spaceChar = "\r")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("[{0}]{1}|{2}|{3}|{4}", "词项", "遗忘词频", "总词频", "词权重", "成熟度"));
            sb.AppendLine("=============================================");
            
            foreach(string keyword in objKeyWordList)
            {
                if (!objKeyBondColl.Contains(keyword)) continue;
                KeyBondMDL<string, string> bond = objKeyBondColl[keyword];
                sb.AppendLine();
                sb.AppendLine(String.Format("【{0}】{1}|{2}", bond.KeyItem.Key,Math.Round( bond.KeyItem.ValidCount*KeyBondHelper.CalcRemeberValue<string,string>(bond.KeyItem.Key,objKeyBondColl) ,4),Math.Round(bond.KeyItem.TotalCount)));
                sb.Append(KeyItemHelper.ShowKeyItemColl(bond.LinkColl, nLinkTopCount, false, bIsOrderbyDesc, false, splitChar, spaceChar));
            }
            return sb.ToString();
        }
        #endregion
    }
}
