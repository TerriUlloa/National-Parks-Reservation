using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;
using Capstone.Exceptions;
using System.Threading;

namespace Capstone
{
    public class CapstoneCLI
    {
        private Dictionary<int, Park> _parks = new Dictionary<int, Park>();

        private CampgroundSqlDAL _db;

        public CapstoneCLI(CampgroundSqlDAL db)
        {
            _db = db;
        }

        public void Start()
        {
            bool quit = false;
            try
            {
                _parks = _db.GetParks();
            }
            catch (Exception ex)
            {

                ApplicationLoadError(ex);
                quit = true;

            }

            if (_parks.Count == 0)
            {
                ApplicationLoadError(new ParkNotFoundException("No parks exist in the database"));
                quit = true;

            }

            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Select a Park for Further Details");

                foreach (var park in _parks)
                {
                    Console.WriteLine($"{park.Value.Id}) {park.Value.Name}");
                }
                Console.WriteLine("Q) quit");

                string userChoice = Console.ReadLine();

                if (userChoice.ToLower() == "q")
                {
                    Console.Clear();
                    Console.WriteLine("Thanks for using our app!");
                    Thread.Sleep(3000);
                    quit = true;
                }
                else
                {
                    try
                    {
                        int choice = int.Parse(userChoice);
                        if (_parks.ContainsKey(choice))
                        {
                            ViewParkInfoMenu(choice);
                        }
                        else
                        {
                            throw new OnlyValidChoiceException("Please enter only valid park numbers");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine(ex.Message);
                        Console.Write("Press any key to return to the park list...");
                        Console.ReadKey();
                    }
                }

            }
        }

