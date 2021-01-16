using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
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

            // We create our bindings here. These are the code behind bindings which allow 
            // type safety. The bindings will only become active when the Window is being shown.
            // We register our subscription in our disposableRegistration, this will cause 
            // the binding subscription to become inactive when the Window is closed.
            // The disposableRegistration is a CompositeDisposable which is a container of 
            // other Disposables. We use the DisposeWith() extension method which simply adds 
            // the subscription disposable to the CompositeDisposable.
            this.WhenActivated(disposableRegistration =>
            {
                Task.Run(WhenActivated).GetAwaiter();
                this.OneWayBind(ViewModel, 
                        viewModel => viewModel.CurrentTerm.ToShow, 
                        view => view.CurrentTerm.Text)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, 
                        viewModel => viewModel.MoveNext, 
                        view => view.MoveNext)
                    .DisposeWith(disposableRegistration);
            });
        }

        private async Task WhenActivated()
        {
            //var set = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("set.json"));
            var set = File.ReadAllLines(@"C:\git\Kanji\JLPT N5 Kanji List.txt")
                .Select(l => new Card
                {
                    ToShow = l.Substring(0, 1),
                    ToPronounce = l.Substring(l.IndexOf(':') +2)
                })
                .ToArray();
            await Dispatcher.BeginInvoke(new Action(() => ViewModel.Set = set));
        }
    }

}
