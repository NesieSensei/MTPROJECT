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
        public string Room;
        public string Time;
        public string Rating;
        public string Description;
        public string Runtime;     
    

        public Movies(string Mnum_, string Title_, string Genre_, string Room_, string Time_, string Rating_, string Description_, string Runtime_)
        {
            Mnum = Mnum_;
            Title = Title_;
            Genre = Genre_;
            Room = Room_;
            Time = Time_;
            Rating = Rating_;
            Description = Description_;
            Runtime = Runtime_;

        }
    }
}