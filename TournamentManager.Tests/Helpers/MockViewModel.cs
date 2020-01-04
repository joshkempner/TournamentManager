using ReactiveUI;

namespace TournamentManager.Tests.Helpers
{
    public class MockViewModel : ReactiveObject, IRoutableViewModel
    {
        public MockViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        public string UrlPathSegment => "Mock VM";
        public IScreen HostScreen { get; }
    }
}
