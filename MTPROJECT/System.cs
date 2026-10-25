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
                Console.Write("Enter choice (0-7): ");
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
                    case "6": RemoveMovie(); break;
                    case "7": RemoveRoom(); break;
                    default:
                        Console.WriteLine("Invalid menu option. Please enter 0 to 7.");
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
            Console.WriteLine("=================================================");
            Console.WriteLine("        MTPROJECT CINEMA SYSTEM        ");
            Console.WriteLine("=================================================");
        }

        private void PrintMenu()
        {
            Console.WriteLine("1. Show Movies");
            Console.WriteLine("2. Reserve Seats");
            Console.WriteLine("3. Add Room");
            Console.WriteLine("4. Add Seats to Room");
            Console.WriteLine("5. Add Movie");
            Console.WriteLine("6. Remove Movie");
            Console.WriteLine("7. Remove Room");
            Console.WriteLine("0. Exit");
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

            Console.WriteLine("\tTitle\t\tGenre\tRoom\tTime\t\tRating\tDescription");
            Console.WriteLine("--------------------------------------------------------------------");

            foreach (Movies movie in movies)
            {
                string startText = movie.Time;
                string endText = "?";

                DateTime startTime = DateTime.Parse(movie.Time);
                int runtimeMinutes = int.Parse(movie.Runtime);
                DateTime endTime = startTime.AddMinutes(runtimeMinutes);

                startText = startTime.ToString("HH:mm");
                endText = endTime.ToString("HH:mm");

                Console.WriteLine($"{movie.Mnum}\t{movie.Title}\t{movie.Genre}\t{movie.Room}\t{startText} - {endText}\t{movie.Rating}\t{movie.Description}");
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

            Room room = GetRoomByld(selected.Room);
            if (room == null)
            {
                ShowError("Room not found for this movie.");
                return;
            }

            Console.WriteLine($"\nRoom {room.Rooms} - max seats: {room.MaxSeats}");
            ShowSeats(room);
            Console.WriteLine("Enter a Seat number:");
            string Seat = Console.ReadLine();

            if (Seat.Trim() == "")
            {
                ShowError("Seat number cannot be empty.");
                return;
            }

            string Finalseats = Seat;
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

       
        private void AddRoom()
        {
            Console.WriteLine("--- ADD ROOM ---");
            Console.WriteLine("Enter Room ID (number): ");
            string roomId = Console.ReadLine();

            if (roomId == "")
            {
                ShowError("Room ID cannot be empty.");
                return;
            }

            if (GetRoomByld(roomId) != null)
            {
                ShowError("Room ID already exists.");
                return;
            }

            Console.WriteLine("Enter Max Seats: ");
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

            Console.WriteLine("\nEnter Room ID: ");
            string roomId = Console.ReadLine();

            Room room = GetRoomByld(roomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }

            Console.WriteLine("Enter Seat number to add: ");
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

            // NOTE: The original code after this point was truncated in the PDF.
            // Keep the rest of your AddMovie() implementation here unchanged.
        }

        // Simplified Renumbering
        private void RenumberMovies()
        {
            for (int i = 0; i < movies.Count; i++)
            {
                string oldNum = movies[i].Mnum;
                string newNum = (i + 1).ToString();

                // If the number changed, update it in the movies list and the reservations list
                if (oldNum != newNum)
                {
                    movies[i].Mnum = newNum;

                    foreach (CustomerR r in reservations)
                    {
                        if (r.Movie == oldNum)
                            r.Movie = newNum;
                    }
                }
            }

            directory.SaveMovies(movies);
            directory.SaveReservations(reservations);
        }

        // ============ REMOVE MOVIE ============
        private void RemoveMovie()
        {
            ShowMovies();
            Console.WriteLine("--- REMOVE MOVIE ---");

            if (movies.Count == 0)
            {
                Console.WriteLine("No movies available.");
                return;
            }

            Console.Write("Enter Movie number to remove: ");
            string mnum = Console.ReadLine();

            if (mnum == "")
            {
                ShowError("Movie number cannot be empty.");
                return;
            }

            Movies toRemove = null;
            foreach (Movies movie in movies)
            {
                if (movie.Mnum == mnum)
                {
                    toRemove = movie;
                    break;
                }
            }

            if (toRemove == null)
            {
                ShowError("Movie not found.");
                return;
            }

            // --- Remove reservations tied to this movie (forward while loop) ---
            int i = 0;
            while (i < reservations.Count)
            {
                if (reservations[i].Movie == mnum)
                    reservations.RemoveAt(i);
                else
                    i++;
            }
            directory.SaveReservations(reservations);

            movies.Remove(toRemove);

            // Re-number remaining movies 1..N and remap reservations
            RenumberMovies();

            Console.WriteLine($"\nMovie '{toRemove.Title}' (No. {mnum}) removed successfully.");
        }

        private void RemoveRoom()
        {
            Console.WriteLine("--- REMOVE ROOM ---");

            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available.");
                return;
            }

            Console.WriteLine("\nAvailable Rooms:");
            foreach (Room r in rooms)
            {
                Console.WriteLine($"  Room {r.Rooms} - Max Seats: {r.MaxSeats}");
            }

            Console.Write("\nEnter Room ID to remove: ");
            string roomId = Console.ReadLine();

            if (roomId == "")
            {
                ShowError("Room ID cannot be empty.");
                return;
            }

            Room room = GetRoomByld(roomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }

            foreach (Movies movie in movies)
            {
                if (movie.Room == roomId)
                {
                    ShowError($"Cannot remove Room {roomId}: movie '{movie.Title}' is scheduled there. Remove the movie first.");
                    return;
                }
            }

            rooms.Remove(room);
            directory.SaveRoom(rooms);

            Console.WriteLine($"\nRoom {roomId} removed successfully.");
        }

        private Room GetRoomByld(string id)
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
                foreach (string s in room.Seats.Split(',', ' '))
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
                Console.Write(taken.Contains(i.ToString()) ? "[X]" : $"({i})");
                if (i % 10 == 0) Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void PrintTicketFor(Movies movie, string seat)
        {
            DateTime startTime = DateTime.Parse(movie.Time);
            int runtimeMinutes = int.Parse(movie.Runtime);
            DateTime endTime = startTime.AddMinutes(runtimeMinutes);

            Console.WriteLine("\n========= TICKET ===========");
            Console.WriteLine($"Movie  : {movie.Title}");
            Console.WriteLine($"Genre  : {movie.Genre}");
            Console.WriteLine($"Rating : {movie.Rating}");
            Console.WriteLine($"Room   : {movie.Room}");
            Console.WriteLine($"Time   : {startTime:HH:mm} - {endTime:HH:mm}");
            Console.WriteLine($"Seat   : {seat}");
            Console.WriteLine("============================");
        }
    }
}