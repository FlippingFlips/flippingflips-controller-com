using FF.Sim.Domain;

namespace FF.SimCOM.Tests
{
    public class FlipsServiceAPI_Tests
    {
        private FlipsService _flip;
        public FlipsServiceAPI_Tests()
        {
            _flip = new FlipsService(StaticTestData.API_URL, StaticTestData.API_KEY);
            _flip.GameId = StaticTestData.TEST_GAME_ID;
        }

        [Fact]
        public void GetPlayers()
        {
            var players = _flip.GetPlayers();
            Assert.NotNull(players[0].Initials);

            var objArr = new object[players.Count, 3];
            for (int i = 0; i < players.Count; i++)
            {
                objArr[i, 0] = players[i].Id;
                objArr[i, 1] = players[i].Initials;
                objArr[i, 2] = players[i].Name;
            }
        }

        [Fact]

        public void CreatePlayer()
        {
            var result = _flip.CreatePlayer("ABC", "Gary");
            Assert.True(result);            
        }

        /// <summary>
        /// Sets up a new game, removes any games in progress
        /// </summary>
        [Fact]
        public void CreateGameInProgress()
        {
            var result = _flip.SetupGame(_flip.GameId, true, 1, 2, 0, 0, 3, "0.99");
            Assert.True(result);
        }

        /// <summary>
        /// Sets up a new game, removes any games in progress . FAILS BECAUSE current GAMEID NOT CARRIED OVER IN THESE TESTS use controller
        /// </summary>
        //[Fact]
        //public void GameCompleted()
        //{
        //    var result = _flip.PostGame(100, 200, 0, 0);
        //    Assert.True(result);
        //}
    }
}