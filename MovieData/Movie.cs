using System;
using System.Runtime.Serialization;


namespace MovieData
{
    [DataContract]
    public class Movie
    {
        [DataMember(Name = "id")]
        public Int32 Id { get; set; }

        [DataMember(Name = "original_title")]
        public String Name { get; set; }
    }
}
