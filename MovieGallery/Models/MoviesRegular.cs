using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using MovieData;
using System.Net;
using System.Text;

namespace MovieGallery.Models
{
    public class MoviesRegular : IMovieCollection
    {
        JsonResult mvList;
        JsonWork jw;

        public MoviesRegular()
        {
            jw = new JsonWork();
        }


        public JsonResult CreateMovieList(String path)
        {            
             return jw.GetObjectFromJson<JsonResult>(path);                
        }

        public IEnumerable<Movie> Movies
        {
            get
            {
                if (mvList != null)
                    return mvList.Movies;

                return (mvList = CreateMovieList(@"D:\GoodFilms.json")).Movies;
            }
        }

        public String GetHomePageFromId(Int32 id)
        {
            var resp = (HttpWebResponse)HttpWebRequest
                .Create(LinkConfig.firstRequestPart 
                        + id.ToString() 
                        + LinkConfig.secondRequestPart)
                .GetResponse();

           return jw.GetObjectFromJson<MovieUrl>(resp).HomePage;
        }
    }
}