using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPF_CrazyBit_server
{
    public class bomb
    {
        Thread b_th;        //폭탄 쓰레드
        //폭탄 생성
        int x;
        int y;
        Random r = new Random((int)DateTime.Now.Ticks);
        Player p;
        #region 생성자
        public bomb(Player p)
        {
            this.p = p;
            this.x = p.X;
            this.y = p.Y;

            p.P_bomb_num--;
            b_th = new Thread(new ThreadStart(bombDrop));
            b_th.IsBackground = true;
            b_th.Start();
        }
        #endregion
        public void bombStop()
        {
            b_th.Abort();
            Console.WriteLine("bombStop()");
            bombExplode();
            
        }
        #region 폭탄생성
        public void bombDrop()
        {
            Console.WriteLine("bombDrop()");
            //p_bomb_num--;
            int idx = Game.index(x, y);

            Data._Game.Map_data[idx] = "b";

            Thread.Sleep(3000);

            bombExplode();
            //box to space
            


        }
        #endregion

        #region 폭탄터짐
        public void bombExplode()
        {
            
            if (Data._Game == null) return;

            block_destroy(x, y, "center");

            for (int i = 1; i <= p.P_power; i++)
            {
                if (!block_destroy(x - i, y, "left"))
                    break;
            }

            for (int i = 1; i <= p.P_power; i++)
            {
                if (!block_destroy(x + i, y, "right"))
                    break;
            }
            for (int i = 1; i <= p.P_power; i++)
            {
                if (!block_destroy(x, y - i, "up"))
                    break;
            }
            for (int i = 1; i <= p.P_power; i++)
            {
                if (!block_destroy(x, y + i, "down"))
                    break;
            }

            //터짐 작업 완료
            p.P_bomb_num++; //남은 폭탄 갯수 증가

            Thread.Sleep(400);
            if (Data._Game == null) return;
            if (Data._Game.Map_data == null) return;

            Data._Game.Map_data[Game.index(x, y)] = "   ";
            //Control.update_map(_x, _y, "   ");

            for (int i = 1; i <= p.P_power; i++)
            {
                if (Data._Game.Map_data[Game.index(x - i, y)].Split('#')[0] == "*")
                {
                    Data._Game.Map_data[Game.index(x - i, y)] = "   ";
                    //Control.update_map(_x - i, _y, "   ");
                }
            }
            for (int i = 1; i <= p.P_power; i++)
            {
                if (Data._Game.Map_data[Game.index(x + i, y)].Split('#')[0] == "*")
                {
                    Data._Game.Map_data[Game.index(x + i, y)] = "   ";
                    //Control.update_map(_x + i, _y, "   ");
                }
            }
            for (int i = 1; i <= p.P_power; i++)
            {
                if (Data._Game.Map_data[Game.index(x, y - i)].Split('#')[0] == "*")
                {
                    Data._Game.Map_data[Game.index(x, y - i)] = "   ";
                    //Control.update_map(_x, _y - i, "   ");
                }
            }
            for (int i = 1; i <= p.P_power; i++)
            {
                if (Data._Game.Map_data[Game.index(x, y + i)].Split('#')[0] == "*")
                {
                    Data._Game.Map_data[Game.index(x, y + i)] = "   ";
                    //Control.update_map(_x, _y + i, "   ");
                }
            }
            Console.WriteLine("bombExplode()  (" + x.ToString() + "," + y.ToString() + ")");
            
        }
        #endregion

        #region 블럭 파괴
        public bool block_destroy(int _x, int _y, string dir)
        {
            if (Data._Game == null) return false;

            if (_x < 0 || _y < 0) //두 좌표가 둘중하나라도 0이하(맵 밖)면 취소
                return false;
            if (_x > 14 || _y > 12)//두 좌표가 둘중하나라도 14이상(맵 밖)이면 취소
                return false;
            if (dir == "center")
            {
                Data._Game.Map_data[Game.index(_x, _y)] = "*#center";
                Data._Game.Map_bomb[Game.index(_x, _y)] = null;       //폭탄 데이터 제거
                return true;
            }
            if (Data._Game.Map_data[Game.index(_x, _y)] == "b") //폭탄이면
            {
                //if(Game.index(x,y) != Game.index(_x,_y)) //자기 위치빼고
                    Data._Game.Map_bomb[Game.index(_x, _y)].bombStop();
                return true;
            }



            //벽이거나 공백이면 중단
            if (Data._Game.Map_data[Game.index(_x, _y)].Split('#')[0] == "W") return false;

            if (((Data._Game.Map_data[Game.index(_x, _y)] == "   " ||
                 Data._Game.Map_data[Game.index(_x, _y)] == "p") ||
                (Data._Game.Map_data[Game.index(_x, _y)] == "n" ||
                 Data._Game.Map_data[Game.index(_x, _y)] == "*"))||
                Data._Game.Map_data[Game.index(_x, _y)] == "sp")  //공백이면
            {
                if (dir == "up") Data._Game.Map_data[Game.index(_x, _y)] = "*#up";
                else if (dir == "down") Data._Game.Map_data[Game.index(_x, _y)] = "*#down";
                else if (dir == "left") Data._Game.Map_data[Game.index(_x, _y)] = "*#left";
                else if (dir == "right") Data._Game.Map_data[Game.index(_x, _y)] = "*#right";
                else if (dir == "center") Data._Game.Map_data[Game.index(_x, _y)] = "*#center";
                //Control.update_map(_x, _y, "*");

                return true;
            }

            if (Data._Game.Map_data[Game.index(_x, _y)].Split('#')[0] == "B") //벽돌이면
            {
                switch (r.Next(0, 20))
                {
                    // 25%  물폭탄,파워
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        if (r.Next(2) == 0) // 50%확률
                            Data._Game.Map_data[Game.index(_x, _y)] = "p";
                        else Data._Game.Map_data[Game.index(_x, _y)] = "n";
                        break;

                    // 10%  핵폭탄
                    case 5:
                    case 6:
                        Data._Game.Map_data[Game.index(_x, _y)] = "sp";
                        //Control.update_map(_x, _y, "n");
                        break;
                    default:
                        Data._Game.Map_data[Game.index(_x, _y)] = "   ";
                        //Control.update_map(_x, _y, "   ");
                        break;
                }
                return false;
            }

            return true;
        }
        #endregion
    }
}
