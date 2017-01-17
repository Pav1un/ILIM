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

    [DataContract]
    class Movie
    {
        [DataMember(Name = "id")]
        public Int32 Id { get; set; }

        [DataMember(Name = "original_title")]
        public String Name { get; set; }
    }

    [DataContract]
    class JsonResult
    {
        [DataMember(Name = "results")]
        public Movie[] Movies { get; set; }
    }

    class IMDBWrapper
    {
        static string path = @"D:\GoodFilms.txt";
        static String site = "https://api.themoviedb.org/3/discover/movie?api_key=6a1f74896198cfbd499eb6f6045873f8&language=en-US&sort_by=popularity.desc&include_adult=false&include_video=false&page=1";
        bool enabled = true;

        public void Start() {
            while (enabled) {
                FlushMovieListToFile(GetMovieList());
                Thread.Sleep(new TimeSpan(12, 0, 0).Milliseconds);
            }
        }

        public void Stop() => enabled = false;

        public JsonResult GetMovieList()
        {           
            var req = (HttpWebRequest)HttpWebRequest.Create(site);
            var resp = (HttpWebResponse)req.GetResponse();
            using (StreamReader stream = new StreamReader(resp.GetResponseStream()))
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(JsonResult));
                var rawJson = stream.ReadToEnd();
                var mvs = (JsonResult)json.ReadObject(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(rawJson)));

                return mvs;
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
