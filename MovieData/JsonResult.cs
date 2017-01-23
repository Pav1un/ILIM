using System;
using System.Runtime.Serialization;

namespace MovieData
{
    [DataContract]
    public class JsonResult
    {
        [DataMember(Name = "results")]
        public Movie[] Movies { get; set; }
    }
}
