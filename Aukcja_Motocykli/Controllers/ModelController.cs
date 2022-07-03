using Aukcja_Motocykli.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aukcja_Motocykli.Database_Context;
using Aukcja_Motocykli.Models;
using Aukcja_Motocykli.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Aukcja_Motocykli.Controllers
{
    [Authorize(Roles = "Admin,Executive")]
    public class ModelController : Controller
    {
        private readonly DbContext_Aukcji _connectDB;

        [BindProperty]
        public ModelViewModel ModelVM { get; set; }

        public ModelController(DbContext_Aukcji connectdb)
        {
            _connectDB = connectdb;
            ModelVM = new ModelViewModel()
            {
                Makes = _connectDB.Makes.ToList(),
                Model = new Models.Model()
            };
        }

        public IActionResult Index()
        {
            var model = _connectDB.Models.Include(m => m.Make); 
            return View(model);
        }

        public IActionResult Create()
        {
            return View(ModelVM);
        }

        [HttpPost, ActionName("Create")]
        public IActionResult CreatePost()
        {
            if(!ModelState.IsValid)
            {
                return View (ModelVM);
            }
            _connectDB.Models.Add(ModelVM.Model);
            _connectDB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            ModelVM.Model = _connectDB.Models.Include(m => m.Make).SingleOrDefault(m => m.Id == id);
            if (ModelVM.Model == null)
            {
                return NotFound();
            }
            return View(ModelVM);
        }

        [HttpPost,ActionName("Edit")]
        public IActionResult EditPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelVM);
            }
            _connectDB.Update(ModelVM.Model);
            _connectDB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete (int id)
        {
            Model model = _connectDB.Models.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _connectDB.Models.Remove(model);
            _connectDB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

                
        [AllowAnonymous]
        [HttpGet("api/models/{MakeID}")]
        public IEnumerable<Model> Models(int MakeID)
        {
            return _connectDB.Models.ToList()
                .Where(m => m.MakeID == MakeID);
        }
        
        /*
        [AllowAnonymous]
        [HttpGet("api/models")]
        public IEnumerable<ModelResources> Models()
        {
            //return _connectDB.Models.ToList();
            var models = _connectDB.Models.ToList();
            return _mapper.Map<List<Model>, List<ModelResources>>(models);

            //var modelResources = models
            //    .Select(m => new ModelResources
            //    {
            //        Id = m.Id,
            //        Name = m.Name,
            //    }).ToList();
        }

        
        [AllowAnonymous]
        [HttpGet("api/models/{MakeID}")]
        public IEnumerable<Model> Models(int MakeID)
        {
            return _connectDB.Models.ToList()
                .Where(m => m.MakeID == MakeID);
        }
        */
    }
}