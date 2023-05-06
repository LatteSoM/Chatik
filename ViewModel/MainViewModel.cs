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
using Chat.ViewModel.TcpLogic;

namespace Chat.ViewModel
{
    internal class MainViewModel : BindingTools
    {
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

        private static ObservableCollection<string> allNickNames = new ObservableCollection<string>();

        public ObservableCollection<string> nickNamesListProperty
        {
            get { return allNickNames; }
            set 
            {
                allNickNames = value;
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

        private static ObservableCollection<string> messageSource = new ObservableCollection<string>();

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
        private static Socket clientSocket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static TcpLogic.TcpClient clnt = new TcpLogic.TcpClient(clientSocket, messageSource, allNickNames);

        public static CancellationTokenSource ClientIsVorking;
        public static CancellationTokenSource ServerIsVorking;

        public static string nickname;

        public MainViewModel()
        {

            /*clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TcpLogic.TcpClient clnt = new TcpLogic.TcpClient(clientSocket, messageSource, allNickNames );*/

            if (!needToHide)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TcpLogic.TcpServer ser = new TcpLogic.TcpServer(socket, clients, messageSource, allNickNames);
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6789);
                socket.Bind(ipPoint);
                socket.Listen(1000);
                ServerIsVorking = new CancellationTokenSource();
                ser.ListenClients(ServerIsVorking.Token);
                clientSocket.Connect(ipAddress, 6789);
                ShowLogs = new BindableCommand(_ => ser.ShowChatLogs());
                ExitToStartWindow = new BindableCommand(_ => CloseHost());
            }
            else
            {
                clientSocket.Connect(ipAddress, 6789);
                clnt.SendNickname(nickname);
                clnt.SendMessage("/show");
                ClientIsVorking = new CancellationTokenSource();
                clnt.ReceiveMessage(ClientIsVorking.Token);

                ExitToStartWindow = new BindableCommand(_ => CloseClient());

            }

            SendCom = new BindableCommand(_ => clnt.SendMessage(_txtBind));
        }

        public BindableCommand SendCom { get; set; }

        public BindableCommand ExitToStartWindow { get; set; }

        public BindableCommand ShowLogs { get; set; }

        public static void CloseHost()
        {
            socket.Close();
            ServerIsVorking.Cancel();

            CloseClient();

            CloseWin();
        }

        public static void CloseClient()
        {
            if (ClientIsVorking != null) { ClientIsVorking.Cancel(); }
            clnt.SendMessage($"ушов-{nickname}");
            clientSocket.Close();
            CloseWin();
        }

        private static void CloseWin(bool needToCloseByMessage = false)
        {
            MainWindow win = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (win != null)
            {
                win.Close();
            }
        }
    }
}
