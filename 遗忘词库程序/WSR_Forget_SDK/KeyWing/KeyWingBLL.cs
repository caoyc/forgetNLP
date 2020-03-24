using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
using WSR_Forget_Core.KeyBond;
namespace WSR_Forget_SDK.KeyWing
{
    public class KeyWingBLL
    {

        #region 全句词翼         
        //public static void UpdateKeyWingColl(List<string> objKeyList,KeyBondColl<string,string> objKeyCloudColl,KeyBondColl<string,string> objKeyWingColl,int nRadiusSize = 7,string splitChar = "\t")
        //{
        //    Dictionary<int,PosWingColl> dict= GetPosWingDict(objKeyList,objKeyCloudColl,nRadiusSize);
        //    UpdateKeyWingColl(objKeyList,dict,objKeyWingColl,splitChar);
        //}
        //public static void UpdateKeyWingColl(List<string> objKeyList,Dictionary<int,PosWingColl> objPosWingDict,KeyBondColl<string,string> objKeyWingColl, string splitChar="\t")
        //{
        //    HashSet<int> objPosSet = new HashSet<int>();
        //    for (int k = 0; k < objKeyList.Count; k++)
        //    {
        //        objPosSet.Add(k);
        //    }
        //    PosWingColl objPosWingColl = new KeyWing.PosWingColl();
        //    foreach (KeyValuePair<int,PosWingColl> pair in objPosWingDict)
        //    {
        //        foreach (PosWingMDL mdl in pair.Value)
        //        {
        //            if (objPosWingColl.Contains(mdl.KeyIndex)) continue;
        //            objPosWingColl.Add(mdl);
        //        }
        //    }
        //    PosWingColl objResultColl = new PosWingColl();

        //    var objOrderedColl = objPosWingColl.OrderByDescending(x => x.Weight);
        //    foreach (PosWingMDL mdl in objOrderedColl)
        //    {
        //        if (objPosSet.Count <= 0) break;
        //        if (mdl.PosSet.All(x => !objPosSet.Contains(x))) continue; //没有包含新词，则跳过
        //        if (!objResultColl.Contains(mdl.KeyIndex))
        //        {
        //            objResultColl.Add(mdl);
        //        }
        //        objPosSet.RemoveWhere(x => mdl.PosSet.Contains(x));

        //    }


        //    foreach (PosWingMDL mdl in objResultColl)
        //    {
        //        SortedSet<int> set = new SortedSet<int>(mdl.PosSet);
        //        StringBuilder sb = new StringBuilder();
        //        int prev = -1;
        //        foreach (int k in set)
        //        {
        //            if (prev >= 0) sb.Append(splitChar);
        //            sb.Append(String.Format("{0}",objKeyList[k]));
        //            prev = k;
        //        }
        //        string keywing = sb.ToString();
        //        foreach (int k in mdl.PosSet)
        //        {
        //            string keyword = objKeyList[k];                   
        //            KeyBondDAL.UpdateKeyBondColl<string,string>(keyword,keywing,objKeyWingColl,new OffsetWeightMDL(1,1),new OffsetWeightMDL(1,1));
        //        }
        //    }
        //}
        //public static Dictionary<int,PosWingColl> GetPosWingDict(List<string> objKeyList,KeyBondColl<string,string> objKeyCloudColl,int nRadiusSize = 7 )
        //{
        //    double dTotalCount = 1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyCloudColl.Parameter) );
        //    double dLogTotalCount = Math.Log(dTotalCount);

        //    Dictionary<int,PosWingColl> dict = new Dictionary<int, PosWingColl>();
        //    for (int k = 0; k < objKeyList.Count; k++)
        //    {

        //        PosWingColl objPosWingColl = new PosWingColl();
        //        HashSet<int> objLinkSet = new HashSet<int>();

        //        string keyword = objKeyList[k];
        //        double weight = 0;
        //        if (objKeyCloudColl.Contains(keyword))
        //        {
        //            KeyItemMDL<string> objKeyItem = objKeyCloudColl[keyword].KeyItem;
        //            if (objKeyItem.ValidCount > objKeyCloudColl.Parameter.Threshold)
        //            {
        //                weight = dLogTotalCount - Math.Log(objKeyItem.ValidCount * KeyBondHelper.CalcRemeberValue(keyword,objKeyCloudColl));
        //            }
        //        }

