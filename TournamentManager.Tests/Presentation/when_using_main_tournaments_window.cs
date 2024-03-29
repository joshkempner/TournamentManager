﻿using System;
using ReactiveDomain;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Presentation;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    [Collection("Presentation")]
    public sealed class when_using_main_tournaments_window
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly TournamentsHostVM _vm;

        public when_using_main_tournaments_window()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));
            _vm = new TournamentsHostVM(_fixture.Dispatcher);
        }

        [Fact]
        public void initial_view_shows_manage_tournaments()
        {
            IRoutableViewModel currentVM = null;
            _vm.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            _vm.NavigateToInitialView();
            AssertEx.IsOrBecomesTrue(() => currentVM is ManageTournamentsVM);
        }
    }
}
