using System;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class ManageTournamentsVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        public ManageTournamentsVM(
            IDispatcher bus,
            IScreen screen)
        {
            HostScreen = screen;
        }

        public void Dispose()
        {
        }

        public string UrlPathSegment => "Manage Tournaments";
        public IScreen HostScreen { get; }
    }
}