        //        //将自身添加进集合
        //        {
        //            objLinkSet.Add(k);
        //            PosWingMDL mdl = new PosWingMDL();
        //            mdl.Weight = weight;
        //            mdl.PosSet.Add(k);
        //            mdl.KeyIndex = objPosWingColl.GetKeyIndex(mdl.PosSet);
        //            if (!objPosWingColl.Contains(mdl.KeyIndex))
        //            {
        //                objPosWingColl.Add(mdl);
        //            }
        //        }
        //        string keyTail = keyword;
        //        for (int t = 1; t <= nRadiusSize; t++)
        //        {
        //            int nPos = k - t;
        //            if (nPos <= 0) break;
        //            string keyHead = objKeyList[nPos];

        //            if (dict.ContainsKey(nPos))
        //            {
        //                if (KeyBondHelper.IsBondValid(keyHead,keyTail,objKeyCloudColl))
        //                {
        //                    objLinkSet.Add(nPos);
        //                    foreach (PosWingMDL headMDL in dict[nPos])
        //                    {                               
        //                        PosWingMDL mdl = new PosWingMDL();
        //                        mdl.Weight = headMDL.Weight + weight;  
        //                        mdl.PosSet.UnionWith(headMDL.PosSet);
        //                        mdl.PosSet.Add(k);
        //                        mdl.KeyIndex = objPosWingColl.GetKeyIndex(mdl.PosSet);
        //                        if (!objPosWingColl.Contains(mdl.KeyIndex))
        //                        {
        //                            objPosWingColl.Add(mdl);
        //                        }
        //                    }
        //                }

        //            }
        //        }

        //        PosWingColl objResultColl = new PosWingColl();

        //        var objOrderedColl= objPosWingColl.OrderByDescending(x => x.Weight);
        //        foreach (PosWingMDL mdl in objOrderedColl)
        //        {
        //            if (objLinkSet.Count <= 0) break;
        //            if (mdl.PosSet.All(x => !objLinkSet.Contains(x))) continue; //没有包含新词，则跳过
        //            if (!objResultColl.Contains(mdl.KeyIndex))
        //            {
        //                objResultColl.Add(mdl);
        //            }
        //            objLinkSet.RemoveWhere(x => mdl.PosSet.Contains(x));

        //        }

        //        dict.Add(k,objResultColl);
        //    }

        //    return dict;
        //}

        #endregion
        #region  局部词翼
       
        /// <summary>
        /// 获得每一个词窗口尺寸内相关的词的位置集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objKeyList"></param>
        /// <param name="objKeyCloudColl"></param>
        /// <param name="nRadiusSize"></param>
        /// <returns></returns>
        public static Dictionary<int,SortedSet<int>> GetPosWingDict<T>(List<T> objKeyList,KeyBondColl<T,T> objKeyCloudColl,int nRadiusSize = 7)
        {
            Dictionary<int,SortedSet<int>> dict = new Dictionary<int,SortedSet<int>>();
            for (int k = 0; k < objKeyList.Count; k++)
            {
                dict.Add(k,new SortedSet<int>());
                T keyTail = objKeyList[k];
                for (int t=1;t<=nRadiusSize;t++)
                {
                    int nPos = k - t;
                    if (nPos <= 0) break;
                    T keyHead = objKeyList[nPos];

                    if (dict.ContainsKey(nPos))
                    {
                        if (KeyBondHelper.IsBondValid(keyHead,keyTail,objKeyCloudColl))
                        {
                            dict[nPos].Add(k);
                            dict[k].Add(nPos);
                        }

                    }
                }
                dict[k].Add(k);

            }

            return dict;
        }

        #endregion

        #region 匹配词翼

