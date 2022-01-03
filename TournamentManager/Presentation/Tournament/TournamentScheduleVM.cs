using System;
using System.Reactive;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleVM : TransientViewModel
    {
        private readonly TournamentScheduleRM _rm;

        public ReactiveCommand<Unit, IRoutableViewModel> AddField { get; }

        public TournamentScheduleVM(
            Guid tournamentId,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            _rm = new TournamentScheduleRM(tournamentId);

            AddField = ReactiveCommand.CreateFromObservable(
                        () => HostScreen.Router.Navigate.Execute(new NewFieldVM(
                                                                        tournamentId,
                                                                        bus,
                                                                        HostScreen)));
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
            base.Dispose(disposing);
        }

        public override string UrlPathSegment => "TournamentSchedule";
    }
}
