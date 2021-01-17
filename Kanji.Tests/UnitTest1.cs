using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Kanji.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private static readonly List<AnkiRecord> AnkiRecords = AnkiRecord.Read();

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const string Hiragana =
            "あいうえおかきくけこがぎぐげごさしすせそざじずぜぞたちつてと" +
            "だぢづでどなにぬねのはひふへほばびぶべぼぱぴぷぺぽまみむめも" +
            "やゆ𛀁よらりるれろわゐゑをんゃょゅっ";

        private const string Katakana =
            "アイウエオカキクケコガギグゲゴサシスセソザジズゼゾタチツテト" +
            "ダヂヅデドナニヌネノハヒフヘホバビブベボパピプペポマミムメモ" +
            "ヤユ𛀀ヨラリルレロワヰヱヲンじャュョッー";

        public record Card(
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
            [JsonProperty("wk_radicals")] string[] WkRadicals);
     

        private static bool IsKana(char c) => Hiragana.Contains(c) || Katakana.Contains(c);
        private static bool IsKanaOnly(string str) => str.All(IsKana);

        [Fact]
        public void Vocab()
        {
            AnkiRecords[1].Should().Be(
                    new AnkiRecord(
                        "会う", "あう", "to meet, to see",
                        "JLPT JLPT_3 JLPT_5 JLPT_N5"));

            AnkiRecords
                .Where(r => r.Expression.Contains("先"))
                .Select(r => r.Expression)
                .Should().Equal("先", "先月", "先週", "先生");
            AnkiRecords
                .Where(r => r.Expression.Contains("生"))
                .Select(r => r.Expression)
                .Should().Equal("生まれる", "学生", "生徒", "先生", "誕生日", "留学生");
        }

       
        [Fact]
        public void Kanji()
        {
            JsonConvert
                .DeserializeObject<Dictionary<string, Card>>(
                    File.ReadAllText(@"C:\git\Kanji\kanji.json"))
                .Where(l => l.Value.JlptNew == 5)
                .Select(l => l.Key)
                .StrJoin("").Should()
                .Be("一二九七人入八十三上下大女山川土千子小中五六円天日月木" +
                    "水火出右四左本白万今午友父北半外母休先名年気百男見車毎" +
                    "行西何来学金雨国東長前南後食校時高間話電聞語読生書");
        }
    }
}