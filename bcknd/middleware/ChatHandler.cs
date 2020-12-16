using BckNd.classes;
using BckNd.classes.dbConn;
using BckNd.models;
using FrntNd.classes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BckNd.middleware
{
    class ChatHandler
    {
        private readonly RequestDelegate _next;

        public ChatHandler(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, chatClientBuffer chatClientBuffer, myAppContext connectionContext)
        {
            if (context.Request.Path == "/Chat")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        IdbConnector con = new sqlConnector();
                        con.init(connectionContext);
                        await Handler(context, webSocket, chatClientBuffer, con);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("ChatPage");
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task Handler(HttpContext context, WebSocket webSocket, chatClientBuffer chatClientBuffer, IdbConnector connectionContext)
        {
            var buffer = new byte[4*1024];
            ArraySegment<byte> answer = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(answer, CancellationToken.None);
            while (webSocket.State == WebSocketState.Open)
            {
                string answerstr = Encoding.UTF8.GetString(answer.Array, answer.Offset, result.Count);

                chatMessage mes = JsonConvert.DeserializeObject<chatMessage>(answerstr);
                parseByCommand(mes, webSocket, chatClientBuffer, connectionContext);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
        }

        private async void parseByCommand(chatMessage message, WebSocket webSocket, chatClientBuffer chatClientBuffer, IdbConnector connectionContext)
        {
            switch (message.command)
            {
                case (chatPacket.Commands.AUTH):
                {
                    if (message.userSender.id == Guid.Empty)
                    {
                        //получаем пользователей из БД
                        chatUser[] users = (chatUser[])await connectionContext.getData(message.userSender.login);
                        chatUser user;
                        //если это новый пользователь
                        if (users.Length == 0)
                        {
                            //даем ему guid
                            Guid id = Guid.NewGuid();
                            user = new chatUser();
                            user.id = id;
                        }
                        else
                        {
                            //если пользователь не новый, то из запроса забираем его id
                            user = users[0];
                        }

                        //проверка на подключенного пользователя
                        chatClient client = chatClientBuffer.getClientByGuid(user.id);
                        //если пользователь уже был подключен
                        if (client != null )
                        {
                            
                            if (client.socket.State == WebSocketState.Open)
                            {
                                //возвращаем пользователю его сообщение
                                await sendMessage(webSocket, message);
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Пользователь уже в сети", CancellationToken.None);
                                return;
                            }
                            else 
                            {
                                //если пользователь уже был в сети, но сокет не открыт, вырубаем его
                                message.userSender = user;
                                await chatClientBuffer.closeClient(message.userSender);
                            }
                        }
                        message.userSender = user;
                        //подключаем
                        await sendMessage(webSocket, message);
                        //отправим всех юзеров
                        chatMessage informationMessage = new chatMessage();
                        informationMessage.connectedUserList(message.userSender);
                        informationMessage.text = JsonConvert.SerializeObject(chatClientBuffer.getOnlineUsers());
                        await sendMessage(webSocket, informationMessage);
                        //добавление в клиенты
                        await chatClientBuffer.newClient(new chatClient(webSocket, message.userSender));
                    }
                    break;
                }
                case (chatPacket.Commands.SEND):
                {
                    await chatClientBuffer.broadcast(message);
                    break;
                }
                case (chatPacket.Commands.INFO):
                {
                        //смена статуса
                        //await chatClientBuffer.changeStatus(message.userSender, message.text);
                        message.userSender.status = message.text;
                        await chatClientBuffer.broadcast(message);
                        connectionContext.Update(message.userSender);

                        /*chatMessage informationMessage = new chatMessage();
                        informationMessage.connectedUserList(message.userSender);
                        informationMessage.text = JsonConvert.SerializeObject(chatClientBuffer.getOnlineUsers());
                        await sendMessage(webSocket, informationMessage);*/
                        break;
                }
                case (chatPacket.Commands.CLOSE):
                {
                    await chatClientBuffer.closeClient(message.userSender);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        /// отправка сообщения
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        async Task sendMessage(WebSocket webSocket, chatMessage message)
        {
            string json = JsonConvert.SerializeObject(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false, CancellationToken.None);
        }
    }
}
