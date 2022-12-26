namespace FF.Sim.Domain.Models
{
    public class PostScoresDto
    {
        public string CRC { get; set; }
        public string FileName { get; set; }
        public long? Score1 { get; set; }
        public long? Score2 { get; set; }
        public long? Score3 { get; set; }
        public long? Score4 { get; set; }
        public object[] NvRam { get; set; }
    }
}