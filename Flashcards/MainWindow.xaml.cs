using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
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

                this.Events().KeyDown
                    .ThrottleFirst(
                        TimeSpan.FromMilliseconds(800), 
                        RxApp.MainThreadScheduler)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(ViewModel.MoveNextCmd)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.ToShow,
                        view => view.CurrentTerm.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.ToShow,
                        view => view.CurrentTerm1.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTerm.ToShow,
                        view => view.CurrentTerm2.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.PreviousTerm.ToShow,
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
            var set = File.ReadAllLines(@"C:\git\Kanji\JLPT N5 Kanji List.txt")
                .Select(l => new Card
                {
                    ToShow = l.Substring(0, 1),
                    ToPronounce = l.Substring(l.IndexOf(':') + 2)
                })
                .ToArray();
            await Dispatcher.BeginInvoke(new Action(() => ViewModel.SourceSet = set));
        }
    }
}
