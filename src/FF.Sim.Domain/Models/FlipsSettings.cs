using Newtonsoft.Json;
using System.IO;

namespace FF.Sim.Domain.Models
{
    public static class FlipSettingsHelper
    {
        public static FlipsSettings GetSettingsFromFile(string file)
        {
            if (!File.Exists(file)) throw new FileNotFoundException($"FlippingFlipsSettings.json not found at: {file}");
            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<FlipsSettings>(json);
        }
    }

    public class FlipsSettings
    {
        public string ApiKey { get; set; }
        public string ServerUrl { get; set; }
        public LatestScoreSettings LatestScoreSettings { get; set; }
    }

    public class LatestScoreSettings
    {
        /// <summary>
        /// Include other user machines in the query
        /// </summary>
        public bool IncludeOtherMachines { get; set; }

        /// <summary>
        /// Latest or Top scores
        /// </summary>
        public bool GetLatestOrTopScores { get; set; }
    }
}
