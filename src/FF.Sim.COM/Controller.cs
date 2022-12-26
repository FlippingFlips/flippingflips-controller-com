using FF.Sim.COM.Interface;
using FF.Sim.Domain;
using FF.Sim.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace FF.Sim.COM
{
    [ComVisible(true)]
    [GuidAttribute(ContractGuids.ControllerClass)]
    public class Controller : IController
    {
        IFlipsService flipsService;
        private SoundPlayer _player;
        List<Player> Players;
        List<ScoreResult> Scores;

        string _gameId { get; set; }
        public object[] NvRam { get; set; }

        private string _crc;        
        private string _fileName;
        private FlipsSettings _flipsSettings;

        private string _soundScoreSent = @".\SoundsFFlips\horse_fx_1.wav";

        public void Run(string gameId, string fileName)
        {
            _flipsSettings = FlipSettingsHelper.GetSettingsFromFile(@".\FlippingFlipsSettings.json");

            //run table file CRC check
            if (fileName == null)
                throw new NullReferenceException("table file must be provided");
            _fileName = Path.GetFileName(fileName);
            _crc = CrcEngine.GetCRC(fileName).ToString("X");    

            //setup controller from user settings
            flipsService = new FlipsService(_flipsSettings.ServerUrl, _flipsSettings.ApiKey);
            _gameId = gameId;

            //setup sounds if available
            if (File.Exists(_soundScoreSent))
                _player = new SoundPlayer(_soundScoreSent);
        }

        public string GetLastError()
        {
            return flipsService?.ErrorMessage;
        }

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
        /// <returns></returns>
        public bool StartGame(bool desktop, int ballsPerGame, int p1, int p2, int p3, int p4, string version)
        {
            var v = version?.Length > 10 ? version?.Substring(0, 10) : version;
            return flipsService.SetupGame(_gameId, desktop, p1, p2, p3, p4, ballsPerGame, v);
        }

        /// <summary>
        /// Submits score to server
        /// </summary>
        /// <param name="p1Score"></param>
        /// <param name="p2Score"></param>
        /// <param name="p3Score"></param>
        /// <param name="p4Score"></param>
        /// <returns></returns>
        public bool EndGame(long p1Score, long p2Score, long p3Score, long p4Score)
        {
            var result = flipsService.PostGame(new PostScoresDto
            {
                CRC =_crc, FileName= _fileName, Score1= p1Score, Score2= p2Score, Score3= p3Score, Score4 = p4Score
            });

            if(result)
                _player?.Play();
            return result;
        }

        public bool EndGameNv()
        {
            var result = flipsService.PostGame(new PostScoresDto { CRC = _crc, NvRam = NvRam, FileName = _fileName });
            if (result)
                _player?.Play();
            return result;
        }

        public bool EndGameNvNoLastScoreSupport(long p1Score, long p2Score, long p3Score, long p4Score)
        {
            var result = flipsService.PostGame(new PostScoresDto { CRC = _crc, NvRam = NvRam, FileName = _fileName, 
                Score1 = p1Score, Score2 = p2Score, Score3 = p3Score, Score4 = p4Score});
            if (result)
                _player?.Play();
            return result;
        }

        public object[,] AvailablePlayers()
        {
            try
            {
                Players = flipsService?.GetPlayers();

                if (Players?.Count > 0)
                {
                    var objArr = new object[Players.Count, 3];
                    for (int i = 0; i < Players.Count; i++)
                    {
                        objArr[i, 0] = Players[i].Id;
                        objArr[i, 1] = Players[i].Initials;
                        objArr[i, 2] = Players[i].Name;
                    }

                    return objArr;
                }
                else
                {
                    return new object[,] { };
                }
            }
            catch
            {
                return new object[,] { };
            }
        }

        public bool CreatePlayer(string init, string name)
        {
            return flipsService.CreatePlayer(init, name);
        }

        public string GetGameInformation()
        {
            var gameInfo = flipsService.GameInformation(_gameId);
            if(gameInfo != null)
            {
                var infoStr = $"{gameInfo.Title} : V{gameInfo.Version} {Environment.NewLine}";
                infoStr += $"{gameInfo.Author} {Environment.NewLine}";
                infoStr += $"Total records: {gameInfo.ScoresCount}{Environment.NewLine}";
                infoStr += $"Last Game: {gameInfo.LastGame.Value.ToString("d")}{Environment.NewLine}";
                return infoStr;
            }
            else
            {
                return string.Empty;
            }
        }

        public object[,] GetScores()
        {
            Scores = flipsService?.GetScores(_gameId, _flipsSettings.LatestScoreSettings.IncludeOtherMachines, _flipsSettings.LatestScoreSettings.GetLatestOrTopScores);
            if (Scores?.Count > 0)
            {
                var objArr = new object[Scores.Count, 6];
                for (int i = 0; i < Scores.Count; i++)
                {
                    objArr[i, 0] = Scores[i].Points;
                    objArr[i, 1] = Scores[i].Initials;
                    objArr[i, 2] = Scores[i].MachineUser;
                    objArr[i, 3] = Scores[i].Balls;
                    objArr[i, 4] = Scores[i].Desktop;
                    objArr[i, 5] = Scores[i].SimVersion;
                }
                return objArr;
            }
            else
            {
                return new object[,] { };
            }
        }
    }
}
