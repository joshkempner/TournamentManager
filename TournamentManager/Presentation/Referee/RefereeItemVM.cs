using System;
using System.Net.Mail;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using Splat;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public sealed class RefereeItemVM : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> EditContactInfo { get; }
        public ReactiveCommand<Unit, Unit> EditCredentials { get; }

        public RefereeItemVM(
            IDispatcher bus,
            RefereeModel model)
        {
            var refereeId = model.RefereeId;
            HostScreen = Locator.Current.GetService<IScreen>();

            model.WhenAnyValue(
                    x => x.GivenName,
                    x => x.Surname,
                    (first, last) => $"{first} {last}")
                .ToProperty(this, x => x.FullName, out _fullName);

            model.WhenAnyValue(x => x.Surname)
                .ToProperty(this, x => x.Surname, out _surname);

            model.WhenAnyValue(x => x.EmailAddress)
                .Where(x => x != null)
                .Select(x => new MailAddress(x, FullName))
                .ToProperty(this, x => x.EmailAddress, out _emailAddress);

            model.WhenAnyValue(x => x.RefereeGrade)
                .ToProperty(this, x => x.RefereeGrade, out _refereeGrade);

            model.WhenAnyValue(x => x.CurrentAge)
                .ToProperty(this, x => x.CurrentAge, out _currentAge);

            model.WhenAnyValue(x => x.MaxAgeBracket)
                .ToProperty(this, x => x.MaxAgeBracket, out _maxAgeBracket);

            EditContactInfo = CommandBuilder.FromAction(
                                () => Threading.RunOnUiThread(() =>
                                {
                                    HostScreen.Router.Navigate
                                        .Execute(new ContactInfoVM(
                                                        refereeId,
                                                        FullName,
                                                        bus,
                                                        HostScreen))
                                        .Subscribe();
                                }));

            EditCredentials = CommandBuilder.FromAction(
                                () => Threading.RunOnUiThread(() =>
                                {
                                    HostScreen.Router.Navigate
                                        .Execute(new CredentialsVM(
                                                        refereeId,
                                                        FullName,
                                                        bus,
                                                        HostScreen))
                                        .Subscribe();
                                }));
        }

        public string Surname => _surname.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _surname;

        public string FullName => _fullName.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _fullName;

        public MailAddress? EmailAddress => _emailAddress.Value;
        private readonly ObservableAsPropertyHelper<MailAddress?> _emailAddress;

        public RefereeMsgs.Grade RefereeGrade => _refereeGrade.Value;
        private readonly ObservableAsPropertyHelper<RefereeMsgs.Grade> _refereeGrade;

        public ushort CurrentAge => _currentAge.Value;
        private readonly ObservableAsPropertyHelper<ushort> _currentAge;

        public TeamMsgs.AgeBracket MaxAgeBracket => _maxAgeBracket.Value;
        private readonly ObservableAsPropertyHelper<TeamMsgs.AgeBracket> _maxAgeBracket;

        public IScreen HostScreen { get; }
    }
}
