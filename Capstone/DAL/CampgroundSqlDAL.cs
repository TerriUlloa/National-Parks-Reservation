using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class CampgroundSqlDAL
    {
        private const string _getLastIdSQL = "SELECT CAST(SCOPE_IDENTITY() as int);";
        private string _connectionString;

        public CampgroundSqlDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get the parks and park information from the database
        /// </summary>
        /// <returns></returns> Returns a dictionary of the parks and park information
        public Dictionary<int, Park> GetParks()
        {
            Dictionary<int, Park> result = new Dictionary<int, Park>();
            string sqlGetParks = @"SELECT * FROM park
                                   ORDER BY park.name;";
            try
            {
                // create my connection object
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // open connection
                    conn.Open();

                    // create my command object
                    SqlCommand cmd = new SqlCommand(sqlGetParks, conn);


                    // execute command
                    SqlDataReader reader = cmd.ExecuteReader();

                    // if applicable loop through result set
                    while (reader.Read())
                    {
                        // populate object(s) to return
                        int parkId = Convert.ToInt32(reader["park_id"]);
                        string name = Convert.ToString(reader["name"]);
                        string location = Convert.ToString(reader["location"]);
                        DateTime establishDate = Convert.ToDateTime(reader["establish_date"]);
                        int area = Convert.ToInt32(reader["area"]);
                        int annualVisitors = Convert.ToInt32(reader["visitors"]);
                        string descripton = Convert.ToString(reader["description"]);

                        Park park = new Park(parkId, name, location, establishDate, area, annualVisitors, descripton);

                        result.Add(parkId, park);
                    }

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get the campgrounds and campground information in a given park from the database
        /// </summary>
        /// <param name="parkId"></param>The Id of the park to get the campgounds from
        /// <returns></returns> Returns a dictionary of the campgrounds in given park and the campground information
        public Dictionary<int, Campground> GetCampgroundByPark(int parkId)
        {
            Dictionary<int, Campground> result = new Dictionary<int, Campground>();
            string sqlGetParks = @"SELECT * FROM campground
                                   WHERE campground.park_id = @parkId;";
            try
            {
                // create my connection object
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // open connection
                    conn.Open();

                    // create my command object
                    SqlCommand cmd = new SqlCommand(sqlGetParks, conn);

                    cmd.Parameters.AddWithValue("@parkId", parkId);

                    // execute command
                    SqlDataReader reader = cmd.ExecuteReader();

                    // if applicable loop through result set
                    while (reader.Read())
                    {
                        // populate object(s) to return

                        int id = Convert.ToInt32(reader["campground_id"]);
                        string name = Convert.ToString(reader["name"]);
                        int openFromMonth = Convert.ToInt32(reader["open_from_mm"]);
                        int openToMonth = Convert.ToInt32(reader["open_to_mm"]);
                        decimal dailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        Campground campground = new Campground(id, parkId, name, openFromMonth, openToMonth, dailyFee);

                        result.Add(id, campground);
                    }
                    return result;
                }
            }
            catch
            {
                throw;
            }


        }/// <summary>

        /// Find the available sites in a given campground between two dates
        /// </summary>
        /// <param name="campgroundId"></param> The campground Id to look for available sites
        /// <param name="fromDate"></param>The date from which to look for available sites
        /// <param name="toDate"></param>The date which to look for available sites
        /// <returns></returns> Returns a dictionary of available sites and their information
        public Dictionary<int, Site> FindAvailableSites(int campgroundId, DateTime fromDate, DateTime toDate)
        {

            Dictionary<int, Site> results = new Dictionary<int, Site>();

            string sqlGetSites = @"SELECT TOP 5 *
                                    FROM site
                                    WHERE site.campground_id = @campgroundId AND site.site_id NOT IN
                                    (
	                                    SELECT site.site_id
	                                    FROM site
	                                    JOIN reservation ON site.site_id = reservation.site_id
	                                    WHERE (reservation.from_date BETWEEN @fromDate AND @toDate) OR
	                                    (reservation.to_date BETWEEN @fromDate AND @toDate)
                                    )
                                    AND site.site_id IN (
	                                    SELECT site_id
	                                    FROM site
	                                    JOIN campground ON site.campground_id = campground.campground_id
	                                    WHERE (campground.open_from_mm <= @fromDateMonth AND campground.open_to_mm >= @toDateMonth)
                                    );";
            try
            {
                // create my connection object
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // open connection
                    conn.Open();

                    // create my command object
                    SqlCommand cmd = new SqlCommand(sqlGetSites, conn);

                    cmd.Parameters.AddWithValue("@campgroundId", campgroundId);
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);
                    cmd.Parameters.AddWithValue("@fromDateMonth", fromDate.Month);
                    cmd.Parameters.AddWithValue("@toDateMonth", toDate.Month);


                    // execute command
                    SqlDataReader reader = cmd.ExecuteReader();


                    // if applicable loop through result set
                    while (reader.Read())
                    {
                        // populate object(s) to return
                        int id = Convert.ToInt32(reader["site_id"]);
                        int siteCampgroundId = Convert.ToInt32(reader["campground_id"]);
                        int siteNum = Convert.ToInt32(reader["site_number"]);
                        int siteOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                        int accessible = Convert.ToInt32(reader["accessible"]);
                        int maxRvLength = Convert.ToInt32(reader["max_rv_length"]);
                        int utilities = Convert.ToInt32(reader["utilities"]);

                        Site site = new Site(id, siteCampgroundId, siteNum, siteOccupancy, accessible, maxRvLength, utilities);

                        results.Add(siteNum, site);
                    }
                    return results;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add a reservation for a campsite
        /// </summary>
        /// <param name="siteId"></param> The site Id to add the reservation.
        /// <param name="reservationName"></param> The name of the person reserving the campsite.
        /// <param name="fromDate"></param> The date from which the reservation starts
        /// <param name="toDate"></param>The date to which the reservation ends
        /// <returns></returns> Returns the reservation Id
        public int AddReservation(int siteId, string reservationName, DateTime fromDate, DateTime toDate)
        {
            int result = 0;

            string SqlAddReservation = @"INSERT INTO reservation(site_id, name, from_date, to_date)
                                       VALUES(@siteId, @reservationName, @fromDate, @toDate);";
            try
            {

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // open connection
                    conn.Open();

                    // create my command object
                    SqlCommand cmd = new SqlCommand(SqlAddReservation + _getLastIdSQL, conn);

                    cmd.Parameters.AddWithValue("@siteId", siteId);
                    cmd.Parameters.AddWithValue("@reservationName", reservationName);
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);

                    // execute command
                    result = (int)cmd.ExecuteScalar();

                    return result;
                }

            }
            catch
            {
                throw;
            }
        }

        public Reservation GetReservation(int reservationId)
        {
            
            string SqlGetReservation = @"   SELECT *
                                            FROM reservation
                                            WHERE reservation.reservation_id = @reservationId";

            Reservation reservation = null;

            try
            {

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // open connection
                    conn.Open();

                    // create my command object
                    SqlCommand cmd = new SqlCommand(SqlGetReservation, conn);

                    cmd.Parameters.AddWithValue("@reservationId", reservationId);

                    // execute command
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // populate object(s) to return
                        int id = Convert.ToInt32(reader["reservation_id"]);
                        int siteId = Convert.ToInt32(reader["site_id"]);
                        string name = Convert.ToString(reader["name"]);
                        DateTime fromDate = Convert.ToDateTime(reader["from_date"]);
                        DateTime toDate = Convert.ToDateTime(reader["to_date"]);
                        DateTime createDate = Convert.ToDateTime(reader["create_date"]);

                        reservation = new Reservation(id, siteId, name, fromDate, toDate, createDate);
                    }
                }
                return reservation;

            }
            catch
            {
                throw;
            }
        }
    }
}
