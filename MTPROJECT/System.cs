using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class System
    {
        private List<Movies> movies;
        private List<Room> rooms;
        private List<CustomerR> reservations;
        private DIRECTORY directory;

        public System()
        {
            directory = new DIRECTORY();
            movies = directory.LoadMovies();
            rooms = directory.LoadRooms();
            reservations = directory.LoadReservations();
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                PrintHeader();
                PrintMenu();
                Console.Write("Enter choice (1- 10): ");
                string choice = Console.ReadLine();

                Console.Clear();
                PrintHeader();
                switch (choice)
                {
                    case "0": running = false; break;
                    case "1": ShowMovies(); break;
                    case "2": Reservation(); break;
                    case "3": AddRoom(); break;
                    case "4": AddSeats(); break;
                    case "5": AddMovie(); break;
                    default:
                        Console.WriteLine("Invalid menu option. Please enter 0 to 5.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void PrintHeader()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("           MTPROJECT CINEMA SYSTEM            ");
            Console.WriteLine("==============================================");
        }

        private void PrintMenu()
        {
            Console.WriteLine("1.  Show Movies");
            Console.WriteLine("2.  Reserve Seats");
            Console.WriteLine("3.  Add Room");
            Console.WriteLine("4.  Add Seats to Room");
            Console.WriteLine("5.  Add Movie");
            Console.WriteLine("0.  Exit");
        }

        private void ShowError(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }

        private void ShowMovies()
        {
            if (movies.Count == 0)
            {
                Console.WriteLine("No movies available.");
                return;
            }

            Console.WriteLine("\tTitle\tGenre\tRoom\tTime\tRating\tDescription");
            Console.WriteLine("---");

            foreach (Movies movie in movies)
            {
                string startText = movie.Time;
                string endText = "?";

                try
                {
                    DateTime startTime = DateTime.Parse(movie.Time);
                    int runtimeMinutes = int.Parse(movie.Runtime);
                    DateTime endTime = startTime.AddMinutes(runtimeMinutes);

                    startText = startTime.ToString("HH:mm");
                    endText = endTime.ToString("HH:mm");
                }
                catch { }

                string desc = movie.Description;
                if (desc == null) desc = "";
                if (desc.Length > 30) desc = desc.Substring(0, 27) + "...";

                Console.WriteLine($"{movie.Mnum}\t{movie.Title}\t\t{movie.Genre}\t{movie.Room}\t{startText} - {endText}\t{movie.Rating}\t{desc}");
            }
        }

        private void Reservation()
        {
            ShowMovies();
            Console.WriteLine("Enter Movie number:");
            string Mid = Console.ReadLine();

            if (Mid == "")
            {
                ShowError("Enter a movie number");
                return;
            }

            Movies selected = null;
            foreach (Movies movie in movies)
            {
                if (movie.Mnum == Mid)
                {
                    selected = movie;
                    break;
                }
            }

            if (selected == null)
            {
                ShowError("Movie not found.");
                return;
            }

            Room room = GetRoomById(selected.Room);
            if (room == null)
            {
                ShowError("Room not found for this movie.");
                return;
            }

            Console.WriteLine($"\nRoom {room.Rooms} - max seats: {room.MaxSeats}");
            ShowSeats(room);

            Console.WriteLine("Enter a Seat number:");
            string Seat = Console.ReadLine();

            if (Seat == "")
            {
                ShowError("Enter a seat number");
                return;
            }

            if (!IsValidSeat(Seat))
            {
                ShowError("Invalid seat number.");
                return;
            }

            if (IsSeatTaken(selected.Mnum, Seat))
            {
                ShowError("Seat already taken for this movie.");
                return;
            }

            string Finalseats = ReservedSeats(Seat);
            if (room.Seats == null || room.Seats == "")
                room.Seats = Finalseats;
            else
                room.Seats += ($",{Finalseats}");

            directory.SaveRoom(rooms);

            reservations.Add(new CustomerR(selected.Mnum, selected.Room, Seat));
            directory.SaveReservations(reservations);

            Console.WriteLine($"\nReservation confirmed: Movie {selected.Title}, Room {selected.Room}, Seat {Seat}");
            PrintTicketFor(selected, Seat);
        }

        private string ReservedSeats(string seat)
        {
            return seat;
        }

        private void AddRoom()
        {
            Console.WriteLine("--- ADD ROOM ---");

            Console.Write("Enter Room ID (number): ");
            string roomId = Console.ReadLine();

            if (roomId == "")
            {
                ShowError("Room ID cannot be empty.");
                return;
            }

            if (GetRoomById(roomId) != null)
            {
                ShowError("Room ID already exists.");
                return;
            }

            Console.Write("Enter Max Seats: ");
            string maxSeats = Console.ReadLine();

            int max = 0;
            try
            {
                max = int.Parse(maxSeats);
                if (max < 1)
                {
                    ShowError("Max Seats must be a positive number.");
                    return;
                }
            }
            catch
            {
                ShowError("Max Seats must be a valid number.");
                return;
            }

            Room newRoom = new Room(roomId, "", maxSeats);
            rooms.Add(newRoom);
            directory.SaveRoom(rooms);

            Console.WriteLine($"\nRoom {roomId} added successfully with {max} seats.");
        }

        private void AddSeats()
        {
            Console.WriteLine("--- ADD SEATS TO ROOM ---");

            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available. Please add a room first.");
                return;
            }

            Console.WriteLine("\nAvailable Rooms:");
            foreach (Room r in rooms)
            {
                Console.WriteLine($"  Room {r.Rooms} - Max Seats: {r.MaxSeats}");
            }

            Console.Write("\nEnter Room ID: ");
            string roomId = Console.ReadLine();

            Room room = GetRoomById(roomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }

            Console.Write("Enter Seat number to add: ");
            string seat = Console.ReadLine();

            int seatNum = 0;
            try
            {
                seatNum = int.Parse(seat);
            }
            catch
            {
                ShowError("Seat must be a number.");
                return;
            }

            if (seatNum < 1)
            {
                ShowError("Seat must be a positive number.");
                return;
            }

            if (SeatExistsInRoom(room, seat))
            {
                ShowError($"Seat {seat} already exists in Room {room.Rooms}.");
                return;
            }

            if (room.Seats == null || room.Seats == "")
                room.Seats = seat;
            else
                room.Seats += ($",{seat}");

            directory.SaveRoom(rooms);
            Console.WriteLine($"\nSeat {seat} added to Room {room.Rooms} successfully.");
        }

        private bool SeatExistsInRoom(Room room, string seat)
        {
            if (room.Seats == null || room.Seats == "") return false;

            foreach (string s in room.Seats.Split(','))
                if (s.Trim() == seat) return true;

            return false;
        }

        private void AddMovie()
        {
            Console.WriteLine("--- ADD MOVIE ---");

            Console.Write("Enter Movie number: ");
            string mnum = Console.ReadLine();

            if (mnum == "")
            {
                ShowError("Movie number cannot be empty.");
                return;
            }

            foreach (Movies m in movies)
            {
                if (m.Mnum == mnum)
                {
                    ShowError("Movie number already exists.");
                    return;
                }
            }

            Console.Write("Enter Title: ");
            string title = Console.ReadLine();

            Console.Write("Enter Genre: ");
            string genre = Console.ReadLine();

            Console.Write("Enter Room ID: ");
            string roomId = Console.ReadLine();

            if (GetRoomById(roomId) == null)
            {
                ShowError("Room not found.");
                return;
            }

            Console.Write("Enter Time (HH:mm): ");
            string time = Console.ReadLine();

             
            try
            {
                DateTime parsedTime = DateTime.Parse(time);
            }
            catch
            {
                ShowError("Time must be in HH:mm format (e.g. 14:30).");
                return;
            }

            Console.Write("Enter Rating: ");
            string rating = Console.ReadLine();

            Console.Write("Enter Description: ");
            string description = Console.ReadLine();

            Console.Write("Enter Runtime (minutes): ");
            string runtime = Console.ReadLine();

            int rt = 0;
            try
            {
                rt = int.Parse(runtime);
                if (rt < 1)
                {
                    ShowError("Runtime must be a positive number.");
                    return;
                }
            }
            catch
            {
                ShowError("Runtime must be a valid number.");
                return;
            }

            // -------- CONFLICT / 20-MIN GAP CHECK --------
            DateTime newStart = DateTime.Parse(time);
            DateTime newEnd = newStart.AddMinutes(rt);

            foreach (Movies existing in movies)
            {
                if (existing.Room != roomId) continue;

                DateTime exStart = DateTime.Parse(existing.Time);
                DateTime exEnd = exStart.AddMinutes(int.Parse(existing.Runtime));

                // 1. Check if they overlap
                if (newStart < exEnd && exStart < newEnd)
                {
                    ShowError($"Time overlaps with '{existing.Title}' ({exStart:HH:mm} - {exEnd:HH:mm}) in Room {roomId}.");
                    return;
                }

                // 2. Check the gap if the new movie is AFTER the existing movie
                if (newStart >= exEnd)
                {
                    double gap = (newStart - exEnd).TotalMinutes;
                    if (gap < 20)
                    {
                        ShowError($"Only {gap} min gap after '{existing.Title}' ({exEnd:HH:mm}). Need at least 20 min for cleaning.");
                        return;
                    }
                }

                // 3. Check the gap if the new movie is BEFORE the existing movie
                if (exStart >= newEnd)
                {
                    double gap = (exStart - newEnd).TotalMinutes;
                    if (gap < 20)
                    {
                        ShowError($"Only {gap} min gap before '{existing.Title}' ({exStart:HH:mm}). Need at least 20 min for cleaning.");
                        return;
                    }
                }
            }

            Movies newMovie = new Movies(mnum, title, genre, roomId, time, rating, description, runtime);
            movies.Add(newMovie);
            directory.SaveMovies(movies);

            Console.WriteLine($"\nMovie {title} added successfully.");
        }

        private Room GetRoomById(string id)
        {
            foreach (Room r in rooms)
                if (r.Rooms == id) return r;
            return null;
        }

        private bool IsValidSeat(string seat)
        {
            int n = 0;
            try
            {
                n = int.Parse(seat);
                return n >= 1;
            }
            catch
            {
                return false;
            }
        }

        private bool IsSeatTaken(string movieNum, string seat)
        {
            foreach (CustomerR r in reservations)
                if (r.Movie == movieNum && r.SNum == seat) return true;
            return false;
        }

        private void ShowSeats(Room room)
        {
            HashSet<string> taken = new HashSet<string>();

            if (room.Seats != null && room.Seats != "")
            {
                foreach (string s in room.Seats.Split(','))
                    if (s.Trim() != "") taken.Add(s.Trim());
            }

            int max = int.Parse(room.MaxSeats);

            int upper = max;
            foreach (string s in taken)
            {
                int n = 0;
                try
                {
                    n = int.Parse(s);
                    if (n > upper) upper = n;
                }
                catch { }
            }

            Console.Write("Seats: ");
            for (int i = 1; i <= upper; i++)
            {
                Console.Write(taken.Contains(i.ToString()) ? "[X]" : $"[{i}]");
                if (i % 10 == 0) Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void PrintTicketFor(Movies movie, string seat)
        {
            DateTime startTime = DateTime.Parse(movie.Time);
            int runtimeMinutes = int.Parse(movie.Runtime);
            DateTime endTime = startTime.AddMinutes(runtimeMinutes);

            Console.WriteLine("\n========== TICKET ==========");
            Console.WriteLine($"Movie   : {movie.Title}");
            Console.WriteLine($"Genre   : {movie.Genre}");
            Console.WriteLine($"Rating  : {movie.Rating}");
            Console.WriteLine($"Room    : {movie.Room}");
            Console.WriteLine($"Time    : {startTime:HH:mm} - {endTime:HH:mm}");
            Console.WriteLine($"Seat    : {seat}");
            Console.WriteLine("============================");
        }
    }
}