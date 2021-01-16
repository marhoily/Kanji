using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
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
        public Card[] Set { get; set; }
        private readonly Random _rnd = new Random();
        // ReactiveCommand allows us to execute logic without exposing any of the 
        // implementation details with the View. The generic parameters are the 
        // input into the command and its output. In our case we don't have any 
        // input or output so we use Unit which in Reactive speak means a void type.
        public ReactiveCommand<Unit, Unit> MoveNext { get; }
        public AppViewModel()
        {
            _synthesizer = GetSpeechSynthesizer();

            MoveNext = ReactiveCommand.Create(() =>
            {
                if (Set != null)
                {
                    while (true)
                    {
                        var currentTerm = Set[_rnd.Next(Set.Length)];
                        if (currentTerm != CurrentTerm)
                        {
                            CurrentTerm = currentTerm;
                            break;
                        }
                    }

                    Pronounce();
                }
            });
        }

        private void Pronounce()
        {
            if (_currentPrompt != null)
                _synthesizer.SpeakAsyncCancel(_currentPrompt);
            _currentPrompt = _synthesizer.SpeakAsync(CurrentTerm.ToPronounce.Replace(",", ", or"));
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

            // Configure the audio output.   
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Rate = -2;

            return synthesizer;
        }

        // In ReactiveUI, this is the syntax to declare a read-write property
        // that will notify Observers, as well as WPF, that a property has 
        // changed. If we declared this as a normal property, we couldn't tell 
        // when it has changed!
        private Card _currentTerm;
        private readonly SpeechSynthesizer _synthesizer;
        private Prompt _currentPrompt;

        public Card CurrentTerm
        {
            get => _currentTerm;
            set => this.RaiseAndSetIfChanged(ref _currentTerm, value);
        }
    }
}