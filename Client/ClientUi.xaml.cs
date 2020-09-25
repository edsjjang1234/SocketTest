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
using JsonPack;
using WriteLogLib;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //MainWindow 생성자에서 client를 생성해서
        //종료시까지 유지
        //접속 Connet시 접속 전달받고
        //Distconnect 구현
        //Connect/Disconnect 교차 실행가능하게 

        SimpleTcpClient client;

        public MainWindow()
        {
            InitializeComponent();
            connBtn.Click += ConnBtn_Click;
            sendBtn.Click += SendBtn_Click;
            sendTxt.KeyDown += SendTxt_KeyDown;
            this.Closed += (s, e) => {  };// MainWindow_Closed;
            
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ConnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                MessageBox.Show("접속중");
            }
            else
            {
                try
                {//닉네임 공백 체크
                    //IP가 IP형식이 맞는지...
                    if (int.TryParse(portTxt.Text, out int port) == false)
                    {
                        MessageBox.Show("포트 숫자 아님"); return;
                    }

                    //if (String.IsNullOrWhiteSpace(nickNameTxt.Text))
                    //{
                    //    MessageBox.Show("닉네임 공백 있음"); return;
                    //}

                    //if(IPAddress.TryParse(ipTxt.Text,out IPAddress ip) == false)
                    //{
                    //    MessageBox.Show("IP 형식 아님"); return;
                    //}
                        
                    client = new SimpleTcpClient(ipTxt.Text, port, nickNameTxt.Text);
                    client.OnMessageReceived += Client_OnMessageReceived;
                    client.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
            }
        }

        private void Client_OnMessageReceived(object sender, string e)
        {
            //람다식
            Dispatcher.Invoke(() => viewTxt.AppendText($"{e}\r"));
        }

        private void SendTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn_Click(null, null);
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(sendTxt.Text) == false)
                    client.SendMessage(sendTxt.Text);
                
                sendTxt.Clear();
                sendTxt.Focus();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }


        //ClientSocket cSocket;
        //public static string NickName = string.Empty;

        //public MainWindow()
        //{
        //    InitializeComponent();

        //    sendBtn.Click += new RoutedEventHandler(SendBtn_Click);
        //    sendTxt.KeyDown += new KeyEventHandler(SendTxt_KeyDown);            
        //    connBtn.Click += new RoutedEventHandler(ConnBtn_Click);
        //    Closed += new EventHandler(Window_Closed);
        //    WriteLog.WriteLogger("클라이언트 실행!!");
        //}

        //private void SendTxt_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Key == Key.Enter)
        //        {
        //            if (sendTxt.Text.Length > 0)
        //            {
        //                sendMessage();
        //            }

        //            sendTxt.Clear();
        //            sendTxt.Focus();
        //        }
        //    }catch(Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }                     
        //} 

        //private void ConnBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        //if(ipTxt.Text.Length>0 && portTxt.Text.Length > 0 && nickNameTxt.Text.Length > 0)
        //        //{

        //        NickName = nickNameTxt.Text;
        //        cSocket = new ClientSocket();
        //        //cSocket.ServerConnect("127.0.0.1", 9800);
        //        cSocket.ServerConnect(ipTxt.Text, Convert.ToInt32(portTxt.Text));
        //        ClientSocket.MessageEvent += new AddMessageEventHandler(this.SetMessage);
        //        //}
        //        //else
        //        //{
        //        //    MessageBox.Show("서버 IP, 서버 PORT, 사용자명을 입력하세요");
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

        //private void SendBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (sendTxt.Text.Length > 0)
        //        {
        //            sendMessage();
        //        }

        //        sendTxt.Clear();
        //        sendTxt.Focus();
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

        //private void SetMessage(string message)
        //{
        //    try
        //    {
        //        viewTxt.AppendText(message + "\n");
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

        //private void sendMessage()
        //{
        //    try
        //    { 
        //        var p = new MessagePacket() { NickName = nickNameTxt.Text, Message = sendTxt.Text };

        //        cSocket.BeginSend(p.ToString());
        //    }
        //    catch(Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}


        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        cSocket.ClientClose();
        //        Environment.Exit(0);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

    }
}
