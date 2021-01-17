using ReactiveUI;
using System.Reactive;

namespace WpfApp1
{
    public interface IAppViewModel
    {
        Card CurrentTerm { get; }
        int CurrentTermIndex { get; set; }
        ReactiveCommand<Unit, Unit> MoveNextCmd { get; }
        Card PreviousTerm { get; }
        Card[] SourceSet { get; set; }

        void MoveNext();
    }
}