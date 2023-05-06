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
    internal class TcpClient
    {
        
        private Socket clientSocket;
        private ObservableCollection<string> _messageSource;

        private ObservableCollection<string> allNicknames;
        public TcpClient(Socket _clientSocket, ObservableCollection<string> messageSource, ObservableCollection<string> _allNick) 
        {
            clientSocket = _clientSocket;
            _messageSource = messageSource;
            allNicknames = _allNick;
        }


        public async Task ReceiveMessage(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await clientSocket.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                if (message.StartsWith("/disconnect")) { MainViewModel.CloseClient(); }

                try
                {
                    Message msgObject = JsonConvert.DeserializeObject<Message>(message);
                    _messageSource.Add($"| ({msgObject.date.ToShortTimeString()}-{msgObject.date.ToShortDateString()}) - Сообщение от {msgObject.nickname} |: {msgObject.msgText}");
                }
                catch (JsonSerializationException)
                {
                    var allnicks = JsonConvert.DeserializeObject<ObservableCollection<string>>(message);
                    allNicknames.Clear();
                    allNicknames.Add(string.Join("\n", allnicks));
                }
            }
        }

        public async Task SendMessage(string message)
        {
            Message msgObj = new Message();
            msgObj.nickname = MainViewModel.nickname;
            msgObj.msgText = message;
            string jsonObj = JsonConvert.SerializeObject(msgObj);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonObj);
            var b = new ArraySegment<byte>(bytes);
            await clientSocket.SendAsync(b, SocketFlags.None);
        }

        public async Task SendNickname(string nickname)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(nickname);
            var b = new ArraySegment<byte>(bytes);
            await clientSocket.SendAsync(b, SocketFlags.None);
        }


    }
}
