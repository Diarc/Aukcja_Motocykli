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
using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;
using cloudscribe.Pagination.Models;

namespace Aukcja_Motocykli.Controllers
{
    [Authorize(Roles = "Admin,Executive")]
    public class BikeController : Controller
    {
        private readonly DbContext_Aukcji _connectDB;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public BikeViewModel BikeVM { get; set; }

        public BikeController(DbContext_Aukcji connectdb, HostingEnvironment hostingEnvironment)
        {
            _connectDB = connectdb;
            _hostingEnvironment = hostingEnvironment;
            BikeVM = new BikeViewModel()
            {
                Makes = _connectDB.Makes.ToList(),
                Models = _connectDB.Models.ToList(),
                Bike = new Models.Bike()
            };
        }

        public IActionResult Index(string searchString, string sortOrder, int pageNumber=1, int pageSize=3)
        {
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.PriceSortStorm = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var Bikes = from b in _connectDB.Bikes.Include(m => m.Make).Include(m => m.Model)
                        select b;

            var BikeCount = Bikes.Count();

            if (!String.IsNullOrEmpty(searchString))
            {
                Bikes = Bikes.Where(b => b.Make.Name.Contains(searchString));
                BikeCount = Bikes.Count();
            }

            //Sorting Logic
            switch (sortOrder)
            {
                case "price_desc":
                    Bikes = Bikes.OrderByDescending(b => b.Price);
                    break;
                default:
                    Bikes = Bikes.OrderBy(b => b.Price);
                    break;
            }

                Bikes = Bikes
                .Skip(ExcludeRecords)
                    .Take(pageSize);

            var result = new PagedResult<Bike>
            {
                Data = Bikes.AsNoTracking().ToList(),
                TotalItems = BikeCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(result);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            BikeVM.Bike = _connectDB.Bikes.SingleOrDefault(b => b.Id == id);

            //Filter the models associated to the selected make
            BikeVM.Models = _connectDB.Models.Where(m => m.MakeID == BikeVM.Bike.MakeID);

            if (BikeVM.Bike == null)
            {
                return NotFound();
            }
            return View(BikeVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _connectDB.Makes.ToList();
                BikeVM.Models = _connectDB.Models.ToList();
                return View(BikeVM);
            }
            _connectDB.Bikes.Update(BikeVM.Bike);

            UploadImageIfAvailable();

            _connectDB.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //Get Method
        public IActionResult Create()
        {
            return View(BikeVM);
        }

        //Post Method
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _connectDB.Makes.ToList();
                BikeVM.Models = _connectDB.Models.ToList();
                return View(BikeVM);
            }
            _connectDB.Bikes.Add(BikeVM.Bike);

            UploadImageIfAvailable();

                _connectDB.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private void UploadImageIfAvailable()
        {
            //Get Bike id we have saved in database
            var BikeID = BikeVM.Bike.Id;

            //Get wwwrootpath to save the file on server
            string wwwrootPath = _hostingEnvironment.WebRootPath;

            //Get uploaded files
            var files = HttpContext.Request.Form.Files;

            //Get the reference of DBSet for the bike we just have saved in database
            var SavedBike = _connectDB.Bikes.Find(BikeID);

            //Upload the files on server and save the image path of user that have uploaded any file
            if (files.Count != 0)
            {
                var ImagePath = @"images\bike\";
                var Extension = Path.GetExtension(files[0].FileName);
                var RelativeImagePath = ImagePath + BikeID + Extension;
                var AbsImagePath = Path.Combine(wwwrootPath, RelativeImagePath);

                //Upload the file on server
                using (var fileStream = new FileStream(AbsImagePath, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                //Set the image path on database
                SavedBike.ImagePath = RelativeImagePath;
            }
        }

        //public IActionResult Edit(int id)
        //{
        //    BikeVM.Model = _connectDB.Models.Include(m => m.Make).SingleOrDefault(m => m.Id == id);
        //    if (BikeVM.Model == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(BikeVM);
        //}

        //[HttpPost, ActionName("Edit")]
        //public IActionResult EditPost(int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(BikeVM);
        //    }
        //    _connectDB.Update(BikeVM.Model);
        //    _connectDB.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Bike bike = _connectDB.Bikes.Find(id);
            if (bike == null)
            {
                return NotFound();
            }
            _connectDB.Bikes.Remove(bike);
            _connectDB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}