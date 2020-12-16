using FrntNd.classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrntNd
{
    public partial class ChatForm : Form
    {
        public ChatForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// "правильный" выход из программы через отключение пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //готовим пакет на отключение и отправляем его на сервер
            chatPacket packet = new chatPacket();
            await con.sendString(JsonConvert.SerializeObject(packet.closeCommand(me)));
            Application.Exit();
        }


        /// <summary>
        /// коннектор
        /// </summary>
        connector con;
        /// <summary>
        /// птекущий пользователь
        /// </summary>
        chatUser me;
        /// <summary>
        /// сохранение всех параметров из дочерней панели
        /// </summary>
        /// <param name="_c"></param>
        /// <param name="_u"></param>
        public void saveConnection(connector _c, chatUser _u)
        {
            con = _c;
            me = _u;
        }

        private async void ChatForm_Load(object sender, EventArgs e)
        {
            //если же подключения нет (первый запуск), то открываем окно авторизации
            if (con == null)
            {
                Auth auth = new Auth();
                auth.Owner = this;
                auth.ShowDialog();
            }
            //запускаем обработчик сообщений
            await handler();
        }

        /// <summary>
        /// класс с активными пользователями
        /// </summary>
        userListAdapter users;

        /// <summary>
        /// обработчик сообщений
        /// </summary>
        /// <returns></returns>
        private async Task handler()
        {
            //определяем класс пользователей
            users = new userListAdapter(UserList);
            //сохраняем свое имя
            this.Text = this.Text + ":" + me.login;
            //если все нормально, крутим бесконечный цикл на обработку
            while (true)
            {
                if (con != null)
                {
                    //получаем строку от сервера
                    string json = await con.reciveString();
                    //преобразуем в объект
                    chatMessage messageObject = JsonConvert.DeserializeObject<chatMessage>(json);
                    //смотрим что за команда
                    switch (messageObject.command)
                    {
                        case (chatPacket.Commands.AUTH):
                            {
                                //при авторизации, добавляем пользователя в список
                                users.append(messageObject.userSender);
                                break;
                            }
                        case (chatPacket.Commands.CLOSE):
                            {
                                //при закрытии соединения, удаляем из списка 
                                users.delete(messageObject.userSender);
                                break;
                            }
                        case (chatPacket.Commands.INFO):
                            {
                                //если пришло информационное сообщение, то смотрим от кого
                                if (messageObject.type == chatPacket.TypeMessage.SYSTEM)
                                {
                                    //если от системы, то это значит что система дает список активных юзеров
                                    //значит перезаполняем список
                                    users.clear();
                                    string onlineUsersJson = messageObject.text;
                                    List<chatUser> chatUsers = JsonConvert.DeserializeObject<List<chatUser>>(onlineUsersJson);
                                    foreach (chatUser user in chatUsers)
                                    {
                                        users.append(user);
                                    }
                                    continue;
                                }
                                else if (messageObject.type == chatPacket.TypeMessage.USER)
                                {
                                    //если же информационное сообщение от пользователя, то значит он сменил статус
                                    //обновим его
                                    messageObject.text = "Сменил статус: '" + messageObject.text + "'";
                                    users.updateUser(messageObject.userSender);
                                }
                                break;

                            }
                    }
                    //выводим сообщение
                    string outputMessage = messageObject.userSender.login.ToString() + ": " + messageObject.text + "\n";
                    MessageField.Text += outputMessage;
                }
                //проверка соединения. Если соединения нет, то запускаем авторизацию
                if (con.getStatus() != System.Net.WebSockets.WebSocketState.Open)
                {
                    Auth auth = new Auth();
                    auth.Owner = this;
                    auth.ShowDialog();
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //формируем сообщение
            chatMessage message = new chatMessage();
            string text = textBox1.Text;
            //если сообщение начинается с \, то это команда на смену статуса
            if (text[0] == '\\')
            {
                text = text.Substring(1);
                message.prepaireStatus(me, text);
            }
            else
            {
                message.prepaireMessage(me, text);
            }
            //оборачиваем сообщение и отправляем
            string json = JsonConvert.SerializeObject(message);
            await con.sendString(json);

        }

        //класс-список пользователей
        class userListAdapter
        {
            List<chatUser> users; //сам список
            ListBox control; //отображение списка
            /// <summary>
            /// инициализация в конструкторе
            /// </summary>
            /// <param name="_control">визуальный компонент для отрисовки</param>
            public userListAdapter(ListBox _control)
            {
                control = _control;
                users = new List<chatUser>();
                control.Items.Clear();
            }
            /// <summary>
            /// добавление пользователя
            /// </summary>
            /// <param name="newUser"></param>
            public void append(chatUser newUser)
            {
                users.Add(newUser);
                control.Items.Add(newUser.login);
            }
            /// <summary>
            /// удаление пользователя
            /// </summary>
            /// <param name="delUser"></param>
            public void delete(chatUser delUser)
            {
                users = users.Where(u => u.id != delUser.id).ToList();
                control.Items.Remove(delUser.login);
            }
            /// <summary>
            /// обновление пользователя
            /// </summary>
            /// <param name="user"></param>
            public void updateUser(chatUser user)
            {
                users = users.Where(u => u.id != user.id).ToList();
                users.Add(user);
            }
            /// <summary>
            /// получение пользователя по имени
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public chatUser getUser(string name)
            {
                return users.Where(u => u.login == name).FirstOrDefault();
            }
            /// <summary>
            /// очистка списка
            /// </summary>
            public void clear()
            {
                users.Clear();
                control.Items.Clear();
            }
        }

        private void UserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //вывод статуса при клике
            if (UserList.SelectedItems.Count == 0)
                return;
            chatUser user = users.getUser(UserList.SelectedItem.ToString());
            if (user != null)
                MessageBox.Show(user.status);
        }
    }
}
