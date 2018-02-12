using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPF_CrazyBit_server
{
    public class Control
    {
        static Thread th;
        public static void MsgSend_ALL(s_sock sock, string msg)
        {
            sock.messageSend_All(msg);
        }
        public static void MsgRecv(Socket ssock,string msg)
        {
            //MessageBox.Show(msg); 확인용 함수
            string[] token = msg.Split('\a');


            switch (token[1].Trim())
            {
                case "CREATE":
                    {
                        try
                        {
                            Data.User_table.AddUser(token[2], 1000);
                            Data.Sock.messageSend_To(ssock, "ALERT\a계정 생성 완료");
                            Console.WriteLine("닉네임 : " + token[2] + "  생성완료");
                        }
                        catch(Exception e)
                        {
                            Data.Sock.messageSend_To(ssock, "ALERT\a" + e.Message);
                        }
                        
                    }
                    break;
                case "LOGIN":
                    {
                        try
                        {
                            string str = Data.User_table.GetUser(token[2]);
                            if (str == string.Empty)
                            {
                                Data.Sock.messageSend_To(ssock, "ALERT\a없는 아이디 입니다");
                                return;
                            }
                            if (!Data.isLogin(token[2]))
                            {
                                Data.Sock.messageSend_To(ssock, "ALERT\a중복로그인");
                                return;
                            }
                            if (Data.Game_start)
                            {
                                Data.Sock.messageSend_To(ssock, "ALERT\a이미 시작된 게임입니다");
                                return;
                            }

                            Data.Sock_hashTable.Add(ssock, token[2]); //소켓으로 이름찾기
                            Data.Sock_table.Add(token[2], ssock);     //이름으로 소켓찾기

                            Data.Game_user.Add(Data.User_table.GetUser(token[2])+"#다오★down");
                            Console.WriteLine("닉네임" + token[2] + " 접속");

                            Data.Sock.messageSend_To(ssock, "ACK_LOGIN\a" + token[2]);

                        }
                        catch (Exception e)
                        {
                            Data.Sock.messageSend_To(ssock, "ALERT\a" + e.Message);
                            return;
                        }
                    }
                    break;
                case "CHAT":
                    {
                        MsgSend_ALL(Data.Sock, "CHAT\a"+token[2]);
                    }
                    break;
                case "CHARACTER_CHANGE":
                    {
                        string name = token[0];
                        string charac = token[2];
                        for (int i = 0; i < Data.Game_user.Count; i++)
                        {
                            string[] info = Data.Game_user[i].Split('#');
                            if (info[0] == name)
                            {
                                Data.Game_user[i] = token[0] + "#" + info[1] + "#" + charac;
                            }
                        }
                    }
                    break;
                case "MAP_CHANGE":
                    {
                        if (token[2] == "PREV")
                        {
                            if (Data.Map == "맵1")
                                Data.Map = "맵3";
                            else if (Data.Map == "맵2")
                                Data.Map = "맵1";
                            else if (Data.Map == "맵3")
                                Data.Map = "맵2";
                        }
                        else if (token[2] == "NEXT")
                        {
                            if (Data.Map == "맵1")
                                Data.Map = "맵2";
                            else if (Data.Map == "맵2")
                                Data.Map = "맵3";
                            else if (Data.Map == "맵3")
                                Data.Map = "맵1";
                        }
                    }
                    break;
                case "READY":
                    {
                        //없으면 추가
                        if (!Data.Game_ready.Contains(token[0]))
                        {
                            Data.Game_ready.Add(token[0]);
                            Data.Sock.messageSend_To(ssock, "ACK_READY");
                            Console.WriteLine(token[0]+"  준비");
                        }
                        //있으면 제거
                        else
                        {
                            Data.Game_ready.Remove(token[0]);
                            Console.WriteLine(token[0] + "  준비해제");
                        }

                        //게임스타트
                        if (Data.Game_ready.Count > 1
                            && Data.Game_ready.Count == Data.Game_user.Count)
                        {
                            if (!Data.Game_start)
                            {
                                Data.Game_start = true;
                                
                                Data._Game = new Game();
                                Data._Game.init();
                                
                                int size = Data.Game_user.Count;
                                for (int i = 0; i < size; i++)
                                {
                                    
                                    string[] token5 = Data.Game_user[i].Split('#');
                                    if(i==0) Data._Game.player_add(new Player(token5[0],0,0,token5[2]));
                                    else if (i == 1) Data._Game.player_add(new Player(token5[0], 14, 0, token5[2]));
                                    else if (i == 2) Data._Game.player_add(new Player(token5[0], 0, 12, token5[2]));
                                    else if (i == 3) Data._Game.player_add(new Player(token5[0], 14, 12, token5[2]));
                                    Data._Game.Player_list[i].print_start();
                                }

                                Data._Game.start();
                                Data.Game_ready.Clear();

                                Control.MsgSend_ALL(Data.Sock, "GAME_START\a" + Data.Map);
                                Console.WriteLine("GAME_START");
                            }
                        }
                    }
                    break;
                case "KEY":
                    {
                        int size = Data.Game_user.Count;
                        for(int i=0;i<size;i++)
                        {
                            string[] token6 = Data.Game_user[i].Split('#');
                            if (token[0] == token6[0]) //닉네임이 일치하면
                            {
                                if(Data._Game != null)
                                Data._Game.Player_list[i].keyEvents(token[2]);
                            }
                        }
                        
                    }
                    break;
                case "CHECK_END":
                    {
                        if(Data._Game != null)
                        Data._Game.check_end();
                    }
                    break;
            }
        }/*
        public static void update_map(int x, int y,string b)
        {
            string str = "UPDATE_INGAME_MAP2\a" + x.ToString() + "#" + y.ToString() + "\a"+b;
            MsgSend_ALL(Data.Sock, str);
        }
          * */
        public static void update_start() 
        {
            th = new Thread(new ThreadStart(update_data));
            th.IsBackground = true;
            th.Start();
        }
        public static void update_stop()
        {
            th.Abort();
        }
        static void update_data()
        {
            while (true)
            {    
                string str = string.Empty;

                //대기실,게임 유저 갱신
                #region 대기실 유저 갱신
                int size = Data.Game_user.Count;
                str = "UPDATE_WAIT_USER\a" + size.ToString() + "\a";
                for (int i = 0; i < Data.Game_user.Count; i++)
                {
                    str += Data.Game_user[i] + "\b";
                }
                MsgSend_ALL(Data.Sock, str);
                #endregion 

                //대기실 레디 갱신
                #region 대기실 레디 갱신
                size = Data.Game_user.Count;

                
                str = "UPDATE_WAIT_READY\a" + size.ToString() + "\a";
                for (int i = 0; i < Data.Game_user.Count; i++)
                {
                    if (Data.Game_ready.Count == 0)
                    {
                        str += i.ToString() + "#off" + "\b";
                    }
                    else
                    {
                        for (int j = 0; j < Data.Game_ready.Count; j++)
                        {
                            string[] token = Data.Game_user[i].Split('#');

                            if (Data.Game_ready[j] == token[0]) //레디에 이름이있으면
                            {
                                str += i.ToString() + "#on" + "\b";
                                break;
                            }
                            if (j == Data.Game_ready.Count - 1) //마지막 까지 안됬으면
                            {
                                str += i.ToString() + "#off" + "\b";
                            }
                        }
                        
                    }
                }
                MsgSend_ALL(Data.Sock, str);
                #endregion

                //대기실 맵 갱신
                #region 대기실 맵 갱신
                str = "UPDATE_WAIT_MAP\a" + Data.Map;

                MsgSend_ALL(Data.Sock, str);
                #endregion

                //인게임 시간 갱신
                #region 인게임 시간 갱신
                if (Data._Game != null)
                {
                    str = "TIME_UPDATE\a" + Data._Game.Time_str;

                    MsgSend_ALL(Data.Sock, str);
                }
                #endregion

                //인게임 유저 정보 갱신
                #region 인게임 유저 정보 갱신
                if (Data._Game != null)
                {
                    size = Data._Game.Player_list.Count;
                    str = "UPDATE_INGAME_USER\a" +
                        size.ToString() + "\a";
                    for(int i=0;i<size;i++)
                    {
                        str+=Data._Game.Player_list[i].GetStrData() + "\b";
                    }
                    MsgSend_ALL(Data.Sock, str);
                }
                #endregion

                //인게임 맵 데이터 갱신
                #region 인게임 맵 데이터 갱신
                if (Data._Game != null)
                {
                    size = Data._Game.Map_data.Count;
                    str = "UPDATE_INGAME_MAP\a" +
                        size.ToString() + "\a" + Data.Map + "\a";
                    for (int i = 0; i < size; i++)
                    {
                        str += Data._Game.Map_data[i] + "★";
                    }
                    Control.MsgSend_ALL(Data.Sock, str);
                }
                #endregion

                if(Data._Game != null)
                Data._Game.check_end();
                Thread.Sleep(50);
            }
        }
        
    }
}
