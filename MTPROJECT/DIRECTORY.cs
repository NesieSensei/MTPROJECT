using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class DIRECTORY
    {
        public List<Movies> LoadMovies()
        {
            List<Movies> allmovie = new List<Movies>();
            string path = "Movie.csv";

            if (!File.Exists(path)) return allmovie;

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    if (data.Length >= 7)
                    {
                        allmovie.Add(new Movies(data[0], data[1], data[2], data[3], data[4], data[5], data[6]));
                    }
                }
            }
            return allmovie;
        }

        public void SaveMovies(List<Movies> allmovie)
        {
            using (StreamWriter writer = new StreamWriter("Movie.csv"))
            {
                foreach (Movies movie in allmovie)
                {
                    writer.WriteLine($"{movie.Mnum},{movie.Title},{movie.Genre},{movie.Rating},{movie.Description},{movie.Runtime},{movie.Price}");
                }
            }
        }

        public List<Room> LoadRooms()
        {
            List<Room> Rooms = new List<Room>();
            string path = "Room.csv";

            if (!File.Exists(path)) return Rooms;

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    Rooms.Add(new Room(data[0], data[1], data[2]));
                }
            }
            return Rooms;
        }

        public void SaveRoom(List<Room> room)
        {
            using (StreamWriter writer = new StreamWriter("Room.csv"))
            {
                foreach (Room rooms in room)
                {
                    writer.WriteLine($"{rooms.Rooms},{rooms.Seats},{rooms.MaxSeats}");
                }
            }
        }

        public List<CustomerR> LoadReservations()
        {
            List<CustomerR> allres = new List<CustomerR>();
            string path = "Reservation.csv";

            if (!File.Exists(path)) return allres;

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    if (data.Length >= 4)
                    {
                        allres.Add(new CustomerR(data[0], data[1], data[2], data[3]));
                    }
                }
            }
            return allres;
        }

        public void SaveReservations(List<CustomerR> allres)
        {
            using (StreamWriter writer = new StreamWriter("Reservation.csv"))
            {
                foreach (CustomerR res in allres)
                {
                    writer.WriteLine($"{res.Movie},{res.RNum},{res.Time},{res.SNum}");
                }
            }
        }

        public List<Schedule> LoadSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();
            string path = "Schedule.csv";

            if (!File.Exists(path)) return schedules;

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    if (data.Length >= 3)
                    {
                        schedules.Add(new Schedule(data[0], data[1], data[2]));
                    }
                }
            }
            return schedules;
        }

        public void SaveSchedules(List<Schedule> schedules)
        {
            using (StreamWriter writer = new StreamWriter("Schedule.csv"))
            {
                foreach (Schedule s in schedules)
                {
                    writer.WriteLine($"{s.MovieNum},{s.RoomId},{s.Time}");
                }
            }
        }

        public void FileWrite()
        {
            if (!File.Exists("Movie.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Movie.csv"))
                {
                }
            }

            if (!File.Exists("Room.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Room.csv"))
                {
                }
            }

            if (!File.Exists("Reservation.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Reservation.csv")) { }
            }

            if (!File.Exists("Schedule.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Schedule.csv"))
                {
                }
            }
        }
    }
}