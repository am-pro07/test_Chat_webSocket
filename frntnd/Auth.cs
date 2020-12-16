using System;
using System.Text;
using System.Windows.Forms;
using System.Net.WebSockets;
using System.Threading;
using FrntNd.classes;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FrntNd
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }
        /// <summary>
        /// наш основной коннектор для доступа к сервера
        /// </summary>
        connector c;

        private async void button1_Click(object sender, EventArgs e)
        {
            //определяем себя
            chatUser me = new chatUser();
            me.login = loginBox.Text;
            chatPacket packet = new chatPacket();
            //готовим пакет на аутентификацию
            string strJson = JsonConvert.SerializeObject(packet.authCommand(me));
            //формируем коннектор
            c = new connector();
            //готовим параметры подключения
            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("uri", new Uri("ws://localhost:64636/Chat"));
            //инициализируем подключение
            c.initialize(d);
            //открываем подключение
            await c.open();
            //отправляем подключение
            await c.sendString(strJson);
            //получаем ответ
            string message = await c.reciveString();
            //разворачиваем ответ
            chatMessage answer = JsonConvert.DeserializeObject<chatMessage>(message);
            //если получили ответ с guid то считаем что подключение удалось
            if (answer.userSender.id != Guid.Empty)
            {
                //открываем основную форму
                ((ChatForm)this.Owner).saveConnection(c, answer.userSender);
                this.Close();
            }
        }



    }
}
