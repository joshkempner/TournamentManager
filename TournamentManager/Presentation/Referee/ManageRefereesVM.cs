using System;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public sealed class ManageRefereesVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        private readonly ManageRefereesRM _rm;

        public ReactiveCommand<Unit, IRoutableViewModel> AddReferee { get; }

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

            AddReferee = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new NewRefereeVM(bus, HostScreen)));
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
