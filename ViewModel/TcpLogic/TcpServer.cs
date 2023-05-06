using Chat.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.ViewModel.TcpLogic
{
    internal class TcpServer
    {
        
        private Socket socket;
        private ObservableCollection<Socket> _clientsListSource;
        private ObservableCollection<string> _messageSource;
        private ObservableCollection<string> allNicknames = new ObservableCollection<string>();

        private ObservableCollection<string> logs = new ObservableCollection<string>();
        private ObservableCollection<string> messageCopy = new ObservableCollection<string>();
        private bool isShowLog = false;

        public TcpServer(Socket _socket, ObservableCollection<Socket> clientsListSource, ObservableCollection<string> messageSource,
            ObservableCollection<string> _allNickNames) 
        {
            socket = _socket;
            _clientsListSource = clientsListSource;
            _messageSource = messageSource;
            allNicknames = _allNickNames;
        }

        public async Task ListenClients(CancellationToken token = default)
        {
            allNicknames.Add("Адмэн");
            while (!token.IsCancellationRequested)
            {
                var client = await socket.AcceptAsync();
                _clientsListSource.Add(client);
                logs.Add($"Вход пользователя {client.RemoteEndPoint}");
                RecieveNickname(client);
                ReceiveMsg(client);
            }
        }

        private async Task ReceiveMsg(Socket client, CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await client.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                Message msgObject = JsonConvert.DeserializeObject<Message>(message);

                try
                {
                    if (msgObject.msgText == "/show" && !message.Contains("/disconnect"))
                    {
                        string jsonObj = JsonConvert.SerializeObject(allNicknames);
                        foreach (var i in MainViewModel.clients)
                        {
                            SendMesg(i, jsonObj);
                        }
                    }
                    else if (msgObject.msgText.Contains("ушов") && !message.Contains("/disconnect"))
                    {
                        logs.Add(msgObject.msgText);
                        allNicknames.Remove(msgObject.msgText.Split('-')[2]);
                        string jsonObj = JsonConvert.SerializeObject(allNicknames);
                        foreach (var i in MainViewModel.clients)
                        {
                            SendMesg(i, jsonObj);
                        }
                }
                    else if (message.Contains("/disconnect"))
                    {
                        MainViewModel.CloseHost();
                    }
                    else
                    {
                        _messageSource.Add($"| ({msgObject.date.ToShortTimeString()}-{msgObject.date.ToShortDateString()}) - Сообщение от {msgObject.nickname} |: {msgObject.msgText}");
                        messageCopy.Add($"| ({msgObject.date.ToShortTimeString()}-{msgObject.date.ToShortDateString()}) - Сообщение от {msgObject.nickname} |: {msgObject.msgText}");
                        foreach (var i in MainViewModel.clients)
                        {
                            SendMesg(i, message);
                        }
                    }
                }
                catch{}
            }
        }

        private async Task SendMesg(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var b = new ArraySegment<byte>(bytes);
            await client.SendAsync(b, SocketFlags.None);
        }

        private async Task RecieveNickname(Socket _client)
        {

            byte[] bytes = new byte[1024];
            var b = new ArraySegment<byte>(bytes);
            await _client.ReceiveAsync(b, SocketFlags.None);
            string nick = Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);

            if (!nick.Contains("msgText"))
            {
                allNicknames.Add(nick);
                string jsonObj = JsonConvert.SerializeObject(allNicknames);
            }

        }

        public void ShowChatLogs()
        {
            if (isShowLog == true)
            {
                _messageSource.Clear();
                foreach (var item in messageCopy)
                {
                    _messageSource.Add(item);
                }
                isShowLog = false;
            }
            else
            {
                _messageSource.Clear();
                foreach (var log in logs)
                {
                    _messageSource.Add(log);
                }
                isShowLog = true;
            }
        }

        public void addToLog(string record)
        {
            logs.Add(record);
        }

    }
}

