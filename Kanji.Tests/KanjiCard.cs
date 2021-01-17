using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Kanji.Tests
{
    public record KanjiCard(
        int? Strokes,
        int? Grade,
        int? Freq,
        [JsonProperty("jlpt_old")] int? JlptOld,
        [JsonProperty("jlpt_new")] int? JlptNew,
        string[] Meanings,
        [JsonProperty("readings_on")] string[] ReadingsOn,
        [JsonProperty("readings_kun")] string[] ReadingsKun,
        [JsonProperty("wk_level")] int? WkLevel,
        [JsonProperty("wk_meanings")] string[] WkMeanings,
        [JsonProperty("wk_readings_on")] string[] WkReadingsOn,
        [JsonProperty("wk_readings_kun")] string[] WkReadingsKun,
        [JsonProperty("wk_radicals")] string[] WkRadicals)
    {
        public static Dictionary<string, KanjiCard> Read()
        {
            return JsonConvert
                .DeserializeObject<Dictionary<string, KanjiCard>>(
                    File.ReadAllText(@"C:\git\Kanji\kanji.json"));
        }

        public IEnumerable<string> Readings() => 
            ReadingsOn
                .Concat(ReadingsKun)
                .Select(r=> r.Replace("-", "").Replace(".", ""))
                .Distinct();

        public override string ToString() => $"{Meanings.First()}";
    }
}