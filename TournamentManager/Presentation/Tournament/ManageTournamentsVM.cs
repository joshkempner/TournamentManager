using System;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Helpers;

namespace TournamentManager.Presentation
{
    public class ManageTournamentsVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        private readonly ManageTournamentsRM _rm;

        public ReactiveCommand<Unit, Unit> AddTournament { get; }

        public ManageTournamentsVM(
            IDispatcher bus,
            IScreen screen)
        {
            HostScreen = screen;

            _rm = new ManageTournamentsRM();
            
            _rm.Tournaments
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new TournamentItemVM(bus, x, screen))
                .Sort(SortExpressionComparer<TournamentItemVM>.Ascending(x => x.FirstDay))
                .Bind(Tournaments)
                .DisposeMany()
                .Subscribe();

            AddTournament = CommandBuilder.FromAction(() =>
            {
                Threading.RunOnUiThread(
                    () => HostScreen.Router.Navigate
                            .Execute(new NewTournamentVM(bus, HostScreen))
                            .Subscribe());
            });
        }

        public IObservableCollection<TournamentItemVM> Tournaments { get; } = new ObservableCollectionExtended<TournamentItemVM>();

        public void Dispose()
        {
            _rm.Dispose();
        }

        public string UrlPathSegment => "Manage Tournaments";
        public IScreen HostScreen { get; }
    }
}
