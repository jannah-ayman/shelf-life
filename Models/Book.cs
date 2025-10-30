﻿namespace WebApplication2Shelf_Life.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryID { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public string Description { get; set; }
        public ICollection<Listings>listings { get; set; } = new List<Listings>();
    }
}
