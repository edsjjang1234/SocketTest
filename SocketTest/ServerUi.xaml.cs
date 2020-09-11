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
        
        }

        private void ServerStartBtn_Click(object sender, RoutedEventArgs e)
        {
            //sSocket = new ServerSocket();
            sSocket = new ServerSocket( );
            sSocket.StartServer();
            ServerSocket.NickNameEvent += new AddNickNameEventHandler(this.SetNickName);
        }

         

        private void SetNickName(string nickName)
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

        public void SetMessage(string message)
        {
            try
            {
                viewTxt.AppendText(message + "\n");
                
                //viewTxt.  
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {             
            ServerSocket.ServerClose();
            Environment.Exit(0);
        }
    }
}