        private void ViewParkInfoMenu(int id)
        {
            bool quit = false;

            while (!quit)
            {
                Console.Clear();
                Console.WriteLine(_parks[id].Name + " National Park");
                Console.WriteLine($"Location:".PadRight(18) + _parks[id].Location);
                Console.WriteLine($"Established:".PadRight(18) + _parks[id].EstablishDate.ToString("MM/dd/yyyy"));
                Console.WriteLine($"Area:".PadRight(18) + string.Format("{0:n0}", _parks[id].Area) + " sq km");
                Console.WriteLine($"Annual Visitors:".PadRight(18) + string.Format("{0:n0}", _parks[id].AnnualVisitors) + "\n");
                Console.WriteLine(_parks[id].Description + "\n");
                Console.WriteLine("1) View Campgrounds");
                // Console.WriteLine("2) Search for Reservation");
                Console.WriteLine("2) Return to Previous Screen");


                string userChoice = Console.ReadLine();


                try
                {
                    int choice = int.Parse(userChoice);
                    if (choice == 1)
                    {
                        ViewCampground(id);
                    }
                    else if (choice == 2)
                    {
                        quit = true;
                    }
                    else
                    {
                        throw new OnlyValidChoiceException("Please enter only valid command");
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Console.Write("Press any key to return to the park info screen...");
                    Console.ReadKey();
                }
            }

        }

        private void ViewCampground(int parkId)
        {
            Dictionary<int, Campground> campgrounds = new Dictionary<int, Campground>();
            bool quit = false;
            try
            {
                campgrounds = _db.GetCampgroundByPark(parkId);
            }

            catch
            {
                quit = DisplayNoCampgroundsFound(quit, parkId);

            }



            if (campgrounds.Count == 0)
            {
                quit = DisplayNoCampgroundsFound(quit, parkId);
            }

            while (!quit)
            {
                Console.Clear();

                DisplayCampgroundsByPark(parkId, campgrounds);

                Console.WriteLine("\nSelect a command\n");
                Console.WriteLine("1) Search for Available Reservation");
                Console.WriteLine("2) Return to Previous Screen");

                string userChoice = Console.ReadLine();


                try
                {
                    int choice = int.Parse(userChoice);

                    if (choice == 1)
                    {
                        SearchForCampgroundReservation(parkId, campgrounds);
                    }
                    else if (choice == 2)
                    {
                        quit = true;
                    }
                    else
                    {
                        throw new OnlyValidChoiceException("Please enter only valid command");
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Console.Write("Press any key to return to the campgrounds screen...");
                    Console.ReadKey();
                }
            }
        }

        private void SearchForCampgroundReservation(int parkId, Dictionary<int, Campground> campgrounds)
        {
            bool quit = false;

            while (!quit)
            {
                Console.Clear();

                DisplayCampgroundsByPark(parkId, campgrounds);

                try
                {
                    Console.WriteLine("\nWhich campground (enter 0 to cancel)?");
                    string userCampgroundChoiceString = Console.ReadLine();
                    int userCampgroundChoice = parseUserChoiceFromString(userCampgroundChoiceString);

                    if (userCampgroundChoice == 0)
                    {
                        quit = true;
                    }
                    else if (!campgrounds.ContainsKey(userCampgroundChoice))
                    {
                        throw new InvalidCampgroundException("The choice of campground you made is not in the list.");
                    }
                    else
                    {
                        CreateReservation(userCampgroundChoice, campgrounds); 
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Enter any key to enter a new campground.");
                    Console.ReadKey();
                }

            }
        }

        private bool DisplayNoCampgroundsFound(bool q, int parkId)
        {
            Console.Clear();
            Console.WriteLine($"No campgrounds exist for {_parks[parkId].Name}");
            Console.WriteLine("Press any key to return to previous screen...");
            Console.ReadKey();
            q = true;
            return q;
        }

        private void DisplayCampgroundsByPark(int parkId, Dictionary<int, Campground> campgrounds)
        {
            Console.WriteLine(_parks[parkId].Name + " National Park Campgrounds" + "\n");
            Console.WriteLine("".PadRight(5) + "Name".PadRight(30) +
                                "Open".PadRight(15) + "Close".PadRight(15) + "Daily Fee");

            foreach (var campground in campgrounds)
            {
                Console.WriteLine($"#{campground.Value.Id}".PadRight(5) + $"{campground.Value.Name}".PadRight(30) +
                                    $"{campground.Value.DisplayOpenFromMonth}".PadRight(15) +
                                    $"{campground.Value.DisplayOpenToMonth}".PadRight(15) +
                                    campground.Value.DailyFee.ToString("C"));
            }
        }

        private void CreateReservation(int userCampgroundChoice, Dictionary<int, Campground> campgrounds)
        {
            bool quit = false;

            while (!quit)
            {
                Console.WriteLine("What is the arrival date? (MM/DD/YYYY)");
                string arrivalDateString = Console.ReadLine();
                DateTime arrivalDate = parseDateFromString(arrivalDateString);

                Console.WriteLine("What is the departure date? (MM/DD/YYYY)");
                string departureDateString = Console.ReadLine();
                DateTime departureDate = parseDateFromString(departureDateString);
                try
                {
                    Dictionary<int, Site> sites = new Dictionary<int, Site>();
                    if (arrivalDate >= DateTime.Now.Date && arrivalDate < departureDate)
                    {
                        sites = _db.FindAvailableSites(userCampgroundChoice, arrivalDate, departureDate);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("The arrival and/or departure dates are invalid.");
                        Console.WriteLine("Would you like to enter a new set of dates? (Y) for yes or (N) for no");
                        string reenterDateChoice = Console.ReadLine();

                        if (reenterDateChoice.ToLower() == "y")
                        {
                            break;
                        }
                        else if (reenterDateChoice.ToLower() == "n")
                        {
                            quit = true;
                            break;
                        }
                        else
                        {
                            throw new InvalidYesNoException("Your input was invalid. Please enter (Y) for yes or (N) for no");
                        }
                    }
              

                if (sites.Count == 0)
                {
                    Console.Clear();
                    Console.WriteLine("No sites are available for the given dates");
                    Console.WriteLine("Would you like to enter a new set of dates? (Y) for yes or (N) for no");
                    string reenterDateChoice = Console.ReadLine();

                    if(reenterDateChoice.ToLower() == "y")
                    {
                        break;
                    }
                    else if (reenterDateChoice.ToLower() == "n")
                        {
                            quit = true;
                            break;
                        }
                    else
                        {
                            throw new InvalidYesNoException("Your input was invalid. Please enter (Y) for yes or (N) for no");
                        }

                    }
               

                Console.WriteLine("Results Matching Your Search Criteria");
                Console.WriteLine("Site No.".PadRight(10) + "Max Occup.".PadRight(12) + "Accessible?".PadRight(12) + "Max RV Length".PadRight(15) + "Utility".PadRight(10) + "Cost");


                foreach (var site in sites)
                {
                    
                    decimal costOfReservation = campgrounds[site.Value.CampgroundId].DailyFee *
                                               (decimal)(departureDate - arrivalDate).TotalDays;

                    Console.WriteLine($"{site.Value.SiteNum}".PadRight(10) + $"{site.Value.SiteOccupancy}".PadRight(12) +
                                        $"{site.Value.DisplayAccessible}".PadRight(12) + $"{site.Value.DisplayMaxRVLength}".PadRight(15) +
                                        $"{site.Value.DisplayUtilities}".PadRight(10) + costOfReservation.ToString("C"));

                }
                Console.WriteLine("Which site should be reserved (enter 0 to cancel)?");

                string userChoice = Console.ReadLine();
                int siteNum = parseUserChoiceFromString(userChoice);

                    if (sites.ContainsKey(siteNum))
                    {
                        Console.WriteLine("What name should the reservation be made under?");
                        string reservationName = Console.ReadLine();
                        int reservationNumber = _db.AddReservation(sites[siteNum].Id, reservationName, arrivalDate, departureDate);
                        Console.WriteLine($"\nThe reservation has been made.\nThe confirmation id is {reservationNumber}");
                        Console.WriteLine($"\nPress any key to return to the campgrounds screen...");
                        Console.ReadKey();
                        quit = true;
                    }
                    else if (siteNum == 0)
                    {
                        quit = true;
                    }
                    else
                    {
                        throw new OnlyValidChoiceException("Please enter only valid site.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Console.Write("Press any key to return to the campgrounds screen...");
                    quit = true;
                    Console.ReadKey();
                }
            }
        }

        private void ApplicationLoadError(Exception ex)
        {
            Console.Clear();

            Console.WriteLine(ex);

            Console.WriteLine("Unexpected error when reading parks from database");

            Console.WriteLine("Application will now close...");

            Thread.Sleep(4000);
        }

        private DateTime parseDateFromString(string date)
        {
            bool dateParsedSuccessful = false;
            DateTime parsedDate = DateTime.Now;

            while (!dateParsedSuccessful)
            {
                try
                {
                    parsedDate = Convert.ToDateTime(date);
                    dateParsedSuccessful = true;
                }

                catch
                {
                    Console.Write("Please re-enter the date in the format of MM/DD/YYYY (e.g. 01/20/2019): ");
                    date = Console.ReadLine();
                }
            }
            return parsedDate;
        }

        private int parseUserChoiceFromString(string choice)
        {
            bool choiceHasBeenParse = false;

            int userChoice = 0;

            while (!choiceHasBeenParse)
            {
                try
                {
                    userChoice = int.Parse(choice);
                    choiceHasBeenParse = true;
                }

                catch
                {
                    Console.Write("Please enter a valid number: ");
                    choice = Console.ReadLine();
                }
            }
            return userChoice;
        }
    }
}
