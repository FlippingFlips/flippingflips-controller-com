using System;

namespace FF.Sim.Domain
{
    public class GameInformation
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public int ScoresCount { get; set; }
        public DateTime? LastGame { get; set; }
    }
}
