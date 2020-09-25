using JsonPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WriteLogLib;

namespace Client
{
    class SimpleTcpClient
    {
        public event EventHandler<string> OnMessageReceived
        {
            add { _OnMessageReceived += value; }
            remove { _OnMessageReceived -= value; }
        }
        private EventHandler<string> _OnMessageReceived;


        public string NickName { get; private set; }

        public string ServerIP { get; private set; }

        public int ServerPort { get; private set; }

        public SimpleTcpClient(string serverIp, int port, string nickName)
        {
            this.ServerIP = serverIp;
            this.ServerPort = port;
            this.NickName = nickName;
        }

        private Socket _Client = null;
        private byte[] _RecvBuffer;
        private const int MAXSIZE = 4096;

        public void Connect()
        {
            try
            {
                _RecvBuffer = new byte[MAXSIZE];
                _Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Client.Connect(ServerIP, ServerPort);
                SendMessage("접속");
                BeginReceive();

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
                throw ex;
            }
        }

        private void BeginReceive()
        {
            _Client.BeginReceive(_RecvBuffer, 0, _RecvBuffer.Length, SocketFlags.None, ReceiveCallBack, _Client);
        }
        
        private void ReceiveCallBack(IAsyncResult IAR)
        {
            try
            {
                Socket tempSocket = (Socket)IAR.AsyncState;
                if (tempSocket.Connected)
                {

                    int rReadSize = tempSocket.EndReceive(IAR);

                    if (rReadSize != 0)
                    {
                        string sData = Encoding.UTF8.GetString(_RecvBuffer, 0, rReadSize);
                        _OnMessageReceived?.Invoke(this, sData);                    
                    }

                    BeginReceive();
                }
            }
            catch (Exception ex)
            {
                _OnMessageReceived?.Invoke(this, ex.Message);
            }
        }
        

        public void SendMessage(string text)
        {
            try
            { 
                if (_Client.Connected)
                {
                    var packet = new MessagePacket() { NickName = NickName, Message = text };
                    byte[] buffer = Encoding.UTF8.GetBytes(packet.ToString());
                    _Client.Send(buffer);
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
    }
}
