using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TruckRecoveryWebApplication.Models;

namespace TruckRecoveryWebApplication
{
    public class Context : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<SystemUser> Users { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.Client> Client { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.OrderStatus> OrderStatus { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.SparePart> SparePart { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.Repair> Repair { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.SparePartsList> SparePartsList { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.Log> Log { get; set; }
        public DbSet<TruckRecoveryWebApplication.Models.Role> Role { get; set; }


        public Context(DbContextOptions<Context> options) : base(options)
        {
            
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Добавляю стандартные статусы
            modelBuilder.Entity<OrderStatus>().HasData(new OrderStatus[]
            {
                new OrderStatus { Id = 1, Status = "Ожидает диагностику" },//создан
                new OrderStatus { Id = 2, Status = "Ожидает запчасти" },//проведена диагностика, ждем запчасти
                new OrderStatus { Id = 3, Status = "На ремонте" }, //Ожидаем начала ремонта или идет ремонт
                new OrderStatus { Id = 4, Status = "Сдача заказа" }, //Ремонт выполнен, ждем заказчика, чтобы сдать заказ
                new OrderStatus { Id = 5, Status = "Заказ закрыт" } //Акты подписаны, оплата получена
            });
            //роли по умолчанию
            modelBuilder.Entity<Role>().HasData(new Role[]
            {
                new Role {Id = 1, Name = "client"},
                new Role {Id = 2, Name = "admin"},
                new Role {Id = 3, Name = "uchet"}
            });
            //первого пользователя
            modelBuilder.Entity<SystemUser>().HasData(new SystemUser[]
            {
                new SystemUser {Id = 1, RoleId = 2, Name = "Администратор", CreatedDate = DateTime.Now, Login="admin", Password=GetHashString("admin")},
                new SystemUser {Id = 2, RoleId = 3, Name = "Сотрудник отдела учета", CreatedDate = DateTime.Now, Login="Uchet", Password=GetHashString("Uchet")}
            });
        }

        /// <summary>
        /// считает хэш от пароля
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string GetHashString(string text)
        {
            //перевожу строку с паролем в массив байт
            //Ну и добавлю немного соли ;)
            //подробнее про соль здесь https://ru.wikipedia.org/wiki/%D0%A1%D0%BE%D0%BB%D1%8C_(%D0%BA%D1%80%D0%B8%D0%BF%D1%82%D0%BE%D0%B3%D1%80%D0%B0%D1%84%D0%B8%D1%8F)
            var pbytes = Encoding.UTF8.GetBytes(text + "СОЛЬ");
            
            //хеширую этот массив байт
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(pbytes);
            
            //Преобразую массив байт в строку Base64
            //подробнее про base64 здесь https://ru.wikipedia.org/wiki/Base64
            return Convert.ToBase64String(hash);
        }
    }
}
