using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
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
            ISubscriber subscriber)
            : base(subscriber)
        {
            _mainWindowVM = mainWindowVM;

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            Subscribe<HostViewMsgs.DisplayOverlay>(this);
        }

        public CommandResponse Handle(HostViewMsgs.DisplayOverlay command)
        {
            _mainWindowVM.OverlayVM = command.GetVM();
            return command.Succeed();
        }
    }
}
