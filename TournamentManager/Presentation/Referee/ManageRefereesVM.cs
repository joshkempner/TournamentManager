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
    public sealed class ManageRefereesVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        private readonly ManageRefereesRM _rm;

        public ReactiveCommand<Unit, Unit> AddReferee { get; }

        public ManageRefereesVM(
            IDispatcher bus,
            IScreen screen)
        {
            HostScreen = screen;

            _rm = new ManageRefereesRM();

            _rm.Referees
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new RefereeItemVM(bus, x, screen))
                .Sort(SortExpressionComparer<RefereeItemVM>.Ascending(x => x.Surname))
                .Bind(Referees)
                .DisposeMany()
                .Subscribe();

            AddReferee = CommandBuilder.FromAction(() =>
            {
                Threading.RunOnUiThread(() => HostScreen.Router.Navigate.Execute(new NewRefereeVM(bus, HostScreen)).Subscribe());
            });
        }

        public IObservableCollection<RefereeItemVM> Referees { get; } = new ObservableCollectionExtended<RefereeItemVM>();
 
        public void Dispose()
        {
            _rm.Dispose();
        }

        public string UrlPathSegment => "Manage Referees";
        public IScreen HostScreen { get; }
    }
}
