#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruckRecoveryWebApplication;
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Context _context;

        public OrdersController(Context context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders.Include(o => o.Client).Include(o => o.Status);
            //перебрать все заказы и исправить статус, если дата доставки уже прошла а статус еще "ожидает запчастей".
            foreach(Order order in orders)
            {
                if(order.DeliveryPartsDate <= DateTime.Now && order.StatusId == 2)
                {
                    order.StatusId = 3;
                    _context.Update(order);
                }
            }

            //Обновляю данные в БД
            await _context.SaveChangesAsync();
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/FinishRepair
        public async Task<IActionResult> FinishRepair(int? OrderId)
        {
            if (OrderId == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(OrderId);
            if(order.StatusId!=3)
            {
                return BadRequest();
            }
            order.StatusId = 4;
            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,OrderNumber,TruckList")] Order order)
        {
            order.CreatedDate = DateTime.Now;
            order.StatusId = 1;

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", order.ClientId);
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status", order.StatusId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", order.ClientId);
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status", order.StatusId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedDate,ClientId,OrderNumber,StatusId,TruckList,DiagnosticsDate,DiagnosticReport,Price,DiscountedPrice,DeliveryPartsDate,CloseDate,IsClosed")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", order.ClientId);
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status", order.StatusId);
            return View(order);
        }

        // GET: Orders/Diagnostics/5
        public async Task<IActionResult> Diagnostics(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
          
            var order = await _context.Orders
                .Include(order=> order.SparePartsList)
                    .ThenInclude(spare=>spare.SparePart)
                .Include(order => order.Repairs)
                .Include(order => order.Client)
                .FirstOrDefaultAsync(order => order.Id == id);

            //пересчет цены и даты
            order.Price = 0;
          //считаю цену работ
            foreach(Repair repair in order.Repairs)
            {
                
                order.Price += repair.Price;
            }

            //если не будет найдено ни одной даты, то ставим сегодняшнюю дату
            //тогда заказ автоматически переведется на стадию "Ожидание ремонта"
            order.DeliveryPartsDate = DateTime.Now;
            //цена и даты запчастей
            foreach (SparePartsList sparePartsList in order.SparePartsList)
            {
                //пересчитываю дату
                if (order.DeliveryPartsDate < sparePartsList.DeliveryDate)
                {
                    order.DeliveryPartsDate = sparePartsList.DeliveryDate;
                }
                //увелисиваю цену
                order.Price += sparePartsList.Count*sparePartsList.SparePart.Price;
            }

            //Пересчет цены со скидкой
            order.DiscountedPrice = order.Price-order.Price*order.Client.Discount/100;

            //Сохраняю пересчитанные данные
            _context.Update(order);
            if (order == null)
            {
                return NotFound();
            }
            order.DiagnosticsDate = DateTime.Now;
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", order.ClientId);
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status", order.StatusId);
            return View(order);
        }

        // POST: Orders/Diagnostics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Diagnostics(int id, [Bind("Id,CreatedDate,ClientId,OrderNumber,StatusId,TruckList,DiagnosticsDate,DiagnosticReport,Price,DiscountedPrice,DeliveryPartsDate,CloseDate,IsClosed")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.StatusId = 2;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", order.ClientId);
            ViewData["StatusId"] = new SelectList(_context.OrderStatus, "Id", "Status", order.StatusId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Orders/CloseOrder/5
        public async Task<IActionResult> CloseOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/CloseOrder/5
        [HttpPost, ActionName("CloseOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.CloseDate = DateTime.Now;
            order.StatusId = 5;
            order.IsClosed = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
