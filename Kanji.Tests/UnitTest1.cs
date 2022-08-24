using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using Xunit;
using Xunit.Abstractions;

namespace Kanji.Tests
{
    public class UnitTest1
    {
        private static readonly List<VocabCard> AnkiRecords = VocabCard.Read();
        private static readonly Dictionary<string, KanjiCard> KanjiCards = KanjiCard.Read();

        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper testOutputHelper) => _output = testOutputHelper;

        [Fact]
        public void Vocab()
        {
            AnkiRecords[1].Should().Be(
                    new VocabCard(
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
            KanjiCards
                .Where(l => l.Value.JlptNew == 5)
                .Select(l => l.Key)
                .StrJoin("").Should()
                .Be("一二九七人入八十三上下大女山川土千子小中五六円天日月木" +
                    "水火出右四左本白万今午友父北半外母休先名年気百男見車毎" +
                    "行西何来学金雨国東長前南後食校時高間話電聞語読生書");
        }

        [Fact]
        public void Vocab_With_Only_Lvl5_Kanji()
        {
            var allowed = new HashSet<char>(KanjiCards
                .Where(l => l.Value.JlptNew == 5)
                .Select(l => l.Key.Single())
                .Concat(Jap.Kana));
            var words = AnkiRecords
                .Where(r => r.Expression.All(allowed.Contains))
                .ToList();
            var kanjiList = words.SelectMany(x => x.Expression)
                .Where(c => !Jap.IsKana(c))
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .ToDictionary(x => x.Key, x => KanjiCards[x.Key.ToString()]);
            var kanjiWithReadings = kanjiList.Select(kanji => words
                .Where(word => word.Expression.Contains(kanji.Key))
                .SelectMany(word => kanji.Value.Readings()
                    .Where(reading => word.Reading.Contains(reading))
                    .OrderByDescending(reading => reading.Length)
                    .Take(1)
                    .Select(reading => (kanji, reading, word)))
                .Distinct())
                .SelectMany(x => x)
                .ToLookup(x => x.kanji.Key.ToString(), x => (x.kanji, x.reading, x.word));
            kanjiWithReadings.Should().HaveCount(43);
            var conflictingReadings = kanjiWithReadings
                .SelectMany(x => x.Select(y => (y.reading, kanji: x.Key)))
                .GroupBy(r => r.reading)
                .SelectMany(r => r.GroupBy(u => u.kanji)
                    .OrderByDescending(g => g.Count())
                    .Skip(1))
                .SelectMany(x => x)
                .ToLookup(x => x.kanji, x => x.reading);

            conflictingReadings.Should().HaveCount(3);

            var interestingKanji = kanjiWithReadings
                .SelectMany(x => x)
                .Where(x => x.reading != x.word.Reading)
                .Select(x => x.kanji.Key)
                .Distinct()
                .ToImmutableHashSet();
            interestingKanji.Should().HaveCount(35);

            var wordsToStudy =
                 AnkiRecords
                .Where(r => r.Expression.All(interestingKanji.Contains))
                .Where(r => r.Expression.Length > 1)
                .Select(w => $"{w.Reading} {w.Expression} {w.Meaning}")
                .ToList();
            wordsToStudy.Should().HaveCount(35);

            var kanjiToStudy = interestingKanji
                .Select(k => kanjiWithReadings[k.ToString()])
                .SelectMany(x => x)
                .DistinctBy(x => x.reading)
                .Select(k => $"{k.reading} {k.kanji.Key} {k.kanji.Value.Meanings.StrJoin()}")
                .ToList();
            kanjiToStudy.Should().HaveCount(76);
            _output.WriteLine(kanjiToStudy.StrJoin("\n"));
            _output.WriteLine(wordsToStudy.StrJoin("\n"));
            // TODO: 午 is present in words, but not as a separate kanji. Why?
            // TODO: list of kanji for selfcheck
            // TODO: group kanji\words
        }
    }
}