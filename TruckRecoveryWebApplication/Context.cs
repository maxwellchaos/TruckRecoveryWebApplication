using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication
{
    public class Context : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<SystemUser> Users { get; set; }

        public DbSet<WebServiceTruckRecovery.Models.Client> Client { get; set; }

        public DbSet<WebServiceTruckRecovery.Models.OrderStatus> OrderStatus { get; set; }

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
                new Role {Id = 1, Name = "клиент"},
                new Role {Id = 2, Name = "админ"},
                new Role {Id = 3, Name = "сотрудник отдела учета"}
            });
            //первого пользователя
            modelBuilder.Entity<SystemUser>().HasData(new SystemUser[]
            {
                new SystemUser {Id = 1, RoleId = 2, CreatedDate = DateTime.Now,Login="admin",Password="admin"}
            });


        }
        public DbSet<WebServiceTruckRecovery.Models.SparePart> SparePart { get; set; }
        public DbSet<WebServiceTruckRecovery.Models.Repair> Repair { get; set; }
        public DbSet<WebServiceTruckRecovery.Models.SparePartsList> SparePartsList { get; set; }
    }
}
