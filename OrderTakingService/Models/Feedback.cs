using System.Collections.Generic;

namespace OrderTakingService.Models
{
    public class Feedback
    {
        public string Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string OrderKey { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public List<FeedbackQuestions> Questions { get; set; } = new List<FeedbackQuestions>();
        public List<FeedbackItems> Items { get; set; } = new List<FeedbackItems>();
    }

    public class FeedbackQuestions
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class FeedbackItems
    {
        public string ItemName { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
    }
}