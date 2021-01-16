using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Speech.Synthesis;
using ReactiveUI;

namespace WpfApp1
{
    public class AppViewModel : ReactiveObject
    {  
        public string[] Set { get; set; }
        private readonly Random _rnd = new Random();
        // ReactiveCommand allows us to execute logic without exposing any of the 
        // implementation details with the View. The generic parameters are the 
        // input into the command and its output. In our case we don't have any 
        // input or output so we use Unit which in Reactive speak means a void type.
        public ReactiveCommand<Unit, Unit> NextTerm { get; }
        public AppViewModel()
        {
            _synthesizer = GetSpeechSynthesizer();

            NextTerm = ReactiveCommand.Create(() =>
            {
                if (Set != null)
                {
                    CurrentTerm = Set[_rnd.Next(Set.Length)];
                    Pronounce();
                }
            });
        }

        private void Pronounce()
        {
            _synthesizer.SpeakAsync(CurrentTerm);
        }

        private static SpeechSynthesizer GetSpeechSynthesizer()
        {
            var ci = new System.Globalization.CultureInfo("ja-JP");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            var synthesizer = new SpeechSynthesizer();
            var jp = synthesizer.GetInstalledVoices()
                .Single(v => v.VoiceInfo.Culture.DisplayName == ci.DisplayName);
            foreach (var installedVoice in synthesizer.GetInstalledVoices())
            {
                Console.WriteLine(installedVoice.VoiceInfo.Culture);
            }

            synthesizer.SelectVoice(jp.VoiceInfo.Name);

            // Configure the audio output.   
            synthesizer.SetOutputToDefaultAudioDevice();
            return synthesizer;
        }

        // In ReactiveUI, this is the syntax to declare a read-write property
        // that will notify Observers, as well as WPF, that a property has 
        // changed. If we declared this as a normal property, we couldn't tell 
        // when it has changed!
        private string _currentTerm;
        private SpeechSynthesizer _synthesizer;

        public string CurrentTerm
        {
            get => _currentTerm;
            set => this.RaiseAndSetIfChanged(ref _currentTerm, value);
        }
    }
}