using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using ForgetNLP_WordLib_Demo.Config;
namespace ForgetNLP_WordLib_Demo
{
    public partial class frmDesktop : Form
    {
        public frmDesktop()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = String.Format("{0} v{1}.{2}.{3}   by 老憨  QQ交流群：217947873 ", this.Text, version.Major, version.Minor, version.Build);

            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(5));

        }


        public void chkDelayTime_CheckedChanged(object sender, EventArgs e)
        {
            nmbReadSpeed.Enabled = dtPickerUpdateTime.Enabled = chkDelayTime.Checked;

        }

        public List<string> GetPartList(string line, bool bIsKeepSplitChar = true, string splitChars = ",，;；。!！?？:：")
        {
            List<string> objResultList = new List<string>();

            string pattern = String.Format("([{0}]+)", splitChars);
            string[] parts = Regex.Split(line, pattern, RegexOptions.Singleline);

            StringBuilder sb = new StringBuilder();
            foreach (string part in parts)
            {

                if (!Regex.IsMatch(part, pattern, RegexOptions.Singleline))
                {
                    sb.Append(part);
                }
                else
                {
                    if (bIsKeepSplitChar) sb.Append(part);
                    objResultList.Add(sb.ToString());
                    sb.Clear();
                }
            }
            if (sb.Length > 0) objResultList.Add(sb.ToString());
            return objResultList;
        }
        public List<string> GetLines(string text, bool bRemoveEmpty = true, bool bRemoveRepeat = true)
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
        public void AppendText(string src)
        {
            this.rtbInfoBoard.AppendText(String.Format("{1}{0}", src, Environment.NewLine));
            this.rtbInfoBoard.ScrollToCaret();
            Application.DoEvents();
        }

        public Encoding GetEncoding()
        {
            Encoding objEncoding = Encoding.Default;
            if (radGb2312.Checked)
            {
                objEncoding = Encoding.Default;
            }
            if (radUTF8.Checked)
            {
                objEncoding = Encoding.UTF8;
            }
            return objEncoding;
        }
        public void ReadForDebugEncoding(string pathfile)
        {
            string sPathFile = pathfile;



            if (File.Exists(sPathFile))
            {


                Encoding objEncoding = GetEncoding();
                using (StreamReader sr = new StreamReader(sPathFile, objEncoding))
                {
                    string line = null;
                    StringBuilder sb = new StringBuilder();
                    int nLoadLineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string text = Regex.Replace(line, "<.*?>", "");
                        if (String.IsNullOrWhiteSpace(text)) continue;
                        sb.AppendLine(text);
                        nLoadLineCount += 1;
                        if (nLoadLineCount > 200)
                        {
                            break;
                        }
                    }
                    AppendText(sb.ToString());
                }

            }
        }
        public void ReadForDebugEncoding()
        {
            string sPathFile = this.tbFilePath.Text;
            if (radFloderMode.Checked)
            {
                if (Directory.Exists(this.tbFilePath.Text))
                {

                    string[] objFileColl = Directory.GetFiles(this.tbFilePath.Text, "*.*", SearchOption.AllDirectories);
                    if (objFileColl.Count() > 0)
                    {
                        sPathFile = objFileColl[0];
                    }
                }
            }
            if (radFileMode.Checked)
            {
                sPathFile = this.tbFilePath.Text;
            }
            this.rtbInfoBoard.Text = String.Empty;
            this.AppendText("【如遇乱码，请尝试更换文件编码】");
            ReadForDebugEncoding(sPathFile);
            this.AppendText("【如遇乱码，请尝试更换文件编码】");
        }
        public void radUTF8_CheckedChanged(object sender, EventArgs e)
        {
            ReadForDebugEncoding();
        }
        public void btnOpenPath_Click(object sender, EventArgs e)
        {
            if (radFileMode.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonHelper.CachePathDAL.GetWorkSpacePath();
                if (!String.IsNullOrEmpty(this.tbFilePath.Text))
                {
                    try
                    {
                        string floder = Path.GetDirectoryName(this.tbFilePath.Text);
                        if (Directory.Exists(floder))
                        {
                            openFileDialog.InitialDirectory = floder;
                        }
                    }
                    catch
                    {
                    }
                }

                openFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.tbFilePath.Text = openFileDialog.FileName;

                }
            }

            if (radFloderMode.Checked)
            {
                FolderBrowserDialog openFloderDialog = new FolderBrowserDialog();
                openFloderDialog.SelectedPath = CommonHelper.CachePathDAL.GetWorkSpacePath();
                if (!String.IsNullOrEmpty(this.tbFilePath.Text))
                {
                    try
                    {
                        string floder = Path.GetDirectoryName(this.tbFilePath.Text);
                        if (Directory.Exists(floder))
                        {
                            openFloderDialog.SelectedPath = floder;
                        }
                    }
                    catch
                    {
                    }
                }
                if (openFloderDialog.ShowDialog() == DialogResult.OK)
                {
                    this.tbFilePath.Text = openFloderDialog.SelectedPath;
                }
            }

            ReadForDebugEncoding();
        }


        public delegate void ExecuteByLineDelegate(string line, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig);
        public delegate void ExecuteByFileDelegate(string filename,StringBuilder objContentBuilder, Config.DataConfig objDataConfig, Config.OptionConfig objOptionConfig);
        public Config.DataConfig gobjDataConfig = new Config.DataConfig();
        public Config.OptionConfig gobjOptionConfig = new Config.OptionConfig();
        public StringBuilder ExecuteByContent(string content, ExecuteByLineDelegate objCallback, bool bIsSaveContent = false)
        {
            StringBuilder objContentBuilder = new StringBuilder();
            double dFileCharLength = content.Length;
            double dLoadCharLength = 0;

            DateTime dtUpdateTime = DateTime.Now;
            this.pgbScanLines.Maximum = Convert.ToInt32(dFileCharLength);
            this.pgbScanLines.Minimum = 0;
            this.pgbScanLines.Value = this.pgbScanLines.Minimum;
            using (StringReader sr = new StringReader(content))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    Application.DoEvents();
                    if (bIsSaveContent) objContentBuilder.AppendLine(line);

                    //if (!String.IsNullOrWhiteSpace(line)) line = Regex.Replace(line, @"(\p{C}+)|(<.*?>)", "");//去除可能的控制符、Html标签
                    if (String.IsNullOrWhiteSpace(line)) continue;


                    dLoadCharLength += line.Length;
                    this.pgbScanLines.Value = this.pgbScanLines.Maximum > dLoadCharLength ? Convert.ToInt32(dLoadCharLength) : this.pgbScanLines.Maximum - 1;

                    string text = line; //这里可以再做一些需要特别处理的数据清洗，如多余的空格等

                    if (!String.IsNullOrEmpty(text))
                    {
                        if (this.chkDelayTime.Checked)
                        {
                            dtUpdateTime = dtPickerUpdateTime.Value.AddSeconds(text.Length / (double)nmbReadSpeed.Value);
                        }

                        if (objCallback != null) objCallback(text, gobjDataConfig, gobjOptionConfig);
                    }
                }
            }
            this.pgbScanLines.Value = this.pgbScanLines.Maximum;
            this.pgbScanLines.Value = this.pgbScanLines.Minimum;

            return objContentBuilder;
        }
        public StringBuilder ExecuteByFile(string sPathFile, Encoding objEncoding, ExecuteByLineDelegate objCallback, bool bIsSaveContent = false)
        {
            StringBuilder objContentBuilder = new StringBuilder();

            if (File.Exists(sPathFile))
            {
                //this.AppendText(String.Format("【进行】{0}", sPathFile));

                FileInfo info = new FileInfo(sPathFile);
                double dFileCharLength = info.Length;
                double dLoadCharLength = 0;

                DateTime dtUpdateTime = DateTime.Now;
                this.pgbScanLines.Maximum = Convert.ToInt32(Math.Min(Int32.MaxValue, dFileCharLength));
                this.pgbScanLines.Minimum = 0;
                this.pgbScanLines.Value = this.pgbScanLines.Minimum;
                using (StreamReader sr = new StreamReader(sPathFile, objEncoding))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Application.DoEvents();
                        if (bIsSaveContent) objContentBuilder.AppendLine(line);
                        //if (!String.IsNullOrWhiteSpace(line)) line = Regex.Replace(line,@"(\p{C}+)|(<.*?>)","");//去除可能的控制符、Html标签
                        //if (!String.IsNullOrWhiteSpace(line)) line = Regex.Replace(line,@"(\p{C}+)","");//去除可能的控制符、Html标签
                        if (String.IsNullOrWhiteSpace(line)) continue;

                        dLoadCharLength += objEncoding.GetByteCount(line);
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
                            if (this.chkDelayTime.Checked)
                            {
                                dtUpdateTime = dtPickerUpdateTime.Value.AddSeconds(text.Length / (double)nmbReadSpeed.Value);
                            }

                            try
                            {
                                if (objCallback != null) objCallback(text, gobjDataConfig, gobjOptionConfig);
                            }
                            catch (Exception ep)
                            {
                                AppendText(ep.Message);
                            }
                        }
                    }

                }
                this.pgbScanLines.Value = this.pgbScanLines.Maximum;
                this.pgbScanLines.Value = this.pgbScanLines.Minimum;

            }

            return objContentBuilder;

        }
        public void ExecuteByDesktop(ExecuteByLineDelegate objCallbackByLine, ExecuteByFileDelegate objCallbackByFile = null)
        {
            DateTime dtStartTime = DateTime.Now;
            Encoding objEncoding = GetEncoding();
            switch (this.tabCorpusSource.SelectedTab.Name)
            {
                case "tabCorpusPageFile":
                    {
                        if (radFileMode.Checked)
                        {
                            string sPathFile = this.tbFilePath.Text;

                            if (objCallbackByFile != null)
                            {
                                StringBuilder objContentBuilder = ExecuteByFile(sPathFile, objEncoding, objCallbackByLine, true);
                                objCallbackByFile(sPathFile, objContentBuilder, gobjDataConfig, gobjOptionConfig);
                            }
                            else
                            {
                                ExecuteByFile(sPathFile, objEncoding, objCallbackByLine);
                            }
                        }

                        if (radFloderMode.Checked)
                        {
                            if (Directory.Exists(this.tbFilePath.Text))
                            {
                                string[] objFileColl = Directory.GetFiles(this.tbFilePath.Text, "*.*", SearchOption.AllDirectories);

                                this.pgbScanFiles.Maximum = Convert.ToInt32(objFileColl.Count());
                                this.pgbScanFiles.Minimum = 0;
                                this.pgbScanFiles.Value = this.pgbScanFiles.Minimum;
                                foreach (string sPathFile in objFileColl)
                                {

                                    if (objCallbackByFile != null)
                                    {
                                        StringBuilder objContentBuilder = ExecuteByFile(sPathFile, objEncoding, objCallbackByLine, true);
                                        objCallbackByFile(sPathFile, objContentBuilder, gobjDataConfig, gobjOptionConfig);
                                    }
                                    else
                                    {
                                        ExecuteByFile(sPathFile, objEncoding, objCallbackByLine);
                                    }
                                    this.pgbScanFiles.Value += 1;
                                    Application.DoEvents();
                                }
                                this.pgbScanFiles.Value = this.pgbScanFiles.Maximum;
                                this.pgbScanFiles.Value = this.pgbScanFiles.Minimum;
                            }
                        }
                        break;
                    }
                case "tabCorpusPageContent":
                    {

                        string content = this.tbCorpusContent.Text;
                        if (objCallbackByFile != null)
                        {
                            StringBuilder objContentBuilder = ExecuteByContent(content, objCallbackByLine, true);
                            objCallbackByFile(String.Empty,objContentBuilder, gobjDataConfig, gobjOptionConfig);
                        }
                        else
                        {
                            ExecuteByContent(content, objCallbackByLine);
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            AppendText(String.Format("完成，共用时{0}秒。", (DateTime.Now - dtStartTime).ToString()));
        }
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

                                        if (String.IsNullOrEmpty(keyword)) return;
                                        WSR_Forget_Core.KeyItem.KeyItemDAL.UpdateKeyItemColl(keyword, objConvertWordColl, new WSR_Forget_Core.Memory.OffsetWeightMDL(0, frequency));
                                        dConvertTotalOffset += frequency;
                                    }
                                }

                            }
                            this.pgbScanLines.Value = this.pgbScanLines.Maximum;
                            this.pgbScanLines.Value = this.pgbScanLines.Minimum;

                        }

                        gobjDataConfig.KeyWord.BasicKeyWordColl = Common.WordLibDAL.FixWordColl(objConvertWordColl,dConvertTotalOffset);

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

                                        if (String.IsNullOrEmpty(keyword)) return;
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
    }
}
