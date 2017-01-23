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
        static string path;
        DataContractJsonSerializer json;
        JsonResult mvList;

        public MoviesRegular(String pathMvs = @"D:\GoodFilms.txt")
        {
            path = pathMvs;
            json = new DataContractJsonSerializer(typeof(JsonResult));
        }


        public JsonResult CreateMovieList(String path = @"D:\GoodFilms.json")
        {
            using (FileStream reader = new FileStream(path, FileMode.Open))
            {
                var mvs = (JsonResult)json.ReadObject(reader);
                return mvs;
            }
        }

        public IEnumerable<Movie> Movies
        {
            get
            {
                if (mvList != null)
                    return mvList.Movies;

                return (mvList = CreateMovieList()).Movies;
            }
        }

        public String GetHomePageFromId(Int32 id)
        {

            var resp = (HttpWebResponse)HttpWebRequest
                .Create(LinkConfig.firstRequestPart + id.ToString() + LinkConfig.secondRequestPart)
                .GetResponse();

            using (StreamReader stream = new StreamReader(resp.GetResponseStream()))
            {
                json = new DataContractJsonSerializer(typeof(MovieUrl));
                var rawJson = stream.ReadToEnd();
                var homePage = (MovieUrl)json.ReadObject(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(rawJson)));

                return homePage.HomePage;
            }
        }
    }
}