        public static Dictionary<string,HashSet<int>> GetKeyWingMatchedList (List<string> objKeyList,KeyItemColl<string> objKeyWordColl, KeyBondColl<string,string> objKeyCloudColl,KeyBondColl<string,string> objKeyWingColl, string splitChar="\t", int nRadiusSize = 7)
        {
            HashSet<int> objKeyPosSet = new HashSet<int>();
            Dictionary<int,Dictionary<int,double>> objPosWeightDict = new Dictionary<int,Dictionary<int,double>>();
            #region 获得每个索引位对应的关联位置的关联系数
            for (int k = 0; k < objKeyList.Count; k++)
            {
                objKeyPosSet.Add(k);

                objPosWeightDict.Add(k,new Dictionary<int,double>());
                string keyTail = objKeyList[k];
                for (int t = 1; t <= nRadiusSize; t++)
                {
                    
                    int nPos = k - t;
                    if (nPos < 0) break;
                    string keyHead = objKeyList[nPos];

                    if (objPosWeightDict.ContainsKey(nPos))
                    {
                        //double dRelateValue= KeyBondHelper.CalcBondRelateValue(keyHead,keyTail,objKeyCloudColl);
                        // if (KeyBondHelper. GetRandomNumber(0,dRelateValue) > KeyBondHelper.GetRandomNumber(0,Math.E))
                        // {                            
                        //     objPosWeightDict[nPos].Add(k,dRelateValue);
                        //     objPosWeightDict[k].Add(nPos,dRelateValue);
                        // }
                        if (KeyBondHelper.IsBondValid(keyHead,keyTail,objKeyCloudColl))
                        {
                            double dLinkValidCount = KeyBondHelper.CalcTailValidCount(keyHead,keyTail,objKeyCloudColl);
                            objPosWeightDict[nPos].Add(k,dLinkValidCount);
                            objPosWeightDict[k].Add(nPos,dLinkValidCount);
                        }
                    }
                }
            }
            #endregion

            Dictionary<string,HashSet<int>> objKeyPosDict = new Dictionary<string,HashSet<int>>();
            #region 将位置转换为词翼
            foreach (KeyValuePair<int,Dictionary<int,double>> pair in objPosWeightDict)
            {
                SortedSet<int> objPosWingSet = new SortedSet<int>();
                objPosWingSet.Add(pair.Key);
                IOrderedEnumerable<KeyValuePair<int,double>> buffer = pair.Value.OrderByDescending(x => x.Value);
                
                foreach (KeyValuePair<int,double> kvp in buffer)
                {
                    StringBuilder sb = new StringBuilder();
                    int nLastPos = -1;
                    foreach (int pos in objPosWingSet)
                    {
                        if (pos - nLastPos > 1) sb.Append(splitChar);
                        sb.Append(objKeyList[pos]);
                        nLastPos = pos;
                    }
                    if (nLastPos + 1 < objKeyList.Count) sb.Append(splitChar);
                    string keywing = sb.ToString();
                    if (!objKeyPosDict.ContainsKey(keywing))
                    {
                        objKeyPosDict.Add(keywing,new HashSet<int>());
                    }
                    objKeyPosDict[keywing].UnionWith(objPosWingSet);

                    objPosWingSet.Add(kvp.Key);
                }

            }
            #endregion

           
            Dictionary<string,double> objKeyWeightDict = new Dictionary<string,double>();
            Dictionary<string,HashSet<int>> objKeyPosExDict = new Dictionary<string,HashSet<int>>();

            double dLogTotalCount = Math.Log(1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyWordColl.Parameter) ));

            #region 获得库中存在的词翼，同时累计匹配词的权重
            foreach (KeyValuePair<string,HashSet<int>> pair  in objKeyPosDict)
            {
                string keywing = pair.Key;
                foreach (int pos in pair.Value)
                {
                    string keyword = objKeyList[pos];
                    double dKeyWordValidCount = objKeyWordColl.Contains(keyword) ?  KeyItemHelper.CalcValidCount(keyword,objKeyWordColl) + 1 : 1;
                    double dKeyWordWeight = dLogTotalCount - Math.Log(dKeyWordValidCount);
                   
                    if (objKeyWingColl.Contains(keyword))
                    {
                        if (objKeyWingColl[keyword].LinkColl.Contains(keywing))
                        {
                            if (!objKeyWeightDict.ContainsKey(keywing))
                            {
                                objKeyWeightDict.Add(keywing,dKeyWordWeight);
                            }
                            else
                            {
                                objKeyWeightDict[keywing] += dKeyWordWeight;
                            }

                            if (!objKeyPosExDict.ContainsKey(keywing))
                            {
                                objKeyPosExDict.Add(keywing,new HashSet<int>());
                            }
                            objKeyPosExDict[keywing].Add(pos);
                        }
                    }
                }
            }
            #endregion


            Dictionary<string,HashSet<int>> dict = new Dictionary<string,HashSet<int>>();
            #region 获得最佳匹配词翼
            foreach (KeyValuePair<string,double> pair in objKeyWeightDict.OrderByDescending(x=>x.Value))
            {
                if (objKeyPosSet.Count <= 0) break;
                if (!objKeyPosSet.Any(x => objKeyPosExDict[pair.Key].Contains(x))) continue;
                if (!dict.ContainsKey(pair.Key)) dict.Add(pair.Key,objKeyPosExDict[pair.Key]);
                objKeyPosSet.RemoveWhere(x=> objKeyPosExDict[pair.Key].Contains(x));
            }
            #endregion
            return dict;
        }
        #endregion
    }
}
