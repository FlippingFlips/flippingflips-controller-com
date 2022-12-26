using FF.Sim.Domain.Models;
using System.Collections.Generic;

namespace FF.Sim.Domain
{
    public interface IFlipsService
    {        
        string ErrorMessage { get; set; }
        string GameId { get; set; }
        bool CreatePlayer(string init, string name);
        GameInformation GameInformation(string gameId);
        List<Player> GetPlayers();
        List<ScoreResult> GetScores(string gameId = null, bool includeOtherMachines = false, bool getTop = false);
        bool PostGame(PostScoresDto postScoresDto);
        bool SetupGame(string gameId, bool isDesktop, int p1, int p2, int p3, int p4, int ballsPerGame, string version);
    }
}