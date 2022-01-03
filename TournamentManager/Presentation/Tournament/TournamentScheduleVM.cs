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

        public override string UrlPathSegment => "TournamentSchedule";
    }
}
