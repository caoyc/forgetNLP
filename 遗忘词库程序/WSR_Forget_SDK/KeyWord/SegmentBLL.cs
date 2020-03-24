using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using WSR_Forget_Core.Memory;
using WSR_Forget_Core.KeyItem;
using WSR_Forget_Core.KeyBond;
namespace WSR_Forget_SDK.KeyWord
{
   
    public class SegmentBLL
    {
        /// <summary>
        /// 显示分词结果
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ShowSegment(List<string> buffer)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string keyword in buffer)
            {
                if (keyword != Environment.NewLine) sb.Append(String.Format("<{0}>", keyword));
                else sb.Append(keyword);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 分词（同时自动维护词典）
        /// </summary>
        /// <param name="text">待分词文本</param>
        /// <param name="objCharBondColl">邻键集合（用于生成词库）</param>
        /// <param name="objKeyWordColl">词库</param>
        /// <param name="maxWordLen">最大词长（建议：细粒度为4、粗粒度为7）</param>
        /// <param name="bUpdateCharBondColl">是否同时更新邻键集合</param>
        /// <param name="bUpdateKeyWordColl">是否同时更新词库</param>
        /// <param name="nRadiusSize">有效键半径</param>
        /// <returns>返回分词结果</returns>
        public static List<string> Segment(string text,KeyBondColl<string,string> objCharBondColl,KeyItemColl<string> objKeyWordColl,int maxWordLen = 7,bool bUpdateCharBondColl = true,bool bUpdateKeyWordColl = true,int nRadiusSize = 7)
        {
            if (String.IsNullOrEmpty(text)) return new List<string>();
            if (maxWordLen <= 0) maxWordLen = text.Length;

            //总词频
            double dLogTotalCount = Math.Log(objKeyWordColl.Parameter.TotalValidCount + 1);// Math.Log(1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyWordColl.Parameter) ) );// Math.Log(objKeyWordColl.Sum(x =>x.VaildCount* KeyWordBLL.CalcRemeberValue<string>(x.Key,objKeyWordColl)));//


            Dictionary<int,List<string>> objKeyWordBufferDict = new Dictionary<int,List<string>>();
            Dictionary<int,double> objKeyWordValueDict = new Dictionary<int,double>();

            for (int k = 0; k < text.Length; k++)
            {
                
                List<string> objKeyWordList = new List<string>();
                double dKeyWordValue = 0;

                for (int len = 0; len < maxWordLen ; len++)
                {
                    int startpos = k - len;
                    if (startpos < 0) break;
                    string keyword = text.Substring(startpos,len + 1);
                    if (len > 0 && !objKeyWordColl.Contains(keyword)) continue;
                    if (len > 0)
                    {
                        if (!objKeyWordColl.Contains(keyword)) continue;
                        double dValidCount = KeyItemHelper.CalcValidCount(keyword, objKeyWordColl);
                        if (dValidCount < objKeyWordColl.Parameter.Threshold) continue;
                        //if (dValidCount < Math.E) continue;//经测试，原始最好

                    }
                    double dTempValue = 0;
                    if (objKeyWordColl.Contains(keyword))
                    {
                        KeyItemMDL<string> mdl = objKeyWordColl[keyword];
                        dTempValue = -( dLogTotalCount - Math.Log(KeyItemHelper.CalcValidCount(keyword, objKeyWordColl)) );
                    }
                    if (objKeyWordValueDict.ContainsKey(startpos - 1))
                    {
                        dTempValue += objKeyWordValueDict[startpos - 1];
                        if (dKeyWordValue == 0 || dTempValue > dKeyWordValue)
                        {
                            dKeyWordValue = dTempValue;
                            objKeyWordList = new List<string>(objKeyWordBufferDict[startpos - 1]);
                            objKeyWordList.Add(keyword);
                        }
                    }
                    else
                    {
                        if (dKeyWordValue == 0 || dTempValue > dKeyWordValue)
                        {
                            dKeyWordValue = dTempValue;
                            objKeyWordList = new List<string>();
                            objKeyWordList.Add(keyword);
                        }
                    }
                }
                objKeyWordBufferDict.Add(k,objKeyWordList);
                objKeyWordValueDict.Add(k,dKeyWordValue);

                if (k > maxWordLen)
                {
                    objKeyWordBufferDict.Remove(k - maxWordLen - 1);
                    objKeyWordValueDict.Remove(k - maxWordLen - 1);
                }
            }

            if (bUpdateCharBondColl || bUpdateKeyWordColl)
            {
                KeyWordBLL.UpdateKeyWordColl(text, objKeyWordColl,maxWordLen);
                
            }

            return objKeyWordBufferDict[text.Length - 1];
        }

        /// <summary>
        /// 分词（支持实时新词发现）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="objKeyWordColl"></param>
        /// <param name="maxWordLen"></param>
        /// <param name="bIsNewWordMining"></param>
        /// <returns></returns>
        public static List<string> SegmentEx(string text,   KeyItemColl<string> objKeyWordColl, int maxWordLen = 7 ,bool bIsNewWordMining=true)
        {

            if (!bIsNewWordMining) return Segment(text, null, objKeyWordColl,maxWordLen,false,false,maxWordLen);

            if (String.IsNullOrEmpty(text)) return new List<string>();
            if (maxWordLen <= 0) maxWordLen = text.Length;

            //总词频
            double dLogTotalCount = Math.Log(objKeyWordColl.Parameter.TotalValidCount + 1);// Math.Log(1.0 / ( 1.0 - MemoryDAL.CalcRemeberValue(1,objKeyWordColl.Parameter) ) );// Math.Log(objKeyWordColl.Sum(x =>x.VaildCount* KeyWordBLL.CalcRemeberValue<string>(x.Key,objKeyWordColl)));//


            Dictionary<int, List<string>> objKeyWordBufferDict = new Dictionary<int, List<string>>();
            Dictionary<int, double> objKeyWordValueDict = new Dictionary<int, double>();

            Dictionary<string, double> objKeyWordProbDict = new Dictionary<string, double>();
            for (int k = 0; k < text.Length; k++)
            {
                
                for (int s = 0; s < maxWordLen; s++)
                {
                    int nStartPos = k - s;
                    if (nStartPos < 0) break;                    
                    string keyword = text.Substring(nStartPos, s + 1);
                    if (keyword.Length>1 && Regex.IsMatch(keyword, @"[\s\p{P}\p{C}]")) break;

                    if (!objKeyWordProbDict.ContainsKey(keyword))
                    {
                        //if (objKeyWordColl.Contains(keyword))
                        //{
                            double dKeywordValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(keyword, objKeyWordColl) + 1;
                            //if (dKeywordValidCount < objKeyWordColl.Parameter.Threshold + 1) dKeywordValidCount = 1;
                            double dLogKeywordProbValue = Math.Log(dKeywordValidCount) - dLogTotalCount;
                            //objKeyWordProbDict.Add(keyword, dLogProbValue);

                            if (s > 0)
                            {
                                string prevword = text.Substring(nStartPos + 1, s);
                                double dPrevValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(prevword, objKeyWordColl) + 1;
                                double dLogProbValue = objKeyWordProbDict[prevword]+Math.Log(dKeywordValidCount) - Math.Log(dPrevValidCount);//Log(P(prevword)*P(keyword|prevword))
                                objKeyWordProbDict.Add(keyword, Math.Min(dLogProbValue,dLogKeywordProbValue));
                            }
                            else
                            {
                                //单字
                                objKeyWordProbDict.Add(keyword, dLogKeywordProbValue);
                            }
                        //}
                        //else
                        //{
                        //    if (s > 0)
                        //    {
                        //        string prevword = text.Substring(nStartPos+1, s);
                        //        double dPrevValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(prevword, objKeyWordColl) + 1;
                        //        double dLogProbValue = objKeyWordProbDict[prevword] - Math.Log(dPrevValidCount);//Log(P(prevword)*P(keyword|prevword))
                        //        objKeyWordProbDict.Add(keyword, dLogProbValue);
                        //    }
                        //    else
                        //    {
                        //        //单字
                        //        objKeyWordProbDict.Add(keyword, -dLogTotalCount);
                        //    }
                        //}
                    }

                  
                }
            }
            for (int k = 0; k < text.Length; k++)
            {

                List<string> objKeyWordList = new List<string>();
                double dKeyWordValue = 0;

                for (int len = 0; len < maxWordLen; len++)
                {
                    int startpos = k - len;
                    if (startpos < 0) break;
                    string keyword = text.Substring(startpos, len + 1);
                    if (!objKeyWordProbDict.ContainsKey(keyword)) break;
                    if (objKeyWordProbDict[keyword] < 1.0 - dLogTotalCount) break;
                    double dTempValue = objKeyWordProbDict[keyword];
                     
                    if (objKeyWordValueDict.ContainsKey(startpos - 1))
                    {
                        dTempValue += objKeyWordValueDict[startpos - 1];
                        if (dKeyWordValue == 0 || dTempValue > dKeyWordValue)
                        {
                            dKeyWordValue = dTempValue;
                            objKeyWordList = new List<string>(objKeyWordBufferDict[startpos - 1]);
                            objKeyWordList.Add(keyword);
                        }
                    }
                    else
                    {
                        if (dKeyWordValue == 0 || dTempValue > dKeyWordValue)
                        {
                            dKeyWordValue = dTempValue;
                            objKeyWordList = new List<string>();
                            objKeyWordList.Add(keyword);
                        }
                    }
                }
                objKeyWordBufferDict.Add(k, objKeyWordList);
                objKeyWordValueDict.Add(k, dKeyWordValue);

                if (k > maxWordLen)
                {
                    objKeyWordBufferDict.Remove(k - maxWordLen - 1);
                    objKeyWordValueDict.Remove(k - maxWordLen - 1);
                }
            }

          

            return objKeyWordBufferDict[text.Length - 1];
        }

    }
}
