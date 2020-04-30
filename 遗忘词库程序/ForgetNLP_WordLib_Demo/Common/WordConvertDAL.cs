using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgetNLP_WordLib_Demo.Common
{
    public class WordLibDAL
    {
        #region FixWordColl
        /// <summary>
        /// 将词条列表转换为标准遗忘词库
        /// </summary>
        /// <param name="objWordCountList"></param>
        /// <returns></returns>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> FixWordColl(List<KeyValuePair<string, double>> objWordCountList)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            double dTotalOffset = 0;
            foreach (var pair in objWordCountList)
            {
                WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(pair.Key, objKeyWordColl, new WSR_Forget_Core.Memory.OffsetWeightMDL(0, pair.Value));
                dTotalOffset += pair.Value;
            }
            return FixWordColl(objKeyWordColl, dTotalOffset);
        }
        /// <summary>
        /// 将词条列表转换为标准遗忘词库
        /// </summary>
        /// <param name="objWordCountDict"></param>
        /// <returns></returns>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> FixWordColl(Dictionary<string, double> objWordCountDict)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            double dTotalOffset = 0;
            foreach (var pair in objWordCountDict)
            {
                WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(pair.Key, objKeyWordColl, new WSR_Forget_Core.Memory.OffsetWeightMDL(0, pair.Value));
                dTotalOffset += pair.Value;
            }
            return FixWordColl(objKeyWordColl, dTotalOffset);
        }
        /// <summary>
        /// 修正词库为标准遗忘词库
        /// </summary>
        /// <param name="objKeyWordColl"></param>
        /// <param name="dTotalOffset"></param>
        /// <returns></returns>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> FixWordColl(WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl, double dTotalOffset)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            if (dTotalOffset <= 0) return objResultWordColl;

            {
                double dUnionTotalOffset = dTotalOffset;


                WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                double dTotalCount = 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objKeyWordColl.Parameter));
                double dProbValue = dTotalCount / dUnionTotalOffset;
                double dTotalVaildCount = dTotalCount;
                if (dUnionTotalOffset < dTotalCount)
                {
                    dProbValue = 1;
                    dTotalVaildCount = dUnionTotalOffset;
                }


                objResultColl.Parameter = objKeyWordColl.Parameter;
                objResultColl.Parameter.Entropy = 0;

                double dClearValidCount = 0;
                foreach (var mdl in objKeyWordColl)
                {
                    double dKeywordValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objKeyWordColl);

                    double dValidCount = dKeywordValidCount * dProbValue;
                    if (dValidCount > objKeyWordColl.Parameter.Threshold)
                    {
                        mdl.ValidCount = dValidCount;
                        mdl.UpdateOffset = 0;


                        if (!objResultColl.Contains(mdl.Key))
                        {
                            dClearValidCount += mdl.ValidCount;

                            double dProb = mdl.ValidCount / dTotalVaildCount;
                            objResultColl.Parameter.Entropy += -dProb * Math.Log(dProb);
                            objResultColl.Add(mdl);
                        }
                    }
                }
                objResultColl.Parameter.TotalOffset = 0;
                objResultColl.Parameter.TotalValidCount = dClearValidCount;



                objResultWordColl = objResultColl;
            }
            return objResultWordColl;
        }
        #endregion
        /// <summary>
        /// 使用objDesWordColl过滤出与objSrcWordColl的差集，结果保留SrcWordColl中的词条、词频
        /// </summary>
        /// <param name="objSrcWordColl"></param>
        /// <param name="objDesWordColl"></param>
        /// <returns></returns>
        /// <remarks>结果词条满足条件：在SrcWordColl中，但不在DesWordColl中</remarks>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> ExceptWordColl(WSR_Forget_Core.KeyItem.KeyItemColl<string> objSrcWordColl, WSR_Forget_Core.KeyItem.KeyItemColl<string> objDesWordColl)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            {
                List<KeyValuePair<string, double>> objWordCountList = new List<KeyValuePair<string, double>>();
                foreach (var mdl in objSrcWordColl)
                {
                    if (objDesWordColl.Contains(mdl.Key)) continue;
                    double dValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objSrcWordColl);
                    objWordCountList.Add(new KeyValuePair<string, double>(mdl.Key, dValidCount));
                }
                objResultWordColl = FixWordColl(objWordCountList);
            }
            return objResultWordColl;
        }
        /// <summary>
        /// 使用objDesWordColl过滤出与objSrcWordColl的交集，结果保留SrcWordColl中的词条、词频。
        /// </summary>
        /// <param name="objSrcWordColl"></param>
        /// <param name="objDesWordColl"></param>
        /// <returns></returns>
        /// <remarks>结果词条满足条件：在SrcWordColl中，并且也在DesWordColl中</remarks>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> IntersectWordColl(WSR_Forget_Core.KeyItem.KeyItemColl<string> objSrcWordColl, WSR_Forget_Core.KeyItem.KeyItemColl<string> objDesWordColl)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            {
                List<KeyValuePair<string, double>> objWordCountList = new List<KeyValuePair<string, double>>();
                foreach (var mdl in objDesWordColl)
                {
                    if (!objSrcWordColl.Contains(mdl.Key)) continue;
                    double dValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objSrcWordColl);
                    objWordCountList.Add(new KeyValuePair<string, double>(mdl.Key, dValidCount));
                }
                objResultWordColl = FixWordColl(objWordCountList);
            }
            return objResultWordColl;
        }
        /// <summary>
        /// 融合两个词库
        /// </summary>
        /// <param name="objSrcWordColl"></param>
        /// <param name="objDesWordColl"></param>
        /// <returns></returns>
        public static WSR_Forget_Core.KeyItem.KeyItemColl<string> UnionWordColl(WSR_Forget_Core.KeyItem.KeyItemColl<string> objSrcWordColl, WSR_Forget_Core.KeyItem.KeyItemColl<string> objDesWordColl)
        {
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
            {
                double dUnionTotalOffset = objSrcWordColl.Parameter.TotalValidCount + objDesWordColl.Parameter.TotalValidCount;
                if (dUnionTotalOffset <= 0) return objResultWordColl;

                WSR_Forget_Core.KeyItem.KeyItemColl<string> objResultColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                double dTotalCount = 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objSrcWordColl.Parameter));
                double dProbValue = dTotalCount / dUnionTotalOffset;
                double dTotalVaildCount = dTotalCount;


                objResultColl.Parameter = objSrcWordColl.Parameter;
                objResultColl.Parameter.Entropy = 0;

                //添加objSrcWordColl入库
                {
                    double dClearValidCount = 0;
                    foreach (var mdl in objSrcWordColl)
                    {

                        double dSrcValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objSrcWordColl);

                        double dValidCount = dSrcValidCount * dProbValue;
                        if (dValidCount > objSrcWordColl.Parameter.Threshold * dProbValue)
                        {
                            WSR_Forget_Core.KeyItem.KeyItemMDL<string> item = new WSR_Forget_Core.KeyItem.KeyItemMDL<string>();
                            item.Key = mdl.Key;
                            if (objResultColl.Contains(mdl.Key)) item = objResultColl[mdl.Key];
                            item.ValidCount += dValidCount;
                            item.UpdateOffset = 0;



                            if (!objResultColl.Contains(item.Key))
                            {
                                dClearValidCount += item.ValidCount;

                                objResultColl.Add(item);
                            }
                        }
                    }
                    objResultColl.Parameter.TotalOffset = 0;
                    objResultColl.Parameter.TotalValidCount += dClearValidCount;
                }
                //添加objDesWordColl入库
                {
                    double dClearValidCount = 0;
                    foreach (var mdl in objDesWordColl)
                    {
                        double dDesValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objDesWordColl);

                        double dValidCount = dDesValidCount * dProbValue;
                        if (dValidCount > objSrcWordColl.Parameter.Threshold * dProbValue)
                        {
                            WSR_Forget_Core.KeyItem.KeyItemMDL<string> item = new WSR_Forget_Core.KeyItem.KeyItemMDL<string>();
                            item.Key = mdl.Key;
                            if (objResultColl.Contains(mdl.Key)) item = objResultColl[mdl.Key];
                            item.ValidCount += dValidCount;
                            item.UpdateOffset = 0;



                            if (!objResultColl.Contains(item.Key))
                            {
                                dClearValidCount += item.ValidCount;

                                objResultColl.Add(item);
                            }
                        }
                    }
                    objResultColl.Parameter.TotalOffset = 0;
                    objResultColl.Parameter.TotalValidCount += dClearValidCount;
                }


                objResultWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl(objResultColl);
            }
            return objResultWordColl;
        }
    }
}
