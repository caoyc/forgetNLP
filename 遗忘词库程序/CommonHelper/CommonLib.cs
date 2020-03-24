using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CommonHelper
{
    public class CommonLib
    {
        /// <summary>
        /// 为给定字符串生成Md5Code。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Code(string input)
        {

            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            return BitConverter.ToString(data).Replace("-","");
        }

        public static double GetRandomNumber(double min,double max)
        {
            return ( new Random(Guid.NewGuid().GetHashCode()) ).NextDouble() * ( max - min ) + min;
        }

        public static string CleanHtml(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml)) return strHtml;
            strHtml = Regex.Unescape(strHtml);
            //删除脚本
            //Regex.Replace(strHtml, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase)
            strHtml = Regex.Replace(strHtml, @"(\<script(.+?)\</script\>)|(\<style(.+?)\</style\>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //删除标签
            var r = new Regex(@"</?[^>]*>", RegexOptions.IgnoreCase);
            Match m;
            for (m = r.Match(strHtml); m.Success; m = m.NextMatch())
            {
                strHtml = strHtml.Replace(m.Groups[0].ToString(), "");
            }
            return strHtml.Trim();
        }


        public static List<string> GetLines(string text, bool bRemoveEmpty = true, bool bRemoveRepeat = true)
        {
            List<string> objResultList = new List<string>();

            HashSet<string> set = new HashSet<string>();
            using (StringReader sr = new StringReader(text))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (bRemoveEmpty && String.IsNullOrEmpty(line)) continue;
                    if (bRemoveRepeat && set.Contains(line)) continue;

                    objResultList.Add(line);
                    set.Add(line);
                }
            }

            return objResultList;
        }

    }
}
