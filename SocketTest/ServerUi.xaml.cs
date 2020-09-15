using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SocketTest
{
    
    public partial class MainWindow : Window
    { 
        ServerSocket sSocket; 

        public MainWindow()
        { 
            InitializeComponent();
            serverStartBtn.Click += new RoutedEventHandler(ServerStartBtn_Click1);
        }

        private void ServerStartBtn_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                sSocket = new ServerSocket();
                sSocket.StartServer();
                ServerSocket.AddNickNameEvent += new AddNickNameEventHandler(this.SetNickName);
                ServerSocket.MessageEvent += new AddMessageEventHandler(this.SetMessage);
                ServerSocket.OutNickNameEvent += new OutNickNameEventHandler(this.outNickName);
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
         
        /// <summary>
        /// 접속한 닉네임 확인하여 접속자 리스트에 뿌려줌.
        /// </summary>
        /// <param name="nickName"></param>
        private void SetNickName(string nickName)
        {
            try
            {
                int chk = 0;
                for (int i = 0; i < nicListBox.Items.Count; i++)
                {
                    if (nickName == nicListBox.Items[i].ToString())
                    {
                        chk = 1;
                    }
                }

                if (chk == 0)
                {
                    nicListBox.Items.Add(nickName);
                }
                chk = 0;
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
        
        /// <summary>
        /// 접속 종료 닉네임 확인하여 접속자 리스트에서 제거
        /// </summary>
        /// <param name="nickName"></param>
        private void outNickName(string nickName)
        {
            try
            {
                for (int i = 0; i < nicListBox.Items.Count; i++)
                {
                    if (nickName == nicListBox.Items[i].ToString())
                        nicListBox.Items.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
        
        /// <summary>
        /// 전송 메세지 UI처리
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(string message)
        {
            try
            {
                viewTxt.AppendText(message + "\n");          
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                ServerSocket.ServerClose();
                Environment.Exit(0);
            }
            catch(Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
    }
}
