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
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    { 
        ServerSocket sSocket;

        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void ServerStartBtn_Click(object sender, RoutedEventArgs e)
        {
            sSocket = new ServerSocket(this);
            sSocket.StartServer();
        }
 

        private void Window_Closed(object sender, EventArgs e)
        {             
            ServerSocket.ServerClose();
            Environment.Exit(0);
        }
    }
}
