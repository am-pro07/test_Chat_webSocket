using FrntNd.classes.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrntNd.classes
{
    public class connector : Iconnector
    {

        protected ClientWebSocket client;
        Uri uri;

        /// <summary>
        /// конструктор
        /// </summary>
        public connector()
        {
            client = new ClientWebSocket();
        }
        /// <summary>
        /// анициализация подключения
        /// </summary>
        /// <param name="parameters">словарь с параметрами</param>
        public void initialize(Dictionary<string, object> parameters)
        {
            //если есть параметр "uri", его сохраняем
            if (parameters.Keys.Contains("uri"))
            {
                uri = new Uri(parameters["uri"].ToString());
            }
        }

        /// <summary>
        /// открытие подключения
        /// </summary>
        public async Task open()
        {
            await client.ConnectAsync(uri, CancellationToken.None);
        }

        /// <summary>
        /// получение сообщения
        /// </summary>
        /// <returns>выводим ассинхронное сообщение</returns>
        public async Task<string> reciveString()
        {
            byte[] buffer = new byte[4 * 1024];
            ArraySegment<byte> answer = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = await client.ReceiveAsync(answer, CancellationToken.None);

            string message = Encoding.UTF8.GetString(answer.Array, answer.Offset, result.Count);
            return message;
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="str">сообщение в виде строки</param>
        public async Task sendString(string str)
        {
            byte[] dataB = Encoding.UTF8.GetBytes(str);
            await client.SendAsync(new ArraySegment<byte>(dataB), WebSocketMessageType.Text, false, CancellationToken.None);
        }

        /// <summary>
        /// закрыте подключения
        /// </summary>
        /// <param name="status">Параметр типа WebSocketCloseStatus для отправки причины закрытия</param>
        /// <param name="description">кастомное описание причины закрытия</param>
        public async Task close(object status, string description)
        {
            await client.CloseAsync((WebSocketCloseStatus)status, description, CancellationToken.None);
        }

        /// <summary>
        /// состояние сокета
        /// </summary>
        /// <returns></returns>
        public WebSocketState getStatus()
        { 
            return client.State;
        }

    }
}
