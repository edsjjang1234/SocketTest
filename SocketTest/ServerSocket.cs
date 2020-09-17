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
    public delegate void AddNickNameEventHandler(string nickName);
    public delegate void OutNickNameEventHandler(string nickName);
    public delegate void AddMessageEventHandler(string massage);

    public class ServerSocket : MainWindow
    {
        public static AddNickNameEventHandler AddNickNameEvent;
        public static AddMessageEventHandler MessageEvent;
        public static OutNickNameEventHandler OutNickNameEvent;

        private static Socket m_ServerSocket; //서버 소켓
        private static List<Socket> m_ClientSocket; //클라이언트 소켓 리스트
        private byte[] szData;  //클라이언트 데이터 버퍼 사이즈        
        string nickName = string.Empty;
        DataTable nickNameDt;
        public void StartServer()
        { 
            try
            { 
                m_ClientSocket = new List<Socket>();    //클라이언트 소켓 리스트 초기화
                nickNameDt = new DataTable();
                nickNameDt.Columns.Add("IP", typeof(string));
                nickNameDt.Columns.Add("NICKNAME", typeof(string));

                m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //소켓 초기화
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9800);  //IP 및 포트  설정
                m_ServerSocket.Bind(ipep);  //서버 바인딩
                m_ServerSocket.Listen(20);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //비동기 서버 이벤트
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed); //이벤트 발생시 Accept_Completed 함수 실행
                m_ServerSocket.AcceptAsync(args);

                Dispatcher.BeginInvoke(new Action(() => MessageEvent("서버 실행 성공!!")));
 
                WriteLog.WriteLogger("서버 실행");

            }
            catch(Exception ex)
            {
                Dispatcher.BeginInvoke(new Action(() => MessageEvent("서버 실행 실패!!")));
                
                WriteLog.WriteLogger(ex.ToString());
            }
        }
         
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            { 
                Socket ClientSocket = e.AcceptSocket;
                m_ClientSocket.Add(ClientSocket);   //접속 요청 클라이언트 소켓 수락 후 리스트에 담음.

                if (m_ClientSocket != null)
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    szData = new byte[1024];
                    args.SetBuffer(szData, 0, 1024);    //수신 버퍼 할당
                    args.UserToken = m_ClientSocket;
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                    ClientSocket.ReceiveAsync(args);
                }
                e.AcceptSocket = null;

                 
                m_ServerSocket.AcceptAsync(e); //요청 소켓 처리 후 수락 대기 상태 변경

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        }
         
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            
            try
            {  
                if (ClientSocket.Connected && e.BytesTransferred > 0)    //클라이언트 소켓이 연결되어있고, 전송 바이트 확인
                { 
                    var szData = e.Buffer.Take(e.BytesTransferred).ToArray();   //수신 데이터
                    string text = Encoding.UTF8.GetString(szData);

                    var jsonParser = MessagePacket.Parse(text); //json 파싱
                    nickName = jsonParser.NickName.ToString();

                    //닉네임과 접속 IP PORT 수집
                    DataRow[] resultRows;
                    resultRows = nickNameDt.Select("[IP] = '" + ClientSocket.RemoteEndPoint.ToString() + "'");

                    if (resultRows.Length == 0)
                    {
                        nickNameDt.Rows.Add(ClientSocket.RemoteEndPoint.ToString(), nickName);
                    }
                    
                    szData = Encoding.UTF8.GetBytes(jsonParser.NickName.ToString() + " : " + jsonParser.Message.ToString());
                      
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AddNickNameEvent(jsonParser.NickName.ToString());//닉네임 이벤트
                        MessageEvent(jsonParser.NickName.ToString() + " : " + jsonParser.Message.ToString());//메세지 이벤트
                    }));

                    for (int i = 0; i < m_ClientSocket.Count; i++)
                    {
                        m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
                    }
                     
                    ClientSocket.ReceiveAsync(e);                   
                }
                else
                {
                    //접속 종료 IP PORT 번호로 닉네임 찾음
                    DataRow[] resultRows;
                    resultRows = nickNameDt.Select("[IP] = '" + ClientSocket.RemoteEndPoint.ToString() + "'");

                    string outNickName = string.Empty;
                    if (resultRows.Length > 0)
                    {
                        outNickName = resultRows[0].ItemArray[1].ToString();
                    }
                    
                    ClientSocket.Disconnect(false);
                    m_ClientSocket.Remove(ClientSocket);

                    szData = Encoding.UTF8.GetBytes(outNickName + " : " + "의 연결이 끊어졌습니다.");
                    for (int i = 0; i < m_ClientSocket.Count; i++)
                    {
                        m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
                    }

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageEvent(outNickName + " : " + "의 연결이 끊어졌습니다.");
                        OutNickNameEvent(outNickName);
                    }));
 
                }
            }catch(Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            }
        } 

        public static void ServerClose()
        {
            try
            {
                if (m_ServerSocket != null)
                    //m_ServerSocket.Shutdown(SocketShutdown.Both);
                   // m_ServerSocket.Disconnect(false);
                    m_ServerSocket.Close(); 
                 
                if (m_ClientSocket != null)                    
                    m_ClientSocket.Clear();                    
            }   
            catch (Exception ex)
            {
                WriteLog.WriteLogger(ex.ToString());
            } 
        } 
    }
}
