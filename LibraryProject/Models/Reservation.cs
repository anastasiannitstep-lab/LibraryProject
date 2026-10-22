using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryProject.Models
{
    public class Reservation
    {
        public int reservation_id { get; set; }
        public int book_id { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public int quantity { get; set; }
        public string status { get; set; } = "active";
    }
}
