using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Speech.Synthesis;
using ReactiveUI;

namespace WpfApp1
{
    public sealed class Card
    {
        public string ToShow { get; set; }
        public string ToPronounce { get; set; }
        public override string ToString()
        {
            return $"{ToShow}: {ToPronounce}";
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
                .Select(x => x.Item2 == null || x.Item1 == -1 ? null : x.Item2[x.Item1])
                .ToProperty(this, x => x.CurrentTerm);
                
            _previousTerm = this
                .WhenAnyValue(x => x.CurrentTermIndex, x => x.SourceSet)
                .Select(x => x.Item2 == null || x.Item1 < 1 ? null : x.Item2[x.Item1-1])
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
            var ci = new System.Globalization.CultureInfo("en-US");//"ja-JP");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            var synthesizer = new SpeechSynthesizer();
            var readOnlyCollection = synthesizer.GetInstalledVoices();
            foreach (var installedVoice in readOnlyCollection)
            {
                Console.WriteLine(installedVoice.VoiceInfo.Culture);
            }
            var jp = readOnlyCollection
                .First(v => v.VoiceInfo.Culture.DisplayName == ci.DisplayName);

            synthesizer.SelectVoice(jp.VoiceInfo.Name);

            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Rate = -2;

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