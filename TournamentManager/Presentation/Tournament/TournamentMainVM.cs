using System;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class TournamentMainVM : TransientViewModel
    {
        public TournamentMainVM(
            Guid id,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            Id = id;
        }

        private Guid Id { get; }

        public override string UrlPathSegment => "Tournament Main";
    }
}
