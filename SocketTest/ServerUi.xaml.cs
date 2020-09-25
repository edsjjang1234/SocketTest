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
using WriteLogLib;

namespace SocketTest
{
    
    public partial class MainWindow : Window
    { 
        ServerSocket sSocket; 

        public MainWindow()
        { 
            InitializeComponent();
            serverStartBtn.Click += ServerStartBtn_Click1;           
        }
        
        private void ServerStartBtn_Click1(object sender, RoutedEventArgs e)
        {
            try
            { 
                sSocket = new ServerSocket();
                sSocket.OnMessageReceived += Client_OnMessageReceived;
                sSocket.OnAccepted += (s, ev) => Dispatcher.Invoke(() => UpdateList());
                sSocket.OnDisconnected += (s, ev) => Dispatcher.Invoke(() => UpdateList());
                sSocket.StartServer(); 
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void UpdateList()
        {
            try
            {
                nicListBox.ItemsSource = null;
                nicListBox.ItemsSource = sSocket.ClientIDs;
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
        
        private void Client_OnMessageReceived(object sender, string e)
        {
            //람다식
            Dispatcher.Invoke(() => viewTxt.AppendText($"{e}\r"));
        } 
    }
}
