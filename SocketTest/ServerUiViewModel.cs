using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WriteLogLib;

namespace SocketTest
{
    public class ServerUiViewModel : BindableBase
    {
        public DelegateCommand<object> ServerStartCommand { get; private set; }
        public ServerUiViewModel()
        {
            ServerStartCommand = new DelegateCommand<object>(ServerStart);
            ServerList = new ObservableCollection<string>();
            ReceiveList = new ObservableCollection<string>();
        }

        public ObservableCollection<string> _ReceiveList;
        public ObservableCollection<string> ReceiveList
        {
            get => _ReceiveList;set => SetProperty(ref _ReceiveList, value);
        }

        public ObservableCollection<string> _ServerList;
        public ObservableCollection<string> ServerList
        {
            get => _ServerList; set => SetProperty(ref _ServerList, value);
        }

        ServerSocket sSocket;
        private void ServerStart(object obj)
        {
            try
            {
                sSocket = new ServerSocket();
                sSocket.OnMessageReceived += Client_OnMessageReceived;
                sSocket.OnAccepted += (s, ev) => UpdateList();
                sSocket.OnDisconnected += (s, ev) => UpdateList();
                //sSocket.OnAccepted += (s, ev) =>  Dispatcher.Invoke(() => UpdateList());
                //sSocket.OnDisconnected += (s, ev) => Dispatcher.Invoke(() => UpdateList());
                sSocket.StartServer();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void Client_OnMessageReceived(object sender, string e)
        {
            Application.Current.Dispatcher.Invoke(() => ReceiveList.Add(e));
            //람다식
            //Dispatcher.Invoke(() => viewTxt.AppendText($"{e}\r"));
        }
        private void UpdateList()
        {
            try
            { 
                Application.Current.Dispatcher.Invoke(() => ServerList.AddRange(sSocket.ClientIDs));
                //nicListBox.ItemsSource = null;
                //nicListBox.ItemsSource = sSocket.ClientIDs;
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
    }
}
