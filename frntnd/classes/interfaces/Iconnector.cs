using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrntNd.classes.interfaces
{
    /// <summary>
    /// интерфейс для коннекта
    /// </summary>
    interface Iconnector
    {
        void initialize(Dictionary<string, object> parameters);
        Task open();
        Task close(object status, string description);
        Task sendString(string str);
        Task<string> reciveString(); 
    }
}
