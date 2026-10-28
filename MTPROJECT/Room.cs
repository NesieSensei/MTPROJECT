using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class Room
    {
        public string Rooms;
        public string Seats;
        public string MaxSeats;

        public Room(string Rooms_, string Seats_, string MaxSeats_)
        {
            Rooms = Rooms_;
            Seats = Seats_;
            MaxSeats = MaxSeats_;
        }
    }
}