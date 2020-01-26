using System;
using ReactiveDomain;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Presentation;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_using_main_referees_window
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly RefereesHostVM _vm;

        public when_using_main_referees_window()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));
            _vm = new RefereesHostVM(_fixture.Dispatcher);
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
