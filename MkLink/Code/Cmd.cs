using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper
{
    class Cmd
    {
        private Process process;

        public Cmd()
        {
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";             //要执行的程序名称 
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;     //可能接受来自调用程序的输入信息 
            process.StartInfo.RedirectStandardOutput = true;    //由调用程序获取输出信息 
            process.StartInfo.CreateNoWindow = true;            //不显示程序窗口 
        }

        ~Cmd()
        {
            Close();
        }

        public void Close()
        {
            process.Close();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="options">参数</param>
        /// <returns>指示命令是否成功执行</returns>
        public bool Start(string command, params object[] options)
        {
            command = string.Format(command, options);
            //启动程序
            process.Start();
            //输入命令
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
            //获取错误指示，0 为执行成功
            if (process.ExitCode == 0) return true;
            return false;
        }
    }
}
