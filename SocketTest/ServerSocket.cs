using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading; 
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Data;
using JsonPack;
using WriteLogLib;

namespace SocketTest
{ 
    public class ClientInfo
    {
        public ClientInfo(Socket socket)
        {
            Socket = socket;
            IP = socket.RemoteEndPoint.ToString();
        }

        public Socket Socket { get; set; }
        public string NickName { get; set; }
        public string IP { get; set; }
    }

    public class ServerSocket
    {
        //메세지 이벤트
        public event EventHandler<string> OnMessageReceived
        {
            add { _OnMessageReceived += value; }
            remove { _OnMessageReceived -= value; }
        }
        private EventHandler<string> _OnMessageReceived;

        public event EventHandler OnAccepted;

        public event EventHandler OnDisconnected;
        

        public IEnumerable<string> ClientIDs
        {
            get { return _ClientList.Keys; }
        }

        Dictionary<string, ClientInfo> _ClientList = new Dictionary<string, ClientInfo>();

        private static Socket _Server; //서버 소켓
         

        public void StartServer()
        {
            _Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //소켓 초기화
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9800);  //IP 및 포트  설정
            _Server.Bind(ipep);  //서버 바인딩
            _Server.Listen(20); 
            SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //비동기 서버 이벤트
            args.Completed += Accept_Completed; //이벤트 발생시 Accept_Completed 함수 실행
            _Server.AcceptAsync(args);

            _OnMessageReceived?.Invoke(this,"서버시작");
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                Socket clientSocket = e.AcceptSocket;
                
                if (clientSocket != null)
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    byte[] szData = new byte[1024];
                    args.SetBuffer(szData, 0, 1024);    //수신 버퍼 할당
                    args.UserToken = clientSocket;
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);

                    var info = new ClientInfo(clientSocket);
                    if (_ClientList.ContainsKey(info.IP))
                        throw new Exception("asdf");

                    _ClientList.Add(info.IP, info);
          
