﻿using Newtonsoft.Json;
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
                  
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    MessageEvent("서버 실행 성공!!");
                });
                WriteLog.WriteLogger("서버 실행");

            }
            catch(Exception ex)
            { 
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    MessageEvent("서버 실행 실패!!");
                });

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
            catch(Exception ex)
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
                   
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        var str = MessagePacket.Parse(text); //json 파싱

                        nickName = str.NickName.ToString();
                        AddNickNameEvent(str.NickName.ToString());//닉네임 이벤트

                        szData = Encoding.UTF8.GetBytes(str.NickName.ToString() + " : " + str.Message.ToString());

                        for (int i = 0; i < m_ClientSocket.Count; i++)
                        {
                            m_ClientSocket[i].Send(szData, szData.Length, SocketFlags.None);
                        }

                        MessageEvent(str.NickName.ToString() + " : " + str.Message.ToString());//메세지 이벤트

                    });

                    ClientSocket.ReceiveAsync(e);                   
                }
                else
                {
                    ClientSocket.Disconnect(false);
                    m_ClientSocket.Remove(ClientSocket); 
                     
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        MessageEvent(nickName + " : " + "의 연결이 끊어졌습니다.");
                        OutNickNameEvent(nickName); 
                    });
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
