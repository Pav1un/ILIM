using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;

namespace MovieData
{
    public class JsonWork
    {
        DataContractJsonSerializer json;

        public T GetObjectFromJson<T>(HttpWebResponse resp)
        {
            using (StreamReader stream = new StreamReader(resp.GetResponseStream()))
            {
                var rawJson = stream.ReadToEnd();
                json = new DataContractJsonSerializer(typeof(T));
                return (T)Convert.ChangeType(json.ReadObject(
                    new System.IO.MemoryStream(Encoding.UTF8.GetBytes(rawJson)))
                    , typeof(T));
            }
        }

        public T GetObjectFromJson<T>(String path)
        {
            json = new DataContractJsonSerializer(typeof(T));
            using (FileStream reader = new FileStream(path, FileMode.Open))
            {
                return (T)Convert.ChangeType(json.ReadObject(reader), typeof(T));
            }
        }

        //need change
        public void CreateJsonFile(JsonResult mvs, String path)
        {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                json.WriteObject(writer, mvs);
            }
        }
    }
}
