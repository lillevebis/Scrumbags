using System;
using System.ComponentModel.DataAnnotations;
using CategoryModels;

namespace Gameblasts.Models
{
    public class Post
    {

        public Post(){}
        public Post(ApplicationUser user, string title, string body, SubCategoryModel subcat)

        {
            this.Title = title;
            this.User = user;
            this.Body = body;
            this.Date = DateTime.Now;
            this.SubCat = subcat;
        }

        [Required]
        [KeyAttribute]
        public int Id{get; set;}

        [Required]
        public string Title{get; set;}

        [Required]
        public ApplicationUser User{get; set;}

        [Required]
        public DateTime Date{get; set;}

        [Required]
        public string Body{get; set;}

        [Required]
        public SubCategoryModel SubCat{get; set;}
    }
}