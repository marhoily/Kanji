using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Kanji.Tests
{
    public record VocabCard(
        string Expression,
        string Reading,
        string Meaning,
        string Tags)
    {
        public static List<VocabCard> Read()
        {
            using var text = File.OpenText(
                @"C:\git\open-anki-jlpt-decks\src\n5.csv");
            var csvReader = new CsvReader(text,
                new CsvConfiguration(new CultureInfo("ja-JP"))
                {
                    PrepareHeaderForMatch = (name, _) => name.ToLower()
                });

            return csvReader.GetRecords<VocabCard>().ToList();
        }

        public override string ToString() => $"{Expression} - {Meaning}";
    }
}