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
        static HttpWebResponse resp;       
        bool enabled = true;
        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(JsonResult));
        
        public HttpWebResponse Response
        {
            get  {
                if (resp == null)
                    resp = (HttpWebResponse)HttpWebRequest.Create(site).GetResponse();
                return resp;
            }
            private set { }
        }

        public void Start() {
            while (enabled) {
                FlushMovieListToFile(GetMovieList());
                Thread.Sleep(new TimeSpan(12, 0, 0).Milliseconds);
            }
        }

        public void Stop() => enabled = false;

        public JsonResult GetMovieList(Boolean isJsonFileCreate = true) { 
            
            using (StreamReader stream = new StreamReader(Response.GetResponseStream()))
            {              
                var rawJson = stream.ReadToEnd();
                var mvs = (JsonResult)json.ReadObject(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(rawJson)));
                if (isJsonFileCreate) {
                    CreateJsonFile(mvs);
                }

                return mvs;
            }           
        }

        public void CreateJsonFile(JsonResult mvs, String path = @"D:\GoodFilms.json") {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                json.WriteObject(writer, mvs);
            }
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
