using FF.Sim.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FF.Sim.Domain
{
    public partial class FlipsService : IFlipsService
    {
        private readonly string serverUrl;
        private readonly string apiKey;
        public HttpClient Client { get; }
        /// <summary>
        /// This is the game in progress Id
        /// </summary>
        public string CurrentGameId { get; set; }
        public string ErrorMessage { get; set; }
        public string GameId { get; set; }
        /// <summary>
        /// Used for camel cased properties
        /// </summary>
        private readonly JsonSerializerSettings serializerSettings;

        public FlipsService(string serverUrl, string apiKey)
        {
            this.serverUrl = serverUrl;
            this.apiKey = apiKey;

            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("FlippingFlips (compatible; HorsePinInc/3.6.9)");
            serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        public bool CreatePlayer(string init, string name)
        {
            var json = new { api_key = apiKey, Initials = init, Name = name };
            var content = CreateStringContent(json);
            try
            {
                var response = Client.PostAsync(serverUrl + $"/usermachine/CreatePlayer?api_key={apiKey}", content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = response.ReasonPhrase + ":" + response.Content.ReadAsStringAsync().Result;
                    return false;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.ToString();
            }

            return false;
        }

        public GameInformation GameInformation(string gameId)
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ErrorMessage = "No api key found";
                return null;
            }

            var json = new
            {
                api_key = apiKey,
                gameId = gameId,                
            };
            var content = CreateStringContent(json);
            try
            {
                var response = Client.PostAsync(serverUrl + $"/usermachine/gameinformation?api_key={apiKey}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var cnt = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<GameInformation>(cnt);
                }
                else
                {
                    SetErrorResponse("GameInfo Error", response);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return null;
        }

        public List<ScoreResult> GetScores(string gameId = null, bool includeOtherMachines = false, bool getTop = false)
        {
            //throw new Exception("no api key");
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ErrorMessage = "No api key found";
                return null;
            }
            
            var json = new
            {
                api_key = apiKey,
                gameId = gameId,               
                getTop = getTop,
                includeOtherMachines = includeOtherMachines
            };
            var content = CreateStringContent(json);
            try
            {
                var response = Client.PostAsync(serverUrl + $"/usermachine/getscores?api_key={apiKey}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var cnt = response.Content.ReadAsStringAsync().Result;
                    var scores = JsonConvert.DeserializeObject<List<ScoreResult>>(cnt.Trim());
                    return scores;
                }
                else
                {
                    SetErrorResponse("GetScores Error", response);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.ToString();                
            }

            return null;
        }

        public List<Player> GetPlayers()
        {
            try
            {
                var playersJson = Client.GetAsync(serverUrl + $"/usermachine/players?api_key={apiKey}")?.Result;
                if (playersJson.IsSuccessStatusCode)
                {
                    var result = playersJson.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return JsonConvert.DeserializeObject<List<Player>>(result);
                    }
                    else
                    {
                        ErrorMessage = "Returned empty players from server";
                    }
                }
                else
                {
                    
                    ErrorMessage = playersJson.ReasonPhrase + ":" + playersJson.Content.ReadAsStringAsync().Result;
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"GetPlayers failed, apikey or no server?: {ex.Message}";
            }
            
            return null;
        }

        public bool PostGame(PostScoresDto post)
        {
            if (string.IsNullOrWhiteSpace(CurrentGameId))
            {
                ErrorMessage = "No game in progress id for the current game";
                return false;
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ErrorMessage = "No API Key available";
                return false;
            }

            if (post.Score1.HasValue && post.Score1.Value > 0)
            {
                var json = new
                {
                    api_key = apiKey,
                    id = CurrentGameId,
                    crc = post.CRC,
                    p1Score = post.Score1,
                    p2Score = post.Score2,
                    p3Score = post.Score3,
                    p4Score = post.Score4
                };
                var content = CreateStringContent(json);
                var response = Client.PostAsync(serverUrl + $"/usermachine/addgameplayed?api_key={apiKey}", content).Result;

                CurrentGameId = null;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    SetErrorResponse("PostGameError", response);
                    return false;
                }
            }

            if (post.NvRam?.Length > 0)
            {
                //convert to bytes, serialize to base64
                byte[] bytes = new byte[post.NvRam.Length];
                post.NvRam.CopyTo(bytes, 0);
                var json = new
                {
                    api_key = apiKey,
                    id = CurrentGameId,
                    NvRamBase64 = Convert.ToBase64String(bytes),
                    crc = post.CRC
                };

                //send to server
                var content = CreateStringContent(json);
                var response = Client.PostAsync(serverUrl + $"/usermachine/addgameplayed?api_key={apiKey}", content).Result;

                //set gameid null and return response
                CurrentGameId = null;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    SetErrorResponse("PostGameNvError", response);
                    return false;
                }
            }
            else
            {
                ErrorMessage = "no scores or nvram";
                return false;
            }
        }

        public bool SetupGame(string gameId, bool isDesktop, int p1, int p2, int p3, int p4, int ballsPerGame, string version)
        {
            byte.TryParse(ballsPerGame.ToString(), out byte bpg);
            byte balls = bpg > 0 ? bpg : (byte)3;
            var setUpOption = new GameSetupOption { api_key = apiKey, GameId = gameId, Player1Id = p1, Player2Id = p2, Player3Id = p3, Player4Id = p4, BallsPerGame = balls, Desktop = isDesktop, SystemVersion = version };
            var content = CreateStringContent(setUpOption);
            var response = Client.PostAsync(serverUrl + $"/usermachine/startgame?api_key={apiKey}", content).Result;
            if (response.IsSuccessStatusCode)
            {
                CurrentGameId = response.Content.ReadAsStringAsync().Result;
                return !string.IsNullOrWhiteSpace(CurrentGameId);
            }

            ErrorMessage = response.ReasonPhrase + ":" + response.Content.ReadAsStringAsync().Result;
            return false;
        }

        private void SetErrorResponse(string msg, HttpResponseMessage response)
        {
            ErrorMessage = msg + ":" +  response.ReasonPhrase + ":" + response.Content.ReadAsStringAsync().Result;
        }

        private StringContent CreateStringContent(object obj) => new StringContent(JsonConvert.SerializeObject(obj, serializerSettings), Encoding.UTF8, "application/json");
    }
}
