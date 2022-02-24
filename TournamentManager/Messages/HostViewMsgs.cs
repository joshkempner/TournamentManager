using System;
using ReactiveDomain.Messaging;
using TournamentManager.Presentation;

namespace TournamentManager.Messages
{
    public class HostViewMsgs
    {
        public class DisplayOverlay : Command
        {
            public readonly Func<OverlayViewModel> GetVM;

            public DisplayOverlay(Func<OverlayViewModel> getVM)
            {
                GetVM = getVM;
            }
        }
    }
}
