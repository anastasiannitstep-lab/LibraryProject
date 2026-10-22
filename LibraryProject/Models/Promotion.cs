using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryProject.Models
{
    public class Promotion
    {
        public int promotion_id { get; set; }
        public string name { get; set; }
        public decimal discount_percent { get; set; }
        public int book_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
