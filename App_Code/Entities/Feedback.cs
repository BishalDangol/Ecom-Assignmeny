using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents customer feedback/reviews.
    /// </summary>
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int MemberId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int Rating { get; set; }
        public string Status { get; set; }
        public string AdminReply { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RepliedAt { get; set; }

        public Feedback() { }

        public override string ToString()
        {
            return $"Feedback: {Subject} (Rating: {Rating})";
        }
    }
}
