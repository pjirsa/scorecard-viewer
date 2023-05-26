using System;

namespace BlazorApp.Shared
{

    public class Match
    {
        public string Title => $"{Team1} vs {Team2}";
        public int Team1 { get; set; }
        public int Team2 { get; set; }
        public DateTime TeeTime { get; set; }
        public int WeekID { get; set; }
    }

}