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
        public TcpServer(Socket _socket, ObservableCollection<Socket> clientsListSource, ObservableCollection<string> messageSource ) 
        {
            socket = _socket;
            _clientsListSource = clientsListSource;
            _messageSource = messageSource;
        }

        public async Task ListenClients(CancellationToken token = default)///cancelation here
        {
            while (!token.IsCancellationRequested)
            {
                var client = await socket.AcceptAsync();
                _clientsListSource.Add(client);
                ReceiveMsg(client);
            }
        }

        private async Task ReceiveMsg(Socket client, CancellationToken token = default)///cancelation here 2
        {
            while (!token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await client.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                _messageSource.Add($"[message from {client.RemoteEndPoint} : {message}]");

                foreach (var item in MainViewModel.clients)
                {
                    if (message.StartsWith("/disconnect")) { MainViewModel.CloseHost(); }//////////////////////////
                    SendMesg(item, message);
                }
            }
        }

        private async Task SendMesg(Socket client, string message)///cancelation here 5
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var b = new ArraySegment<byte>(bytes);
            await client.SendAsync(b, SocketFlags.None);
        }
    }
}

