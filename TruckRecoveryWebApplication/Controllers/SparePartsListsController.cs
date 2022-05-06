#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruckRecoveryWebApplication;
using TruckRecoveryWebApplication.Models;
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication.Controllers
{
    public class SparePartsListsController : Controller
    {
        private readonly Context _context;

        public SparePartsListsController(Context context)
        {
            _context = context;
        }

        // GET: SparePartsLists
        public async Task<IActionResult> Index()
        {
            var context = _context.SparePartsList.Include(s => s.Order).Include(s => s.SparePart);
            return View(await context.ToListAsync());
        }

        // GET: SparePartsLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sparePartsList = await _context.SparePartsList
                .Include(s => s.Order)
                .Include(s => s.SparePart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sparePartsList == null)
            {
                return NotFound();
            }

            return View(sparePartsList);
        }

        // GET: SparePartsLists/Create
        public IActionResult Create(int? OrderId)
        {
            if (OrderId != null)
            {
                //Если получили id
                SparePartsList sparePartsList = new SparePartsList();
                sparePartsList.OrderId = (int)OrderId;
                ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
                ViewData["SparePartId"] = new SelectList(_context.SparePart, "Id", "Name");
                return View(sparePartsList);
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["SparePartId"] = new SelectList(_context.SparePart, "Id", "Name");
            return View();
        }

        // POST: SparePartsLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SparePartId,Count,OnStock,DeliveryDate,OrderId")] SparePartsList sparePartsList)
        {
            if (ModelState.IsValid)
            {
                if (sparePartsList.DeliveryDate == null)
                    sparePartsList.DeliveryDate = DateTime.Now;
                SparePart sparePart = await _context.SparePart.FindAsync(sparePartsList.SparePartId);
                string spareString = "Добавлены запчасти " + sparePart.Name
                    + " в количестве " + sparePartsList.Count.ToString() 
                    + " с датой доставки " + sparePartsList.DeliveryDate.ToString();
                Log.AddLog(sparePartsList.OrderId, spareString, _context);

                _context.Add(sparePartsList);
                await _context.SaveChangesAsync();
                //перенаправляю на диагностику
                return RedirectToAction("Diagnostics", "Orders", new { id = sparePartsList.OrderId });
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", sparePartsList.OrderId);
            ViewData["SparePartId"] = new SelectList(_context.SparePart, "Id", "Name", sparePartsList.SparePartId);
            return View(sparePartsList);
        }

        // GET: SparePartsLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sparePartsList = await _context.SparePartsList.FindAsync(id);
            if (sparePartsList == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", sparePartsList.OrderId);
            ViewData["SparePartId"] = new SelectList(_context.SparePart, "Id", "Name", sparePartsList.SparePartId);
            return View(sparePartsList);
        }

        // POST: SparePartsLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SparePartId,Count,OnStock,DeliveryDate,OrderId")] SparePartsList sparePartsList)
        {
            if (id != sparePartsList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sparePartsList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SparePartsListExists(sparePartsList.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", sparePartsList.OrderId);
            ViewData["SparePartId"] = new SelectList(_context.SparePart, "Id", "Name", sparePartsList.SparePartId);
            return View(sparePartsList);
        }

        // GET: SparePartsLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var sparePartsList = await _context.SparePartsList.Include(s=>s.SparePart).FirstAsync(s=>s.Id==id);
            //сохраняю идентификатор перед удалением
            int OrderId = sparePartsList.OrderId;

            string spareString = "Удалены запчасти " + sparePartsList.SparePart.Name
                   + " в количестве " + sparePartsList.Count.ToString()
                   + " с датой доставки " + sparePartsList.DeliveryDate.ToString();
            Log.AddLog(sparePartsList.OrderId, spareString, _context);

            _context.SparePartsList.Remove(sparePartsList);
            await _context.SaveChangesAsync();
            //перенаправляю на диагностику
            return RedirectToAction("Diagnostics", "Orders", new { id = OrderId });
        }

        private bool SparePartsListExists(int id)
        {
            return _context.SparePartsList.Any(e => e.Id == id);
        }
    }
}
