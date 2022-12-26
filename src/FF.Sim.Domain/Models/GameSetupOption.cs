namespace FF.Sim.Domain
{
    public class GameSetupOption
    {
        public string api_key { get; set; }
        public string GameId { get; set; }
        public int Player1Id { get; set; }
        public int? Player2Id { get; set; }
        public int? Player3Id { get; set; }
        public int? Player4Id { get; set; }
        public byte BallsPerGame { get; set; }
        public bool Desktop { get; set; }
        public string SystemVersion { get; set; }
    }
}
