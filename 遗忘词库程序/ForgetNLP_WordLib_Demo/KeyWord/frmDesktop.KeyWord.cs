using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ForgetNLP_WordLib_Demo.Config;
namespace ForgetNLP_WordLib_Demo 
{
   partial class frmDesktop
    {
        #region 词库：原始
        /// <summary>
        /// 原始/操作/从文本导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            //openFileDialog.FileName = "OriginalKeyWord.coll";

            openFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    AppendText("正在导入文本词库，请稍候……");

                    string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    if (File.Exists(sKeyWordPathFile))
                    {
                        WSR_Forget_Core.KeyItem.KeyItemColl<string> objConvertWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                        double dConvertTotalOffset = 0;

                        string sPathFile = sKeyWordPathFile;

                        {
                            //this.AppendText(String.Format("【进行】{0}", sPathFile));

                            FileInfo info = new FileInfo(sPathFile);
                            double dFileCharLength = info.Length;
                            double dLoadCharLength = 0;

                            DateTime dtUpdateTime = DateTime.Now;
                            this.pgbScanLines.Maximum = Convert.ToInt32(Math.Min(Int32.MaxValue, dFileCharLength));
                            this.pgbScanLines.Minimum = 0;
                            this.pgbScanLines.Value = this.pgbScanLines.Minimum;
                            using (StreamReader sr = new StreamReader(sPathFile, Encoding.UTF8))
                            {
                                string line = null;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    Application.DoEvents();
                                    if (String.IsNullOrWhiteSpace(line)) continue;

                                    dLoadCharLength += Encoding.UTF8.GetByteCount(line);
                                    if (this.pgbScanLines.Maximum <= dLoadCharLength)
                                    {
                                        dFileCharLength = dFileCharLength - dLoadCharLength + 1;
                                        this.pgbScanLines.Maximum = Convert.ToInt32(Math.Min(Int32.MaxValue, dFileCharLength));
                                        this.pgbScanLines.Value = 0;
                                        dLoadCharLength = 0;

                                    }
                                    this.pgbScanLines.Value = this.pgbScanLines.Maximum > dLoadCharLength ? Convert.ToInt32(dLoadCharLength) : this.pgbScanLines.Maximum - 1;

                                    string text = line; //这里可以再做一些需要特别处理的数据清洗，如多余的空格等
                                                        //if(this.pgbScanLines.Value*1.0/this.pgbScanLines.Maximum>0.65)  this.AppendText(line);
                                 
                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        string pattern = @"^\s*[\[【](?<Word>.*?)[\]】](\s*(?<Freq>\d+(\.\d+)?))(.*?)$";
                                        string groupName = "Word | Freq";

                                        string[] objGroupNames = Regex.Split(groupName, "[|｜]+", RegexOptions.Singleline);
                                        Dictionary<string, string> objNameContentDict = new Dictionary<string, string>();
                                        List<string> objGroupNameList = new List<string>();

                                        Match objMatch = Regex.Match(text, pattern, RegexOptions.Singleline);
                                        if (!objMatch.Success) continue;

                                        foreach (string name in objGroupNames)
                                        {
                                            string trimName = name.Trim();
                                            if (String.IsNullOrEmpty(trimName)) continue;
                                            objGroupNameList.Add(trimName);
                                            if (!objNameContentDict.ContainsKey(trimName))
                                            {
                                                objNameContentDict.Add(trimName, objMatch.Groups[trimName].Value);
                                            }
                                        }
                                        if (objNameContentDict.Count <= 0) continue;


                                        string word = (objGroupNameList.Count > 0 && objNameContentDict.ContainsKey(objGroupNameList[0])) ? objNameContentDict[objGroupNameList[0]] : String.Empty;
                                        string keyword = String.IsNullOrWhiteSpace(word.Trim()) ? String.Empty : word.Trim();

                                        string freq = (objGroupNameList.Count > 1 && objNameContentDict.ContainsKey(objGroupNameList[1])) ? objNameContentDict[objGroupNameList[1]] : String.Empty;
                                        double frequency = String.IsNullOrWhiteSpace(freq.Trim()) ? 1 : Convert.ToDouble(freq.Trim());

                                        if (String.IsNullOrEmpty(keyword)) continue;
                                        WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objConvertWordColl, new WSR_Forget_Core.Memory.OffsetWeightMDL(0, frequency));
                                        dConvertTotalOffset += frequency;

                                        
                                    }
                                }

                            }
                            this.pgbScanLines.Value = this.pgbScanLines.Maximum;
                            this.pgbScanLines.Value = this.pgbScanLines.Minimum;

                        }

                        gobjDataConfig.KeyWord.BasicKeyWordColl = Common.WordLibDAL.FixWordColl(objConvertWordColl, dConvertTotalOffset);

                    }
                    else
                    {
                        AppendText("【警告】文本词库不存在。");
                    }

                    AppendText("加载完毕。");

                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }



            }
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKeyWordExecute_Click(object sender, EventArgs e)
        {
            ExecuteByDesktop(KeyWord_Basic_UpdateKeyWordColl);
          
        }
        /// <summary>
        /// 分词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKeyWordSegment_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(this.tbKeyWordContent.Text))
            {
                List<string> objLineList = this.GetLines(this.tbKeyWordContent.Text, false, false);
                for (int k = 0; k < objLineList.Count; k++)
                {
                    string line = objLineList[k];
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        this.AppendText(this.KeyWord_Basic_Segment_ShowResult(line, gobjDataConfig, gobjOptionConfig));
                    }
                }
            }
        }
      
        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKeyWordReview_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(this.tbKeyWordContent.Text))
            {
                List<string> objLineList = GetLines(this.tbKeyWordContent.Text);
                if (radioButton1.Checked) //仅显示列表中的词
                {
                    this.rtbInfoBoard.Text = KeyWord_Basic_ShowKeyWordColl(gobjDataConfig, objLineList, false);
                }
                if (radioButton2.Checked) //显示包含列表中词的所有词
                {
                    this.rtbInfoBoard.Text = KeyWord_Basic_ShowKeyWordColl(gobjDataConfig, objLineList, true);
                }
            }
            else
            {
                this.rtbInfoBoard.Text = KeyWord_Basic_ShowKeyWordColl(gobjDataConfig);
            }
        }
        /// <summary>
        /// 原始/关键词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string line = this.tbKeyWordContent.Text;
            if (!String.IsNullOrWhiteSpace(line))
            {
                this.rtbInfoBoard.Text = this.KeyWord_Basic_CatchKeyWord(line, gobjDataConfig, gobjOptionConfig);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory= CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            saveFileDialog.FileName = "OriginalKeyWord.coll";

            saveFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    AppendText("正在保存原始词库，请稍候……");
                    string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));
                 
                    gobjDataConfig.KeyWord.BasicKeyWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.BasicKeyWordColl);
                    CommonHelper.SerialLib.SerializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(gobjDataConfig.KeyWord.BasicKeyWordColl, sKeyWordPathFile);
                    AppendText("保存完毕。");
                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }
            }            
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            openFileDialog.FileName = "OriginalKeyWord.coll";

            openFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    AppendText("正在加载原始词库，请稍候……");

                    string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    if (File.Exists(sKeyWordPathFile))
                    {
                        gobjDataConfig.KeyWord.BasicKeyWordColl = CommonHelper.SerialLib.DeserializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(sKeyWordPathFile);
                        if (gobjDataConfig.KeyWord.BasicKeyWordColl == null) gobjDataConfig.KeyWord.BasicKeyWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                    }
                    else
                    {
                        AppendText("【警告】原始词库不存在。");
                    }

                    AppendText("加载完毕。");

                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }



            }
        }
        /// <summary>
        /// 导出到文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            saveFileDialog.FileName = "OriginalKeyWord.txt";

            saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    AppendText("正在导出原始词库，请稍候……");
                    string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));
                 
                    gobjDataConfig.KeyWord.BasicKeyWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.BasicKeyWordColl);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("【词条】遗忘词频");
                    sb.AppendLine("=============================");
                    var buffer = gobjDataConfig.KeyWord.BasicKeyWordColl.OrderByDescending(x => x.ValidCount);
                    foreach (var mdl in buffer)
                    {                      
                        sb.AppendLine(String.Format("【{0}】{1}",mdl.Key,Math.Round(mdl.ValidCount,4)));
                    }
                    AppendText("正在写入文件中，请稍候……");
                    File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                    AppendText("导出完毕。");
                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }
            }
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
        
                if (MessageBox.Show("是否确定要清空原始词库？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    AppendText("取消清空操作。");
                    return;
                }
                AppendText("正在清空词库，请稍候……");

                gobjDataConfig.KeyWord.BasicCharBondColl.Clear();
                gobjDataConfig.KeyWord.BasicKeyWordColl.Clear();

                AppendText("清空完毕。");
          
        }


        #endregion


        #region 类库：原始
        public string KeyWord_Basic_CatchKeyWord(string text, DataConfig objDataConfig, OptionConfig objOptionConfig)
        {

            //计算信息熵，同时词信息熵排序
            
            frmDesktop desktop = this;

            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;


            List<string> objKeyWordList = KeyWord_Basic_Segment(text, objDataConfig, objOptionConfig);


            Dictionary<string, double> dict = new Dictionary<string, double>();

            double dTotalEntropy = 0;
            double dTotalVaildCount = objKeyWordColl.Parameter.TotalValidCount + 1;// 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objKeyWordColl.Parameter));
            double dLogTotalCount = Math.Log(dTotalVaildCount);

            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();
            foreach (string keyword in objKeyWordList)
            {
                if (objKeyWordColl.Contains(keyword))
                {
                    WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl = objKeyWordColl[keyword];
                    double dValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objKeyWordColl);
                    double dProbValue = dValidCount / dTotalVaildCount;
                    double dEntropy = -dProbValue * Math.Log(dProbValue);
                    dTotalEntropy += dEntropy;


                    if (!dict.ContainsKey(keyword)) dict.Add(keyword, 0);
                    if (objKeyWordColl.Contains(keyword))
                    {
                        dict[keyword] += dLogTotalCount - Math.Log(dValidCount);
                    }
                }
                else
                {
                    double dValidCount = 1;
                    double dProbValue = dValidCount / dTotalVaildCount;
                    double dEntropy = -dProbValue * Math.Log(dProbValue);
                    dTotalEntropy += dEntropy;

                    if (!dict.ContainsKey(keyword)) dict.Add(keyword, 0);
                    if (objKeyWordColl.Contains(keyword))
                    {
                        dict[keyword] += dLogTotalCount - Math.Log(dValidCount);
                    }
                }
            }

            //根据平均信息熵计算有效词数量
            double dAverageWordCount = Math.Pow(Math.E, dTotalEntropy - 0.5772156649 + 1);//依据平均信息熵



            var ordered = from pair in dict
                          orderby pair.Value descending
                          select pair;

            int nRecordCount = 0;
            bool bIsOverLine = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(WSR_Forget_SDK.KeyWord.SegmentBLL.ShowSegment(objKeyWordList));
            sb.AppendLine("===================");
            foreach (var pair in ordered)
            {
                if (nRecordCount >= dAverageWordCount && !bIsOverLine)
                {
                    sb.AppendLine("-------------------------");
                    bIsOverLine = true;
                }
                sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                nRecordCount += 1;
            }
            return sb.ToString();
        }

        public List<string> KeyWord_Basic_Segment (string text, DataConfig objDataConfig, OptionConfig objOptionConfig)
        {

            WSR_Forget_Core.KeyBond.KeyBondColl<string, string> objCharBondColl = objDataConfig.KeyWord.BasicCharBondColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = null;// objDataConfig.KeyWord.BasicKeyWordColl;


            int nMaxWordLen = Convert.ToInt32(this.numericUpDown4.Value);
            List<string> objLines = this.GetLines(text, false, false);
            List<string> objResultWordList = new List<string>();
            for (int k = 0; k < objLines.Count; k++)
            {
                string line = objLines[k];

                if (this.checkBox8.Checked)
                {
                    #region 实时发现新词
                    //WSR_Forget_Core.KeyItem.KeyItemColl<string> objTempWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                    //WSR_Forget_SDK.KeyWord.KeyWordBLL.UpdateKeyWordCollByNGram(line, objTempWordColl, nMaxWordLen);
                    //objTempWordColl.Parameter.TotalValidCount += objDataConfig.KeyWord.BasicKeyWordColl.Parameter.TotalValidCount;
                    //foreach (var mdl in objTempWordColl)
                    //{
                    //    if (objDataConfig.KeyWord.BasicKeyWordColl.Contains(mdl.Key))
                    //    {
                    //        double dVaildCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(mdl.Key, objDataConfig.KeyWord.BasicKeyWordColl);
                    //        mdl.ValidCount += dVaildCount;
                    //        mdl.TotalCount += dVaildCount;


                    //    }
                    //}
                    //objKeyWordColl = objTempWordColl;
                    #endregion

                    #region 实时发现新词（新版）
                    objKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
                    List<string> objResult = WSR_Forget_SDK.KeyWord.SegmentBLL.SegmentEx(text,  objKeyWordColl, nMaxWordLen,true);
                    if (objResultWordList.Count > 0) objResultWordList.Add(Environment.NewLine);
                    objResultWordList.AddRange(objResult);
                    #endregion
                }
                else
                {
                    objKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
                    List<string> objResult = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(text, objCharBondColl, objKeyWordColl, nMaxWordLen, false, false);
                    if (objResultWordList.Count > 0) objResultWordList.Add(Environment.NewLine);
                    objResultWordList.AddRange(objResult);
                }

               

                #region 使用分词的语句更新词库
                if (this.checkBox7.Checked)
                {
                    WSR_Forget_SDK.KeyWord.KeyWordBLL.UpdateKeyWordCollByNGram(line, objDataConfig.KeyWord.BasicKeyWordColl, nMaxWordLen);
                }
                #endregion
            }

            return objResultWordList;
        }

        public string KeyWord_Basic_Segment_ShowResult(string text, DataConfig objDataConfig, OptionConfig objOptionConfig)
        { 
            List<string> objResultWordList = KeyWord_Basic_Segment(text, objDataConfig, objOptionConfig);

            return WSR_Forget_SDK.KeyWord.SegmentBLL.ShowSegment(objResultWordList);
        }
        public string KeyWord_Basic_ShowKeyWordColl(DataConfig objDataConfig, List<string> objKeyWordList = null, bool bShowContainWord = true)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown3.Value);
            bool bIsOnlyWord = this.checkBox6.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton7.Checked)
            {
                bIsOrderByDesc = false;
            }
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
            if (objKeyWordList == null || objKeyWordList.Count <= 0)
            {
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objKeyWordColl, nTopCount, bIsOnlyWord, bIsOrderByDesc);
            }
            else
            {
                WSR_Forget_Core.KeyItem.KeyItemColl<string> objTempWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                if (bShowContainWord)
                {
                    foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objKeyWordColl)
                    {
                        if (objKeyWordList.Any(x => mdl.Key.Contains(x)) && !objTempWordColl.Contains(mdl.Key))
                        {
                            objTempWordColl.Add(mdl);
                        }
                    }
                }
                else
                {
                    foreach (string keyword in objKeyWordList)
                    {
                        if (!objTempWordColl.Contains(keyword) && objKeyWordColl.Contains(keyword))
                        {
                            objTempWordColl.Add(objKeyWordColl[keyword]);
                        }
                    }
                }
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objTempWordColl, nTopCount, bIsOnlyWord);
            }
        }

        public void KeyWord_Basic_UpdateKeyWordColl(string line, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig)
        {
            frmDesktop desktop = this;
            string pattern = desktop.textBox2.Text;
            string groupName = desktop.textBox1.Text;

            int nMaxWordSize = Convert.ToInt32(this.numericUpDown5.Value);

            Match objMatch = Regex.Match(line, pattern, RegexOptions.Singleline);
            if (!objMatch.Success) return;

            string content = objMatch.Groups[groupName].Value;// CommonHelper.CommonLib.CleanHtml(objMatch.Groups[groupName].Value);

            List<string> objLinesList = this.GetLines(content, true, false);
            foreach (string text in objLinesList)
            {
              

                WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
                //WSR_Forget_Core.KeyBond.KeyBondColl<string, string> objBasicCharBondColl = objDataConfig.KeyWord.BasicCharBondColl;

                WSR_Forget_SDK.KeyWord.KeyWordBLL.UpdateKeyWordColl(text, objBasicKeyWordColl, nMaxWordSize);
                if (objBasicKeyWordColl.Parameter.TotalOffset > objBasicKeyWordColl.Parameter.ContainerSize)
                {
                    objDataConfig.KeyWord.BasicKeyWordColl = objBasicKeyWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(objBasicKeyWordColl);
                    GC.Collect();
                    if (this.checkBox9.Checked) desktop.AppendText(String.Format("{1}【词库信息熵】{0}", Math.Round(objBasicKeyWordColl.Parameter.Entropy, 4), Environment.NewLine));

                }
                 
            }
        }
        #endregion

        #region 词库：基准
        /// <summary>
        /// 基准/操作/从文本导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            //openFileDialog.FileName = "OriginalKeyWord.coll";

            openFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    AppendText("正在导入文本词库，请稍候……");

                    string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    if (File.Exists(sKeyWordPathFile))
                    {
                        WSR_Forget_Core.KeyItem.KeyItemColl<string> objConvertWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                        double dConvertTotalOffset = 0;

                        string sPathFile = sKeyWordPathFile;

                        {
                            //this.AppendText(String.Format("【进行】{0}", sPathFile));

                            FileInfo info = new FileInfo(sPathFile);
                            double dFileCharLength = info.Length;
                            double dLoadCharLength = 0;

                            DateTime dtUpdateTime = DateTime.Now;
                            this.pgbScanLines.Maximum = Convert.ToInt32(Math.Min(Int32.MaxValue, dFileCharLength));
                            this.pgbScanLines.Minimum = 0;
                            this.pgbScanLines.Value = this.pgbScanLines.Minimum;
                            using (StreamReader sr = new StreamReader(sPathFile, Encoding.UTF8))
                            {
                                string line = null;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    Application.DoEvents();
                                    if (String.IsNullOrWhiteSpace(line)) continue;

                                    dLoadCharLength += Encoding.UTF8.GetByteCount(line);
                                    if (this.pgbScanLines.Maximum <= dLoadCharLength)
                                    {
                                        dFileCharLength = dFileCharLength - dLoadCharLength + 1;
                                        this.pgbScanLines.Maximum = Convert.ToInt32(Math.Min(Int32.MaxValue, dFileCharLength));
                                        this.pgbScanLines.Value = 0;
                                        dLoadCharLength = 0;

                                    }
                                    this.pgbScanLines.Value = this.pgbScanLines.Maximum > dLoadCharLength ? Convert.ToInt32(dLoadCharLength) : this.pgbScanLines.Maximum - 1;

                                    string text = line; //这里可以再做一些需要特别处理的数据清洗，如多余的空格等

                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        string pattern = @"^\s*[\[【](?<Word>.*?)[\]】](\s*(?<Freq>\d+(\.\d+)?))(.*?)$";
                                        string groupName = "Word | Freq";

                                        string[] objGroupNames = Regex.Split(groupName, "[|｜]+", RegexOptions.Singleline);
                                        Dictionary<string, string> objNameContentDict = new Dictionary<string, string>();
                                        List<string> objGroupNameList = new List<string>();

                                        Match objMatch = Regex.Match(text, pattern, RegexOptions.Singleline);
                                        if (!objMatch.Success) continue;

                                        foreach (string name in objGroupNames)
                                        {
                                            string trimName = name.Trim();
                                            if (String.IsNullOrEmpty(trimName)) continue;
                                            objGroupNameList.Add(trimName);
                                            if (!objNameContentDict.ContainsKey(trimName))
                                            {
                                                objNameContentDict.Add(trimName, objMatch.Groups[trimName].Value);
                                            }
                                        }
                                        if (objNameContentDict.Count <= 0) continue;


                                        string word = (objGroupNameList.Count > 0 && objNameContentDict.ContainsKey(objGroupNameList[0])) ? objNameContentDict[objGroupNameList[0]] : String.Empty;
                                        string keyword = String.IsNullOrWhiteSpace(word.Trim()) ? String.Empty : word.Trim();

                                        string freq = (objGroupNameList.Count > 1 && objNameContentDict.ContainsKey(objGroupNameList[1])) ? objNameContentDict[objGroupNameList[1]] : String.Empty;
                                        double frequency = String.IsNullOrWhiteSpace(freq.Trim()) ? 1 : Convert.ToDouble(freq.Trim());

                                        if (String.IsNullOrEmpty(keyword)) continue;
                                        WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objConvertWordColl, new WSR_Forget_Core.Memory.OffsetWeightMDL(0, frequency));
                                        dConvertTotalOffset += frequency;
                                    }
                                }

                            }
                            this.pgbScanLines.Value = this.pgbScanLines.Maximum;
                            this.pgbScanLines.Value = this.pgbScanLines.Minimum;

                        }

                        gobjDataConfig.KeyWord.StandardWordColl = Common.WordLibDAL.FixWordColl(objConvertWordColl, dConvertTotalOffset);

                    }
                    else
                    {
                        AppendText("【警告】文本词库不存在。");
                    }

                    AppendText("加载完毕。");

                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }



            }
        }
        /// <summary>
        /// 基准/生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            ExecuteByDesktop(KeyWord_Standard_UpdateKeyWordColl);
        }
        /// <summary>
        /// 基准/查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            string content = this.textBox5.Text;
            if (!String.IsNullOrWhiteSpace(content))
            {
                List<string> objKeyWordList = GetLines(content);
                if (radioButton11.Checked) //仅显示列表中的词
                {
                    this.rtbInfoBoard.Text = KeyWord_Standard_ShowKeyWordColl(gobjDataConfig, objKeyWordList, false);
                }
                if (radioButton12.Checked) //显示包含列表中词的所有词
                {
                    this.rtbInfoBoard.Text = KeyWord_Standard_ShowKeyWordColl(gobjDataConfig, objKeyWordList, true);
                }
            }
            else
            {
                this.rtbInfoBoard.Text = KeyWord_Standard_ShowKeyWordColl(gobjDataConfig);
            }
        }
        /// <summary>
        /// 基准/分词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            string content = this.textBox5.Text;
            if (!String.IsNullOrWhiteSpace(content))
            {
                var lines = this.GetLines(content, true, false);
                foreach (string line in lines)
                {
                    this.AppendText(this.KeyWord_Standard_Segment(line, gobjDataConfig, gobjOptionConfig));
                }
            }
        }
        /// <summary>
        /// 基准/关键词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKeyWordKeyWord_Click(object sender, EventArgs e)
        {
            string line = this.textBox5.Text;
            if (!String.IsNullOrWhiteSpace(line))
            {
                this.rtbInfoBoard.Text = this.KeyWord_Standard_CatchKeyWord(line, gobjDataConfig, gobjOptionConfig);
            }
        }

        /// <summary>
        /// 词库/基准/清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认清空基准词库？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                gobjDataConfig.KeyWord.StandardWordColl.Clear();
                this.AppendText(String.Format("【提示】基准词库清空完成。"));
            }
        }

        /// <summary>
        /// 词库/基准/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            saveFileDialog.FileName = "StandardWord.coll";

            saveFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    AppendText("正在保存基准词库，请稍候……");
                    string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    gobjDataConfig.KeyWord.StandardWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.StandardWordColl);
                    CommonHelper.SerialLib.SerializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(gobjDataConfig.KeyWord.StandardWordColl, sKeyWordPathFile);
                    AppendText("保存完毕。");
                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }
            }


 
        }
        /// <summary>
        /// 词库/基准/加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            openFileDialog.FileName = "StandardWord.coll";

            openFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    AppendText("正在加载基准词库，请稍候……");

                    string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    if (File.Exists(sKeyWordPathFile))
                    {
                        gobjDataConfig.KeyWord.StandardWordColl = CommonHelper.SerialLib.DeserializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(sKeyWordPathFile);
                        if (gobjDataConfig.KeyWord.StandardWordColl == null) gobjDataConfig.KeyWord.StandardWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                    }
                    else
                    {
                        AppendText("【警告】基准词库不存在。");
                    }

                    AppendText("加载完毕。");

                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }



            }
        }
        /// <summary>
        /// 导出到文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
            saveFileDialog.FileName = "StandardWord.txt";

            saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    AppendText("正在导出基准词库，请稍候……");
                    string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                    var objKeyWordColl = gobjDataConfig.KeyWord.StandardWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.StandardWordColl);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("【词条】遗忘词频");
                    sb.AppendLine("=============================");
                    var buffer = objKeyWordColl.OrderByDescending(x => x.ValidCount);
                    foreach (var mdl in buffer)
                    {
                        sb.AppendLine(String.Format("【{0}】{1}", mdl.Key, Math.Round(mdl.ValidCount, 4)));
                    }
                    AppendText("正在写入文件中，请稍候……");
                    File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                    AppendText("导出完毕。");
                }
                catch (Exception ep)
                {
                    AppendText(String.Format("【错误】{0}", ep.Message));
                }

            }
        }
        #endregion

 

        #region 基准：类库
        public string KeyWord_Standard_CatchKeyWord(string text, DataConfig objDataConfig, OptionConfig objOptionConfig)
        {

            //计算信息熵，同时词信息熵排序


            frmDesktop desktop = this;

            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.StandardWordColl;


            int nMaxWordLen = (int)this.numericUpDown7.Value;

            List<string> objKeyWordList = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(text, null, objKeyWordColl, nMaxWordLen, false, false);


            Dictionary<string, double> dict = new Dictionary<string, double>();

            double dTotalEntropy = 0;
            double dTotalVaildCount = 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objKeyWordColl.Parameter));
            double dLogTotalCount = Math.Log(dTotalVaildCount);

            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();
            foreach (string keyword in objKeyWordList)
            {
                if (objKeyWordColl.Contains(keyword))
                {
                    WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl = objKeyWordColl[keyword];
                    double dValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objKeyWordColl);
                    double dProbValue = dValidCount / dTotalVaildCount;
                    double dEntropy = -dProbValue * Math.Log(dProbValue);
                    dTotalEntropy += dEntropy;


                    if (!dict.ContainsKey(keyword)) dict.Add(keyword, 0);
                    if (objKeyWordColl.Contains(keyword))
                    {
                        dict[keyword] += dLogTotalCount - Math.Log(dValidCount);
                    }
                }
                else
                {
                    double dValidCount = 1;
                    double dProbValue = dValidCount / dTotalVaildCount;
                    double dEntropy = -dProbValue * Math.Log(dProbValue);
                    dTotalEntropy += dEntropy;

                    if (!dict.ContainsKey(keyword)) dict.Add(keyword, 0);
                    if (objKeyWordColl.Contains(keyword))
                    {
                        dict[keyword] += dLogTotalCount - Math.Log(dValidCount);
                    }
                }
            }

            //根据平均信息熵计算有效词数量
            double dAverageWordCount = Math.Pow(Math.E, dTotalEntropy - 0.5772156649 + 1);//依据平均信息熵



            var ordered = from pair in dict
                          orderby pair.Value descending
                          select pair;

            int nRecordCount = 0;
            bool bIsOverLine = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(WSR_Forget_SDK.KeyWord.SegmentBLL.ShowSegment(objKeyWordList));
            sb.AppendLine("===================");
            foreach (var pair in ordered)
            {
                if (nRecordCount >= dAverageWordCount && !bIsOverLine)
                {
                    sb.AppendLine("-------------------------");
                    bIsOverLine = true;
                }
                sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                nRecordCount += 1;
            }
            return sb.ToString();
        }
        public string KeyWord_Standard_Segment(string text, DataConfig objDataConfig, OptionConfig objOptionConfig)
        {
            frmDesktop desktop = this;

            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.StandardWordColl;

          
            int nMaxWordLen = (int)this.numericUpDown7.Value;
            List<string> objLineList = this.GetLines(text, true, false);
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < objLineList.Count; k++)
            {
                List<string> objKeyWordList = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(text, null, objKeyWordColl, nMaxWordLen, false, false);
                sb.AppendLine(WSR_Forget_SDK.KeyWord.SegmentBLL.ShowSegment(objKeyWordList));
            }
            return sb.ToString();
        }
        public string KeyWord_Standard_ShowKeyWordColl(DataConfig objDataConfig, List<string> objKeyWordList = null, bool bShowContainWord = true)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown6.Value);
            bool bIsOnlyWord = this.checkBox10.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton13.Checked)
            {
                bIsOrderByDesc = false;
            }
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.StandardWordColl;
            if (objKeyWordList == null || objKeyWordList.Count <= 0)
            {
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objKeyWordColl, nTopCount, bIsOnlyWord, bIsOrderByDesc);
            }
            else
            {
                WSR_Forget_Core.KeyItem.KeyItemColl<string> objTempWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                if (bShowContainWord)
                {
                    foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objKeyWordColl)
                    {
                        if (objKeyWordList.Any(x => mdl.Key.Contains(x)) && !objTempWordColl.Contains(mdl.Key))
                        {
                            objTempWordColl.Add(mdl);
                        }
                    }
                }
                else
                {
                    foreach (string keyword in objKeyWordList)
                    {
                        if (!objTempWordColl.Contains(keyword) && objKeyWordColl.Contains(keyword))
                        {
                            objTempWordColl.Add(objKeyWordColl[keyword]);
                        }
                    }
                }
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objTempWordColl, nTopCount, bIsOnlyWord);
            }
        }

        public void KeyWord_Standard_UpdateKeyWordColl(string line, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig)
        {
            frmDesktop desktop = this;
            string pattern = desktop.textBox4.Text;
            string groupName = desktop.textBox3.Text;


            Match objMatch = Regex.Match(line, pattern, RegexOptions.Singleline);
            if (!objMatch.Success) return;

            string content = objMatch.Groups[groupName].Value;// CommonHelper.CommonLib.CleanHtml(objMatch.Groups[groupName].Value);

            List<string> objLinesList = this.GetLines(content, true, false);
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
            WSR_Forget_Core.KeyBond.KeyBondColl<string, string> objBasicCharBondColl = objDataConfig.KeyWord.BasicCharBondColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objStandardWordColl = objDataConfig.KeyWord.StandardWordColl;

            int nMaxWordLen = Convert.ToInt32(this.numericUpDown9.Value);
            foreach (string part in objLinesList)
            {


                if (String.IsNullOrEmpty(part)) continue;
                List<string> objKeyWordList = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(part, objBasicCharBondColl, objBasicKeyWordColl, nMaxWordLen, false, false);
                foreach (string keyword in objKeyWordList)
                {
                    WSR_Forget_Core.Memory.OffsetWeightMDL objOffsetWeight = this.checkBox2.Checked ? new WSR_Forget_Core.Memory.OffsetWeightMDL(0, 1) : new WSR_Forget_Core.Memory.OffsetWeightMDL(1, 1);
                    WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objStandardWordColl,objOffsetWeight);
                }
            }

            if (objStandardWordColl.Parameter.TotalOffset >= objStandardWordColl.Parameter.ContainerSize)
            {
                objStandardWordColl = objDataConfig.KeyWord.StandardWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl(objStandardWordColl);
                GC.Collect();
                if (this.checkBox13.Checked) desktop.AppendText(String.Format("{1}【词库信息熵】{0}", Math.Round(objStandardWordColl.Parameter.Entropy, 4), Environment.NewLine));

            }
        }

        #endregion

        #region 词库：相对
        /// <summary>
        /// 导出到文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                if (this.radioButton4.Checked)
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");


                    saveFileDialog.FileName = "TopicWord_KL.txt";

                    saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog.FileName;
                        try
                        {
                            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();

                            {

                                WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = gobjDataConfig.KeyWord.StandardWordColl;
                                WSR_Forget_Core.KeyItem.KeyItemColl<string> objTopicKeyWordColl = gobjDataConfig.KeyWord.TopicWordColl;

                                double dTopicTotalCount = objTopicKeyWordColl.Parameter.TotalValidCount + 1;// 1 + objTopicKeyWordColl.Sum(x => WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(x, objTopicKeyWordColl));
  
                                double dTotalVaildCount = objBasicKeyWordColl.Parameter.TotalValidCount + 1;// 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objBasicKeyWordColl.Parameter));
                                double dLogTotalCount = Math.Log(dTotalVaildCount);


                                foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objTopicKeyWordColl)
                                {
                                    double dTopicValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objTopicKeyWordColl);
                                    double dTopicProbValue = dTopicValidCount / dTopicTotalCount;
                                   

                                    double dStdValidCount = !objBasicKeyWordColl.Contains(mdl.Key) ? 1 : WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(objBasicKeyWordColl[mdl.Key], objBasicKeyWordColl) + 1;
                                    double dStdProbValue = dStdValidCount / dTotalVaildCount;
                                    double dResultWeight = dTopicValidCount * (Math.Log(dTopicProbValue) - Math.Log(dStdProbValue));


                                    if (!objWordProbDict.ContainsKey(mdl.Key)) objWordProbDict.Add(mdl.Key, dResultWeight);

                                }
                            }

                            string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                           
                            AppendText("正在导出主题词库，请稍候……");
                          

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("【词条】权重（KL散度）");
                            sb.AppendLine("=============================");
                            var buffer = objWordProbDict.OrderByDescending(x => x.Value);
                            foreach (var pair in buffer)
                            {
                                sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                            }
                            AppendText("正在写入文件中，请稍候……");
                            File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                            AppendText("导出完毕。");
                        }
                        catch (Exception ep)
                        {
                            AppendText(String.Format("【错误】{0}", ep.Message));
                        }

                    }
                }
                if (this.radioButton3.Checked)
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");


                    saveFileDialog.FileName = "IncrementWord_KL.txt";

                    saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog.FileName;
                        try
                        {
                            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();

                            {
                                WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = gobjDataConfig.KeyWord.StandardWordColl;
                                WSR_Forget_Core.KeyItem.KeyItemColl<string> objIncrementKeyWordColl = gobjDataConfig.KeyWord.IncrementWordColl;

                                double dIncrementTotalCount = objIncrementKeyWordColl.Parameter.TotalValidCount + 1;// 1 + objIncrementKeyWordColl.Sum(x => WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(x, objIncrementKeyWordColl));
                               
                                double dTotalVaildCount = objBasicKeyWordColl.Parameter.TotalValidCount + 1;// 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objBasicKeyWordColl.Parameter));
                                double dLogTotalCount = Math.Log(dTotalVaildCount);


                               foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objIncrementKeyWordColl)
                                {
                                    double dIncrementValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objIncrementKeyWordColl);
                                    double dIncrementProbValue = dIncrementValidCount / dIncrementTotalCount;
                                 
                                    double dStdValidCount = !objBasicKeyWordColl.Contains(mdl.Key) ? 1 : WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(objBasicKeyWordColl[mdl.Key], objBasicKeyWordColl) + 1;
                                    double dStdProbValue = dStdValidCount / dTotalVaildCount;
                                    double dResultWeight = dIncrementValidCount * (Math.Log(dIncrementProbValue) - Math.Log(dStdProbValue));


                                    if (!objWordProbDict.ContainsKey(mdl.Key)) objWordProbDict.Add(mdl.Key, dResultWeight);

                                }
                            }

                            string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                           

                            AppendText("正在导出热词词库，请稍候……");
                            
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("【词条】遗忘词频");
                            sb.AppendLine("=============================");
                            var buffer = objWordProbDict.OrderByDescending(x => x.Value);
                            foreach (var pair in buffer)
                            {
                                sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                            }
                            AppendText("正在写入文件中，请稍候……");
                            File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                            AppendText("导出完毕。");
                        }
                        catch (Exception ep)
                        {
                            AppendText(String.Format("【错误】{0}", ep.Message));
                        }

                    }
                }

            }
            else
            {
                if (this.radioButton4.Checked)
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");


                    saveFileDialog.FileName = "TopicWord.txt";

                    saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog.FileName;
                        try
                        {

                            string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = null;

                            AppendText("正在导出主题词库，请稍候……");
                            objKeyWordColl = gobjDataConfig.KeyWord.TopicWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.TopicWordColl);


                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("【词条】遗忘词频");
                            sb.AppendLine("=============================");
                            var buffer = objKeyWordColl.OrderByDescending(x => x.ValidCount);
                            foreach (var mdl in buffer)
                            {
                                sb.AppendLine(String.Format("【{0}】{1}", mdl.Key, Math.Round(mdl.ValidCount, 4)));
                            }
                            AppendText("正在写入文件中，请稍候……");
                            File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                            AppendText("导出完毕。");
                        }
                        catch (Exception ep)
                        {
                            AppendText(String.Format("【错误】{0}", ep.Message));
                        }

                    }
                }
                if (this.radioButton3.Checked)
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");


                    saveFileDialog.FileName = "IncrementWord.txt";

                    saveFileDialog.Filter = "词库文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog.FileName;
                        try
                        {

                            string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = null;


                            AppendText("正在导出热词词库，请稍候……");
                            objKeyWordColl = gobjDataConfig.KeyWord.IncrementWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.IncrementWordColl);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("【词条】遗忘词频");
                            sb.AppendLine("=============================");
                            var buffer = objKeyWordColl.OrderByDescending(x => x.ValidCount);
                            foreach (var mdl in buffer)
                            {
                                sb.AppendLine(String.Format("【{0}】{1}", mdl.Key, Math.Round(mdl.ValidCount, 4)));
                            }
                            AppendText("正在写入文件中，请稍候……");
                            File.WriteAllText(sKeyWordPathFile, sb.ToString(), Encoding.UTF8);
                            AppendText("导出完毕。");
                        }
                        catch (Exception ep)
                        {
                            AppendText(String.Format("【错误】{0}", ep.Message));
                        }

                    }
                }

            }

        }

        /// <summary>
        /// 词库/相对/生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button22_Click(object sender, EventArgs e)
        {
            if (this.radioButton4.Checked) //主题词库
            {
                this.ExecuteByDesktop(this.KeyWord_Topic_UpdateKeyWordColl);
            }
            if (this.radioButton3.Checked) //热词词库
            {
                gobjDataConfig.KeyWord.IncrementWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl(gobjDataConfig.KeyWord.StandardWordColl);
                this.ExecuteByDesktop(this.KeyWord_Increment_UpdateKeyWordColl);
            }
        }
        /// <summary>
        /// 词库/相对/查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked) //显示相对结果
            {

                if (this.radioButton4.Checked) //主题词库
                {
                    this.rtbInfoBoard.Text = this.KeyWord_Topic_ShowKeyWordColl_KL(gobjDataConfig);
                }
                if (this.radioButton3.Checked) //热词词库
                {
                    this.rtbInfoBoard.Text = this.KeyWord_Increment_ShowKeyWordColl_KL(gobjDataConfig);
                }
            }
            else
            {
                if (this.radioButton4.Checked) //主题词库
                {
                    this.rtbInfoBoard.Text = this.KeyWord_Topic_ShowKeyWordColl(gobjDataConfig);
                }
                if (this.radioButton3.Checked) //热词词库
                {
                    this.rtbInfoBoard.Text = this.KeyWord_Increment_ShowKeyWordColl(gobjDataConfig);
                }
            }
        }

        /// <summary>
        /// 词库/相对/清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button23_Click(object sender, EventArgs e)
        {

            if (this.radioButton4.Checked) //主题词库
            {
                if (MessageBox.Show("确认清空主题词库？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    gobjDataConfig.KeyWord.TopicWordColl.Clear();
                    this.AppendText(String.Format("【提示】主题词库清空完成。"));
                }
            }
            if (this.radioButton3.Checked) //热词词库
            {
                if (MessageBox.Show("确认清空热词词库？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    gobjDataConfig.KeyWord.IncrementWordColl.Clear();
                    this.AppendText(String.Format("【提示】主题词库清空完成。"));
                }
            }
        }

        /// <summary>
        /// 相对/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button25_Click(object sender, EventArgs e)
        {
            if (this.radioButton4.Checked)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
                saveFileDialog.FileName = "TopicWord.coll";

                saveFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog.FileName;
                    try
                    {
                        AppendText("正在保存主题词库，请稍候……");
                        string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                        gobjDataConfig.KeyWord.TopicWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.TopicWordColl);
                        CommonHelper.SerialLib.SerializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(gobjDataConfig.KeyWord.TopicWordColl, sKeyWordPathFile);
                        AppendText("保存完毕。");
                    }
                    catch (Exception ep)
                    {
                        AppendText(String.Format("【错误】{0}", ep.Message));
                    }
                }
            }
            if (this.radioButton3.Checked)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
                saveFileDialog.FileName = "IncrementWord.coll";

                saveFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog.FileName;
                    try
                    {
                        AppendText("正在保存热词词库，请稍候……");
                        string sKeyWordPathFile = saveFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                        gobjDataConfig.KeyWord.IncrementWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl<string>(gobjDataConfig.KeyWord.IncrementWordColl);
                        CommonHelper.SerialLib.SerializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(gobjDataConfig.KeyWord.IncrementWordColl, sKeyWordPathFile);
                        AppendText("保存完毕。");
                    }
                    catch (Exception ep)
                    {
                        AppendText(String.Format("【错误】{0}", ep.Message));
                    }
                }
            }
        }
        /// <summary>
        /// 相对/加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button24_Click(object sender, EventArgs e)
        {
            if (this.radioButton4.Checked)
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
                openFileDialog.FileName = "TopicWord.coll";

                openFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        AppendText("正在加载主题词库，请稍候……");

                        string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                        if (File.Exists(sKeyWordPathFile))
                        {
                            gobjDataConfig.KeyWord.TopicWordColl = CommonHelper.SerialLib.DeserializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(sKeyWordPathFile);
                            if (gobjDataConfig.KeyWord.TopicWordColl == null) gobjDataConfig.KeyWord.TopicWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                        }
                        else
                        {
                            AppendText("【警告】主题词库不存在。");
                        }

                        AppendText("加载完毕。");

                    }
                    catch (Exception ep)
                    {
                        AppendText(String.Format("【错误】{0}", ep.Message));
                    }



                }
            }
            if (this.radioButton3.Checked)
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetSubWorkSpacePath("KeyWord");
                openFileDialog.FileName = "IncrementWord.coll";

                openFileDialog.Filter = "词库文件(*.coll)|*.coll|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        AppendText("正在加载热词词库，请稍候……");

                        string sKeyWordPathFile = openFileDialog.FileName;// Path.GetFullPath(String.Format(@"{0}\{1}", floder, "BasicKeyWord.coll"));

                        if (File.Exists(sKeyWordPathFile))
                        {
                            gobjDataConfig.KeyWord.IncrementWordColl = CommonHelper.SerialLib.DeserializeBinary<WSR_Forget_Core.KeyItem.KeyItemColl<string>>(sKeyWordPathFile);
                            if (gobjDataConfig.KeyWord.IncrementWordColl == null) gobjDataConfig.KeyWord.IncrementWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();

                        }
                        else
                        {
                            AppendText("【警告】热词词库不存在。");
                        }

                        AppendText("加载完毕。");

                    }
                    catch (Exception ep)
                    {
                        AppendText(String.Format("【错误】{0}", ep.Message));
                    }



                }
            }
        }
       
        #endregion

        #region 相对：类库
        public string KeyWord_Topic_ShowKeyWordColl(DataConfig objDataConfig, List<string> objKeyWordList = null, bool bShowContainWord = true)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown10.Value);
            bool bIsOnlyWord = this.checkBox14.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton17.Checked)
            {
                bIsOrderByDesc = false;
            }
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.TopicWordColl;
            if (objKeyWordList == null || objKeyWordList.Count <= 0)
            {
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objKeyWordColl, nTopCount, bIsOnlyWord, bIsOrderByDesc);
            }
            else
            {
                WSR_Forget_Core.KeyItem.KeyItemColl<string> objTempWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                if (bShowContainWord)
                {
                    foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objKeyWordColl)
                    {
                        if (objKeyWordList.Any(x => mdl.Key.Contains(x)) && !objTempWordColl.Contains(mdl.Key))
                        {
                            objTempWordColl.Add(mdl);
                        }
                    }
                }
                else
                {
                    foreach (string keyword in objKeyWordList)
                    {
                        if (!objTempWordColl.Contains(keyword) && objKeyWordColl.Contains(keyword))
                        {
                            objTempWordColl.Add(objKeyWordColl[keyword]);
                        }
                    }
                }
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objTempWordColl, nTopCount, bIsOnlyWord);
            }
        }
        public string KeyWord_Topic_ShowKeyWordColl_KL(DataConfig config)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown10.Value);
            bool bIsOnlyWord = this.checkBox14.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton17.Checked)
            {
                bIsOrderByDesc = false;
            }

            WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = config.KeyWord.StandardWordColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objTopicKeyWordColl = config.KeyWord.TopicWordColl;

            double dTopicTotalCount = objTopicKeyWordColl.Parameter.TotalValidCount + 1;// 1 + objTopicKeyWordColl.Sum(x => WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(x, objTopicKeyWordColl));
            double dTopicTotalEntropy = 0;
            double dTotalVaildCount = objBasicKeyWordColl.Parameter.TotalValidCount + 1;// 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objBasicKeyWordColl.Parameter));
            double dLogTotalCount = Math.Log(dTotalVaildCount);


            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();
            foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objTopicKeyWordColl)
            {
                double dTopicValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objTopicKeyWordColl);
                double dTopicProbValue = dTopicValidCount / dTopicTotalCount;
                double dTopicEntropy = -dTopicProbValue * Math.Log(dTopicProbValue);
                dTopicTotalEntropy += dTopicEntropy;

                double dStdValidCount = !objBasicKeyWordColl.Contains(mdl.Key) ? 1 : WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(objBasicKeyWordColl[mdl.Key], objBasicKeyWordColl) + 1;
                double dStdProbValue = dStdValidCount / dTotalVaildCount;
                double dResultWeight = dTopicValidCount * (Math.Log(dTopicProbValue) - Math.Log(dStdProbValue));


                if (!objWordProbDict.ContainsKey(mdl.Key)) objWordProbDict.Add(mdl.Key, dResultWeight);

            }

            //根据平均信息熵计算有效词数量
            double dAverageWordCount = Math.Pow(Math.E, dTopicTotalEntropy - 0.5772156649 + 1);//依据平均信息熵



            IOrderedEnumerable<KeyValuePair<string, double>> ordered = null;
            if (!bIsOrderByDesc)
            {
                ordered = from pair in objWordProbDict
                          orderby pair.Value ascending
                          select pair;
            }
            else
            {
                ordered = from pair in objWordProbDict
                          orderby pair.Value descending
                          select pair;
            }
            int nRecordCount = 0;
            bool bIsOverLine = false;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("===================");
            foreach (var pair in ordered)
            {
                if (nRecordCount >= nTopCount) break;
                if (nRecordCount >= dAverageWordCount && !bIsOverLine)
                {
                    sb.AppendLine("-------------------------");
                    bIsOverLine = true;
                }
                if (bIsOnlyWord)
                {
                    if (pair.Key.Length > 1 && !Regex.IsMatch(pair.Key, @"^[\s\p{P}A-Za-z0-9]+$")) sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                }
                else
                {
                    sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                }
                nRecordCount += 1;
            }
            return sb.ToString();
        }

        public string KeyWord_Increment_ShowKeyWordColl(DataConfig objDataConfig, List<string> objKeyWordList = null, bool bShowContainWord = true)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown10.Value);
            bool bIsOnlyWord = this.checkBox14.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton17.Checked)
            {
                bIsOrderByDesc = false;
            }
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objKeyWordColl = objDataConfig.KeyWord.IncrementWordColl;
            if (objKeyWordList == null || objKeyWordList.Count <= 0)
            {
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objKeyWordColl, nTopCount, bIsOnlyWord, bIsOrderByDesc);
            }
            else
            {
                WSR_Forget_Core.KeyItem.KeyItemColl<string> objTempWordColl = new WSR_Forget_Core.KeyItem.KeyItemColl<string>();
                if (bShowContainWord)
                {
                    foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objKeyWordColl)
                    {
                        if (objKeyWordList.Any(x => mdl.Key.Contains(x)) && !objTempWordColl.Contains(mdl.Key))
                        {
                            objTempWordColl.Add(mdl);
                        }
                    }
                }
                else
                {
                    foreach (string keyword in objKeyWordList)
                    {
                        if (!objTempWordColl.Contains(keyword) && objKeyWordColl.Contains(keyword))
                        {
                            objTempWordColl.Add(objKeyWordColl[keyword]);
                        }
                    }
                }
                return WSR_Forget_Core.KeyItem.KeyItemHelper.ShowKeyItemColl(objTempWordColl, nTopCount, bIsOnlyWord);
            }
        }
        public string KeyWord_Increment_ShowKeyWordColl_KL(DataConfig config)
        {
            int nTopCount = Convert.ToInt32(this.numericUpDown10.Value);
            bool bIsOnlyWord = this.checkBox14.Checked;
            bool bIsOrderByDesc = true;
            if (this.radioButton17.Checked)
            {
                bIsOrderByDesc = false;
            }

            WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = config.KeyWord.StandardWordColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objIncrementKeyWordColl = config.KeyWord.IncrementWordColl;

            double dIncrementTotalCount = objIncrementKeyWordColl.Parameter.TotalValidCount + 1;// 1 + objIncrementKeyWordColl.Sum(x => WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(x, objIncrementKeyWordColl));
            double dIncrementTotalEntropy = 0;
            double dTotalVaildCount = objBasicKeyWordColl.Parameter.TotalValidCount + 1;// 1.0 / (1.0 - WSR_Forget_Core.Memory.MemoryDAL.CalcRemeberValue(1, objBasicKeyWordColl.Parameter));
            double dLogTotalCount = Math.Log(dTotalVaildCount);


            Dictionary<string, double> objWordProbDict = new Dictionary<string, double>();
            foreach (WSR_Forget_Core.KeyItem.KeyItemMDL<string> mdl in objIncrementKeyWordColl)
            {
                double dIncrementValidCount = WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount<string>(mdl, objIncrementKeyWordColl);
                double dIncrementProbValue = dIncrementValidCount / dIncrementTotalCount;
                double dIncrementEntropy = -dIncrementProbValue * Math.Log(dIncrementProbValue);
                dIncrementTotalEntropy += dIncrementEntropy;

                double dStdValidCount = !objBasicKeyWordColl.Contains(mdl.Key) ? 1 : WSR_Forget_Core.KeyItem.KeyItemHelper.CalcValidCount(objBasicKeyWordColl[mdl.Key], objBasicKeyWordColl) + 1;
                double dStdProbValue = dStdValidCount / dTotalVaildCount;
                double dResultWeight = dIncrementValidCount * (Math.Log(dIncrementProbValue) - Math.Log(dStdProbValue));


                if (!objWordProbDict.ContainsKey(mdl.Key)) objWordProbDict.Add(mdl.Key, dResultWeight);

            }

            //根据平均信息熵计算有效词数量
            double dAverageWordCount = Math.Pow(Math.E, dIncrementTotalEntropy - 0.5772156649 + 1);//依据平均信息熵



            IOrderedEnumerable<KeyValuePair<string, double>> ordered = null;
            if (!bIsOrderByDesc)
            {
                ordered = from pair in objWordProbDict
                          orderby pair.Value ascending
                          select pair;
            }
            else
            {
                ordered = from pair in objWordProbDict
                          orderby pair.Value descending
                          select pair;
            }
            int nRecordCount = 0;
            bool bIsOverLine = false;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("===================");
            foreach (var pair in ordered)
            {
                if (nRecordCount >= nTopCount) break;
                if (nRecordCount >= dAverageWordCount && !bIsOverLine)
                {
                    sb.AppendLine("-------------------------");
                    bIsOverLine = true;
                }
                if (bIsOnlyWord)
                {
                    if (pair.Key.Length > 1 && !Regex.IsMatch(pair.Key, @"^[\s\p{P}A-Za-z0-9]+$")) sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                }
                else
                {
                    sb.AppendLine(String.Format("【{0}】{1}", pair.Key, Math.Round(pair.Value, 4)));
                }
                nRecordCount += 1;
            }
            return sb.ToString();
        }

        public void KeyWord_Topic_UpdateKeyWordColl(string line, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig)
        {
            frmDesktop desktop = this;
            string pattern = desktop.textBox8.Text;
            string groupName = desktop.textBox7.Text;


            Match objMatch = Regex.Match(line, pattern, RegexOptions.Singleline);
            if (!objMatch.Success) return;

            string content = objMatch.Groups[groupName].Value;// CommonHelper.CommonLib.CleanHtml(objMatch.Groups[groupName].Value);

            List<string> objLinesList = this.GetLines(content, true, false);
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
            WSR_Forget_Core.KeyBond.KeyBondColl<string, string> objBasicCharBondColl = objDataConfig.KeyWord.BasicCharBondColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objTopicWordColl = objDataConfig.KeyWord.TopicWordColl;

            int nMaxWordLen = Convert.ToInt32(this.numericUpDown12.Value);
            foreach (string part in objLinesList)
            {


                if (String.IsNullOrEmpty(part)) continue;
                List<string> objKeyWordList = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(part, objBasicCharBondColl, objBasicKeyWordColl, nMaxWordLen, false, false);
                foreach (string keyword in objKeyWordList)
                {
                    WSR_Forget_Core.Memory.OffsetWeightMDL objOffsetWeight = this.checkBox3.Checked ? new WSR_Forget_Core.Memory.OffsetWeightMDL(0, 1) : new WSR_Forget_Core.Memory.OffsetWeightMDL(1, 1);
                    WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objTopicWordColl, objOffsetWeight);
                }
            }

            if (objTopicWordColl.Parameter.TotalOffset >= objTopicWordColl.Parameter.ContainerSize)
            {
                objTopicWordColl = objDataConfig.KeyWord.TopicWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl(objTopicWordColl);
                GC.Collect();
                if (this.checkBox17.Checked) desktop.AppendText(String.Format("{1}【词库信息熵】{0}", Math.Round(objTopicWordColl.Parameter.Entropy, 4), Environment.NewLine));

            }
        }
        public void KeyWord_Increment_UpdateKeyWordColl(string line, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig)
        {
            frmDesktop desktop = this;
            string pattern = desktop.textBox8.Text;
            string groupName = desktop.textBox7.Text;


            Match objMatch = Regex.Match(line, pattern, RegexOptions.Singleline);
            if (!objMatch.Success) return;

            string content = objMatch.Groups[groupName].Value;// CommonHelper.CommonLib.CleanHtml(objMatch.Groups[groupName].Value);

            List<string> objLinesList = this.GetLines(content, true, false);
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objBasicKeyWordColl = objDataConfig.KeyWord.BasicKeyWordColl;
            WSR_Forget_Core.KeyBond.KeyBondColl<string, string> objBasicCharBondColl = objDataConfig.KeyWord.BasicCharBondColl;
            WSR_Forget_Core.KeyItem.KeyItemColl<string> objIncrementWordColl = objDataConfig.KeyWord.IncrementWordColl;

            int nMaxWordLen = Convert.ToInt32(this.numericUpDown12.Value);
            foreach (string part in objLinesList)
            { 
                if (String.IsNullOrEmpty(part)) continue;
                List<string> objKeyWordList = WSR_Forget_SDK.KeyWord.SegmentBLL.Segment(part, objBasicCharBondColl, objBasicKeyWordColl, nMaxWordLen, false, false);
                foreach (string keyword in objKeyWordList)
                {
                    WSR_Forget_Core.Memory.OffsetWeightMDL objOffsetWeight =this.checkBox3.Checked?new WSR_Forget_Core.Memory.OffsetWeightMDL(0,1):  new WSR_Forget_Core.Memory.OffsetWeightMDL(1,1);
                    WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objIncrementWordColl, objOffsetWeight);
                }
            }

            if (objIncrementWordColl.Parameter.TotalOffset >= objIncrementWordColl.Parameter.ContainerSize)
            {
                objIncrementWordColl = objDataConfig.KeyWord.IncrementWordColl = WSR_Forget_Core.KeyItem.KeyItemDAL.ClearKeyItemColl(objIncrementWordColl);
                GC.Collect();
                if (this.checkBox17.Checked) desktop.AppendText(String.Format("{1}【词库信息熵】{0}", Math.Round(objIncrementWordColl.Parameter.Entropy, 4), Environment.NewLine));

            }
        }

        #endregion


    }
}
