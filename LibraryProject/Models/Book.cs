using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryProject.Models
{
    public class Book
    {
        public int book_id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string publisher { get; set; }
        public string genre { get; set; }
        public int? pages_count { get; set; }
        public int? publish_year { get; set; }
        public decimal cost_price { get; set; }
        public decimal retail_price { get; set; }
        public int stock_quantity { get; set; }
        public int? parent_book_id { get; set; }
    }
}
