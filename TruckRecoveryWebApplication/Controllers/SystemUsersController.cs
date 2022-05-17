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
    [Authorize(Roles = "admin")]
    public class SystemUsersController : Controller
    {
        private readonly Context _context;

        //доступ всем
        //Залогиниться - показать страницу
        [AllowAnonymous]
        // GET: Users
        public IActionResult Login(string ReturnUrl)
        {
            if (User.FindFirstValue(ClaimTypes.Role) == "client")
            {
                return RedirectToAction("Index", "DearClients");
            }
            return View();

        }


        //доступ всем
        //Залогиниться проверить пароль
        [HttpPost]
        [AllowAnonymous]
        // GET: Users
        public async Task<IActionResult> Login([Bind("Login,Password")] SystemUser model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Неверный логин или пароль";
                return View(model);
            }
            //получить хеш пароля
            string hash = Context.GetHashString(model.Password); 
            //ищем в бд пару логин-пароль
            SystemUser UserLogin = await _context.Users.Include(u => u.Role)
                .Where(u => u.Login == model.Login && u.Password == hash).SingleOrDefaultAsync();


            if (UserLogin == null)//не нашли
            {
                ViewBag.Err = "Неверный логин или пароль";
                return View(model);
            }
            //нашли - создаем клаймы с ролью доступа, именем подключившегося и id, чтобы его писать в базу
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,UserLogin.Name),
                new Claim(ClaimTypes.Role,UserLogin.Role.Name),
                new Claim(ClaimTypes.NameIdentifier,UserLogin.Id.ToString())
            };

            
            var claimeIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimePrincipal = new ClaimsPrincipal(claimeIdentity);

            await HttpContext.SignInAsync("Cookies", claimePrincipal);//добавляем клаймы в шифрованные куки

            if (model.ReturnUrl == null)
            {
                //если некуда перенаправлять - перенаправляю к списку заказов
                return RedirectToAction("Index", "Orders");
            }
            return Redirect(model.ReturnUrl);//перенаправляем на нужную страницу, на которую попытались получить доступ
        }

        public SystemUsersController(Context context)
        {
            _context = context;
        }

        //доступнео всем
        //разлогиниться - зайти под другим userом
        // GET: Users
        [AllowAnonymous]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Redirect("/");

        }

        // GET: SystemUsers
        public async Task<IActionResult> Index()
        {
            var context = _context.Users.Include(s => s.Role);
            return View(await context.ToListAsync());
        }

        // GET: SystemUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUser = await _context.Users
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemUser == null)
            {
                return NotFound();
            }

            return View(systemUser);
        }

        // GET: SystemUsers/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            return View();
        }

        // POST: SystemUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,CreatedDate,RoleId")] SystemUser systemUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", systemUser.RoleId);
            return View(systemUser);
        }

        // GET: SystemUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUser = await _context.Users.FindAsync(id);
            if (systemUser == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", systemUser.RoleId);
            return View(systemUser);
        }

        // POST: SystemUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,Password,CreatedDate,RoleId")] SystemUser systemUser)
        {
            if (id != systemUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemUserExists(systemUser.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", systemUser.RoleId);
            return View(systemUser);
        }

        // GET: SystemUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUser = await _context.Users
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemUser == null)
            {
                return NotFound();
            }

            return View(systemUser);
        }

        // POST: SystemUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemUser = await _context.Users.FindAsync(id);
            _context.Users.Remove(systemUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemUserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
