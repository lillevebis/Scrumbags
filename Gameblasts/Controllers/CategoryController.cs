using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gameblasts.Data;
using Gameblasts.Models;
using Gameblasts.Models.CategoryModels;
using Gameblasts.Models.PostViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gameblasts.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db;

        private UserManager<ApplicationUser> UserManager { get; set; }
        private readonly SignInManager<ApplicationUser> SignInManager;

        public CategoryController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.db = db;
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }
        
        [HttpGet]
        public IActionResult Forum()
        {
            var catList = db.Categories.Where(s => s.parent == null).Include("children").ToList();
            return View(catList);
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public IActionResult CreateCategory(CategoryFormModel model)
        {
            CategoryModel newCat;
            if (db.Categories.Find(model.parentID) != null)
            {
            var parentCatList = db.Categories.Where(s => s.id == model.parentID).Include("children").Distinct().ToList();
            var parentCat = parentCatList.First();           
                newCat = new CategoryModel(model.name, parentCat, model.imageURL);
                parentCat.children.Add(newCat);
            } else
                newCat = new CategoryModel(model.name, null, model.imageURL);

            db.Categories.Add(newCat);
            db.SaveChanges();
            return View("Forum", db.Categories.Where(s =>  s.parent == null).Include("children").ToList());
        }

        [Authorize(Roles="Admin")]
        public IActionResult AddCategory(CategoryFormModel formModel)
        {
            var model = new CategoryModel(null, db.Categories.Find(formModel.parentID), null);
            return View(model);
        }

        
        public IActionResult AddThread(int id)
        {
            var model = new Post(null, null, null, id);
            return View(model);
        }

        public async Task<IActionResult> CreateThread(Post model)
        {
            var parentCat = await db.Categories.Where(c => c.id == model.SubCategory).Include("children").FirstAsync();
            var user =  await UserManager.GetUserAsync(HttpContext.User);
            var thread = new CategoryModel(model.Title, db.Categories.Find(model.SubCategory));
            db.Categories.Add(thread);
            db.SaveChanges();
            Post OP = new Post(user, model.Title, model.Body, thread.id);
            thread.threads.Add(OP);
            db.Posts.Add(OP);
            parentCat.children.Add(thread);
            db.SaveChanges();

            thread = db.Categories.Where(s => s.id == thread.id).Include("threads").Include("parent").Include("threads.User").First();
            return View("thread", thread);
        }

        [HttpPost]
        public IActionResult AddSubCategoryDemo(CategoryFormModel model)
        {
            var parentCatList = db.Categories.Where(s => s.id == model.parentID).Include("children").Distinct().ToList();
            var parentCat = parentCatList.First();            
            var newCat = new CategoryModel(model.name, parentCat);
            parentCat.children.Add(newCat);
            db.Categories.Add(newCat);
            db.SaveChanges();
            return View("ForumCategoryDemo", db.Categories.Where(s =>  s.parent == null).Include("children").ToList());
        }

        [HttpPost]
        public IActionResult ForumPost(CategoryModel category)
        {
            if (category == null)
                return View("../Home/Forum");

            var model = new Post(null, null, null, category.id);
            return View(model);
        }

        public IActionResult OpenCategory(int id)
        {
            var Category = db.Categories.Where(e => e.id == id).Include("children").Include("threads").Include("children.threads.User").First();
            return View("SubCatForum", Category);
        }
        public IActionResult OpenTopCategory(int id)
        {
            var Category = db.Categories.Where(e => e.id == id).Include("children").Include("threads").First();
            return View("TopCatForum", Category);
        }

        public IActionResult OpenThread(int id)
        {
            var Category = db.Categories.Where(e => e.id == id).Include("parent").Include("threads").Include("threads.User").First();
            return View("thread", Category);
        }
    }
}

