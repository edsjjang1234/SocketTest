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
using System.Windows.Threading;
using SocketJsonLib;
namespace Client
{
    public class ClientSocket
    {
        MainWindow main = null;
        private Socket cSocket = null;
        private Socket cbSocket;
        private byte[] recvBuffer;
        private const int MAXSIZE = 4096;
        private string HOST = string.Empty;
        private int PORT;
         
        public ClientSocket(MainWindow main)
        {
            this.main = main;
        }

        public void ServerConnect(string HOST, int PORT)
        {
            try
            {
                recvBuffer = new byte[MAXSIZE];
                cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //clientSocket.BeginConnect(HOST, PORT, new AsyncCallback(ConnectCallBack), clientSocket);
                cSocket.BeginConnect("127.0.0.1", 9800, new AsyncCallback(ConnectCallBack), cSocket);

            }
            catch (Exception ex)
            {
               // WriteLog.WriteSetLog(ex.ToString());
            }
        }

        private void ConnectCallBack(IAsyncResult IAR)
        {
            try
            {

                string message = string.Empty;
                Socket tempSocket = (Socket)IAR.AsyncState;
                IPEndPoint ipep = (IPEndPoint)tempSocket.RemoteEndPoint;

                // this.BeginSend("접속");

                main.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    //message = JsonConvert.SerializeObject(ClientSetJson.AddJson(main.nickNameTxt.Text, main.sendTxt.Text));
                    message = JsonConvert.SerializeObject(SocketJsonLib.SocketJson.AddJson(main.nickNameTxt.Text, "접속"));
                    this.BeginSend(message);
                    //this.BeginSend(main.nickNameTxt.Text + "접속");
                });
                 
                    //WriteLog.WriteSetLog("서버 접속");

                tempSocket.EndConnect(IAR);
                cbSocket = tempSocket;
                cbSocket.BeginReceive(this.recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ReceiveCallBack, cbSocket);
            }
            catch (Exception ex)
            {
                //WriteLog.WriteSetLog(ex.ToString());
            }
        }

        private void ReceiveCallBack(IAsyncResult IAR)
        {
            try
            {

                Socket tempSocket = (Socket)IAR.AsyncState;
                int rReadSize = tempSocket.EndReceive(IAR);

                if (rReadSize != 0)
                {
                    string sData = Encoding.UTF8.GetString(recvBuffer, 0, rReadSize);
                    string text = sData.Replace("\0", "").Trim();
                    ShowMeaasge(text);
                    //WriteLog.WriteSetLog("메세지 (" + text + ") 받음");
                }

                Receive();
            }
            catch (Exception ex)
            {
                //WriteLog.WriteSetLog(ex.ToString());
            }
        }

        public void Receive()
        {
            try
            {
                cbSocket.BeginReceive(this.recvBuffer, 0, recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), cbSocket);
            }
            catch (Exception ex)
            {
                //WriteLog.WriteSetLog(ex.ToString());
            }
        }
              

        public void BeginSend(string message)
        {
            try
            {
                if (cSocket.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                   
                    cSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), message);
                    
                }
            }
            catch (Exception ex)
            {
                //WriteLog.WriteSetLog(ex.ToString());
            }
        }

        private void SendCallBack(IAsyncResult IAR)
        {
            try
            {
                string message = (string)IAR.AsyncState;
             
            }
            catch (Exception ex)
            {
                //WriteLog.WriteSetLog(ex.ToString());
            }
        }

        private void ShowMeaasge(string message)
        {
            try
            {
                main.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    main.viewTxt.AppendText(message + "\n");
                });
            }
            catch (Exception ex)
            {
               // WriteLog.WriteSetLog(ex.ToString());
            }
        }

        public void ClientClose()
        {
            cSocket.Close();
            cbSocket.Close();
        }
    } 
}
