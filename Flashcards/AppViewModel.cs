using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Speech.Synthesis;
using System.Threading;
using ReactiveUI;

namespace WpfApp1
{
    public sealed class Card
    {
        public string Kanji { get; set; }
        public string Meaning { get; set; }
        public string ToPronounce { get; set; }
        public override string ToString()
        {
            return $"{Kanji}: {ToPronounce}";
        }
    }
    public class AppViewModel : ReactiveObject
    {
        private readonly Random _rnd = new Random();
        public ReactiveCommand<Unit, Unit> MoveNextCmd { get; }
        public AppViewModel()
        {
            _synthesizer = GetSpeechSynthesizer();
            MoveNextCmd = ReactiveCommand.Create(MoveNext);
            _currentTerm = this
                .WhenAnyValue(x => x.CurrentTermIndex, x => x.SourceSet)
                .Select(x => x.Item2 == null || x.Item1 == -1 ? new Card {ToPronounce = "<press any key>"} : x.Item2[x.Item1])
                .ToProperty(this, x => x.CurrentTerm);

            _previousTerm = this
                .WhenAnyValue(x => x.CurrentTermIndex, x => x.SourceSet)
                .Select(x => x.Item2 == null || x.Item1 < 1 ? null : x.Item2[x.Item1 - 1])
                .ToProperty(this, x => x.PreviousTerm);

            this.WhenAnyValue(x => x.CurrentTerm)
                .Subscribe(_ => Pronounce());
        }
        public void MoveNext()
        {
            if (SourceSet == null) return;
            int nextVal = (CurrentTermIndex + 1) % SourceSet.Length;
            if (nextVal == 0)
                _rnd.Shuffle(SourceSet);
            CurrentTermIndex = nextVal;

        }
        private void Pronounce()
        {
            if (CurrentTerm == null) return;
            if (_currentPrompt != null)
                _synthesizer.SpeakAsyncCancel(_currentPrompt);
            _currentPrompt = _synthesizer.SpeakAsync(CurrentTerm.ToPronounce);
        }

        private static SpeechSynthesizer GetSpeechSynthesizer()
        {
            var culture = new CultureInfo("ja-JP");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            var synthesizer = new SpeechSynthesizer();
            var voices = synthesizer.GetInstalledVoices();
            synthesizer.SelectVoice(voices
                .First(v => v.VoiceInfo.Culture.DisplayName == culture.DisplayName)
                .VoiceInfo.Name);
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Rate = -3;
            return synthesizer;
        }

        private readonly SpeechSynthesizer _synthesizer;
        private Prompt _currentPrompt;

        private int _currentTermIndex = -1;
        public int CurrentTermIndex
        {
            get => _currentTermIndex;
            set => this.RaiseAndSetIfChanged(ref _currentTermIndex, value);
        }
        private Card[] _sourceSet;
        public Card[] SourceSet
        {
            get => _sourceSet;
            set => this.RaiseAndSetIfChanged(ref _sourceSet, value);
        }
        private readonly ObservableAsPropertyHelper<Card> _currentTerm;
        public Card CurrentTerm => _currentTerm.Value;

        private readonly ObservableAsPropertyHelper<Card> _previousTerm;
        public Card PreviousTerm => _previousTerm.Value;

    }
}