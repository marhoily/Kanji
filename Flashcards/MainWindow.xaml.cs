using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using ReactiveUI;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<AppViewModel>
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
                Task.Run(async () =>
                {
                    var set = JsonConvert.DeserializeObject<string[]>(
                        File.ReadAllText("set.json"));
                    await Dispatcher.BeginInvoke(new Action(() =>
                        ViewModel.Set = set));
                }).GetAwaiter();
                this.OneWayBind(ViewModel, 
                        viewModel => viewModel.CurrentTerm, 
                        view => view.CurrentTerm.Text)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, 
                        viewModel => viewModel.NextTerm, 
                        view => view.Next)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
