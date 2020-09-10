using Newtonsoft.Json;
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
using SocketJsonLib;
namespace Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientSocket cSocket;
         
        public MainWindow()
        {
            InitializeComponent();
            cSocket = new ClientSocket(this);
             
            WriteLog.WriteSetLog("클라이언트 실행");
        }

        private void ConnBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if(ipTxt.Text.Length>0 && portTxt.Text.Length > 0 && nickNameTxt.Text.Length > 0)
                //{
                //cSocket.ServerConnect(ipTxt.Text, Convert.ToInt32(portTxt.Text));
                cSocket.ServerConnect("127.0.0.1", 9800);
                //}
                //else
                //{
                //    MessageBox.Show("서버 IP, 서버 PORT, 사용자명을 입력하세요");
                //}
            }
            catch(Exception ex)
            {
                WriteLog.WriteSetLog(ex.ToString());
            }
        }
         
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            //cSocket = new ClientSocket(this);
            try
            {
                string message = JsonConvert.SerializeObject(SocketJsonLib.SocketJson.AddJson(nickNameTxt.Text, sendTxt.Text));
                
                cSocket.BeginSend(message);
 
                WriteLog.WriteSetLog("메세지 (" + sendTxt.Text + ") 전송");
                sendTxt.Clear();
                sendTxt.Focus();
            }
            catch (Exception ex)
            {
                WriteLog.WriteSetLog(ex.ToString());
            }
        } 

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                cSocket.ClientClose();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                WriteLog.WriteSetLog(ex.ToString());
            }
        }
    }
}
