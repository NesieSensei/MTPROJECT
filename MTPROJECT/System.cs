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
                Console.Write("Enter choice (0-8): ");
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
                    case "7": RemoveSeats(); break; // NEW
                    case "8": RemoveRoom(); break;  // SHIFTED
                    default:
                        Console.WriteLine("Invalid menu option. Please enter 0 to 8.");
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
            Console.WriteLine("================================================================================");
            Console.WriteLine("                          MTPROJECT CINEMA SYSTEM                               ");
            Console.WriteLine("================================================================================");
        }

        private void PrintMenu()
        {
            Console.WriteLine("1. Show Movies");
            Console.WriteLine("2. Reserve Seats");
            Console.WriteLine("3. Add Room");
            Console.WriteLine("4. Add Seats to Room");
            Console.WriteLine("5. Add Movie");
            Console.WriteLine("6. Remove Movie");
            Console.WriteLine("7. Remove Seats from Room"); // NEW
            Console.WriteLine("8. Remove Room");            // SHIFTED
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

            foreach (Movies movie in movies)
            {
                // Calculate End Time
                DateTime startTime = DateTime.Parse(movie.Time);
                int runtimeMinutes = int.Parse(movie.Runtime);
                DateTime endTime = startTime.AddMinutes(runtimeMinutes);

                string startText = startTime.ToString("HH:mm");
                string endText = endTime.ToString("HH:mm");

                // Print Movie Section
                Console.WriteLine($"Movie No.  : {movie.Mnum}");
                Console.WriteLine($"Title      : {movie.Title}");
                Console.WriteLine($"Genre      : {movie.Genre}");
                Console.WriteLine($"Room       : {movie.Room}");
                Console.WriteLine($"Time       : {startText} - {endText}");
                Console.WriteLine($"Runtime    : {runtimeMinutes} mins");
                Console.WriteLine($"Rating     : {movie.Rating}");
                Console.WriteLine($"Description: {movie.Description}");

                // Separator between movies
                Console.WriteLine("================================================================================");
            }
        }

        private void Reservation()
        {
            ShowMovies();
            Console.WriteLine("Enter Movie number:");
            string Mid = Console.ReadLine();
           
            Console.Clear();
            PrintHeader();
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
            ShowSeats(room, selected.Mnum);
            Console.WriteLine("Enter a Seat number:");
            string Seat = Console.ReadLine();

            if (Seat.Trim() == "")
            {
                ShowError("Seat number cannot be empty.");
                return;
            }

            if (IsSeatTaken(selected.Mnum, Seat.Trim()))
            {
                ShowError($"Seat {Seat} is already reserved for this movie.");
                return;
            }

            reservations.Add(new CustomerR(selected.Mnum, selected.Room, Seat.Trim()));
            directory.SaveReservations(reservations);
            Console.Clear();
            PrintHeader();
            Console.WriteLine($"\nReservation confirmed: Movie {selected.Title}, Room {selected.Room}, Seat {Seat}");
            PrintTicket(selected, Seat);
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

            Console.WriteLine("How many seats do you want to add? ");
            string amountInput = Console.ReadLine();

            int amountToAdd = 0;
            try
            {
                amountToAdd = int.Parse(amountInput);
                if (amountToAdd < 1)
                {
                    ShowError("Amount must be a positive number.");
                    return;
                }
            }
            catch
            {
                ShowError("Amount must be a valid number.");
                return;
            }

            int currentMax = int.Parse(room.MaxSeats);
            int newMax = currentMax + amountToAdd;
            room.MaxSeats = newMax.ToString();

            directory.SaveRoom(rooms);
            Console.WriteLine($"\n{amountToAdd} seats added to Room {room.Rooms} successfully. New max seats: {newMax}.");
        }

        private void RemoveSeats()
        {
            Console.WriteLine("--- REMOVE SEATS FROM ROOM ---");

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

            Console.WriteLine("\nEnter Room ID: ");
            string roomId = Console.ReadLine();

            Room room = GetRoomByld(roomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }

            Console.WriteLine("How many seats do you want to remove? ");
            string amountInput = Console.ReadLine();

            int amountToRemove = 0;
            try
            {
                amountToRemove = int.Parse(amountInput);
                if (amountToRemove < 1)
                {
                    ShowError("Amount must be a positive number.");
                    return;
                }
            }
            catch
            {
                ShowError("Amount must be a valid number.");
                return;
            }

            int currentMax = int.Parse(room.MaxSeats);
            if (amountToRemove >= currentMax)
            {
                ShowError($"Cannot remove {amountToRemove} seats. Room only has {currentMax} seats. Use 'Remove Room' if you want to delete it entirely.");
                return;
            }

            int newMax = currentMax - amountToRemove;
            room.MaxSeats = newMax.ToString();

            // Clean up reservations that are out of the new bounds
            int i = 0;
            while (i < reservations.Count)
            {
                if (reservations[i].RNum == roomId)
                {
                    int seatNum;
                    if (int.TryParse(reservations[i].SNum, out seatNum) && seatNum > newMax)
                    {
                        reservations.RemoveAt(i);
                        continue;
                    }
                }
                i++;
            }
            directory.SaveReservations(reservations);
            directory.SaveRoom(rooms);

            Console.WriteLine($"\n{amountToRemove} seats removed from Room {room.Rooms} successfully. New max seats: {newMax}.");
        }

        private void AddMovie()
        {
            Console.WriteLine("--- ADD MOVIE ---");

            int nextNum = 1;
            foreach (Movies m in movies)
            {
                int currentNum;
                try
                {
                    currentNum = int.Parse(m.Mnum);
                    if (currentNum >= nextNum)
                        nextNum = currentNum + 1;
                }
                catch { }
            }
            string mnum = nextNum.ToString();
            Console.WriteLine($"Assigned Movie Number: {mnum}");
           


            Console.WriteLine("Available Rooms:");
            foreach (Room r in rooms)
            {
                Console.WriteLine($"  Room {r.Rooms} - Max Seats: {r.MaxSeats}");
            }
            Console.Write("Enter Room ID: ");
            string roomId = Console.ReadLine();
            if (GetRoomByld(roomId) == null)
            {
                ShowError($"Room {roomId} does not exist. Please add the room first.");
                return;
            }


            Console.Write("Enter Time (HH:mm): ");
            string time = Console.ReadLine();
            DateTime newStartTime;
            try
            {
                newStartTime = DateTime.Parse(time);
            }
            catch
            {
                ShowError("Invalid time format. Please use HH:mm (ex.14:30).");
                return;
            }


            Console.Write("Enter Runtime (in minutes): ");
            string runtime = Console.ReadLine();
            int runMin;
            try
            {
                runMin = int.Parse(runtime);
                if (runMin < 1)
                {
                    ShowError("Runtime must be a valid positive number.");
                    return;
                }
            }
            catch
            {
                ShowError("Runtime must be a valid positive number.");
                return;
            }

            
            DateTime newEndTime = newStartTime.AddMinutes(runMin);
            DateTime gapAllowance = newStartTime.AddMinutes(-20); 

            foreach (Movies existingMovie in movies)
            {
                // Only check movies in the same room
                if (existingMovie.Room == roomId)
                {
                    DateTime exStartTime = DateTime.Parse(existingMovie.Time);
                    int exRuntime = int.Parse(existingMovie.Runtime);
                    DateTime exEndTime = exStartTime.AddMinutes(exRuntime);

                    // Condition A: New movie starts before the existing movie finishes + 20 mins
                    // Condition B: New movie ends + 20 mins, after the existing movie starts
                    if (newStartTime < exEndTime.AddMinutes(20) && newEndTime.AddMinutes(20) > exStartTime)
                    {
                        ShowError($"Schedule conflict in Room {roomId}!");
                        ShowError($"'{existingMovie.Title}' runs from {exStartTime:HH:mm} to {exEndTime:HH:mm}.");
                        ShowError($"The room needs 20 minutes for cleaning");
                        return;
                    }
                }
            }

            Console.Write("Enter Title: ");
            string title = Console.ReadLine();
            if (title == "")
            {
                ShowError("Title cannot be empty.");
                return;
            }

            Console.Write("Enter Genre: ");
            string genre = Console.ReadLine();
            if (genre == "")
            {
                ShowError("Genre cannot be empty.");
                return;
            }

            Console.Write("Enter Rating (ex. PG-13, R): ");
            string rating = Console.ReadLine();
            if (rating == "")
            {
                ShowError("Rating cannot be empty.");
                return;
            }

            Console.Write("Enter Description: ");
            string description = Console.ReadLine();
            if (description == "")
            {
                ShowError("Description cannot be empty.");
                return;
            }

            Movies newMovie = new Movies(mnum, title, genre, roomId, time, rating, description, runtime);
            movies.Add(newMovie);
            directory.SaveMovies(movies);

            Console.WriteLine($"\nMovie '{title}' (No. {mnum}) added successfully to Room {roomId}.");
        }
        private void RenumberMovies()
        {
            for (int i = 0; i < movies.Count; i++)
            {
                string oldNum = movies[i].Mnum;
                string newNum = (i + 1).ToString();

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
            RenumberMovies();

            Console.WriteLine($"\nMovie '{toRemove.Title}' removed successfully.");
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

            // Also clean up any lingering reservations in this room
            int i = 0;
            while (i < reservations.Count)
            {
                if (reservations[i].RNum == roomId)
                    reservations.RemoveAt(i);
                else
                    i++;
            }
            directory.SaveReservations(reservations);

            Console.WriteLine($"\nRoom {roomId} removed successfully.");
        }

        private Room GetRoomByld(string id)
        {
            foreach (Room r in rooms)
                if (r.Rooms == id) return r;
            return null;
        }

        private bool IsSeatTaken(string movieNum, string seat)
        {
            foreach (CustomerR r in reservations)
                if (r.Movie == movieNum && r.SNum == seat) return true;
            return false;
        }

        private void ShowSeats(Room room, string movieNum)
        {
            int max = int.Parse(room.MaxSeats);
            List<string> takenSeats = new List<string>();

            // 1. Check reservations
            foreach (CustomerR r in reservations)
            {
                if (r.Movie == movieNum)
                {
                    takenSeats.Add(r.SNum.Trim());
                }
            }

            // 2. Check blocked seats
            if (room.Seats != null && room.Seats != "")
            {
                foreach (string s in room.Seats.Split(',', ' '))
                {
                    if (s.Trim() != "")
                    {
                        takenSeats.Add(s.Trim());
                    }
                }
            }

            int seatsPerRow = 10;

            Console.WriteLine("\n                 [ SCREEN THIS WAY ]");
            Console.WriteLine("--------------------------------------------------");

            for (int i = 1; i <= max; i++)
            {
                bool isTaken = false;

                // Manually check if the seat is in the taken list
                foreach (string s in takenSeats)
                {
                    if (s == i.ToString())
                    {
                        isTaken = true;
                        break;
                    }
                }

                // Print seat
                if (isTaken)
                    Console.Write($" {i,2}[X] ");
                else
                    Console.Write($" {i,2}[ ] ");

                if (i % seatsPerRow == 0)
                    Console.WriteLine();
            }

            if (max % seatsPerRow != 0)
                Console.WriteLine();

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("  [ ] = Available    [X] = Taken\n");
        }

        private void PrintTicket(Movies movie, string seat)
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