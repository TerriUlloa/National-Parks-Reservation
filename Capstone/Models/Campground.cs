using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    /// <summary>
    /// Campground class holds information about the campground and a couple derived properties for display.
    /// </summary>
    public class Campground : BaseItem
    {
        
        public string Name { get; }            // The campground name
        public int ParkId { get; }          // The park Id the campground exists in
        public int OpenFromMonth { get; }      // The  month the campground opens
        public int OpenToMonth { get; }        // The month the campground closes
        public decimal DailyFee { get; }       // The daily fee for campground usage

        private Dictionary<int, string> _months = new Dictionary<int, string>() {
                    { 1, "January" },
                    { 2, "Febuary" },
                    { 3, "March" },
                    { 4, "April" },
                    { 5, "May" },
                    { 6, "June" },
                    { 7, "July" },
                    { 8, "August" },
                    { 9, "September" },
                    { 10, "October" },
                    { 11, "November" },
                    { 12, "December" },
                };      // A dictonary of months to for string conversion

        public string DisplayOpenToMonth            // Derived property to convert the date the campground closes to a string
        {
            get
            {
                string result = _months[OpenToMonth];

                return result;
            }
        }

        public string DisplayOpenFromMonth          // Derived property to convert the date the campground closes to a string
        {
            get
            {
                string result = _months[OpenFromMonth];

                return result;
            }
        }

        public Campground(int id, int parkId, string name, int openFromMonth, int openToMonth, decimal dailyFee): base(id)
        {
            ParkId = parkId;
            Name = name;
            OpenFromMonth = openFromMonth;
            OpenToMonth = openToMonth;
            DailyFee = dailyFee;
        }
    }
}
