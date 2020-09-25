using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using WriteLogLib;

namespace Client
{
    public class ClientUiViewModel : BindableBase
    {
        public DelegateCommand<object> connectionCommand { get; private set; }
        public DelegateCommand<object> sendCommand { get; private set; }

        public ClientUiViewModel()
        {
            connectionCommand = new DelegateCommand<object>(Connection);
            sendCommand = new DelegateCommand<object>(MessageSend);
            ClientList = new ObservableCollection<string>(); 
        }
       
        private ObservableCollection<string> _ClientList;

        public ObservableCollection<string> ClientList
        {
            get => _ClientList; set => SetProperty(ref _ClientList, value);
        }

        public string SendText { get; set; }
        
        public string IpText { get; set; }
        public string Port { get; set; }
        public string NickName { get; set; }
       
        
         
        SimpleTcpClient client;
        private void Connection(object obj)
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
                    if (int.TryParse(Port, out int PortNo) == false)
                    {
                        MessageBox.Show("포트 숫자 아님"); return;
                    }

                    if (NickName.Trim().Any(x => char.IsWhiteSpace(x)))
                    {
                        MessageBox.Show("닉네임 공백 있음"); return;
                    }
                    
                    if (IPAddress.TryParse(IpText, out IPAddress Ip) == false)
                    {
                        MessageBox.Show("IP 형식 아님"); return;
                    }
                     
                    client = new SimpleTcpClient(Ip.ToString(), PortNo, NickName);
                    client.OnMessageReceived += Client_OnMessageReceived;
                    client.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MessageSend(object obj)
        {
            try
            {                
                if (String.IsNullOrWhiteSpace(SendText) == false)
                    client.SendMessage(SendText);
               
                //sendTxt.Clear();
                //sendTxt.Focus();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
        //리시브 데이터 UI에 업데이트
        private void Client_OnMessageReceived(object sender, string e)
        {
            //Dispatcher.Invoke
            //ClientList.Add(e);
            //_reciveText = e;
            //List.Add(e);
            // Dispatcher.Invo List.Add(e);
            //람다식
            Application.Current.Dispatcher.Invoke(() => ClientList.Add(e));
        }


    }
}
