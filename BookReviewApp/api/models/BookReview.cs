using System;

namespace BookReviewApp.Models
{
    public class BookModelReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

