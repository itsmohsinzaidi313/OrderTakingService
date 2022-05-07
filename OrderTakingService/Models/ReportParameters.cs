using System;

namespace OrderTakingService.Models
{
    public class ReportParameters
    {
        public string ReportName { get; set; }
        public int Type { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string ShiftFirst { get; set; }
        public string ShiftLast { get; set; }

        public string DayFirst { get; set; }
        public string DayLast { get; set; }

        public string TiltId { get; set; }
        public string UserId { get; set; }

        public bool DayWiseSale { get; set; }
        public bool ShiftWiseSale { get; set; }
        public bool DateWiseSale { get; set; }
        public bool UserWiseSale { get; set; }
        public bool TiltWiseSale { get; set; }
    }
}