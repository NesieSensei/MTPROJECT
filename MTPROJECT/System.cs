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
        private List<Schedule> schedules;
        private DIRECTORY directory;

        
        public System()
        {
            directory = new DIRECTORY();
            movies = directory.LoadMovies();
            rooms = directory.LoadRooms();
            reservations = directory.LoadReservations();
            schedules = directory.LoadSchedules();
        }

        private void ClearScreen()
        {
            Console.Clear();
            Console.Write("\u001b[3J");
        }

        public void Run()
        {
            bool running = true;
            while (running)
            {
                ClearScreen();
                PrintHeader();
                Console.WriteLine("1. Customer");
                Console.WriteLine("2. Admin");
                Console.WriteLine("0. Exit");
                Console.Write("Enter choice (0-2): ");

                string choice = Console.ReadLine();
                ClearScreen();
                PrintHeader();

                switch (choice)
                {
                    case "1": RunCustomer(); break;
                    case "2": RunAdmin(); break;
                    case "0": running = false; break;
                    default:
                        Console.WriteLine("Invalid option. Please enter 0, 1, or 2.");
                        Pause();
                        break;
                }
            }
        }

        private void RunCustomer()
        {
            bool running = true;
            while (running)
            {
                ClearScreen();
                PrintHeader();
                Console.WriteLine("1. Show Movies & Showtimes");
                Console.WriteLine("2. Reserve Seats");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice (0-2): ");

                string choice = Console.ReadLine();
                ClearScreen();
                PrintHeader();

                switch (choice)
                {
                    case "1":
                        ShowMovies();
                        break;
                    case "2":
                        Reservation();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please enter 0, 1, or 2.");
                        break;
                }

                if (running && choice != "0") Pause();
            }
        }

        private void RunAdmin()
        {
            Console.Write("Enter admin password: ");
            string pass = Console.ReadLine();
            if (pass != "admin123")
            {
                ShowError("Incorrect password. Returning to main menu.");
                Pause();
                return;
            }

            bool running = true;
            while (running)
            {
                ClearScreen();
                PrintHeader();
                Console.WriteLine("1. Show Movies & Showtimes");
                Console.WriteLine("2. Reserve Seats");
                Console.WriteLine("3. Add Room");
                Console.WriteLine("4. Add Seats to Room");
                Console.WriteLine("5. Add Movie");
                Console.WriteLine("6. Remove Movie");
                Console.WriteLine("7. Remove Seats from Room");
                Console.WriteLine("8. Remove Room");
                Console.WriteLine("9. Schedule Movie");
                Console.WriteLine("10. Remove Schedule");
                Console.WriteLine("11. View Reservation");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice (0-11): ");

                string choice = Console.ReadLine();
                ClearScreen();
                PrintHeader();

                switch (choice)
                {
                    case "1":
                        ShowMovies();
                        break;
                    case "2":
                        Reservation();
                        break;
                    case "3":
                        AddRoom();
                        break;
                    case "4":
                        AddSeats();
                        break;
                    case "5":
                        AddMovie();
                        break;
                    case "6":
                        RemoveMovie();
                        break;
                    case "7":
                        RemoveSeats();
                        break;
                    case "8":
                        RemoveRoom();
                        break;
                    case "9":
                        ScheduleMovie();
                        break;
                    case "10":
                        RemoveSchedule();
                        break;
                    case "11":
                        ViewReservation();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid menu option. Please enter 0 to 11.");
                        break;
                }

                if (running && choice != "0")
                    Pause();
            }
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void PrintHeader()
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("                          MTPROJECT CINEMA SYSTEM                               ");
            Console.WriteLine("================================================================================");
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
                Console.WriteLine($"Movie No.  : {movie.Mnum}");
                Console.WriteLine($"Title      : {movie.Title}");
                Console.WriteLine($"Genre      : {movie.Genre}");
                Console.WriteLine($"Rating     : {movie.Rating}");
                Console.WriteLine($"Runtime    : {movie.Runtime} mins");
                Console.WriteLine($"Price      : P{movie.Price}");
                Console.WriteLine($"Description: {movie.Description}");

                Console.WriteLine("--- Showtimes ---");
                bool hasSchedule = false;

                int runtime;
                bool validRuntime = int.TryParse(movie.Runtime, out runtime);

                foreach (Schedule schedule in schedules)
                {
                    if (schedule.MovieNum == movie.Mnum)
                    {
                        hasSchedule = true;
                        DateTime startTime;
                        if (!DateTime.TryParse(schedule.Time, out startTime))
                        {
                            Console.WriteLine($"  Room {schedule.RoomId} | [Invalid time: {schedule.Time}]");
                            continue;
                        }

                        string timeRange;
                        if (validRuntime)
                        {
                            DateTime endTime = startTime.AddMinutes(runtime);
                            timeRange = $"{startTime:HH:mm} - {endTime:HH:mm}";
                        }
                        else
                        {
                            timeRange = $"{startTime:HH:mm}";
                        }

                        Room room = GetRoomByld(schedule.RoomId);
                        int totalSeats = 0;
                        if (room != null)
                        {
                            int.TryParse(room.MaxSeats, out totalSeats);
                        }

                        int takenSeats = 0;
                        foreach (CustomerR r in reservations)
                        {
                            if (r.Movie == movie.Mnum && r.Time == schedule.Time)
                            {
                                takenSeats++;
                            }
                        }


                        int availableSeats = totalSeats - takenSeats;
                        if (availableSeats < 0) availableSeats = 0;

                        string seatInfo;
                        if (room == null)
                        {
                            seatInfo = "Seats: [Room data missing]";
                        }
                        else if (availableSeats == 0)
                        {
                            seatInfo = $"Seats: {availableSeats} [SOLD OUT]";
                        }
                        else
                        {
                            seatInfo = $"Seats: {availableSeats} available";
                        }

                        Console.WriteLine($"  Room {schedule.RoomId} | {timeRange} | {seatInfo}");
                    }
                }

                if (!hasSchedule)
                {
                    Console.WriteLine("  [No showtimes scheduled yet]");
                }
                Console.WriteLine("================================================================================");
            }
        }

        private void Reservation()
        {
            ShowMovies();
            Console.WriteLine("Enter Movie number to reserve:");
            string Mid = Console.ReadLine();

            if (Mid == "")
            {
                ShowError("Enter a movie number");
                return;
            }

            Movies selectedMovie = null;
            foreach (Movies m in movies)
            {
                if (m.Mnum == Mid)
                {
                    selectedMovie = m;
                    break;
                }
            }

            if (selectedMovie == null)
            {
                ShowError("Movie not found.");
                return;
            }

            int runtime;
            if (!int.TryParse(selectedMovie.Runtime, out runtime))
            {
                ShowError("This movie has an invalid runtime. Please ask an admin.");
                return;
            }

            List<Schedule> movieSchedules = new List<Schedule>();
            foreach (Schedule s in schedules)
            {
                if (s.MovieNum == Mid)
                {
                    movieSchedules.Add(s);
                }
            }

            if (movieSchedules.Count == 0)
            {
                ShowError("This movie has no scheduled showtimes yet. Please ask an admin.");
                return;
            }
            ClearScreen();
            PrintHeader();
            Console.WriteLine($"\nAvailable Showtimes for '{selectedMovie.Title}':");
            for (int i = 0; i < movieSchedules.Count; i++)
            {
                DateTime start;
                if (!DateTime.TryParse(movieSchedules[i].Time, out start))
                {
                    Console.WriteLine($"{i + 1}. Room {movieSchedules[i].RoomId} | [Invalid time]");
                    continue;
                }
                DateTime end = start.AddMinutes(runtime);
                Console.WriteLine($"{i + 1}. Room {movieSchedules[i].RoomId} | {start:HH:mm} - {end:HH:mm}");
            }

            Console.Write("\nSelect a showtime: ");
            string schedChoice = Console.ReadLine();

            int schedIndex;
            if (!int.TryParse(schedChoice, out schedIndex))
            {
                ShowError("Invalid showtime selection.");
                return;
            }

            if (schedIndex < 1 || schedIndex > movieSchedules.Count)
            {
                ShowError("Invalid showtime selection.");
                return;
            }

            Schedule chosenSchedule = movieSchedules[schedIndex - 1];
            DateTime chosenStart;
            if (!DateTime.TryParse(chosenSchedule.Time, out chosenStart))
            {
                ShowError("That showtime has an invalid time. Please ask an admin.");
                return;
            }

            Room room = GetRoomByld(chosenSchedule.RoomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }
            ClearScreen();
            PrintHeader();
            Console.WriteLine($"\nRoom {room.Rooms} - max seats: {room.MaxSeats}");
            ShowSeats(room, selectedMovie.Mnum, chosenSchedule.Time);

            Console.WriteLine("Enter a Seat number:");
            string Seat = Console.ReadLine();

            if (Seat.Trim() == "")
            {
                ShowError("Seat number cannot be empty.");
                return;
            }

            int seatNum;
            int maxSeats;
            if (!int.TryParse(Seat.Trim(), out seatNum) ||
                !int.TryParse(room.MaxSeats, out maxSeats) ||
                seatNum < 1 || seatNum > maxSeats)
            {
                ShowError("Invalid seat number for this room.");
                return;
            }

            if (IsSeatTaken(selectedMovie.Mnum, Seat.Trim(), chosenSchedule.Time))
            {
                ShowError($"Seat {Seat} is already reserved for this showtime.");
                return;
            }

            reservations.Add(new CustomerR(selectedMovie.Mnum, chosenSchedule.RoomId, chosenSchedule.Time, Seat.Trim()));
            directory.SaveReservations(reservations);

            ClearScreen();
            PrintHeader();
            Console.WriteLine($"\nReservation confirmed: {selectedMovie.Title}, Room {chosenSchedule.RoomId}, Seat {Seat.Trim()}");
            PrintTicket(selectedMovie, chosenSchedule.Time, Seat.Trim(), chosenSchedule.RoomId);
        }

        private void AddMovie()
        {
            Console.WriteLine("--- ADD MOVIE ---");

            int nextNum = 1;
            foreach (Movies m in movies)
            {
                int currentNum;
                if (!int.TryParse(m.Mnum, out currentNum))
                {
                    currentNum = 0;
                }
                if (currentNum >= nextNum)
                {
                    nextNum = currentNum + 1;
                }
            }
            string mnum = nextNum.ToString();
            Console.WriteLine($"Assigned Movie Number: {mnum}");

            Console.Write("Enter Title: ");
            string title = Console.ReadLine().Replace(",", " ").Trim();
            if (title == "")
            {
                ShowError("Title cannot be empty.");
                return;
            }

            Console.Write("Enter Genre: ");
            string genre = Console.ReadLine().Replace(",", " ").Trim();
            if (genre == "")
            {
                ShowError("Genre cannot be empty.");
                return;
            }

            Console.Write("Enter Runtime (in minutes): ");
            string runtime = Console.ReadLine().Trim();

            int runMin;
            if (!int.TryParse(runtime, out runMin) || runMin < 1)
            {
                ShowError("Runtime must be a valid positive number.");
                return;
            }

            Console.Write("Enter Rating (ex. PG-13, R): ");
            string rating = Console.ReadLine().Replace(",", " ").Trim();
            if (rating == "")
            {
                ShowError("Rating cannot be empty.");
                return;
            }

            Console.Write("Enter Description: ");
            string description = Console.ReadLine().Replace(",", " ").Trim();
            if (description == "")
            {
                ShowError("Description cannot be empty.");
                return;
            }

            Console.Write("Enter Price (200): ");
            string priceInput = Console.ReadLine().Trim();

            decimal price;
            if (!decimal.TryParse(priceInput, out price) || price < 0)
            {
                ShowError("Price must be a valid non-negative number.");
                return;
            }

            Movies newMovie = new Movies(mnum, title, genre, rating, description,
                                         runMin.ToString(), price.ToString("0.00"));
            movies.Add(newMovie);
            directory.SaveMovies(movies);

            Console.WriteLine($"\nMovie '{title}' (No. {mnum}) added successfully at P{price:0.00}. Don't forget to schedule it!");
        }

        private void ScheduleMovie()
        {
            Console.WriteLine("--- SCHEDULE MOVIE ---");

            if (movies.Count == 0)
            {
                Console.WriteLine("No movies available. Add a movie first.");
                return;
            }
            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available. Add a room first.");
                return;
            }

            Console.WriteLine("\nAvailable Movies:");
            foreach (Movies m in movies)
            {
                Console.WriteLine($"  [{m.Mnum}] {m.Title} ({m.Runtime} mins) - ${m.Price}");
            }

            Console.Write("\nEnter Movie Number to schedule: ");
            string mNum = Console.ReadLine();

            Movies selectedMovie = null;
            foreach (Movies m in movies)
            {
                if (m.Mnum == mNum)
                {
                    selectedMovie = m;
                    break;
                }
            }

            if (selectedMovie == null)
            {
                ShowError("Movie not found.");
                return;
            }

            int runMin;
            if (!int.TryParse(selectedMovie.Runtime, out runMin))
            {
                ShowError("This movie has an invalid runtime.");
                return;
            }
            ClearScreen();
            PrintHeader();
            Console.WriteLine("\nAvailable Rooms:");
            foreach (Room r in rooms)
            {
                Console.WriteLine($"  Room {r.Rooms} - Max Seats: {r.MaxSeats}");
            }

            Console.Write("\nEnter Room ID: ");
            string roomId = Console.ReadLine();
            Room selectedRoom = GetRoomByld(roomId);
            if (selectedRoom == null)
            {
                ShowError("Room not found.");
                return;
            }
            ClearScreen();
            PrintHeader();
            Console.WriteLine($"\n--- Existing Schedule for Room {selectedRoom.Rooms} ---");

            List<Schedule> roomSchedules = new List<Schedule>();
            foreach (Schedule s in schedules)
            {
                if (s.RoomId == roomId)
                {
                    roomSchedules.Add(s);
                }
            }

            if (roomSchedules.Count == 0)
            {
                Console.WriteLine("  [No movies scheduled in this room yet.]");
            }
            else
            {
                roomSchedules.Sort((a, b) =>
                {
                    DateTime ta, tb;
                    DateTime.TryParse(a.Time, out ta);
                    DateTime.TryParse(b.Time, out tb);
                    return ta.CompareTo(tb);
                });

                Console.WriteLine("  Time          | Movie                          | Runtime");
                Console.WriteLine("  ------------------------------------------------------------");
                foreach (Schedule s in roomSchedules)
                {
                    Movies schMovie = null;
                    foreach (Movies m in movies)
                    {
                        if (m.Mnum == s.MovieNum)
                        {
                            schMovie = m;
                            break;
                        }
                    }

                    DateTime start;
                    int rt = 0;
                    if (!DateTime.TryParse(s.Time, out start))
                    {
                        Console.WriteLine($"  [Invalid time]  | {(schMovie != null ? schMovie.Title : "Unknown Movie"),-30} | -");
                        continue;
                    }

                    if (schMovie != null && int.TryParse(schMovie.Runtime, out rt))
                    {
                        DateTime end = start.AddMinutes(rt);
                        Console.WriteLine($"  {start:HH:mm}-{end:HH:mm}   | {schMovie.Title,-30} | {rt} mins");
                    }
                    else
                    {
                        Console.WriteLine($"  {start:HH:mm}         | {(schMovie != null ? schMovie.Title : "Unknown Movie"),-30} | ?");
                    }
                }
                Console.WriteLine("  ------------------------------------------------------------");
                Console.WriteLine("  Note: 20-minute cleaning gap is required between movies.");
            }

            Console.Write("\nEnter Start Time (HH:mm Military Time): ");
            string timeInput = Console.ReadLine();
            DateTime newStartTime;
            if (!DateTime.TryParse(timeInput, out newStartTime))
            {
                ShowError("Invalid time format. Please use HH:mm (ex. 14:30).");
                return;
            }

            DateTime newEndTime = newStartTime.AddMinutes(runMin);

            foreach (Schedule existing in schedules)
            {
                if (existing.RoomId == roomId)
                {
                    Movies existingMovie = null;
                    foreach (Movies m in movies)
                    {
                        if (m.Mnum == existing.MovieNum)
                        {
                            existingMovie = m;
                            break;
                        }
                    }

                    DateTime exStartTime;
                    int exRuntime;
                    if (existingMovie == null ||
                        !DateTime.TryParse(existing.Time, out exStartTime) ||
                        !int.TryParse(existingMovie.Runtime, out exRuntime))
                    {
                        continue;
                    }

                    DateTime exEndTime = exStartTime.AddMinutes(exRuntime);

                    if (newStartTime < exEndTime.AddMinutes(20) && newEndTime.AddMinutes(20) > exStartTime)
                    {
                        ShowError($"Schedule conflict in Room {roomId}!");
                        ShowError($"'{existingMovie.Title}' runs from {exStartTime:HH:mm} to {exEndTime:HH:mm}.");
                        ShowError($"The room needs 20 minutes for cleaning between movies.");
                        return;
                    }
                }
            }

            schedules.Add(new Schedule(selectedMovie.Mnum, roomId, timeInput.Trim()));
            directory.SaveSchedules(schedules);

            Console.WriteLine($"\nSuccessfully scheduled '{selectedMovie.Title}' in Room {roomId} at {newStartTime:HH:mm} (Ends at {newEndTime:HH:mm}).");
        }

        private void RemoveSchedule()
        {
            Console.WriteLine("--- REMOVE SCHEDULE ---");
            if (schedules.Count == 0)
            {
                Console.WriteLine("No schedules available to remove.");
                return;
            }

            Console.WriteLine("\nCurrent Schedules:");
            for (int i = 0; i < schedules.Count; i++)
            {
                Movies m = null;
                foreach (Movies movie in movies)
                {
                    if (movie.Mnum == schedules[i].MovieNum)
                    {
                        m = movie;
                        break;
                    }
                }

                string title = m != null ? m.Title : "Unknown Movie";
                Console.WriteLine($"{i + 1}. Movie: {title} | Room {schedules[i].RoomId} | {schedules[i].Time}");
            }

            Console.Write("\nEnter the number of the schedule to remove: ");
            string input = Console.ReadLine();

            int index;
            if (!int.TryParse(input, out index))
            {
                ShowError("Invalid selection.");
                return;
            }

            if (index < 1 || index > schedules.Count)
            {
                ShowError("Invalid selection.");
                return;
            }

            schedules.RemoveAt(index - 1);
            directory.SaveSchedules(schedules);
            Console.WriteLine("\nSchedule removed successfully.");
        }

        private void AddRoom()
        {
            Console.WriteLine("--- ADD ROOM ---");
            Console.WriteLine("Enter Room ID (number): ");
            string roomId = Console.ReadLine().Replace(",", "").Trim();

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

            int max;
            if (!int.TryParse(maxSeats, out max))
            {
                ShowError("Max Seats must be a valid number.");
                return;
            }
            if (max < 1)
            {
                ShowError("Max Seats must be a positive number.");
                return;
            }

            Room newRoom = new Room(roomId, "", max.ToString());
            rooms.Add(newRoom);
            directory.SaveRoom(rooms);
            Console.WriteLine($"\nRoom {roomId} added successfully with {max} seats.");
        }

        private void AddSeats()
        {
            Console.WriteLine("--- ADD SEATS TO ROOM ---");
            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available.");
                return;
            }

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

            int amountToAdd;
            if (!int.TryParse(amountInput, out amountToAdd) || amountToAdd < 1)
            {
                ShowError("Amount must be a valid positive number.");
                return;
            }

            int currentMax;
            if (!int.TryParse(room.MaxSeats, out currentMax))
            {
                ShowError("This room has an invalid seat count.");
                return;
            }

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

            int amountToRemove;
            if (!int.TryParse(amountInput, out amountToRemove) || amountToRemove < 1)
            {
                ShowError("Amount must be a valid positive number.");
                return;
            }

            int currentMax;
            if (!int.TryParse(room.MaxSeats, out currentMax))
            {
                ShowError("This room has an invalid seat count.");
                return;
            }

            if (amountToRemove >= currentMax)
            {
                ShowError($"Cannot remove {amountToRemove} seats. Room only has {currentMax} seats.");
                return;
            }

            int newMax = currentMax - amountToRemove;
            room.MaxSeats = newMax.ToString();

            int i = 0;
            while (i < reservations.Count)
            {
                if (reservations[i].RNum == roomId)
                {
                    int seatNum;
                    if (!int.TryParse(reservations[i].SNum, out seatNum))
                    {
                        seatNum = 0;
                    }

                    if (seatNum > newMax)
                    {
                        reservations.RemoveAt(i);
                        continue;
                    }
                }
                i++;
            }
            directory.SaveReservations(reservations);
            directory.SaveRoom(rooms);
            Console.WriteLine($"\n{amountToRemove} seats removed. New max seats: {newMax}.");
        }

        private void RemoveMovie()
        {
            ShowMovies();
            Console.WriteLine("--- REMOVE MOVIE ---");
            if (movies.Count == 0) return;

            Console.Write("Enter Movie number to remove: ");
            string mnum = Console.ReadLine();

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

            reservations.RemoveAll(r => r.Movie == mnum);
            schedules.RemoveAll(s => s.MovieNum == mnum);

            movies.Remove(toRemove);

            for (int i = 0; i < movies.Count; i++)
            {
                string oldNum = movies[i].Mnum;
                string newNum = (i + 1).ToString();
                if (oldNum != newNum)
                {
                    movies[i].Mnum = newNum;
                    foreach (CustomerR r in reservations)
                    {
                        if (r.Movie == oldNum) r.Movie = newNum;
                    }
                    foreach (Schedule s in schedules)
                    {
                        if (s.MovieNum == oldNum) s.MovieNum = newNum;
                    }
                }
            }

            directory.SaveMovies(movies);
            directory.SaveReservations(reservations);
            directory.SaveSchedules(schedules);
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

            foreach (Room r in rooms)
            {
                Console.WriteLine($"  Room {r.Rooms} - Max Seats: {r.MaxSeats}");
            }
            Console.Write("\nEnter Room ID to remove: ");
            string roomId = Console.ReadLine();

            Room room = GetRoomByld(roomId);
            if (room == null)
            {
                ShowError("Room not found.");
                return;
            }

            bool hasSchedule = false;
            foreach (Schedule s in schedules)
            {
                if (s.RoomId == roomId)
                {
                    hasSchedule = true;
                    break;
                }
            }

            if (hasSchedule)
            {
                ShowError($"Cannot remove Room {roomId}: Movies are scheduled there. Remove the schedules first.");
                return;
            }

            rooms.Remove(room);
            directory.SaveRoom(rooms);

            reservations.RemoveAll(r => r.RNum == roomId);
            directory.SaveReservations(reservations);

            Console.WriteLine($"\nRoom {roomId} removed successfully.");
        }

        private void ViewReservation()
        {
            Console.WriteLine("--- VIEW RESERVATION ---");

            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            List<CustomerR> sorted = reservations
                .OrderBy(r => r.Movie)
                .ThenBy(r => r.RNum)
                .ThenBy(r => r.Time)
                .ToList();

            Console.WriteLine($"Total reservations on file: {sorted.Count}");
            Console.Write("Enter ticket number: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("Ticket number cannot be empty.");
                return;
            }

            int ticketNum;
            if (!int.TryParse(input.Trim(), out ticketNum))
            {
                ShowError("Ticket number must be a valid number.");
                return;
            }

            if (ticketNum < 1 || ticketNum > sorted.Count)
            {
                ShowError($"Ticket #{ticketNum} does not exist. Valid range: 1 to {sorted.Count}.");
                return;
            }

            CustomerR found = sorted[ticketNum - 1];

            ClearScreen();
            PrintHeader();
            Console.WriteLine($"\n--- TICKET #{ticketNum} DETAILS ---\n");

            string titleT = "";
            string genreT = "";
            string ratingT = "";
            string runtimeT = "";
            string priceT = "";
            string timeDisplay = found.Time;

            foreach (Movies m in movies)
            {
                if (m.Mnum == found.Movie)
                {
                    titleT = m.Title;
                    genreT = m.Genre;
                    ratingT = m.Rating;
                    runtimeT = m.Runtime;
                    priceT = m.Price;

                    DateTime start;
                    int runtime;
                    if (DateTime.TryParse(found.Time, out start) &&
                        int.TryParse(m.Runtime, out runtime))
                    {
                        timeDisplay = $"{start:HH:mm} - {start.AddMinutes(runtime):HH:mm}";
                    }
                    break;
                }
            }

            Console.WriteLine($"Ticket   : #{ticketNum}");
            Console.WriteLine($"Movie No.: {found.Movie}");
            Console.WriteLine($"Title    : {titleT}");
            Console.WriteLine($"Genre    : {genreT}");
            Console.WriteLine($"Rating   : {ratingT}");
            Console.WriteLine($"Runtime  : {runtimeT} mins");
            Console.WriteLine($"Price    : P{priceT}");
            Console.WriteLine($"Room     : {found.RNum}");
            Console.WriteLine($"Time     : {timeDisplay}");
            Console.WriteLine($"Seat     : {found.SNum}");
            Console.WriteLine("================================================================================");
        }

        private Room GetRoomByld(string id)
        {
            foreach (Room r in rooms)
            {
                if (r.Rooms == id)
                {
                    return r;
                }
            }
            return null;
        }

        private bool IsSeatTaken(string movieNum, string seat, string time)
        {
            foreach (CustomerR r in reservations)
            {
                if (r.Movie == movieNum && r.SNum == seat && r.Time == time)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShowSeats(Room room, string movieNum, string time)
        {
            int max;
            if (!int.TryParse(room.MaxSeats, out max))
            {
                max = 0;
            }
            List<string> takenSeats = new List<string>();

            foreach (CustomerR r in reservations)
            {
                if (r.Movie == movieNum && r.Time == time)
                {
                    takenSeats.Add(r.SNum.Trim());
                }
            }

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
                bool isTaken = takenSeats.Contains(i.ToString());
                Console.Write(isTaken ? $" {i,2}[X] " : $" {i,2}[ ] ");
                if (i % seatsPerRow == 0)
                {
                    Console.WriteLine();
                }
            }

            if (max % seatsPerRow != 0)
            {
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("  [ ] = Available    [X] = Taken\n");
        }

        private void PrintTicket(Movies movie, string timeStr, string seat, string roomId)
        {
            DateTime startTime;
            int runtime;
            if (!DateTime.TryParse(timeStr, out startTime) || !int.TryParse(movie.Runtime, out runtime))
            {
                ShowError("Could not print ticket details.");
                return;
            }
            DateTime endTime = startTime.AddMinutes(runtime);

            decimal price;
            if (!decimal.TryParse(movie.Price, out price))
                price = 0m;

            decimal Total = price;

            string ticketNo = (reservations.Count).ToString();
            string purchaseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("              CINEMA RECEIPT                  ");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Ticket #      : #0{ticketNo}");
            Console.WriteLine($"Purchase Date : {purchaseTime}");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Movie         : {movie.Title}");
            Console.WriteLine($"Genre         : {movie.Genre}");
            Console.WriteLine($"Rating        : {movie.Rating}");
            Console.WriteLine($"Room          : {roomId}");
            Console.WriteLine($"Showtime      : {startTime:HH:mm} - {endTime:HH:mm}");
            Console.WriteLine($"Seat          : {seat}");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Total      : P{Total}");
            Console.WriteLine("==============================================");
            Console.WriteLine("        Thank you for your purchase!          ");
            Console.WriteLine("==============================================");
        }
    }
}