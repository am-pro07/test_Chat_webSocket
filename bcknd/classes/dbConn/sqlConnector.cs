using BckNd.models;
using FrntNd.classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BckNd.classes.dbConn
{
    /// <summary>
    /// класс, реализующий интерфейс подключения для SQL
    /// </summary>
    public class sqlConnector : IdbConnector
    {
        /// <summary>
        /// //контекст для entity
        /// </summary>
        myAppContext db;
        
        /// <summary>
        /// подключение 
        /// </summary>
        /// <returns></returns>
        public Task dbConnect()
        {
            return null;
        }

        /// <summary>
        /// получение данных
        /// </summary>
        /// <param name="wheres">условие</param>
        /// <returns></returns>
        public async Task<object> getData(object wheres)
        {
            return await db.Users.Where(a => a.login == (string)wheres).ToArrayAsync();
        }

        /// <summary>
        /// инициализация подключения
        /// </summary>
        /// <param name="param"></param>
        public void init(object param)
        {
            db = (myAppContext)param;
        }
        /// <summary>
        /// вставка данных
        /// </summary>
        /// <param name="data">сами данные</param>
        /// <returns></returns>
        public Task insertData(object data)
        {
            db.Users.AddAsync((chatUser)data);
            return db.SaveChangesAsync();
        }

        /// <summary>
        /// обновление данных
        /// </summary>
        /// <param name="row"></param>
        public async void Update(object row)
        {
            var user = db.Users.Where(c => c.id == ((chatUser)row).id).FirstOrDefault();
            user.status = ((chatUser)row).status;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

        }
    }
}
