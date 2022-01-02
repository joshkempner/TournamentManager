using System;
using ReactiveDomain;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_managing_a_referee
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _hostScreen = new MockHostScreen();
        private readonly RefereeModel _model;
        private readonly RefereeItemVM _vm;

        public when_managing_a_referee()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));

            _model = new RefereeModel(
                            Guid.NewGuid(),
                            "John",
                            "Smith")
                            {
                                RefereeGrade = RefereeMsgs.Grade.Grassroots,
                                EmailAddress = "john.smith@aol.com",
                                CurrentAge = 16,
                                MaxAgeBracket = TournamentMsgs.AgeBracket.U14
                            };
            _vm = new RefereeItemVM(
                        _fixture.Dispatcher,
                        _model,
                        _hostScreen);
            _vm.Activator.Activate();
        }

        [Fact]
        public void displays_correct_initial_data()
        {
            AssertEx.IsOrBecomesTrue(() => _model.RefereeId == _vm.RefereeId);
            AssertEx.IsOrBecomesTrue(() => _model.Surname == _vm.Surname);
            AssertEx.IsOrBecomesTrue(() => _model.GivenName == _vm.GivenName);
            AssertEx.IsOrBecomesTrue(() => _model.EmailAddress == _vm.EmailAddress);
            AssertEx.IsOrBecomesTrue(() => _model.RefereeGrade == _vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => _model.MaxAgeBracket == _vm.MaxAgeBracket);
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
        }

        [Fact]
        public void displays_updated_data()
        {
            _model.GivenName = "Jonathan";
            _model.Surname = "Smyth";
            _model.EmailAddress = "Jon.Smyth@gmail.com";
            _model.RefereeGrade = RefereeMsgs.Grade.Regional;
            _model.CurrentAge = 40;
            _model.MaxAgeBracket = TournamentMsgs.AgeBracket.Adult;
            AssertEx.IsOrBecomesTrue(() => _model.Surname == _vm.Surname);
            AssertEx.IsOrBecomesTrue(() => _model.GivenName == _vm.GivenName);
            AssertEx.IsOrBecomesTrue(() => _model.EmailAddress == _vm.FullEmail?.Address);
            AssertEx.IsOrBecomesTrue(() => _model.RefereeGrade == _vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => _model.MaxAgeBracket == _vm.MaxAgeBracket);
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
        }

        [Fact]
        public void displays_correct_age_range()
        {
            _model.CurrentAge = 16;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 18;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 20;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 30;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 45;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 55;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 65;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
            _model.CurrentAge = 75;
            AssertEx.IsOrBecomesTrue(() => RefereeItemVM.AgeToAgeRange(_model.CurrentAge) == _vm.AgeRange);
        }

        [Fact]
        public void can_edit_referee_credentials()
        {
            IRoutableViewModel currentVM = null;
            _hostScreen.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            _vm.EditCredentials.Execute().Subscribe();
            AssertEx.IsOrBecomesTrue(() => currentVM is CredentialsVM vm &&
                                           vm.RefereeId == _model.RefereeId);
        }

        [Fact]
        public void can_edit_referee_contact_info()
        {
            IRoutableViewModel currentVM = null;
            _hostScreen.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            _vm.EditContactInfo.Execute().Subscribe();
            AssertEx.IsOrBecomesTrue(() => currentVM is ContactInfoVM vm &&
                                           vm.RefereeId == _model.RefereeId);
        }
    }
}
