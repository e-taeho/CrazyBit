using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace WPF_CrazyBit_server
{
    public class Player
    {
        Thread th1;         //플레이어 쓰레드
        
        int x = 0;                  //x좌표
        int y = 0;                  //y좌표
        int canvas_x = 0;           //실제 그려지는 x좌표
        int canvas_y = 0;           //실제 그려지는 y좌표

        string name;               //플레이어 이름
        string character;            //캐릭터 이름 배찌 다오 같은거

        int p_power = 1;            //물줄기 길이
        int p_bomb_num = 2;         //물폭탄 최대 수
        int p_speed = 6;            //캐릭터 스피드

        bool alive = true;

        bool iskey = false;

        bool iskey_up = false;
        bool iskey_down = false;
        bool iskey_left = false;
        bool iskey_right = false;


        //프로퍼티
        public int X { get { return x; } }                  //x좌표
        public int Y { get { return y; } }                  //y좌표
        public int P_power {get{return p_power;}}           //물줄기 길이
        public int P_bomb_num { get { return p_bomb_num; } set { p_bomb_num = value; } }     //물폭탄 최대 수


        public bool Alive 
        {
            get{return alive;}
            set
            {
                if (value == false)
                {
                    character = "죽음";
                    Data._Game.Rank_list.Insert(0, name);  //죽은 애들 이름 순서대로 나열
                }
                alive = value;
            }
        }
        public Player(string _name, int _x, int _y, string _character)
        {
            //플레이어 이름,x,y좌표 캐릭터이름
            name = _name;
            canvas_x = _x*48;
            canvas_y = _y*48;
            character = _character;
        }
        public void keyEvents(string key)
        {
            if(!alive) return;
            switch (key)
            {
                case "VK_UP_PRE": keyEvents_Up_Pre(); break;
                case "VK_LEFT_PRE": keyEvents_Left_Pre(); break;
                case "VK_DOWN_PRE": keyEvents_Down_Pre(); break;
                case "VK_RIGHT_PRE": keyEvents_Right_Pre(); break;
                case "VK_UP_REL": keyEvents_Up_Rel(); break;
                case "VK_LEFT_REL": keyEvents_Left_Rel(); break;
                case "VK_DOWN_REL": keyEvents_Down_Rel(); break;
                case "VK_RIGHT_REL": keyEvents_Right_Rel(); break;
                case "SPACE": make_bomb(); break;
            }
        }
        //좌표 변경 함수
        void cp_to_p()
        {
            //움직이기전 좌표 널값
            Data._Game.Map_player[Game.index(x, y)] = null;
            x = (canvas_x+24) / 48;    //실제 x좌표로 변경
            y = (canvas_y+24) / 48;   //실제 y좌표로 변경

            //이동후 좌표 갱신
            Data._Game.Map_player[Game.index(x, y)] = this;
        }
        public void print_start()
        {
            th1 = new Thread(new ThreadStart(update));
            th1.IsBackground = true;
            th1.Start();
        }
        public void print_end()
        {
            th1.Abort();
        }
        public void update()
        {
            while (true)
            {
                if (!alive) break; //죽었으면 쓰레드 중지
                move(); //움직이고
                cp_to_p(); //좌표변경
                check_tile(x,y); //이벤트 체크
                Thread.Sleep(50);
            }
        }
        public string GetStrData()
        {
            //이름#X좌표#Y좌표#캐릭터이름 을 리턴
            string str = name + '#' + canvas_x.ToString() + '#' + canvas_y.ToString() + '#' + character;
            return str;
        }
        #region move()
        void move()
        {
            if (iskey_up)
            {
                int yy = canvas_y - p_speed;    
                int x1 = (canvas_x) / 48;
                int x2 = (canvas_x +47) / 48;
                int y1;

                if (yy < 0) y1 = -1;    
                else y1 = yy/48;

                if (Data._Game.isCollision(x,y,x1, y1))  return;
                if (Data._Game.isCollision(x, y, x2, y1)) return;

                canvas_y = yy;      
            }
            if (iskey_down)
            {
                int yy = (canvas_y + 47 + p_speed);   
                int x1 = (canvas_x) / 48;
                int x2 = (canvas_x + 47) / 48;
                int y2;

                if (yy > 48*13) y2 = 13;
                else y2 = yy/48;

                if (Data._Game.isCollision(x, y, x1, y2)) return;
                if (Data._Game.isCollision(x, y, x2, y2)) return;

                canvas_y = yy-47;
            }
            if (iskey_left)
            {
                int xx = (canvas_x - p_speed);
                int x1;
                int y1 = (canvas_y) / 48;
                int y2 = (canvas_y + 47) / 48;

                if (xx < 0) x1 = -1;
                else x1 = xx/48;

                if (Data._Game.isCollision(x, y, x1, y1)) return;
                if (Data._Game.isCollision(x, y, x1, y2)) return;

                canvas_x = xx;
            }
            if (iskey_right)
            {
                int xx = canvas_x + 47 + p_speed;
                int x2;
                int y1 = (canvas_y) / 48;
                int y2 = (canvas_y + 47) / 48;

                if (xx > 48*15) x2 = 15;
                else x2 = xx / 48;

                if (Data._Game.isCollision(x,y,x2, y1)) return;
                if (Data._Game.isCollision(x,y,x2, y2)) return;

                canvas_x = xx-47;
            }
            //움직임이 끝난 이후 발 밑 체크
        }
        #endregion
        void make_bomb()
        {
            if (p_bomb_num > 0) //남은 폭탄 갯수가 0 이상 일 경우
            {
                if (Data._Game.Map_bomb[Game.index(x, y)] == null)
                    Data._Game.Map_bomb[Game.index(x, y)] = new bomb(this);
            }
        }
        void bomb_num_up() { p_bomb_num++; }
        void bomb_power_up() { p_power++; }

        

        //체크타일
        void check_tile(int _x, int _y)
        {
            if (!alive) return;

            if (Data._Game.Map_data[Game.index(_x, _y)].Split('#')[0] == "*") // 물줄기면
            {
                Console.WriteLine(name + "님이 죽음ㅋㅋ");
                Alive = false; //생존변수를 오프해줌
            }
            else if (Data._Game.Map_data[Game.index(_x, _y)] == "p")
            {
                Data._Game.Map_data[Game.index(_x, _y)] = "   ";
                //Control.update_map(_x, _y, "   ");
                p_power++; //파워 업
            }
            else if (Data._Game.Map_data[Game.index(_x, _y)] == "n")
            {
                Data._Game.Map_data[Game.index(_x, _y)] = "   ";
                //Control.update_map(_x, _y, "   ");
                p_bomb_num++; //물풍선 개수 업
            }
            else if (Data._Game.Map_data[Game.index(_x, _y)] == "sp")
            {
                Data._Game.Map_data[Game.index(_x, _y)] = "   ";
                //Control.update_map(_x, _y, "   ");
                p_power+=10; // 핵폭탄
            }
        }
        void keyEvents_Up_Pre()
        {
            if (!iskey)
            {
                iskey_up = true;
                iskey = true;
                switch (character.Split('★')[0])
                {
                    case "다오": character = "다오★up"; break;
                    case "배찌": character = "배찌★up"; break;
                    case "디즈니": character = "디즈니★up"; break;
                    case "모스": character = "모스★up"; break;
                }

                    
            }
            
        }
        void keyEvents_Left_Pre()
        {
            if (!iskey)
            {
                iskey_left = true;
                iskey = true;
                switch (character.Split('★')[0])
                {
                    case "다오": character = "다오★left"; break;
                    case "배찌": character = "배찌★left"; break;
                    case "디즈니": character = "디즈니★left"; break;
                    case "모스": character = "모스★left"; break;
                }
            }
            
        }
        void keyEvents_Down_Pre()
        {
            if (!iskey)
            {
                iskey_down = true;
                iskey = true;
                switch (character.Split('★')[0])
                {
                    case "다오": character = "다오★down"; break;
                    case "배찌": character = "배찌★down"; break;
                    case "디즈니": character = "디즈니★down"; break;
                    case "모스": character = "모스★down"; break;
                }
            }
            
        }
        void keyEvents_Right_Pre()
        {
            if (!iskey)
            {
                iskey_right = true;
                iskey = true;
                switch (character.Split('★')[0])
                {
                    case "다오": character = "다오★right"; break;
                    case "배찌": character = "배찌★right"; break;
                    case "디즈니": character = "디즈니★right"; break;
                    case "모스": character = "모스★right"; break;
                }
            }
            
        }
        //===========================================================

        void keyEvents_Up_Rel()
        {
            if (iskey && iskey_up)
            {
                iskey_up = false;
                iskey = false;
            }
            
        }
        void keyEvents_Left_Rel()
        {
            if (iskey && iskey_left)
            {
                iskey_left = false;
                iskey = false;
            }
        }
        void keyEvents_Down_Rel()
        {
            if (iskey && iskey_down)
            {
                iskey_down = false;
                iskey = false;
            }
        }
        void keyEvents_Right_Rel()
        {
            if (iskey && iskey_right)
            {
                iskey_right = false;
                iskey = false;
            }
        }
    }
}