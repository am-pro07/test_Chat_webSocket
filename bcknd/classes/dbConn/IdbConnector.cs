using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BckNd.classes.dbConn
{
    interface IdbConnector
    {
        public void init(object param);
        public Task dbConnect();
        public Task insertData(object data);
        public Task<object> getData(object wheres = null);
        public void Update(object row);
    }
}
