using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using MovieData;

namespace FileWatcherService
{
    public partial class Service1 : ServiceBase
    {
        IMDBWrapper iw;

        public Service1() {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;            
        }
        
        protected override void OnStart(string[] args)  {
            iw = new IMDBWrapper();            
            Thread loggerThread = new Thread(new ThreadStart(iw.Start));
            loggerThread.Start();            
        }

        protected override void OnStop() => iw.Stop();
    }    

    public class IMDBWrapper
    {
        static string path = @"D:\GoodFilms.txt";
        static String site = "https://api.themoviedb.org/3/discover/movie?api_key=6a1f74896198cfbd499eb6f6045873f8&language=en-US&sort_by=popularity.desc&include_adult=false&include_video=false&page=1";
        bool enabled = true;
        JsonWork jw = new JsonWork();


        public HttpWebResponse GetResponse(String requestString) =>
              (HttpWebResponse)HttpWebRequest.Create(requestString).GetResponse();        
        

        public void Start() {
            while (enabled) {
                FlushMovieListToFile(GetMovieList());
                Thread.Sleep(new TimeSpan(12, 0, 0).Milliseconds);
            }
        }

        public void Stop() => enabled = false;

        public JsonResult GetMovieList(Boolean isJsonFileCreate = true) {
            var mvs = jw.GetObjectFromJson<JsonResult>(GetResponse(site));
            if (isJsonFileCreate) {
                jw.CreateJsonFile(mvs, @"D:\GoodFilms.json");
            }

            return mvs;
        }        

        public void FlushMovieListToFile(JsonResult movieLst)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                foreach (var mv in movieLst.Movies)
                {
                    writer.WriteLine(String.Format(" Name: {0}  id: {1} ", mv.Name, mv.Id));
                    writer.Flush();
                }
            }
        }

    }
}
