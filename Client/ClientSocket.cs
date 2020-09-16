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
    public delegate void AddMessageEventHandler(string massage);

    public class ClientSocket : MainWindow
    {
        public static AddMessageEventHandler MessageEvent;
        
        private Socket cSocket = null;
        private Socket cbSocket;
        private byte[] recvBuffer;
        private const int MAXSIZE = 4096; 
        
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
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void ConnectCallBack(IAsyncResult IAR)
        {
            try
            { 
                string message = string.Empty;
                Socket tempSocket = (Socket)IAR.AsyncState;
                IPEndPoint ipep = (IPEndPoint)tempSocket.RemoteEndPoint;
                var json = new MessagePacket() { NickName = MainWindow.NickName, Message = "접속" };
                BeginSend(json.ToString()); 

                tempSocket.EndConnect(IAR);
                cbSocket = tempSocket;
                cbSocket.BeginReceive(this.recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ReceiveCallBack, cbSocket);
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void ReceiveCallBack(IAsyncResult IAR)
        {
            try
            {
                Socket tempSocket = (Socket)IAR.AsyncState;
                if (tempSocket.Connected)
                { 
                    int rReadSize = tempSocket.EndReceive(IAR);
                    //string text = string.Empty;
                    if (rReadSize != 0)
                    {
                        string sData = Encoding.UTF8.GetString(recvBuffer, 0, rReadSize);
                        string text = sData.Replace("\0", "").Trim();

                        Dispatcher.BeginInvoke(new Action(() => MessageEvent(text)));

                    }

                    Receive();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
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
                WriteLog.WriteLogger(ex.ToString());
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
                WriteLog.WriteLogger(ex.ToString());
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
                WriteLog.WriteLogger(ex.ToString());
            }
        } 

        public void ClientClose()
        {
            try
            {  
                if (cSocket != null)
                    //cSocket.Shutdown(SocketShutdown.Both);
                    cSocket.Close();
                if (cbSocket != null)
                    //cbSocket.Shutdown(SocketShutdown.Both);
                    cbSocket.Close();
            }
            catch(Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
    } 
}
