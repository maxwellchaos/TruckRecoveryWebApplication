#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruckRecoveryWebApplication;
using TruckRecoveryWebApplication.Models;

namespace TruckRecoveryWebApplication.Controllers
{
    /// <summary>
    /// контроллер отвечает за обработку интерфейса для наших дорогих и уважаемых клиентов
    /// </summary>
      [Authorize(Roles = "client")]
    public class DearClientsController : Controller
    {
        private readonly Context _context;

        public DearClientsController(Context context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl)
        {
            return View();

        }

        //доступ всем
        //Залогиниться проверить пароль
        // GET: Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Bind("Login,Password")] SystemUser model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Неверный номер телефона или номер договора";
                return View(model);
            }

            //найти всех клиентов с номером телефона, как во введенном логине
            Client client = await _context.Client.Include(client => client.Orders).FirstOrDefaultAsync(client => client.Tel == model.Login);
            if (client == null)
            {
                ViewBag.Err = "Неверный номер телефона или номер договора";
                return View(model);
            }
           // Role role = await _context.Role.FindAsync(1);
            //Если нашелся клиент, то проверяем его номер договора
            foreach (Order order in client.Orders)
            {
                if (order.OrderNumber == model.Password)
                {

                    //нашли - создаем клаймы с ролью доступа, именем подключившегося и id, чтобы его писать в базу
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,client.Name),
                        new Claim(ClaimTypes.Role,"client"),
                        new Claim(ClaimTypes.NameIdentifier,client.Id.ToString())
                    };

                    var claimeIdentity = new ClaimsIdentity(claims, "Cookie");
                    var claimePrincipal = new ClaimsPrincipal(claimeIdentity);

                    await HttpContext.SignInAsync("Cookies", claimePrincipal);//добавляем клаймы в шифрованные куки

                    return RedirectToAction("Index", "DearClients", client.Id);
                }
            }
            ViewBag.Err = "Неверный номер телефона или номер договора";
            return View(model);
        }


        // GET: DearClients
        public async Task<IActionResult> Index(int ClientId)
        {
            if (ClientId == 0)
            {
                ClientId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            var client = await _context.Client
                .Include(client => client.Orders)
                    .ThenInclude(order=>order.Status)
                .FirstOrDefaultAsync(client => client.Id == ClientId);
            return View(client);
        }

        // GET: DearClients/Details/5
        public async Task<IActionResult> Details(int? OrderId)
        {
            if (OrderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(order=>order.Status)
                .FirstOrDefaultAsync(m => m.Id == OrderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
    }
}
