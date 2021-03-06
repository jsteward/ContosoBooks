﻿using ContosoBooks.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoBooks.Common;
using Microsoft.Extensions.Caching.Memory;

namespace ContosoBooks.Controllers
{
    [Route("[controller]")]
    public class BookController : Controller
    {
        [FromServices]
        public BookContext BookContext { get; set; }
       

        [HttpGet("Index")]
        [Cacheable(10, 15)]
        public IActionResult Index()
        {
            return View(GetBooks());
        }

        [HttpGet]
        [Cacheable(10, 15)]
        
        public IActionResult Get()
        {
            return new ObjectResult(GetBooks());
        }

        public List<Book> GetBooks()
        {
            return BookContext.Books.Include(b => b.Author).ToList();
        }

        public async Task<ActionResult> Details(int id)
        {
            Book book = await BookContext.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.BookID == id);
            if (book == null)
            {
               
                return HttpNotFound();
            }
            return View(book);
        }

        public ActionResult Create()
        {
            ViewBag.Items = GetAuthorsListItems();
            return View();
        }

        private IEnumerable<SelectListItem> GetAuthorsListItems(int selected = -1)
        {
            var tmp = BookContext.Authors.ToList();  
            return tmp
                .OrderBy(author => author.LastName)
                .Select(author => new SelectListItem
                {
                    Text = String.Format("{0}, {1}", author.LastName, author.FirstMidName),
                    Value = author.AuthorID.ToString(),
                    Selected = author.AuthorID == selected
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Title", "Year", "Price", "Genre", "AuthorID")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BookContext.Books.Add(book);
                    await BookContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }
            return View(book);
        }
    }
}