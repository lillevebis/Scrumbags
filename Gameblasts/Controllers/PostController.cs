
using Gameblasts.Models.PostViewModels;
using Microsoft.AspNetCore.Mvc;
//using Gameblast.Models;
using System.Threading.Tasks;
using Gameblasts.Data;
using Microsoft.AspNetCore.Identity;
using Gameblasts.Models;
using CategoryModels;

namespace Gameblasts.Controllers
{
    public class PostController : Controller
    {   
        private ApplicationDbContext ApplicationDbContext;
        private UserManager<ApplicationUser> UserManager { get; set; }
        private readonly SignInManager<ApplicationUser> SignInManager;
        
        public PostController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.ApplicationDbContext = db;
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public async Task<IActionResult> AddPost(AddEditPostViewModel vm, string title, string body, SubCategoryModel subcat)

        {   
            Post newpost = new Post(await GetCurrentUserAsync(), title, body, subcat);
            
            ApplicationDbContext.Posts.Add(newpost);
            ApplicationDbContext.SaveChanges();

            return View("../Home/Contact");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }
    }
}

