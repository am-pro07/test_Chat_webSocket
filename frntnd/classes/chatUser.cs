using System;

namespace FrntNd.classes
{
    /// <summary>
    /// пользователь
    /// </summary>
    public class chatUser
    {
        /// <summary>
        /// идентификатор пользователя с защитой на выдачу пустых данных
        /// </summary>
        private Guid _id;
        public Guid id { get { return (_id == null) ? Guid.Empty : _id; } set { _id = value; } }

        /// <summary>
        /// статус пользователя с выдачей значения по умолчанию
        /// </summary>
        private string _status;
        public string status { get { return (_status == null) ? "---" : _status; } set { _status = value; } }

        /// <summary>
        /// логин пользователя
        /// </summary>
        public string login { get; set; }

    }
}
