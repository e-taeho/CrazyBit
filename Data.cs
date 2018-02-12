using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CrazyBit_server
{
    public class Data
    {
        //게임
        static Game game = null;

        //Table
        enum Object
        {
            obj_space,//초록
            obj_wall,//나무랑 밖
            obj_block,//블럭 0
            obj_player
        };
        static Map_Table map_table = new Map_Table();
        static User_Table user_table = new User_Table();
        
        static s_sock sock;
        static Hashtable sock_name = new Hashtable();           //소켓에 이름을 붙여주기
        static Hashtable sock_table = new Hashtable();          //이름으로 소켓찾기
        static List<string> game_user = new List<string>();     //접속한 유저 리스트
        static List<string> game_ready = new List<string>();    //접속한 유저 레디 리스트
        static Hashtable ingame_user = new Hashtable();         //게임속 유저 테이블
        

        static string map = "맵1";
        static bool game_start = false;                         //게임시작 여부

        //게임 클래스 프로퍼티
        static public Game _Game { get { return game; } set { game = value; } }

        //Table 프로퍼티
        public static Map_Table Map_table { get { return map_table; } }
        public static User_Table User_table { get { return user_table; } }

        //접속한 유저 프로퍼티
        public static List<string> Game_user { get { return game_user; } }
        public static List<string> Game_ready { get { return game_ready; } }
        public static Hashtable InGame_user { get { return ingame_user; } }
        

        //소켓
        public static s_sock Sock { get { return sock; } set { sock = value; } }
        //소켓이름
        public static Hashtable Sock_hashTable { get { return sock_name; } }
        //이름으로 소켓찾기
        public static Hashtable Sock_table { get { return sock_table; } }

        //맵
        public static string Map { get { return map; } set { map = value; } }
        //게임시작 여부
        public static bool Game_start { get { return game_start; } set { game_start = value; } }
        
        
        public static bool isLogin(string name)
        {
            for (int i = 0; i < game_user.Count; i++)
            {
                string[] token = game_user[i].Split('#');
                if (token[0] == name) return false;
            }
            return true;
        }

        public static void user_Delete(string name)
        {
            for (int i = 0; i < game_user.Count; i++)
            {
                string[] token = game_user[i].Split('#');
                if (token[0] == name)
                {
                    game_user.RemoveAt(i);
                    break;
                }
            }
        }
        public static void user_Refresh()
        {
            for (int i = 0; i < game_user.Count; i++)
            {
                string cha = game_user[i].Split('#')[2];
                string str = user_table.GetUser(game_user[i].Split('#')[0]);
                game_user[i] = str +"#"+ cha;
            }
        }
    }
}
