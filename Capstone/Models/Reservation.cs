using Capstone.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation : BaseItem
    {

        public int SiteId { get; }
        public string Name { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public DateTime CreateDate { get; }


        public Reservation(int id, int siteId, string name, DateTime fromDate, DateTime toDate, DateTime createDate) : base(id)
        {
            SiteId = siteId;
            Name = name;
            FromDate = fromDate;
            ToDate = toDate;
            CreateDate = createDate;
        }

        public bool VerifyValidFromDateAndToDate()
        {
            bool result = false;

            if (FromDate > ToDate && FromDate >= DateTime.Now)
            {
                result = true;
            }

            return result;
        }

        
    }
}
