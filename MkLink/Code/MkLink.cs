using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenHelper.Common;
using TenHelper.Windows;

namespace MkLink.Code
{
    class MkLink
    {
        public static ResultInfo MappingMode(string option, string link, string target)
        {
            ResultInfo result = DataDeal(ref option, ref link, ref target);
            if (!result.IsSuccess) return result;

            try
            {
                if (File.Exists(target)) File.Move(target, link);
                else if (Directory.Exists(target)) DirectoryExtension.Move(target, link);
                else return new ResultInfo(false, "请确保文件或目录存在");

                Cmd cmd = new Cmd();
                result = cmd.Start(MkLinkCommand, option, target, link);
                cmd.Close();
                return result;
            }
            catch (Exception e)
            {
                return new ResultInfo(false, e.Message);
            }
        }

        public static ResultInfo LinkMode(string option, string link, string target)
        {
            ResultInfo result = DataDeal(ref option, ref link, ref target);
            if (result.IsSuccess)
            {
                Cmd cmd = new Cmd();
                result = cmd.Start(MkLinkCommand, option, link, target);
                if (result.IsSuccess)
                {
                    result.SetResult(true, "执行成功");
                }
                cmd.Close();
            }
            return result;
        }

        public static ResultInfo Mk(string mode, string option, string link, string target)
        {
            ResultInfo result = new ResultInfo(false, "未知错误");
            switch (mode.Trim())
            {
                case "Mapping":
                    result = MappingMode(option, link, target);
                    break;
                case "Link":
                    result = LinkMode(option, link, target);
                    break;
                default:
                    break;

            }
            return result;
        }

        public static ResultInfo BatchMk(string filePath)
        {
            ResultInfo result = new ResultInfo(false, "未知错误");
            if (File.Exists(filePath))
            {
                Queue<ResultInfo> resultQueue = new Queue<ResultInfo>();
                StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"));
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ResultInfo res = new ResultInfo(false, "未知错误");
                    string[] strArray = line.Split('|').ToArray();
                    string target = "";
                    string link = "";
                    string option = "";
                    string mode = "Link";
                    if (strArray.Length < 2) { res.SetResult(false, "命令格式不对"); }
                    else
                    {
                        link = strArray[0]; target = strArray[1];
                        if (strArray.Length > 2) { option = strArray[2]; }
                        if (strArray.Length > 3) { mode = strArray[3]; }
                    }

                    res = Mk(mode, option, link, target);
                    resultQueue.Enqueue(res);
                }
                sr.Close();

                WriterLog(resultQueue);

                bool isSuccess = true;
                string message = "执行结束";
                foreach (ResultInfo res in resultQueue)
                {
                    isSuccess = isSuccess && res.IsSuccess;
                }
                if (!isSuccess) message += "，存在命令执行不成功";
                message += "\n请到日志查看结果";

                result.SetResult(isSuccess, message);
            }
            else
            {
                result.SetResult(false, "找不到指定文件");
            }
            return result;
        }

        private static void WriterLog(Queue<ResultInfo> resultQueue)
        {
            string logDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log";
            if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

            string logFile = logDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-DD") + ".txt";

            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine();
            sw.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
            foreach (ResultInfo res in resultQueue)
            {
                sw.WriteLine(res.IsSuccess + "  " + res.Message.Replace("句柄无效。", "").Replace("\r\n", ""));
            }
            sw.Close();
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="option"></param>
        /// <param name="link"></param>
        /// <param name="target"></param>
        /// <returns>数据是否正确</returns>
        private static ResultInfo DataDeal(ref string option, ref string link, ref string target)
        {
            option = option.Trim();
            link = link.Trim();
            target = target.Trim();

            if (String.IsNullOrEmpty(link) || String.IsNullOrEmpty(target)) return new ResultInfo(false, "路径不能为空");

            link = Path.GetFullPath(link.Trim());
            target = Path.GetFullPath(target.Trim());

            if (File.Exists(target) && option != "h")
            {
                return new ResultInfo(false, "文件只能使用硬链接（h）");
            }
            if (Directory.Exists(target) && option != "d" && option != "j")
            {
                return new ResultInfo(false, "目录只能使用符号链接（d）或 软链接（j）");
            }

            return new ResultInfo(true, "");
        }

        private static string MkLinkCommand = "mklink /{0} \"{1}\" \"{2}\"";
    }
}
