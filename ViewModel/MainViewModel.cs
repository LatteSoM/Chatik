using Chat.ViewModel.HelpTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Chat.ViewModel
{
    internal class MainViewModel : BindingTools
    {

        /*private static ObservableCollection<Socket> clients = new ObservableCollection<Socket>();*/

        public static ObservableCollection<Socket> clients = new ObservableCollection<Socket>();

        public ObservableCollection<Socket> _clientsListSource
        {
            get { return clients; }
            set
            {
                clients = value;
                OnPropertyChanged();
            }
        }


        public static string ipAddress = "";

        private string txtBind = "sdfghjkl";

        public string _txtBind
        {
            get { return txtBind; }
            set
            {
                txtBind = value;
                OnPropertyChanged();
            }
        }


        /*List<string> messageSource = new List<string>();*/

        private ObservableCollection<string> messageSource = new ObservableCollection<string>();

        public ObservableCollection<string> _messageSource
        {
            get { return messageSource; }
            set
            {
                messageSource = value;
                OnPropertyChanged();
            }
        }


        private string visProperty = "Visible";

        public static bool needToHide = true;

        public string _visPropertty
        {
            get
            {
                if (needToHide)
                {
                    visProperty = "Hidden";
                }
                return visProperty;
            }
        }

        private static Socket socket;
        private static Socket clientSocket;

        public static CancellationTokenSource ClientIsVorking;
        public static CancellationTokenSource ServerIsVorking;

        public MainViewModel()
        {

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TcpLogic.TcpClient clnt = new TcpLogic.TcpClient(clientSocket, messageSource);

            if (!needToHide)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TcpLogic.TcpServer ser = new TcpLogic.TcpServer(socket, clients, messageSource);
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6789);
                socket.Bind(ipPoint);
                socket.Listen(1000);
                ServerIsVorking = new CancellationTokenSource();//////////////////////////
                ser.ListenClients(ServerIsVorking.Token);////////////////////////
                clientSocket.Connect(ipAddress, 6789);

                /*if (ServerIsVorking.IsCancellationRequested) { CloseHost(); }*/

                ExitToStartWindow = new BindableCommand(_ => CloseHost());
            }
            else
            {
                /* clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);*/
                clientSocket.Connect(ipAddress, 6789);
                ClientIsVorking = new CancellationTokenSource();///////////////////////////////////
                clnt.ReceiveMessage(ClientIsVorking.Token);

                ExitToStartWindow = new BindableCommand(_ => CloseClient());/////////////////////////////

            }

            SendCom = new BindableCommand(_ => clnt.SendMessage(_txtBind));
        }

        public BindableCommand SendCom { get; set; }

        public BindableCommand ExitToStartWindow { get; set; }///////////////////////////

        public static void CloseHost()//////////////////////////
        {
            socket.Close();
            ServerIsVorking.Cancel();

            CloseClient();

            CloseWin();
        }

        public static void CloseClient()///////////////////////////
        {
            if (ClientIsVorking != null) { ClientIsVorking.Cancel(); }
            clientSocket.Close();
            CloseWin();
        }

        private static void CloseWin(bool needToCloseByMessage = false)//////////////////////
        {
            MainWindow win = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (win != null)
            {
                win.Close();
            }
        }


        /*private async Task ReceiveMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await clientSocket.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                _messageSource.Add(message);
            }
        }

        private async Task SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var b = new ArraySegment<byte>(bytes);
            await clientSocket.SendAsync(b, SocketFlags.None);
        }*/


        /*private async Task ListenClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                _clientsListSource.Add(client);
                ReceiveMsg(client);
            }
        }

        private async Task ReceiveMsg(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await client.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                _messageSource.Add($"[message from {client.RemoteEndPoint} : {message}]");

                foreach (var item in clients)
                {
                    SendMesg(item, message);
                }
            }
        }

        private async Task SendMesg(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var b = new ArraySegment<byte>(bytes);
            await client.SendAsync(b, SocketFlags.None);
        }
    }*/
    }
}
