using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;


namespace WPF_CrazyBit_server
{
    public class wb_sock
    {
        Socket server;
        Socket client;
        List<Socket> clients = new List<Socket>();
        Hashtable sock_hashTable = new Hashtable();
        Thread th;
        //Data _data;

        public Hashtable Sock_hashTable { get { return sock_hashTable; } }

        //=========== SERVER ===============================================
        public wb_sock(/*Data d,*/ int port)
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            server = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipep);

            //_data = d;
            Console.WriteLine("서버오픈");

            th = new Thread(new ParameterizedThreadStart(wait_client));
            th.IsBackground = true;
            th.Start(server);

        }
        public void Close()
        {
            //th.Suspend();
            server.Close();

            Console.WriteLine("서버종료");
        }

        void wait_client(object obj)
        {
            server.Listen(20);
            server_run();
        }
        public void server_run()
        {
            while (true)
            {
                client = server.Accept();
                IPEndPoint ip = (IPEndPoint)client.RemoteEndPoint;
                string log = string.Format("{0}:{1} 접속", ip.Address, ip.Port);
                Console.WriteLine(log);

                //1. 소켓 저장
                clients.Add(client);

                //2. 스레드 생성
                Thread th = new Thread(new ParameterizedThreadStart(RecvThread));
                th.IsBackground = true;
                th.Start(client);
                ////////////////////////////////11111111111111111111111111
            }
        }
        //=================================================================

        //==============================================================
        public void RecvThread(object dataa)
        {
            Socket socket = (Socket)dataa;
            while (true)
            {
                ////////////////////////////////22222222222222222
                if (recvmessage(socket) == false)
                {
                    string name = (string)sock_hashTable[socket];
                    //_data.user_Delete(name);
                    sock_hashTable.Remove(socket);
                    clients.Remove(socket);
                    socket.Close();
                    break;
                }
            }
        }

        //데이터 수신
        private bool recvmessage(Socket client)
        {
            byte[] data;
            data = ReceiveData(client);
            if (data != null)
            {
                ////////////////////////////////33333333333333333
                Console.WriteLine(Encoding.Default.GetString(data));
                return true;
            }
            else
            {
                Console.WriteLine("수신 오류-서버 종료");
                return false;
            }
        }

        //데이터 전송
        public void messageSend_All(string mesage)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                SendData(clients[i], Encoding.Default.GetBytes(mesage));
            }
        }

        public void messageSend_To(Socket sock, string mesage)
        {
            SendData(sock, Encoding.Default.GetBytes(mesage));
        }
        //===============================================================


        //[내부함수]데이터 전송
        private void SendData(Socket socket, byte[] data)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                // 전송할 데이터의 크기 전달
                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = socket.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = socket.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //[내부함수]데이터 수신
        private byte[] ReceiveData(Socket socket)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기 
                byte[] data_size = new byte[4];
                recv_data = socket.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;

                byte[] data = new byte[size];

                // 실제 데이터 수신
                while (total < size)
                {
                    recv_data = socket.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }


}

