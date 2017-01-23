using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieGallery.Models;

namespace MovieGallery.Controllers
{
    public class HomeController : Controller
    {
        //mb use DI. easy project.. no need
        IMovieCollection mvs = new MoviesRegular();             
        
        public ActionResult Index()
        {
            return View();
        }

       
        public PartialViewResult Update() {
            return PartialView(mvs);
        }

        public RedirectResult Behold(Int32 id) {            
            return Redirect(mvs.GetHomePageFromId(id));            
        }
    }
}