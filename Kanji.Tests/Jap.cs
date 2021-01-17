using System.Linq;
using JetBrains.Annotations;

namespace Kanji.Tests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class Jap
    {
        public const string Hiragana =
            "あいうえおかきくけこがぎぐげごさしすせそざじずぜぞたちつてと" +
            "だぢづでどなにぬねのはひふへほばびぶべぼぱぴぷぺぽまみむめも" +
            "やゆ𛀁よらりるれろわゐゑをんゃょゅっ";

        public const string Katakana =
            "アイウエオカキクケコガギグゲゴサシスセソザジズゼゾタチツテト" +
            "ダヂヅデドナニヌネノハヒフヘホバビブベボパピプペポマミムメモ" +
            "ヤユ𛀀ヨラリルレロワヰヱヲンじャュョッー";


        public static bool IsKana(char c) => Hiragana.Contains(c) || Katakana.Contains(c);
        public static bool IsKanaOnly(string str) => str.All(IsKana);
    }
}