using System;
using ReactiveDomain;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Presentation;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_using_main_window
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MainWindowVM _vm;

        public when_using_main_window()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));
            _vm = new MainWindowVM(_fixture.Dispatcher);
        }

        [Fact]
        public void initial_view_shows_manage_referees()
        {
            IRoutableViewModel currentVM = null;
            _vm.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            _vm.NavigateToInitialView();
            AssertEx.IsOrBecomesTrue(() => currentVM is ManageRefereesVM);
        }
    }
}
