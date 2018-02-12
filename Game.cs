using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace WPF_CrazyBit_server
{
    public class Game
    {
       
        public const int map_width = 15;
        public const int map_height = 13;
        static string[] stage1 = //KFC맵    [W#1]노란블럭 [W#2] 주황블럭 [W#3] KFC블럭 ==>셋다 깨지는블럭
        { 
              "   ","   ","   ","B#1","   ","   ","   ","   ","   ","   ","   ","B#1","   ","   ","   ",
              "   ","   ","B#1","B#2","   ","   ","B#3","   ","B#3","   ","   ","B#2","B#1","   ","   ",
              "   ","B#1","B#2","B#1","B#2","B#3","   ","B#3","   ","B#3","B#2","B#1","B#2","B#1","   ",
              "B#1","B#2","B#1","   ","   ","   ","   ","B#3","   ","   ","   ","   ","B#1","B#2","B#1",
              "B#2","B#1","   ","B#3","   ","   ","B#3","B#1","B#3","   ","   ","B#3","   ","B#1","B#2",
              "B#1","   ","B#3","   ","   ","B#3","W#1","W#2","W#3","B#3","   ","   ","B#3","   ","B#1",
              "B#2","B#1","   ","B#2","B#3","B#1","W#4","W#5","W#6","B#1","B#3","B#2","   ","B#1","B#2",
              "B#1","   ","B#3","   ","   ","B#3","W#7","W#8","W#9","B#3","   ","   ","B#3","   ","B#1",
              "B#2","B#1","   ","B#3","   ","   ","B#3","B#1","B#3","   ","   ","B#3","   ","B#1","B#2",
              "B#1","B#2","B#1","   ","   ","   ","   ","B#3","   ","   ","   ","   ","B#1","B#2","B#1",
              "   ","B#2","B#2","B#1","B#2","B#3","   ","B#3","   ","B#3","B#2","B#1","B#2","B#1","   ",
              "   ","   ","B#1","B#2","   ","   ","B#3","   ","B#3","   ","   ","B#2","B#1","   ","   ",    
              "   ","   ","   ","B#1","   ","   ","   ","   ","   ","   ","   ","B#1","   ","   ","   " 
        };
        static string[] stage2 = new string[]
        {    
              
              "   ","   ","B#1","B#1","B#1","B#1","B#1","   ","   ","B#1","B#1","B#1","B#1","   ","   ",
              "   ","W#1","   ","W#1","   ","B#1","W#1","   ","W#1","B#1","   ","W#1","   ","W#1","   ",
              "B#1","W#1","B#1","W#1","   ","B#4","B#1","B#4","   ","B#4","   ","W#1","B#1","W#1","B#1",
              "B#1","   ","B#4","W#1","B#1","B#4","   ","   ","W#1","B#4","   ","W#1","B#4","   ","B#1",
              "B#1","W#1","B#1","   ","B#4","   ","B#4","B#1","B#4","   ","B#4","   ","B#1","W#1","B#1",
              "   ","W#1","B#4","W#1","W#1","W#1","B#3","B#2","B#3","W#1","W#1","W#1","B#4","W#1","   ",
              "   ","   ","B#1","   ","B#1","B#1","B#2","B#2","B#2","B#1","B#1","   ","B#1","   ","   ",
              "B#1","W#1","B#5","W#1","   ","W#1","B#3","B#2","B#3","W#1","   ","W#1","B#5","W#1","B#1",
              "B#1","W#1","B#1","   ","B#5","   ","B#5","B#1","B#5","   ","B#5","   ","B#1","W#1","B#1",
              "B#1","   ","B#5","W#1","B#1","B#5","W#1","   ","W#1","B#5","B#1","W#1","B#5","   ","B#1",
              "B#1","W#1","B#1","W#1","   ","B#5","B#1","B#5","   ","B#5","   ","W#1","B#1","W#1","B#1",
              "   ","W#1","   ","W#1","   ","B#1","W#1","   ","W#1","B#1","   ","W#1","   ","W#1","   ",    
              "   ","   ","B#1","B#1","B#1","B#1","   ","   ","B#1","B#1","B#1","B#1","B#1","   ","   "  
             
        };
        static string[] stage3 = new string[]
        {    
             
              "   ","   ","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","   ","   ",
              "   ","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","   ",
              "B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1",
              "B#1","B#1","B#1","B#1","   ","   ","   ","   ","   ","   ","   ","B#1","B#1","B#1","B#1",
              "B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1",
              "B#1","B#1","B#1","B#1","   ","B#1","B#1","B#1","B#1","B#1","   ","B#1","B#1","B#1","B#1",
              "B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1",
              "B#1","B#1","B#1","B#1","   ","B#1","B#1","B#1","B#1","B#1","   ","B#1","B#1","B#1","B#1",
              "B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1","W#1","   ","W#1","B#1","W#1","B#1",
              "B#1","B#1","B#1","B#1","   ","   ","   ","   ","   ","   ","   ","B#1","B#1","B#1","B#1",
              "B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1",
              "   ","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","B#1","   ",
              "   ","   ","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","W#1","B#1","   ","   "     
              
        };


        List<Player> player_list = new List<Player>();
        List<string> map_data = new List<string>();                 //맵 데이터
        List<Player> map_player = new List<Player>();           //플레이어 위치 보관
        List<bomb> map_bomb = new List<bomb>();                 //폭탄 위치 보관
        List<string> rank_list = new List<string>();            //순위

        int time = 0;
        string time_str = string.Empty;
        
        
        public List<Player> Player_list {get { return player_list;}}
        public List<string> Map_data { get { return map_data; } }
        public List<Player> Map_player { get { return map_player; } }
        public List<bomb> Map_bomb { get { return map_bomb; } }
        public List<string> Rank_list { get { return rank_list; } }

        public string Time_str {get{return time_str;}}
        
        
        //초기화
        public void init()
        {
            //맵 데이터 추가
            if (Data.Map == "맵1")
            {
                for (int i = 0; i < stage1.Length; i++)
                {
                    map_data.Add(stage1[i]);
                    map_player.Add(null);
                    map_bomb.Add(null);
                }
            }
            else if (Data.Map == "맵2")
            {
                for (int i = 0; i < stage2.Length; i++)
                {
                    map_data.Add(stage2[i]);
                    map_player.Add(null);
                    map_bomb.Add(null);
                }
            }
            else if (Data.Map == "맵3")
            {
                for (int i = 0; i < stage3.Length; i++)
                {
                    map_data.Add(stage3[i]);
                    map_player.Add(null);
                    map_bomb.Add(null);
                }
            }
            
            time = 180;
        }
        public static int index(int x, int y)
        {
            int idx = (y * map_width + x);
            if (idx < 0 || idx > 194) return 0; //idx가 0~194가 아닐경우 

            return y * map_width + x;

        }
        //충돌체크
        public bool isCollision(int x,int y,int x1,int y1)
        {
            int idx = y * map_width + x;            //이전 위치
            int idx1 = y1 * map_width + x1;         //이후 위치

            if (x1 < 0 || y1 < 0) //두 좌표가 둘중하나라도 0이하(맵 밖)면 취소
                return true;
            if (x1 > 14 || y1 > 12)//두 좌표가 둘중하나라도 14이상(맵 밖)이면 취소
                return true;

            if (map_data[idx1].Split('#')[0] == "W") return true;
            if (map_data[idx1].Split('#')[0] == "B") return true;
            if (map_data[idx1].Split('#')[0] == "b")
            {
                
                if(map_data[idx].Split('#')[0] == "b")
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        //플레이어 체크
        public bool isPlayer(int x, int y)
        {
            int idx = y * map_width + x;
            if (map_player[idx] != null) return true; //널값이 아니라면 플레이어 존재!

            return false;
        }
        public void player_add(Player p)
        {
            //리스트에 추가
            player_list.Add(p);
        }
        public void time_change_min()
        {
            string minTosec = ((time % 3600) % 60).ToString();
            string secTomin = ((time % 3600) / 60).ToString();
            string[] minTosec_ZeroArray = new string[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09" };
            if (time <= 9 || time <= 70 && time >= 60 || time <= 130 && time >= 120 || time>=180)
            {
                for (int i = 9; i >= 0; i--)
                {
                    if (minTosec == i.ToString())
                    {
                        minTosec = minTosec_ZeroArray[i];
                        break;
                    }
                }
            }
            time_str = secTomin + ":" + minTosec;

            //Console.Write(secTomin + " : ");
            //if (time > 10)
            //    Console.WriteLine(minTosec);
            //else if (time / 10 == 0)
            //    Console.WriteLine("0" + minTosec);
        }
        public void time_update()
        {
            for (int i = 180; i >= 0; i--)
            {
                time_change_min();
                time--;
                
                //Thread.SpinWait(1000);
                Thread.Sleep(1000);
                if (time == -1)
                {
                    end_draw();
                }
            }

        }
        public void check_end()
        {
            //죽은 사람 수랑 살아있는 사람 차이가 0보다 작으면
            if (Data._Game.Player_list.Count - Data._Game.Rank_list.Count <= 1)
            {
                //게임 종료
                Data._Game.end();
            }
        }
        public void start()
        {
            Thread th = new Thread(new ThreadStart(time_update));
            th.IsBackground = true;
            th.Start();
        }
        public void end_draw()
        {
            Console.WriteLine("게임 종료");
        }
        public void end()
        {
            string name = string.Empty;
            for (int i = 0; i < player_list.Count; i++)
            {
                bool is1st = true;
                for (int j = 0; j < rank_list.Count; j++)
                {
                    //1번이라도 죽은사람이랑 일치하면
                    if (player_list[i].GetStrData().Split('#')[0] == rank_list[j])
                    {
                        is1st = false;
                        break;
                    }
                }
                if(is1st)
                {
                    name = player_list[i].GetStrData().Split('#')[0];
                    break;
                }
            }
            rank_list.Insert(0,name);
            
            
            //레이팅 증감
            switch(player_list.Count)
            {
                case 2:
                    {
                        Data.User_table.UpdateUser(rank_list[0], 8);
                        Data.User_table.UpdateUser(rank_list[1], -8); 
                    }
                    break;
                case 3:
                    {
                        Data.User_table.UpdateUser(rank_list[0], 10);
                        Data.User_table.UpdateUser(rank_list[1], 2);
                        Data.User_table.UpdateUser(rank_list[1], -6);
                    }
                    break;
                case 4:
                    {
                        Data.User_table.UpdateUser(rank_list[0], 12);
                        Data.User_table.UpdateUser(rank_list[1], 4);
                        Data.User_table.UpdateUser(rank_list[2], -4);
                        Data.User_table.UpdateUser(rank_list[3], -12);
                    }
                    break;
            }
            //점수 업데이트 후 갱신
            Data.user_Refresh();
            string str = string.Empty;
            for (int i = 0; i < rank_list.Count; i++)
            {
                Console.WriteLine();
                str += string.Format("{0}등 {1}\n", i + 1,rank_list[i]);
                
            }
            
            Control.MsgSend_ALL(Data.Sock,"GAME_END\a"+str);

            Console.WriteLine("게임 종료");

            //청소
            Data.Game_start = false;

            for (int i = 0; i < Data._Game.Player_list.Count; i++)
            {
                //Data._Game.Player_list[i].bombStop(); //폭탄도 꺼준다
                Data._Game.Player_list[i].print_end(); //쓰레드 다 꺼준다
            }
                Data._Game.map_data.Clear();
                Data._Game.player_list.Clear();
                Data._Game.rank_list.Clear();
                Data._Game.map_player.Clear();

                Data._Game.map_data = null;
                Data._Game.map_player = null;
                Data._Game.player_list = null;
                Data._Game.rank_list = null;
            
            Data._Game = null;
        }
    }
}