                    clientSocket.ReceiveAsync(args);
                    OnAccepted?.Invoke(this, EventArgs.Empty);
                }
                 
                e.AcceptSocket = null;
                clientSocket.AcceptAsync(e); //요청 소켓 처리 후 수락 대기 상태 변경

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket clientSocket = (Socket)sender;
           
            try
            {
                var key = clientSocket.RemoteEndPoint.ToString();
                if (clientSocket.Connected && e.BytesTransferred > 0)    //클라이언트 소켓이 연결되어있고, 전송 바이트 확인
                {
                    var szData = e.Buffer.Take(e.BytesTransferred).ToArray();   //수신 데이터
                    string text = Encoding.UTF8.GetString(szData); 
                    var packet = MessagePacket.Parse(text); //json 파싱

                    _ClientList[key].NickName = packet.NickName;
                    
                    _OnMessageReceived?.Invoke(this, packet.NickName + " : " + packet.Message);

                    MessageSend(packet.NickName.ToString() + " : " + packet.Message.ToString());

                    clientSocket.ReceiveAsync(e);
                }
                else
                {
                    if (_ClientList.ContainsKey(key))
                    {
                        var nickName = _ClientList[key].NickName;
                        _ClientList.Remove(key);

                        MessageSend(nickName + " : " + "의 연결이 끊어졌습니다.");
                        
                        _OnMessageReceived?.Invoke(this, nickName + " : " + "의 연결이 끊어졌습니다.");
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        } 
        
        private void MessageSend(string text)
        {
            try
            {
                byte[] sendMessage = Encoding.UTF8.GetBytes(text);

                foreach (var v in _ClientList.Values)
                    v.Socket.Send(sendMessage, sendMessage.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }

































        //public void StartServer()
        //{ 
        //    try
        //    { 
        //        //m_ClientSocket = new List<Socket>();    //클라이언트 소켓 리스트 초기화
        //        //nickNameDt = new DataTable();   //접속 클라이언트 IP NickName 수집하기 위한 DataTable
        //        //nickNameDt.Columns.Add("IP", typeof(string));
        //        //nickNameDt.Columns.Add("NICKNAME", typeof(string));

        //        m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //소켓 초기화
        //        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9800);  //IP 및 포트  설정
        //        m_ServerSocket.Bind(ipep);  //서버 바인딩
        //        m_ServerSocket.Listen(20);
        //        SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //비동기 서버 이벤트
        //        args.Completed += Accept_Completed; //이벤트 발생시 Accept_Completed 함수 실행
        //        m_ServerSocket.AcceptAsync(args);

        //        Dispatcher.BeginInvoke(new Action(() => MessageEvent("서버 실행 성공!!")));

        //        WriteLog.WriteLogger("서버 실행");

        //    }
        //    catch(Exception ex)
        //    {
        //        Dispatcher.BeginInvoke(new Action(() => MessageEvent("서버 실행 실패!!")));

        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

        //private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        //{
        //    try
        //    { 
        //        Socket clientSocket = e.AcceptSocket;

        //        _ClientList.Add(clientSocket, new ClientInfo() { IP = clientSocket.RemoteEndPoint.ToString() });

        //        if (m_ClientSocket != null)
        //        {
        //            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        //            szData = new byte[1024];
        //            args.SetBuffer(szData, 0, 1024);    //수신 버퍼 할당
        //            args.UserToken = m_ClientSocket;
        //            args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
        //            clientSocket.ReceiveAsync(args);
        //        }
        //        e.AcceptSocket = null;


        //        m_ServerSocket.AcceptAsync(e); //요청 소켓 처리 후 수락 대기 상태 변경

        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //}

        //private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        //{
        //    Socket ClientSocket = (Socket)sender;

        //    try
        //    {  
        //        if (ClientSocket.Connected && e.BytesTransferred > 0)    //클라이언트 소켓이 연결되어있고, 전송 바이트 확인
        //        { 
        //            var szData = e.Buffer.Take(e.BytesTransferred).ToArray();   //수신 데이터
        //            string text = Encoding.UTF8.GetString(szData);


        //            var jsonParser = MessagePacket.Parse(text); //json 파싱
        //            nickName = jsonParser.NickName.ToString();


        //            //닉네임과 접속 IP PORT 수집
        //            DataRow[] resultRows;
        //            resultRows = nickNameDt.Select("[IP] = '" + ClientSocket.RemoteEndPoint.ToString() + "'");

        //            if (resultRows.Length == 0)
        //            {
        //                nickNameDt.Rows.Add(ClientSocket.RemoteEndPoint.ToString(), nickName);
        //            }

        //            szData = Encoding.UTF8.GetBytes(jsonParser.NickName.ToString() + " : " + jsonParser.Message.ToString());

        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                AddNickNameEvent(jsonParser.NickName.ToString());//닉네임 이벤트
        //                MessageEvent(jsonParser.NickName.ToString() + " : " + jsonParser.Message.ToString());//메세지 이벤트
        //            }));

        //            for (int i = 0; i < m_ClientSocket.Count; i++)
        //            {
        //                m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
        //            }

        //            ClientSocket.ReceiveAsync(e);                   
        //        }
        //        else
        //        {
        //            //접속 종료 IP PORT 번호로 닉네임 찾아서 변수에 저장
        //            DataRow[] resultRows;
        //            resultRows = nickNameDt.Select("[IP] = '" + ClientSocket.RemoteEndPoint.ToString() + "'");

        //            string outNickName = string.Empty;
        //            if (resultRows.Length > 0)
        //            {
        //                outNickName = resultRows[0].ItemArray[1].ToString();

        //                //찾은 닉네임 Rowindex 확인하여 DataTable에서 삭제
        //                int rowIndex = nickNameDt.Rows.IndexOf(resultRows[0]);
        //                nickNameDt.Rows[rowIndex].Delete();
        //                nickNameDt.AcceptChanges();
        //            }

        //            ClientSocket.Disconnect(false);
        //            m_ClientSocket.Remove(ClientSocket);

        //            szData = Encoding.UTF8.GetBytes(outNickName + " : " + "의 연결이 끊어졌습니다.");
        //            for (int i = 0; i < m_ClientSocket.Count; i++)
        //            {
        //                m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
        //            }

        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                MessageEvent(outNickName + " : " + "의 연결이 끊어졌습니다.");
        //                OutNickNameEvent(outNickName);
        //            }));

        //        }
        //    }catch(Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    }
        //} 

        //public static void ServerClose()
        //{
        //    try
        //    {
        //        if (m_ServerSocket != null)
        //            //m_ServerSocket.Shutdown(SocketShutdown.Both);                  
        //            m_ServerSocket.Close(); 

        //        if (m_ClientSocket != null)                    
        //            m_ClientSocket.Clear(); 

        //    }   
        //    catch (Exception ex)
        //    {
        //        WriteLog.WriteLogger(ex.ToString());
        //    } 
        //} 
    }
}
