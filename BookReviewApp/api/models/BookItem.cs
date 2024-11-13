using System;

namespace BookReviewApp.Models
{
    public class BookServiceItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Category { get; set; }
    }

}

