using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aukcja_Motocykli.Models;
using Aukcja_Motocykli.Database_Context;
using Microsoft.AspNetCore.Authorization;

namespace Aukcja_Motocykli.Controllers
{
    [Authorize(Roles = "Admin,Executive")]
    public class MakeController : Controller
    {
        private readonly DbContext_Aukcji _connectDB;

        public MakeController(DbContext_Aukcji connectDB)
        {
            _connectDB = connectDB;
        }

        public IActionResult Index()
        {
            return View(_connectDB.Makes.ToList());
        }

        //HTTP Get Method
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Make make)
        {
            if (ModelState.IsValid)
            {
                _connectDB.Add(make);
                _connectDB.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(make);
        }

        //Post Method
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var make = _connectDB.Makes.Find(id);
            if (make == null)
            {
                return NotFound();
            }
            _connectDB.Makes.Remove(make);
            _connectDB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var make = _connectDB.Makes.Find(id);
            if (make == null)
            {
                return NotFound();
            }
            return View(make);
        }

        [HttpPost]
        public IActionResult Edit(Make make)
        {
            if (ModelState.IsValid)
            {
                _connectDB.Update(make);
                _connectDB.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(make);
        }
    }
}