using FrntNd.classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BckNd.classes
{
    class chatClientBuffer
    {
        private List<chatClient>  onlineClients = new List<chatClient>();

        /// <summary>
        /// конструктор
        /// </summary>
        public chatClientBuffer()
        {
            Task.Run(async () => await inspector());
        }

        /// <summary>
        /// обработка выхода нового пользозвателя
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task newClient(chatClient client)
        {
            onlineClients.Add(client);

            //формирование сообщения
            chatMessage chatMessage = new chatMessage();
            chatMessage.authCommand(client.user);
            chatMessage.text = "Вошел в чат";

            await broadcast(chatMessage);
        }

        /// <summary>
        /// получения списка пользователей в сети
        /// </summary>
        /// <returns></returns>
        public List<chatUser> getOnlineUsers()
        {
            List<chatUser> list = new List<chatUser>();
            foreach (chatClient client in onlineClients)
                list.Add(client.user);
            return list;
        }

        /// <summary>
        /// получаение пользователя по его guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public chatClient getClientByGuid(Guid id)
        {
            chatClient[] clients = onlineClients.Where(client => client.user.id == id).ToArray();
            return (clients.Length!=0)?clients[0]:null;
        }

        /// <summary>
        /// обработка выхода пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task closeClient(chatUser user)
        {
            //закрываем сокет
            chatClient chatClient = onlineClients.Where(client => client.user.id == user.id).First();
            
            onlineClients = onlineClients.Where(client => client.user.id != user.id).ToList();
            using (chatMessage m = new chatMessage())
            {
                m.command = chatPacket.Commands.CLOSE;
                m.text = "Вышел из сети";
                m.type = chatPacket.TypeMessage.SYSTEM;
                m.userSender = user;
                await broadcast(m);
            }
            //await chatClient.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal Closure", CancellationToken.None);
        }

        /// <summary>
        /// рассылка всем активным клиентам
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task broadcast(chatMessage message)
        {
            foreach (chatClient client in onlineClients)
            {
                if (client.socket.State != WebSocketState.Open)
                    continue;
                WebSocket socket = client.socket;
                string json = JsonConvert.SerializeObject(message);
                var buffer = Encoding.UTF8.GetBytes(json);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        /// <summary>
        /// инспектор который будет следить за отключением клиента
        /// </summary>
        /// <returns></returns>
        private async Task inspector()
        {
            while (true)
            {
                //Console.WriteLine("TESTO"); //работает видимо из-за синхронной функции доступа к единому выводу. Но нагружает ЦП
                foreach (chatClient client in onlineClients)
                {
                    if (client.socket.State == WebSocketState.Aborted)
                       await closeClient(client.user);
                }
                //задержка в бесконечном цикле от перегрузки ЦП
                await Task.Delay(10000); //работает и не нагружает ЦП
            }
        }
    }
}
