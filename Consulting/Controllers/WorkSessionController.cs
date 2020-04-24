using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Consulting.Models;
using Microsoft.AspNetCore.Http;

namespace Consulting.Controllers
{
    public class WorkSessionController : Controller
    {
        private readonly ConsultingContext _context;

        public WorkSessionController(ConsultingContext context)
        {
            _context = context;
        }

        // GET: WorkSession
        public async Task<IActionResult> Index(string ContractID)
        {
            if (!string.IsNullOrEmpty(ContractID))
            {
                Response.Cookies.Append("ContractID", ContractID);
                HttpContext.Session.SetString("ContractID", ContractID);
            }
            else if (Request.Query["ContractID"].Any())
            {
                Response.Cookies.Append("ContractID", Request.Query["ContractID"].ToString());
                HttpContext.Session.SetString("ContractID", Request.Query["ContractID"].ToString());
                ContractID = Request.Query["ContractID"].ToString();
            }
            else if (Request.Cookies["ContractID"] != null)
            {
                ContractID = Request.Cookies["ContractID"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractID") != null)
            {
                ContractID = HttpContext.Session.GetString("ContractID");
            }
            else
            {
                TempData["message"]= "Please select Contarct";
                return RedirectToAction("Index","Contract");
            }
            var consultingContext = _context.WorkSession.Include(w => w.Consultant).Include(w => w.Contract)
                .Where(a => a.ContractId == Convert.ToInt32(ContractID));

            var workSession = consultingContext.ToList();
            ViewBag.totalHours = workSession.Sum(a => a.HoursWorked);
            ViewBag.totalCost = workSession.Sum(a => a.HoursWorked * a.HourlyRate);
            return View(await consultingContext.ToListAsync());
        }

        // GET: WorkSession/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // GET: WorkSession/Create
        public IActionResult Create()
        {
            string ContractID = String.Empty;
            if (Request.Cookies["ContractID"] != null)
            {
                ContractID = Request.Cookies["ContractID"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractID") != null)
            {
                ContractID = HttpContext.Session.GetString("ContractID");
            }
            ViewBag.Contract = ContractID;

            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName");
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name");
            return View();
        }

        // POST: WorkSession/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            string ContractID = String.Empty;
            if (Request.Cookies["ContractID"] != null)
            {
                ContractID = Request.Cookies["ContractID"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractID") != null)
            {
                ContractID = HttpContext.Session.GetString("ContractID");
            }
            ViewBag.Contract = ContractID;
            try
            { 
            if (ModelState.IsValid)
            {
                 
                _context.Add(workSession);
                await _context.SaveChangesAsync();
                TempData["message"] = "Worksession created successfully.";
                return RedirectToAction(nameof(Index));
            }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                TempData["message"] = ex.GetBaseException().Message;
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: WorkSession/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string ContractID = String.Empty;
            if (Request.Cookies["ContractID"] != null)
            {
                ContractID = Request.Cookies["ContractID"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractID") != null)
            {
                ContractID = HttpContext.Session.GetString("ContractID");
            }
            ViewBag.Contract = ContractID;
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession.FindAsync(id);
            if (workSession == null)
            {
                return NotFound();
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            ViewBag.Contract = workSession.ContractId;
            return View(workSession);
        }

        // POST: WorkSession/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            string ContractID = String.Empty;
            if (Request.Cookies["ContractID"] != null)
            {
                ContractID = Request.Cookies["ContractID"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractID") != null)
            {
                ContractID = HttpContext.Session.GetString("ContractID");
            }
            ViewBag.Contract = ContractID;
            if (id != workSession.WorkSessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkSessionExists(workSession.WorkSessionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.GetBaseException().Message);
                    TempData["message"] = ex.GetBaseException().Message;
                }
                TempData["message"] = "Worksession record updated.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: WorkSession/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // POST: WorkSession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workSession = await _context.WorkSession.FindAsync(id);

            try { 
            _context.WorkSession.Remove(workSession);
            await _context.SaveChangesAsync();
            TempData["message"] = "Worksession deleted sucessfully";
            }
            catch(Exception ex) {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                TempData["message"] = ex.GetBaseException().Message;
            }
            return RedirectToAction(nameof(Index));
            
           
        }
           

        private bool WorkSessionExists(int id)
        {
            return _context.WorkSession.Any(e => e.WorkSessionId == id);
        }
    }
}
