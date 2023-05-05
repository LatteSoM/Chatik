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
        public TcpClient(Socket _clientSocket, ObservableCollection<string> messageSource) 
        {
            clientSocket = _clientSocket;
            _messageSource = messageSource;
            
        }


        public async Task ReceiveMessage(CancellationToken token)///cancelation here 3
        {
            while (!token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                var b = new ArraySegment<byte>(bytes);
                await clientSocket.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                if (message.StartsWith("/disconnect")) { MainViewModel.CloseClient(); }//////////////////

                _messageSource.Add(message);
            }
        }

        public async Task SendMessage(string message)///cancelation here 4
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var b = new ArraySegment<byte>(bytes);
            await clientSocket.SendAsync(b, SocketFlags.None);
        }

    }
}
