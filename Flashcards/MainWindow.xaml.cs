using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

namespace WpfApp1
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new AppViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                Task.Run(WhenActivated).GetAwaiter();

                Debug.Assert(ViewModel != null, nameof(ViewModel) + " != null");
                this.Events().KeyDown
                    .ThrottleFirst(
                        TimeSpan.FromMilliseconds(800),
                        RxApp.MainThreadScheduler)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(ViewModel.MoveNextCmd)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.Kanji,
                        view => view.Kanji.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.Meaning,
                        view => view.Meaning.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.ToPronounce,
                        view => view.Pronounciation.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.PreviousTerm.ToPronounce,
                        view => view.PreviousTerm.Text)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.MoveNextCmd,
                        view => view.MoveNext)
                    .DisposeWith(disposableRegistration);
            });
        }

        private async Task WhenActivated()
        {
            var set = File.ReadAllLines(@"C:\git\Kanji\JLPT N5 Words.txt")
                .TakeWhile(l => l != "")
                .Select(l => new Card
                {
                    ToPronounce = l.Split(' ')[0],
                    Kanji = l.Split(' ')[1],
                    Meaning = l.Split(' ').Skip(2).StrJoin(" ")
                })
                .ToArray();
            await Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Debug.Assert(ViewModel != null, nameof(ViewModel) + " != null");
                    ViewModel.SourceSet = set;
                }));
        }
    }
    public static class StringUtils
    {
        public static string StrJoin<T>(this IEnumerable<T> src, string separator = ", ")
        {
            return string.Join(separator, src);
        }
    }
}
