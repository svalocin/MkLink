using MyHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkLink.Code
{
    struct Result
    {
        public Result(bool isSuccess, string info)
        {
            IsSuccess = isSuccess;
            Info = info;
        }
        public bool IsSuccess;
        public string Info;
    }

    class MkLink
    {
        private static string MkLinkCommand = "mklink /{0} \"{1}\" \"{2}\"";
        private static string MoveCommand = "move /y \"{0}\" \"{1}\"";

        public static Result Mapping(string option, string link, string target)
        {
            Cmd cmd = new Cmd();
            Result result;
            if (cmd.Start(MoveCommand, target, link))
            {
                if (cmd.Start(MkLinkCommand, option, target, link))
                {
                    result = new Result(true, "执行成功");
                }
                else
                {
                    cmd.Start("move /y \"{0}\" \"{1}\"", link, target);
                    result = new Result(false, "链接失败");
                }
            }
            else
            {
                result = new Result(false, "移动文件失败");
            }
            cmd.Close();
            return result;
        }

        public static Result Link(string option, string link, string target)
        {
            Cmd cmd = new Cmd();
            Result result;
            if (cmd.Start(MkLinkCommand, option, link, target))
            {
                result = new Result(true, "执行成功");
            }
            else
            {
                result = new Result(false, "链接失败");
            }
            cmd.Close();
            return result;
        }

        public static Result Start(string mode, string option, string link, string target)
        {
            Result result = new Result(false, "未知错误");
            switch (mode)
            {
                case "Mapping":
                    result = Mapping(option, link, target);
                    break;
                case "Link":
                    result = Link(option, link, target);
                    break;
                default:
                    break;

            }
            return result;
        }

        public static Result BatchStart(string filePath)
        {
            Result result = new Result(false, "未知错误");
            if (File.Exists(filePath))
            {
                Queue<Result> resultQueue = new Queue<Result>();
                StreamReader sr = new StreamReader(filePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Result res = new Result(false, "未知错误");
                    string[] strArray = line.Split('|').ToArray();
                    string target = "";
                    string link = "";
                    string option = "d";
                    string mode = "Link";
                    if (strArray.Length < 2) { res.Info = "命令格式不对"; }
                    else
                    {
                        link = strArray[0]; target = strArray[1];
                        if (strArray.Length > 2) { option = strArray[2]; }
                        if (strArray.Length > 3) { mode = strArray[3]; }
                    }
                    res = Start(mode, option, link, target);
                    resultQueue.Enqueue(res);
                }
                sr.Close();

                WriterLog(resultQueue);

                result.IsSuccess = true;
                foreach (Result res in resultQueue)
                {
                    result.IsSuccess = result.IsSuccess && res.IsSuccess;
                }
                result.Info = "执行结束";
                if (!result.IsSuccess) result.Info += "，存在命令执行不成功";
                result.Info += "\n请到日志查看结果";
            }
            else
            {
                result.Info = "找不到指定文件";
            }
            return result;
        }

        private static void WriterLog(Queue<Result> resultQueue)
        {
            string logDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log";
            if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

            string logFile = logDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-DD") + ".txt";

            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine();
            sw.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
            foreach (Result res in resultQueue)
            {
                sw.WriteLine(res.IsSuccess + "  " + res.Info);
            }
            sw.Close();
        }
    }
}
