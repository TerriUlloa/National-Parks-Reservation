using Capstone.DAL;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Transactions;


namespace Capstone.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        //Used to begin a transaction during initialize and rollback during cleanup
        string _connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=npcampground;Integrated Security=True";
        private TransactionScope _tran = null;
        private CampgroundSqlDAL _db;

        [TestInitialize]
        public void Initialize()
        {
            _db = new CampgroundSqlDAL(_connectionString);

            _tran = new TransactionScope();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _tran.Dispose(); //<-- disposing the transaction without committing it means it will get rolled back
        }

        [TestMethod]
        public void CreateReservationTest()
        {
            string reservationName = "Test reservation";

            DateTime fromDate = new DateTime(1900, 01, 20);

            DateTime toDate = new DateTime(1900, 01, 21);

            DateTime currentDate = DateTime.Now;

            int reservationId = _db.AddReservation(1, reservationName, fromDate, toDate);            

            Reservation reservation = _db.GetReservation(reservationId);

            Assert.AreEqual(reservationName, reservation.Name);
            Assert.AreEqual(fromDate.Date, reservation.FromDate.Date);
            Assert.AreEqual(toDate.Date, reservation.ToDate.Date);
            Assert.AreEqual(currentDate.Date, reservation.CreateDate.Date);


        }

       [TestMethod]
        public void GetCampgroundsByParkTest()
        {
            Dictionary<int, Campground> results = _db.GetCampgroundByPark(1);

            int expectedParkId = 1;

            foreach (var item in results)
            {
                Assert.AreEqual(expectedParkId, item.Value.ParkId);
            }
        }

        [TestMethod]
        public void GetParksTest()
        {
            Park _expectedPark1 = new Park(1, "Acadia", "Maine", Convert.ToDateTime("1919-02-26"), 47389, 2563129, "Covering most of Mount Desert Island and other coastal islands, Acadia features the tallest mountain on the Atlantic coast of the United States, granite peaks, ocean shoreline, woodlands, and lakes. There are freshwater, estuary, forest, and intertidal habitats.");
            Park _expectedPark2 = new Park(2, "Arches", "Utah", Convert.ToDateTime("1929-04-12"), 76518, 1284767, "This site features more than 2,000 natural sandstone arches, including the famous Delicate Arch. In a desert climate, millions of years of erosion have led to these structures, and the arid ground has life-sustaining soil crust and potholes, which serve as natural water-collecting basins. Other geologic formations are stone columns, spires, fins, and towers.");
            Park _expectedPark3 = new Park(3, "Cuyahoga Valley", "Ohio", Convert.ToDateTime("2000-10-11"), 32860, 2189849, "This park along the Cuyahoga River has waterfalls, hills, trails, and exhibits on early rural living. The Ohio and Erie Canal Towpath Trail follows the Ohio and Erie Canal, where mules towed canal boats. The park has numerous historic homes, bridges, and structures, and also offers a scenic train ride.");


            Dictionary<int, Park> _expectedparks = new Dictionary<int, Park>
            {
                { 1, _expectedPark1 },
                { 2, _expectedPark2 },
                { 3, _expectedPark3 }
            };

            Dictionary<int, Park> _parks = _db.GetParks();

            for (int i=1; i <= _parks.Count; i++)
            {
                Assert.AreEqual(_expectedparks[i].Id, _parks[i].Id);
                Assert.AreEqual(_expectedparks[i].Name, _parks[i].Name);
                Assert.AreEqual(_expectedparks[i].Location, _parks[i].Location);
                Assert.AreEqual(_expectedparks[i].EstablishDate, _parks[i].EstablishDate);
                Assert.AreEqual(_expectedparks[i].Area, _parks[i].Area);
                Assert.AreEqual(_expectedparks[i].AnnualVisitors, _parks[i].AnnualVisitors);
                Assert.AreEqual(_expectedparks[i].Description, _parks[i].Description);

            }

        }

        [TestMethod]
        public void FindAvailableSitesTest()
        {
           
            const int _campgroundId = 1;
            DateTime _arrivalDate = Convert.ToDateTime("03/01/2019");
            DateTime _departureDate = Convert.ToDateTime("03/03/2019");
            Site _expectedSite1 = new Site(1, 1, 1, 6, 0, 0, 0);
            Site _expectedSite2 = new Site(2, 1, 2, 6, 0, 0, 0);
            Site _expectedSite3 = new Site(3, 1, 3, 6, 0, 0, 0);
            Site _expectedSite4 = new Site(6, 1, 6, 6, 1, 0, 1);
            Site _expectedSite5 = new Site(7, 1, 7, 6, 0, 20, 0);

            Dictionary<int, Site> _expectedSites = new Dictionary<int, Site>
            {
                {1, _expectedSite1},
                {2, _expectedSite2},
                {3, _expectedSite3},
                {6, _expectedSite4},
                {7, _expectedSite5}
            };

            Dictionary<int, Site> _sites = _db.FindAvailableSites(_campgroundId, _arrivalDate, _departureDate);

            foreach (Site site in _sites.Values)
            {
                Assert.AreEqual(_expectedSites[site.SiteNum].Id, site.Id,"invalid Id");
                Assert.AreEqual(_expectedSites[site.SiteNum].CampgroundId, site.CampgroundId,"campground");
                Assert.AreEqual(_expectedSites[site.SiteNum].SiteNum, site.SiteNum,"site");
                Assert.AreEqual(_expectedSites[site.SiteNum].SiteOccupancy, site.SiteOccupancy,"occupancy");
                Assert.AreEqual(_expectedSites[site.SiteNum].Accessible, site.Accessible,"accessible");
                Assert.AreEqual(_expectedSites[site.SiteNum].DisplayAccessible, site.DisplayAccessible);
                Assert.AreEqual(_expectedSites[site.SiteNum].MaxRVLength, site.MaxRVLength,"rv length");
                Assert.AreEqual(_expectedSites[site.SiteNum].DisplayMaxRVLength, site.DisplayMaxRVLength);
                Assert.AreEqual(_expectedSites[site.SiteNum].Utilities, site.Utilities,"Utilities");
                Assert.AreEqual(_expectedSites[site.SiteNum].DisplayUtilities, site.DisplayUtilities);
            }
        }
    }
}
