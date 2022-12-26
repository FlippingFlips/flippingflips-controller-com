using System;
using System.Runtime.InteropServices;

namespace FF.Sim.COM.Interface
{
    [ComVisible(true)]
    [Guid(ContractGuids.ControllerInterface)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IController
    {
        /// <summary>
        /// Sets up game, reads users settings file
        /// </summary>
        /// <param name="gameId">Id of server game</param>
        /// <param name="fileName">Table filename, passed from VP</param>
        [ComVisible(true)]
        void Run(string gameId, string fileName);

        /// <summary>
        /// Creates a player on the server. Must be unique initials
        /// </summary>
        /// <param name="init"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [ComVisible(true)]
        bool CreatePlayer(string init, string name);

        /// <summary>
        /// Sets up a game in progress
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="desktop"></param>
        /// <param name="ballsPerGame"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [ComVisible(true)]
        bool StartGame(bool desktop, int ballsPerGame, int p1, int p2, int p3, int p4, string version);

        /// <summary>
        /// Posts the scores
        /// </summary>
        /// <param name="p1Score"></param>
        /// <param name="p2Score"></param>
        /// <param name="p3Score"></param>
        /// <param name="p4Score"></param>
        /// <returns></returns>
        [ComVisible(true)]
        bool EndGame(long p1Score, long p2Score, long p3Score, long p4Score);

        /// <summary>
        /// Ends a PinMame game
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        bool EndGameNv();

        /// <summary>
        /// Ends a pinmame game with no last score support, like older games system 9
        /// </summary>
        /// <param name="p1Score"></param>
        /// <param name="p2Score"></param>
        /// <param name="p3Score"></param>
        /// <param name="p4Score"></param>
        /// <returns></returns>
        [ComVisible(true)]
        bool EndGameNvNoLastScoreSupport(long p1Score, long p2Score, long p3Score, long p4Score);

        [ComVisible(true)]
        object[] NvRam { get; set; }

        /// <summary>
        /// Returns available players to VP in object array. Id, Initials and Name
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        object[,] AvailablePlayers();

        /// <summary>
        /// Access last error message from visual pinball
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        string GetLastError();

        /// <summary>
        /// Returns game information for this game
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        string GetGameInformation();

        /// <summary>
        /// Gets scores for this game
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        object[,] GetScores();
    }
}
