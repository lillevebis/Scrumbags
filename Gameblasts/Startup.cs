using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gameblasts.Data;
using Gameblasts.Models;
using Gameblasts.Models.CategoryModels;
using Gameblasts.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Gameblasts
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime when in default mode. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime when in Development Mode. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DevelopmentConnection")));


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // Called by Configure(). This needs an async function because
        // all the userManager and roleManager functions are async.
        public async Task CreateUsersAndRoles(IServiceScope serviceScope)
        {
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                // First create the admin role if it doesn't already exists.
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if(!await roleManager.RoleExistsAsync("Moderator"))
            {
                // Then create moderator role if it doesn't already exists.
                await roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

            if(!await roleManager.RoleExistsAsync("Member"))
            {
                // Then create member role if it doesn't already exists.
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }

            if(!await roleManager.RoleExistsAsync("Banned"))
            {
                // Then create banned role if it doesn't already exists.
                await roleManager.CreateAsync(new IdentityRole("Banned"));
            }

            if(!await roleManager.RoleExistsAsync("Writer"))
            {
                // Then create writer role if it doesn't already exists.
                await roleManager.CreateAsync(new IdentityRole("Writer"));
            }

            var user = await userManager.FindByNameAsync("admin@uia.no");
            if(user == null)
            {
                // Then add one admin user if it doesn't already exists.
                var adminUser = new ApplicationUser( "admin@uia.no", "admin@uia.no" );
                await userManager.CreateAsync(adminUser, "Password1.");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            user = await userManager.FindByNameAsync("moderator@uia.no");
            if(user == null)
            {
                // Add one moderator user if it doesn't already exists.
                var moderatorUser = new ApplicationUser ("moderator@uia.no", "moderator@uia.no");
                await userManager.CreateAsync(moderatorUser, "Password1.");
                await userManager.AddToRoleAsync(moderatorUser, "Moderator");
            }

            user = await userManager.FindByNameAsync("user@uia.no");
            if(user == null)
            {
                // Add one regular user if it doesn't already exists.
                var userUser = new ApplicationUser ("user@uia.no", "user@uia.no" );
                await userManager.CreateAsync(userUser, "Password1.");
                await userManager.AddToRoleAsync(userUser, "Member");
            }

            user = await userManager.FindByNameAsync("vebis");
            if(user == null)
            {
                // Add one regular user if it doesn't already exists.
                var userUser = new ApplicationUser ("vebis", "vebis@uia.no" );
                await userManager.CreateAsync(userUser, "Password1.");
                await userManager.AddToRoleAsync(userUser, "Member");
            }

            user = await userManager.FindByNameAsync("marty");
            if(user == null)
            {
                // Add one regular user if it doesn't already exists.
                var userUser = new ApplicationUser ("marty", "marty@uia.no" );
                await userManager.CreateAsync(userUser, "Password1.");
                await userManager.AddToRoleAsync(userUser, "Member");
            }            


        }

        public void CreateCategories(ApplicationDbContext db, string topCatName, string imageURL = null)
        {
            var TopCat1 = new CategoryModel(topCatName, null, imageURL);

            var SubCat1 = new CategoryModel("General Discussion", TopCat1);
            TopCat1.children.Add(SubCat1);
            var SubCat2 = new CategoryModel("News", TopCat1);
            TopCat1.children.Add(SubCat2);
            var SubCat3 = new CategoryModel("Media", TopCat1);
            TopCat1.children.Add(SubCat3);
            var SubCat4 = new CategoryModel("Looking to play", TopCat1);
            TopCat1.children.Add(SubCat4);
            var SubCat5 = new CategoryModel("Support", TopCat1);
            TopCat1.children.Add(SubCat5);
            
            db.Categories.Add(TopCat1);
            db.Categories.Include("CategoryModel");

            db.Categories.Add(SubCat1);
            db.Categories.Add(SubCat2);
            db.Categories.Add(SubCat3);
            db.Categories.Add(SubCat4);
            db.Categories.Add(SubCat5);
        }

            
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // Browser Link is not compatible with Kestrel 1.1.0
                // For details on enabling Browser Link, see https://go.microsoft.com/fwlink/?linkid=840936
                // app.UseBrowserLink()

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    // Add regular data here
                   
                   CreateCategories(db, "CsGO", "http://www.esports.je/media/com_jticketing/images/7a8f4596ae3d9971dbc0c620675d1958-Counter-Strike-Global-Offensive-Logo.jpg");
                   CreateCategories(db, "Dota2", "http://i.imgur.com/soaxrw9.png");
                   CreateCategories(db, "Hearthstone", "http://www.kassquatch.com/wp-content/uploads/2014/05/Hearthstone_Logo.png?w=240");
                   CreateCategories(db, "LoL", "http://support.lol.garena.com/img/league-logo.png");
                   CreateCategories(db, "Overwatch", "https://upload.wikimedia.org/wikipedia/commons/1/10/Overwatch_text_logo.svg");
                   CreateCategories(db, "QuakeChampions", "https://cdn.mmos.com/wp-content/uploads/2016/06/quake-champions-logo.jpg");
                   CreateCategories(db, "StarCraft2", "http://cdn3.dualshockers.com/wp-content/uploads/2010/08/starcraft_II_logo.png");
                   CreateCategories(db, "SuperSmash", "http://www.zeldainformer.com/supersmashbrostitle.jpg");
                   CreateCategories(db, "UnrealTournament", "http://2.bp.blogspot.com/-1rPuditcrUw/U2yYnZryA7I/AAAAAAAASpw/64DzBwpZoog/s1600/unreal-tournament.png");

                    // Then create the standard users and roles
                    CreateUsersAndRoles(serviceScope).Wait();
                    db.SaveChanges();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                "ProfilePage",
                "Profile/{id}",
                new { controller = "Profile", action = "Profile" , }
                );
            });
        }
    }
}
