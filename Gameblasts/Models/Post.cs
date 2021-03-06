using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gameblasts.Models.CategoryModels;

namespace Gameblasts.Models
{   
    /** Post class:
    *   Parameter: user(ApplicationUser, ownerofpost), title(string, body(string), subcategory(string)
    *   Properties: Id(int, Primarykey), Title(string), User(ApplicationUser)
    *               Date(Time when post was created), Body(string, Content of post), 
    *               SubCategory(string, the category the post was created in).
    */
    public class Post
    {

        public Post(){}
        public Post(ApplicationUser user, string title, string body, int subcategory, int parentPost = 0)
        {
            this.Title = title;
            this.User = user;
            this.Body = body;
            this.Date = DateTime.Now;
            this.SubCategory = subcategory;
            this.parentPost = parentPost;
            this.replies = new List<Post>();
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
        public int SubCategory{get; set;}

        public int parentPost{get; set;}
        public List<Post> replies{get; set;}
    }
}