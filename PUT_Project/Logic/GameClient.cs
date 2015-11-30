using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace PUT_Project.Logic
{
    class GameClient
    {
        Socket _clientSocket;
        Thread _recieveThread;

        public string IpAddressV4;

        bool _threadShutdownFlag = false;

        public delegate void SocketDataArrived(Packet packet);
        public event SocketDataArrived DataArrivedEvent;

        public GameClient()
        {
            App.Current.Exit += OnExit;
        }

        public void StartClient(string ipAddress, string port)
        {
            _recieveThread = new Thread(Receieve);

            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), Convert.ToInt32(port)));

            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IpAddressV4 = ip.ToString();
                    break;
                }
            }

            Packet initialPacket = new Packet();
            initialPacket.DataID = DATAIDENTIFIER.INITIAL_CONNECTION_MSG;
            initialPacket.Name = GameManager.Instance.Name;
            initialPacket.Message = IpAddressV4;

            Send(initialPacket);

            _recieveThread.Start();
            
        }

        private void Receieve()
        {
            while (!_threadShutdownFlag)
            {
                Thread.Sleep(500);
                byte[] dataBuffer = new byte[256];
                try
                {
                    int arrivedDataSize = _clientSocket.Receive(dataBuffer, 0, dataBuffer.Length, 0);
                    if (arrivedDataSize > 0)
                    {
                        if (DataArrivedEvent != null)
                        {
                            DataArrivedEvent(new Packet(dataBuffer));
                        }
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == (int)SocketError.ConnectionReset)
                    {
                        Packet pack = new Packet();
                        pack.DataID = DATAIDENTIFIER.CHATMSG;
                        pack.Name = "!:CLIENT:!";
                        pack.Message = "Connection closed by remote host";
                        if (DataArrivedEvent != null)
                        {
                            DataArrivedEvent(pack);
                            _threadShutdownFlag = true;
                        }
                    }
                }
            }
        }

        public void Send(Packet packet)
        {
            byte[] dataBytes = packet.GetDataStream();
            _clientSocket.Send(dataBytes, 0, dataBytes.Length, 0);
        }

        public void OnExit(object s, ExitEventArgs e)
        {
            _threadShutdownFlag = true;
            _recieveThread.Abort();
            _clientSocket.Close();
        }
    }
}

