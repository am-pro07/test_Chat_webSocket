namespace FrntNd.classes
{
    /// <summary>
    /// класс сообщения. Является частью пакета
    /// </summary>
    class chatMessage:chatPacket
    {
        //хранилице сообщения
        private string _text;
        //доступ с проверкой
        public string text { get { return (_text == null) ? "" : _text; } set { _text = value; } }

        /// <summary>
        /// подготовка сообщения для отправки
        /// </summary>
        /// <param name="sender">отправитель</param>
        /// <param name="message">текст сообщения</param>
        /// <returns>сформированный пакет сообщения</returns>
        public chatPacket prepaireMessage(chatUser sender, string message)
        {
            userSender = sender;
            text = message;
            command = Commands.SEND;
            type = TypeMessage.USER;
            return this;
        }
        /// <summary>
        /// подготовка статуса
        /// </summary>
        /// <param name="sender">отправитель</param>
        /// <param name="message">текст статуса</param>
        /// <returns>сформированный пакет сообщения</returns>
        public chatPacket prepaireStatus(chatUser sender, string message)
        {
            userSender = sender;
            text = message;
            command = Commands.INFO;
            type = TypeMessage.USER;
            return this;
        }
    }
}
