#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruckRecoveryWebApplication;
using TruckRecoveryWebApplication.Models;

namespace TruckRecoveryWebApplication.Controllers
{
    [Authorize(Roles = "admin,uchet")]
    public class RepairsController : Controller
    {
        private readonly Context _context;

        public RepairsController(Context context)
        {
            _context = context;
        }

     
        // GET: Repairs/Create/5
        // или
        // GET: Repairs/Create
        public IActionResult Create(int? OrderId)
        {
            if(OrderId != null)
            {
                Repair repair = new Repair();
                repair.OrderId = (int)OrderId;
                ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
                return View(repair);
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            return View();
        }

        // POST: Repairs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,uchet")]
        public async Task<IActionResult> Create([Bind("Name,Price,OrderId")] Repair repair)
        {
            if (ModelState.IsValid)
            {
                _context.Add(repair);
                Log.AddLog(repair.OrderId, "Добавлена работа "+repair.Name+" стоимостью "+repair.Price.ToString(), _context, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

                await _context.SaveChangesAsync();
                
                //перенаправляю на диагностику
                return RedirectToAction("Diagnostics", "Orders", new { id = repair.OrderId });
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", repair.OrderId);
            return View(repair);
        }

        // GET: Repairs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var repairDelete = await _context.Repair.FindAsync(id);
            //сохраняю идентификатор перед удалением
            int OrderId = repairDelete.OrderId;
            Log.AddLog(OrderId, "Удалена работа " + repairDelete.Name + " стоимостью " + repairDelete.Price.ToString(), _context, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            _context.Repair.Remove(repairDelete);

            await _context.SaveChangesAsync();
            //перенаправляю на диагностику
            return RedirectToAction("Diagnostics", "Orders", new { id = OrderId });
           
        }

       
        // POST: Repairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var repair = await _context.Repair.FindAsync(id);
            _context.Repair.Remove(repair);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RepairExists(int id)
        {
            return _context.Repair.Any(e => e.Id == id);
        }
    }
}
