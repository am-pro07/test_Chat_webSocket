namespace FrntNd.classes
{
    /// <summary>
    /// пакет сообщения для отправки
    /// </summary>
    class chatPacket
    {
        /// <summary>
        /// отправитель
        /// </summary>
        public chatUser userSender { get; set; }

        /// <summary>
        /// команда
        /// </summary>
        public Commands command { get;  set; }

        /// <summary>
        /// тип источника, кто отправил
        /// </summary>
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
            type = TypeMessage.USER;//отправляем с клиента
            return this;
        }

        /// <summary>
        /// подготавливаем команду на выход
        /// </summary>
        /// <param name="user">отправитель</param>
        /// <returns>пакет для отправки</returns>
        public chatPacket closeCommand(chatUser user)
        {
            userSender = user;
            command = Commands.CLOSE;
            type = TypeMessage.USER;//отправляем с клиента

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
