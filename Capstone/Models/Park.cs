using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    /// <summary>
    /// Park class holds information about the park
    /// </summary>
    public class Park : BaseItem
    {
        public string Name { get; }                    // The park name
        public string Location { get; }                // The park location
        public DateTime EstablishDate { get; }         // The date the park was established
        public int Area { get; }                       // The area of the park in square kilometers
        public int AnnualVisitors { get; }             // The number of annual visitors
        public string Description { get; }             // The description of the park

        public Park(int id, string name, string location, DateTime establishDate, int area, int annualVisitors, string description): base(id)
        {
            Name = name;
            Location = location;
            EstablishDate = establishDate;
            Area = area;
            AnnualVisitors = annualVisitors;
            Description = description;
        }
    }
}
