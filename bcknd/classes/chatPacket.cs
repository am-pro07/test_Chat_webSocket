
namespace FrntNd.classes
{
    /// <summary>
    /// пакет сообщения для отправки
    /// </summary>
    class chatPacket
    {
        //отправитель
        public chatUser userSender { get; set; }
        //команда
        public Commands command { get;  set; }
        //тип источника, кто отправил
        public TypeMessage type { get; set; }

        /// <summary>
        /// подготавливаем команду аутентификации
        /// </summary>
        /// <param name="user">отправитель</param>
        /// <returns>пакет для отправки</returns>
        public chatPacket authCommand(chatUser user)
        {
            userSender = user;
            command = Commands.AUTH;
            type = TypeMessage.SYSTEM;//отправляем с сервера
            return this;
        }

        /// <summary>
        /// подготавливаем команду выдачи активных пользователей
        /// </summary>
        /// <param name="user">отправитель</param>
        /// <returns>пакет для отправки</returns>
        public chatPacket connectedUserList(chatUser user)
        {
            userSender = user;
            command = Commands.INFO;
            type = TypeMessage.SYSTEM;//отправляем с сервера

            return this;
        }

        /// <summary>
        /// "перечислятор" типов команд
        /// </summary>
        public enum Commands : int
        {
            AUTH = 1,//аутентификация
            SEND = 2,//простое сообщение
            CLOSE = 3,//выход
            INFO = 4//информация (статус)
        }

        /// <summary>
        /// "перечислятор" типов отправителей
        /// </summary>
        public enum TypeMessage : int
        {
            SYSTEM = 1,//инициатор - система
            USER = 2//инициатор - пользователь

        }
    }
}
