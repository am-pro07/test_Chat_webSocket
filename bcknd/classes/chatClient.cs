using FrntNd.classes;
using System.Net.WebSockets;

namespace BckNd.classes
{
    /// <summary>
    /// класс клиента. Содержит не только пользователя, но и сокет
    /// </summary>
    class chatClient
    {
        public WebSocket socket { get; set; }
        public chatUser user { get; set; }
        public chatClient(WebSocket _webSocket, chatUser _chatUser)
        {
            socket = _webSocket;
            user = _chatUser;
        }
    }
}
