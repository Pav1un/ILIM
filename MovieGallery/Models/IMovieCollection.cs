using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieData;

namespace MovieGallery.Models
{
    public interface IMovieCollection
    {
        IEnumerable<Movie> Movies { get; }
        String GetHomePageFromId(Int32 id);
    }
}
