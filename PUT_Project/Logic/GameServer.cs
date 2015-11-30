using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;

namespace PUT_Project.Logic
{
    class GameServer
    {
        private Socket _listenerSocket;
        private Socket _handlerSocket;
        private Thread _receiveThread;
        private Thread _acceptThread;
        private string _ipAddressV4;
        private string _ipAddressV6;
        private bool _commReady;
        bool _threadShutdownFlag = false;

        public bool CommReady
        {
            get
            {
                return _commReady;
            }
        }

        public delegate void SocketDataArrived(Packet packet);
        public event SocketDataArrived DataArrivedEvent;

        public string IpAddresIP4
        {
            get
            {
                return _ipAddressV4;
            }
        }

        public string IpAddressIP6
        {
            get
            {
                return _ipAddressV6;
            }
        }

        public GameServer()
        {
            App.Current.Exit += OnExit;
        }

        public void StartServer(string port)
        {


            _receiveThread = new Thread(Receieve);
            //_acceptThread = new Thread(() => Accept(_listenerSocket)));

            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _ipAddressV4 = ip.ToString();
                    break;
                }
            }
            
            //_ipAddressV6 = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();

            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, Convert.ToInt32(port)));

            _listenerSocket.Listen(0);

            _handlerSocket = _listenerSocket.Accept();

            _listenerSocket.Close();

            Packet initialPacket = new Packet();
            initialPacket.DataID = DATAIDENTIFIER.INITIAL_CONNECTION_MSG;
            initialPacket.Name = GameManager.Instance.Name;
            initialPacket.Message = _ipAddressV4;

            Send(initialPacket);

            _receiveThread.Start();

        }

        private void Receieve()
        {

            while (!_threadShutdownFlag)
            {
                Thread.Sleep(500);
                byte[] dataBuffer = new byte[256];
                try
                {
                    int bytesNumber = _handlerSocket.Receive(dataBuffer, 0, dataBuffer.Length, 0);

                    if (bytesNumber > 0)
                    {
                        if (DataArrivedEvent != null)
                        {

                            DataArrivedEvent(new Packet(dataBuffer));
                        }
                    }
                }
                catch(SocketException e)
                {
                    if (e.ErrorCode == (int)SocketError.ConnectionReset)
                    {
                        Packet pack = new Packet();
                        pack.DataID = DATAIDENTIFIER.CHATMSG;
                        pack.Name = "!:SERVER:!";
                        pack.Message = "Connection closed by remote client";
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
            _handlerSocket.Send(dataBytes, 0, dataBytes.Length, 0);
        }
    
        public void OnExit(object s, ExitEventArgs e)
        {
            _threadShutdownFlag = true;
            _receiveThread.Abort();
            _listenerSocket.Close();
            _handlerSocket.Close();
        }

    }
}
