using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteLogLib
{
    public class WriteLog
    {
        public static void WriteLogger(string log)
        {
            string filePath = @"D:\Log\" + DateTime.Now.ToString("yyyyMMdd") + "_Log.txt";
            string DirPath = @"D:\Log";
            string logStr = string.Empty;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fl = new FileInfo(filePath);
            StreamWriter sw;

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath); //디렉토리 경로에 폴더 없으면 생성
                if (!fl.Exists)
                {
                    sw = new StreamWriter(filePath);
                    logStr = string.Format("{0} : {1}", DateTime.Now, log);
                    sw.WriteLine(logStr);
                    sw.Close();
                    sw.Dispose();
                }
                else
                {
                    sw = File.AppendText(filePath);
                    logStr = string.Format("{0} : {1}", DateTime.Now, log);
                    sw.WriteLine(logStr);
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception e)
            {

            }
        }

    }
}

