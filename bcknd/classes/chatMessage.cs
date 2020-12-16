using System;

namespace FrntNd.classes
{
    /// <summary>
    /// класс сообщения. Является частью пакета
    /// </summary>
    class chatMessage :chatPacket, IDisposable
    {
        //хранилице сообщения
        private string _text;
        //доступ с проверкой
        public string text { get { return (_text == null) ? "" : _text; } set { _text = value; } }

        public void Dispose()
        {
            
        }

    }
}
