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

            if (!File.Exists(path))
            {
                return allmovie;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    allmovie.Add(new Movies(data[0], data[1], data[2], data[3],
                                            data[4], data[5], data[6], data[7]));
                }
            }
            return allmovie;
        }

        public void SaveMovies(List<Movies> allmovie)
        {
            string path = ("Movie.csv");

            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (Movies movie in allmovie)
                {
                    writer.WriteLine($"{movie.Mnum},{movie.Title},{movie.Genre},{movie.Room},{movie.Time},{movie.Rating},{movie.Description},{movie.Runtime}");
                }
            }
        }

        public List<Room> LoadRooms()
        {
            List<Room> Rooms = new List<Room>();
            string path = "Room.csv";

            if (!File.Exists(path))
            {
                return Rooms;
            }

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
            string path = ("Room.csv");

            using (StreamWriter writer = new StreamWriter(path))
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

            if (!File.Exists(path))
            {
                return allres;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    allres.Add(new CustomerR(data[0], data[1], data[2]));
                }
            }
            return allres;
        }

        public void SaveReservations(List<CustomerR> allres)
        {
            string path = ("Reservation.csv");

            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (CustomerR res in allres)
                {
                    writer.WriteLine($"{res.Movie},{res.RNum},{res.SNum}");
                }
            }
        }

        public void FileWrite()
        {
            if (!File.Exists("Movie.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Movie.csv"))
                {
                    writer.WriteLine("1,Inception,Sci-Fi,1,10:00,PG-13,Dream thief heist,148,2026-02-20");
                    writer.WriteLine("2,Interstellar,Sci-Fi,2,13:00,PG-13,Space exploration,169,2026-02-20");
                    writer.WriteLine("3,The Godfather,Crime,1,14:30,R,Mafia family saga,175,2026-02-20");
                }
            }

            if (!File.Exists("Room.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Room.csv"))
                {
                    writer.WriteLine("1,,50");
                    writer.WriteLine("2,,40");
                    writer.WriteLine("3,,30");
                }
            }

            if (!File.Exists("Reservation.csv"))
            {
                using (StreamWriter writer = new StreamWriter("Reservation.csv"))
                {
                    
                }
            }
        }
    }
}