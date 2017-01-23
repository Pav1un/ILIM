using System;
using System.Runtime.Serialization;

namespace MovieData
{
    [DataContract]
    public class MovieUrl
    {
        [DataMember(Name = "homepage")]
        public String HomePage { get; set; }
    }
}
