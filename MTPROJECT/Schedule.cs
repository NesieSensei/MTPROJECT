using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class Schedule
    {
        public string MovieNum;
        public string RoomId;
        public string Time;

        public Schedule(string movieNum, string roomId, string time)
        {
            MovieNum = movieNum;
            RoomId = roomId;
            Time = time;
        }
    }
}