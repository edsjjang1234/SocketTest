﻿using Newtonsoft.Json;
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
        public static string _nickName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            sendBtn.Click += new RoutedEventHandler(SendBtn_Click1);
            sendTxt.KeyDown += new KeyEventHandler(SendTxt_KeyDown);            
            connBtn.Click += new RoutedEventHandler(ConnBtn_Click1);

            WriteLog.WriteLogger("클라이언트 실행!!");
        }

        private void SendTxt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (sendTxt.Text.Length > 0)
                    {
                        sendMessage();
                    }
                     
                    sendTxt.Clear();
                    sendTxt.Focus();
                }
            }catch(Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }                     
        } 

        private void ConnBtn_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                //if(ipTxt.Text.Length>0 && portTxt.Text.Length > 0 && nickNameTxt.Text.Length > 0)
                //{
                //cSocket.ServerConnect(ipTxt.Text, Convert.ToInt32(portTxt.Text));
                _nickName = nickNameTxt.Text;
                cSocket = new ClientSocket();
                cSocket.ServerConnect("127.0.0.1", 9800);
                ClientSocket.MessageEvent += new AddMessageEventHandler(this.SetMessage);
                //}
                //else
                //{
                //    MessageBox.Show("서버 IP, 서버 PORT, 사용자명을 입력하세요");
                //}
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void SendBtn_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sendTxt.Text.Length > 0)
                {
                    sendMessage();
                }
               
                sendTxt.Clear();
                sendTxt.Focus();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
 
        private void SetMessage(string message)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    viewTxt.AppendText(message + "\n");
                });
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
 
        private void sendMessage()
        {
            try
            { 
                var p = new MessagePacket() { NickName = nickNameTxt.Text, Message = sendTxt.Text };
 
                cSocket.BeginSend(p.ToString());
            }
            catch(Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
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
                WriteLog.WriteLogger(ex.ToString());
            }
        }
 
    }
}
