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
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication.Controllers
{
    [Authorize(Roles = "admin,uchet")]
    public class ClientsController : Controller
    {
        private readonly Context _context;

        public ClientsController(Context context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            ViewBag.UserRole = User.FindFirstValue(ClaimTypes.Role);
            return View(await _context.Client.ToListAsync());
        }


        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            ViewBag.UserRole = User.FindFirstValue(ClaimTypes.Role);

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            Client client = new Client();
            client.Discount = 0;
            return View(client);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Tel,Company,Discount")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create","Orders",new { ClientId = client.Id});
            }
            return View(client);
        }

        [Authorize(Roles = "admin")]
        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Tel,Company,Discount")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }


        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
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

        //логин   пароль
        //4r6543564  тойота-4879659843	
        //46543564  рено 123123пппп
        //46543564  тойота-4879659843

        public async Task<IActionResult> Login([Bind("Login,Password")] SystemUser model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Неверный номер телефона или номер договора";
                return View(model);
            }

            //найти всех клиентов с номером телефона, как во введенном логине
            Client client = await _context.Client.Include(client => client.Orders).FirstOrDefaultAsync(client => client.Tel == model.Login);
            if(client == null)
            {
                ViewBag.Err = "Неверный номер телефона или номер договора";
                return View(model);
            }
            //Если нашелся клиент, то проверяем его номер договора
            foreach (Order order in client.Orders)
            {
                if(order.OrderNumber == model.Password)
                {

                    //нашли - создаем клаймы с ролью доступа, именем подключившегося и id, чтобы его писать в базу
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,client.Name),
                        new Claim(ClaimTypes.Role,"user"),
                    };

                    var claimeIdentity = new ClaimsIdentity(claims, "Cookies");
                    var claimePrincipal = new ClaimsPrincipal(claimeIdentity);

                    await HttpContext.SignInAsync("Cookies", claimePrincipal);//добавляем клаймы в шифрованные куки

                    return RedirectToAction("ClientIndex", "Orders",client.Id);
                }
            }
            ViewBag.Err = "Неверный номер телефона или номер договора";
            return View(model);
        }
    }
}
