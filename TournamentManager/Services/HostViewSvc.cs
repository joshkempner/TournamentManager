using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using TournamentManager.Helpers;
using TournamentManager.Messages;
using TournamentManager.Presentation;

namespace TournamentManager.Services
{
    internal class HostViewSvc :
        ReactiveTransientSubscriber,
        IHandleCommand<HostViewMsgs.DisplayOverlay>
    {
        private readonly MainWindowVM _mainWindowVM;

        public HostViewSvc(
            MainWindowVM mainWindowVM,
            IDispatcher bus)
            : base(bus)
        {
            _mainWindowVM = mainWindowVM;

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            Subscribe<HostViewMsgs.DisplayOverlay>(this);
        }

        public CommandResponse Handle(HostViewMsgs.DisplayOverlay command)
        {
            Threading.RunOnUiThread(() => _mainWindowVM.OverlayVM = command.GetVM());
            return command.Succeed();
        }
    }
}
