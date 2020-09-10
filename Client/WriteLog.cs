using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class WriteLog
    {
        /// <summary>
        /// 로그생성
        /// </summary>
        /// <param name="log"></param>
        public static void WriteSetLog(string log)
        {
            try
            {
                StreamWriter writer;
                writer = File.AppendText(@"C:\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");         //Text File이 저장될 위치(파일명)                                                                                                        
                writer.WriteLine("[" + DateTime.Now + log);    //저장될 string
                writer.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
