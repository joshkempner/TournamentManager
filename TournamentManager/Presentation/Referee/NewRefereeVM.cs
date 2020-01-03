using System;
using System.Net.Mail;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using Splat;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class NewRefereeVM : ReactiveObject, IRoutableViewModel
    {
        public ReactiveCommand<Unit, Unit> AddReferee { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public NewRefereeVM(
            IDispatcher bus,
            IScreen screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            _givenName = string.Empty;
            _surname = string.Empty;
            _emailAddress = string.Empty;

            this.WhenAnyValue(x => x.RefereeGrade)
                .Subscribe(g =>
                {
                    if (g == RefereeMsgs.Grade.Intramural)
                        MaxAgeBracket = TeamMsgs.AgeBracket.U8;
                    else if (MaxAgeBracket == TeamMsgs.AgeBracket.U8)
                        MaxAgeBracket = TeamMsgs.AgeBracket.U10;
                });

            this.WhenAnyValue(
                    x => x.RefereeGrade,
                    x => x.Birthdate,
                    x => x.Age,
                    (g, bd, a) => g == RefereeMsgs.Grade.Intramural && a > 0 || bd != default)
                .ToProperty(this, x => x.IsDetailFullySpecified, out _isDetailFullySpecified);

            this.WhenAnyValue(x => x.EmailAddress)
                .Select(ValidateEmailAddress)
                .ToProperty(this, x => x.IsEmailAddressValid, out _isEmailAddressValid);

            this.WhenAnyValue(
                    x => x.GivenName,
                    x => x.Surname,
                    x => x.IsEmailAddressValid,
                    x => x.IsDetailFullySpecified,
                    (gn, sn, emailOk, detailsOk) => !string.IsNullOrWhiteSpace(gn) && !string.IsNullOrWhiteSpace(sn) && emailOk && detailsOk)
                .ToProperty(this, x => x.CanAddReferee, out _canAddReferee);

            AddReferee =
                CommandBuilder.FromAction(
                    this.WhenAnyValue(x => x.CanAddReferee),
                    () =>
                    {
                        var refId = Guid.NewGuid();
                        var cmd = MessageBuilder.New(() => new RefereeMsgs.AddReferee(
                                                                refId,
                                                                GivenName,
                                                                Surname,
                                                                RefereeGrade));
                        bus.Send(cmd);
                        bus.Send(MessageBuilder
                                    .From(cmd)
                                    .Build(() => new RefereeMsgs.AddOrUpdateEmailAddress(
                                                                refId,
                                                                EmailAddress)));
                        if (RefereeGrade == RefereeMsgs.Grade.Intramural)
                        {
                            bus.Send(MessageBuilder
                                        .From(cmd)
                                        .Build(() => new RefereeMsgs.AddOrUpdateAge(
                                                                refId,
                                                                Age)));
                        }
                        else
                        {
                            bus.Send(MessageBuilder
                                        .From(cmd)
                                        .Build(() => new RefereeMsgs.AddOrUpdateBirthdate(
                                                                refId,
                                                                Birthdate)));
                        }
                        bus.Send(MessageBuilder
                                    .From(cmd)
                                    .Build(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                                refId,
                                                                MaxAgeBracket)));
                    });

            this.WhenAnyObservable(x => x.AddReferee)
                .InvokeCommand(HostScreen.Router.NavigateBack);

            Cancel = HostScreen.Router.NavigateBack;

            // Default values
            Age = 12;
            Birthdate = DateTime.Today - TimeSpan.FromDays(365.25 * 20);
        }

        private bool ValidateEmailAddress(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) return false;
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(emailAddress); // performs validation on the provided address.
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GivenName
        {
            get => _givenName;
            set => this.RaiseAndSetIfChanged(ref _givenName, value);
        }
        private string _givenName;

        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }
        private string _surname;

        public string EmailAddress
        {
            get => _emailAddress;
            set => this.RaiseAndSetIfChanged(ref _emailAddress, value);
        }
        private string _emailAddress;

        public RefereeMsgs.Grade RefereeGrade
        {
            get => _refereeGrade;
            set => this.RaiseAndSetIfChanged(ref _refereeGrade, value);
        }
        private RefereeMsgs.Grade _refereeGrade;

        public DateTime Birthdate
        {
            get => _birthdate;
            set => this.RaiseAndSetIfChanged(ref _birthdate, value);
        }
        private DateTime _birthdate;

        public ushort Age
        {
            get => _age;
            set => this.RaiseAndSetIfChanged(ref _age, value);
        }
        private ushort _age;

        public TeamMsgs.AgeBracket MaxAgeBracket
        {
            get => _maxAgeBracket;
            set => this.RaiseAndSetIfChanged(ref _maxAgeBracket, value);
        }
        private TeamMsgs.AgeBracket _maxAgeBracket;

        public bool IsEmailAddressValid => _isEmailAddressValid.Value;
        private readonly ObservableAsPropertyHelper<bool> _isEmailAddressValid;

        public bool IsDetailFullySpecified => _isDetailFullySpecified.Value;
        private readonly ObservableAsPropertyHelper<bool> _isDetailFullySpecified;

        public bool CanAddReferee => _canAddReferee.Value;
        private readonly ObservableAsPropertyHelper<bool> _canAddReferee;
        public string UrlPathSegment => "Add Referee";
        public IScreen HostScreen { get; }
    }
}
