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
using SocketJsonLib;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace SocketTest
{
    public delegate void AddNickNameEventHandler(string nickName);
    public delegate void AddMessageEventHandler(string massage);

    public class ServerSocket : MainWindow
    {
        public static AddNickNameEventHandler NickNameEvent;
        public AddMessageEventHandler MessageEvent;

        private static Socket m_ServerSocket; //서버 소켓
        private static List<Socket> m_ClientSocket; //클라이언트 소켓 리스트
        private byte[] szData;  //클라이언트 데이터 버퍼 사이즈
       // MainWindow main = null;
        string nickName = string.Empty;
        //public ServerSocket(MainWindow main)
        //{
        //    this.main = main;
        //}

        public void StartServer()
        { 
            try
            { 
                m_ClientSocket = new List<Socket>();    //클라이언트 소켓 리스트 초기화
                m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //소켓 초기화
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9800);  //IP 및 포트  설정
                m_ServerSocket.Bind(ipep);  //서버 바인딩
                m_ServerSocket.Listen(20);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //비동기 서버 이벤트
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed); //이벤트 발생시 Accept_Completed 함수 실행
                m_ServerSocket.AcceptAsync(args);
                //test();
                MessageEvent += new AddMessageEventHandler(SetMessage);
                MessageEvent("test");
                
                //SetMessage("서버 실행 성공!!");
                
                //ShowMesssge("서버 실행 성공!!");
            }
            catch(Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
                SetMessage("서버 실행 실패!!");
                //ShowMesssge("서버 실행 실패!!");
                WriteSetLog(ex.ToString());

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
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                WriteSetLog(ex.ToString());
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
                  
                    string message = string.Empty;
                    int chk = 0;

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    { 
                        nickName = JsonConvert.SerializeObject(SocketJsonLib.SocketJson.ParserJson("nickName", text));
                        nickName = nickName.Replace("\"", "");
                        NickNameEvent(nickName);//닉네임 이벤트
                        

                    });
                    
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        message = JsonConvert.SerializeObject(SocketJsonLib.SocketJson.ParserJson("message", text));
                        message = message.Replace("\"", "");
                        // szData = null;
                        szData = Encoding.UTF8.GetBytes(nickName + " : " + message);
                         
                        for (int i = 0; i < m_ClientSocket.Count; i++)
                        {
                            m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
                        }
                         
                       // SetMessage(nickName + " : " + message);
                         
                        //this.ShowMesssge(nickName + " : " + message);
                    });

                    //szData = null;
                    
                    ClientSocket.ReceiveAsync(e);                   
                }
                else
                {
                    ClientSocket.Disconnect(false);
                    m_ClientSocket.Remove(ClientSocket);
                    SetMessage(nickName + "의 연결이 끊어졌습니다.");
                    //this.ShowMesssge(nickName + "의 연결이 끊어졌습니다.");
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        //for (int i = 0; i < main.nicListBox.Items.Count; i++)
                        //{
                        //    if (nickName == main.nicListBox.Items[i].ToString())
                        //        main.nicListBox.Items.RemoveAt(i);
                        //}
                    });
                }
            }catch(Exception ex)
            {
                WriteSetLog(ex.ToString());
            }
        }
         
        //private void ShowMesssge(string message)
        //{
        //    try
        //    {
        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
        //        (ThreadStart)delegate ()
        //        {
        //            main.viewTxt.AppendText(message + "\n");
        //        });
        //    }catch(Exception ex)
        //    {
        //        WriteSetLog(ex.ToString());
        //    }
        //}

        public static void ServerClose()
        {
            try
            {
                if (m_ServerSocket != null)
                {
                    m_ServerSocket.Close();

                }

                if (m_ClientSocket != null)
                    m_ClientSocket.Clear();
            }
            catch (Exception ex)
            {
                WriteSetLog(ex.ToString());
            }
           
        }

        public static void WriteSetLog(string log)
        {
            try
            {
                StreamWriter writer;
                writer = File.AppendText(@"C:\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");         //Text File이 저장될 위치(파일명)                                                                                                        
                writer.WriteLine("[" + DateTime.Now + log);    //저장될 string
                writer.Close();
                writer.Dispose();
            }
            catch (Exception e)
            {
                 
            }
        }
    }
}
