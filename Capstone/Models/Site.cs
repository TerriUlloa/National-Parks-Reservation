using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    /// <summary>
    /// The site class stores the information about the camp site
    /// </summary>
    public class Site : BaseItem
    {                 
        public int CampgroundId { get; }       //The campground Id
        public int SiteNum { get; }            //The site number
        public int SiteOccupancy { get; }      //The maximum occupancy of the site
        public int Accessible { get; }         //Is the site accessible
        public string DisplayAccessible        //A derived variable to display the accessible string message
        {
            get
            {
                string result = "No";
                if (Accessible == 1)
                {
                    result = "Yes";
                }
                return result;
            }
        }
        public int MaxRVLength { get; }        //The maximum RV length
        public string DisplayMaxRVLength       //A derived variable to display the maximum RV length message
        {
            get
            {
                string result = "N/A";
                if (MaxRVLength > 0)
                {
                    result = MaxRVLength.ToString();
                }
                return result;

            }
        }
        public int Utilities { get; }          //Does the site have utilities
        public string DisplayUtilities         //A derived variable to display the string message if the site has utilities
        {
            get
            {
                string result = "N/A";
                if (Utilities == 1)
                {
                    result = "Yes";
                }
                return result;
            }
        }

        public Site(int id, int campgroundId, int siteNum, int siteOccupancy, int accessible, int maxRvLength, int utilities): base(id)
        {        
            CampgroundId = campgroundId;
            SiteNum = siteNum;
            SiteOccupancy = siteOccupancy;
            Accessible = accessible;
            MaxRVLength = maxRvLength;
            Utilities = utilities;
        }
    }
}
