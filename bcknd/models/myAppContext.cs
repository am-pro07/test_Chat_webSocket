using FrntNd.classes;
using Microsoft.EntityFrameworkCore;
using System;

namespace BckNd.models
{
    class myAppContext: DbContext, IDisposable
    {
        public DbSet<chatUser> Users { get; set; }
        public myAppContext(DbContextOptions<myAppContext> options) : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
