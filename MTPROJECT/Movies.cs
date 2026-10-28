using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class Movies
    {
        public string Mnum;
        public string Title;
        public string Genre;
        public string Rating;
        public string Description;
        public string Runtime;
        public string Price;

        public Movies(string Mnum_, string Title_, string Genre_, string Rating_,
                      string Description_, string Runtime_, string Price_ = "0")
        {
            Mnum = Mnum_;
            Title = Title_;
            Genre = Genre_;
            Rating = Rating_;
            Description = Description_;
            Runtime = Runtime_;
            Price = Price_;
        }
    }
}