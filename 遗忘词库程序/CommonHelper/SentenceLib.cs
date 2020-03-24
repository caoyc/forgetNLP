using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
namespace CommonHelper
{
    public class SentenceLib
    {

        #region 括号匹配
        /// <summary>
        /// 判断两个括号是否相匹配
        /// </summary>
        /// <param name="sLeftChar"></param>
        /// <param name="sRightChar"></param>
        /// <returns></returns>
        /// <remarks>
        /// 如果任何括号不在列表中，则匹配失败。
        /// </remarks>
        public static bool IsMatchSymbol(string sLeftChar, string sRightChar)
        {
            string sLeftSymbol = "\"‘“([{⦅〈《「『【〔〖〘〚〝︵︷︹︻︽︿﹁﹃﹙﹛﹝（［｛｟｢";
            string sRightSymbol = "\"’”)]}⦆〉》」』】〕〗〙〛〞︶︸︺︼︾﹀﹂﹄﹚﹜﹞）］｝｠｣";

            int nLeftPos = sLeftSymbol.IndexOf(sLeftChar);
            int nRightPos = sRightSymbol.IndexOf(sRightChar);

            return (nLeftPos >= 0 && nRightPos >= 0 && nLeftPos == nRightPos);
        }
        /// <summary>
        /// 是否左括号
        /// </summary>
        /// <param name="sLeftChar"></param>
        /// <returns></returns>
        public static bool IsLeftSymbol(string sLeftChar)
        {
            return "\"‘“([{⦅〈《「『【〔〖〘〚〝︵︷︹︻︽︿﹁﹃﹙﹛﹝（［｛｟｢".IndexOf(sLeftChar) >= 0;
        }
        /// <summary>
        /// 是否右括号
        /// </summary>
        /// <param name="sRightChar"></param>
        /// <returns></returns>
        public static bool IsRightSymbol(string sRightChar)
        {
            return "\"’”)]}⦆〉》」』】〕〗〙〛〞︶︸︺︼︾﹀﹂﹄﹚﹜﹞）］｝｠｣".IndexOf(sRightChar) >= 0;
        }
        /// <summary>
        /// 是否为结束符
        /// </summary>
        /// <param name="sWord"></param>
        /// <returns></returns>
        public static bool IsStopSymbol(string sWord)
        {
            return Regex.IsMatch(sWord, @"[!?。！？,，、:：；;]");
            //return Regex.IsMatch(sWord, @"[!?。！？]");
        }

        #endregion

        /// <summary>
        /// 将段落拆成句子列表。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> GetSentenceList(string text)
        {
            List<string> objSentenceList = new List<string>();
            using (StringReader sr = new StringReader(text))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                   
                    Stack<string> objStack = new Stack<string>();//栈：用于匹配各种括号对
                    bool bPrevCharIsStopSymbol = false;//前一个字符是否为结束字符
                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < line.Length; k++)
                    {
                        string ch = line.Substring(k, 1);

                        sb.Append(ch);

                        if (IsLeftSymbol(ch))
                        {
                            if (objStack.Count > 0)
                            {
                                string sStackTopWord = objStack.Pop();

                                //相同类型括号不能嵌套（小括号除外），否则视为结束。
                                if (sStackTopWord != ch || Regex.IsMatch(sStackTopWord, @"[\(\（]"))
                                {
                                    objStack.Push(sStackTopWord);
                                    objStack.Push(ch);
                                }
                                else
                                {
                                     #region 因同括号嵌套结束（小括号除外），主要用于防止因括号失配导致的拆句子失败。

                                    sb.Remove(sb.Length - 1, 1);//移除末尾的未匹配括号
                                    //将句子加入结果列表
                                    objSentenceList.Add(sb.ToString());

                                    //重置各种缓冲变量
                                    sb = new StringBuilder();
                                    objStack = new Stack<string>();
                                    bPrevCharIsStopSymbol = false;

                                    //并将左括号作为新句子的开始。
                                    sb.Append(ch);
                                    objStack.Push(ch);
                                     #endregion
                                }
                            }
                            else
                            {
                                //栈空，左括号直接压入
                                objStack.Push(ch);
                            }
                        }

                        if (IsRightSymbol(ch) && objStack.Count>0)
                        {
                            string sStackTopWord = objStack.Pop();

                            if (!IsMatchSymbol(sStackTopWord, ch))
                            {
                                objStack.Push(sStackTopWord);//括号不匹配，则原左括号压回，抛弃当前右括号（括号配对不允许交叉）
                            }
                            if (objStack.Count <= 0 && bPrevCharIsStopSymbol)
                            {
                                 #region 括号匹配完毕，且前字符为结束符，则句子结束。

                                //将句子加入结果列表
                                objSentenceList.Add(sb.ToString());

                                //重置各种缓冲变量
                                sb = new StringBuilder();
                                objStack = new Stack<string>();
                                bPrevCharIsStopSymbol = false;
                                 #endregion
                            }
                        }

                        if (IsStopSymbol(ch))
                        {
                            if (objStack.Count <= 0)
                            {
                                #region 括号外结束符，句子结束


                                //将句子加入结果列表
                                objSentenceList.Add(sb.ToString());

                                //重置各种缓冲变量
                                sb = new StringBuilder();
                                objStack = new Stack<string>();
                                bPrevCharIsStopSymbol = false;
                                #endregion
                            }
                            else
                            {
                                //括号内结束符
                                bPrevCharIsStopSymbol = true;
                            }
                        }

                        if (objStack.Count > 0 && ch== "…")
                        {
                            //当有括号时，省略号视为结束符。
                            bPrevCharIsStopSymbol = true;
                        }
                    }

                    #region 自然段至末尾，句子结束
                    //将句子加入结果列表
                    objSentenceList.Add(sb.ToString());
                    #endregion
                                     

                }
            }
            return objSentenceList;
        }
    }
}
