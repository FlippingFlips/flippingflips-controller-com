using FF.Sim.COM;

namespace FF.SimCOM.Tests
{
    /// <summary>
    /// Integration tests with a running Flips API
    /// </summary>
    public class ControllerTests
    {
        public readonly Controller Controller;
        public ControllerTests()
        {
            Controller = new Controller();
            Controller.Server = StaticTestData.API_URL;
            Controller.ApiKey = StaticTestData.API_KEY;
            Controller.GameId = StaticTestData.TEST_GAME_ID;
        }

        [Fact]
        public void GetPlayers_Tests()
        {
            Controller.Run();
            var players = Controller.AvailablePlayers();
            Assert.True(players?.Length > 0);
        }
    }
}