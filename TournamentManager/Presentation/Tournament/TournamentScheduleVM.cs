using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleVM : TransientViewModel
    {
        private readonly TournamentScheduleRM _rm;

        public TournamentScheduleVM(
            Guid tournamentId,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            _rm = new TournamentScheduleRM(tournamentId);

            _rm.GameSlots
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new GameSlotSummaryVM(x))
                .Bind(GameSlots)
                .DisposeMany()
                .Subscribe();
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
        }

        public IObservableCollection<GameSlotSummaryVM> GameSlots { get; } = new ObservableCollectionExtended<GameSlotSummaryVM>();

        public override string UrlPathSegment => "TournamentSchedule";
    }
}
