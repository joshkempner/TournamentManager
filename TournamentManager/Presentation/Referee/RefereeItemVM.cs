using System;
using System.Net.Mail;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;
using TournamentManager.Messages;
// ReSharper disable RedundantDefaultMemberInitializer

namespace TournamentManager.Presentation
{
    public sealed class RefereeItemVM : ReactiveObject, IActivatableViewModel
    {
        public ReactiveCommand<Unit, IRoutableViewModel> EditContactInfo { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditCredentials { get; }

        public RefereeItemVM(
            IDispatcher bus,
            RefereeModel model,
            IScreen screen)
        {
            RefereeId = model.RefereeId;
            HostScreen = screen;
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                model.WhenAnyValue(x => x.Surname)
                    .ToProperty(this, x => x.Surname, out _surname)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.GivenName)
                    .ToProperty(this, x => x.GivenName, out _givenName)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.EmailAddress)
                    .ToProperty(this, x => x.EmailAddress, out _emailAddress)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.RefereeGrade)
                    .ToProperty(this, x => x.RefereeGrade, out _refereeGrade)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.CurrentAge)
                    .ToProperty(this, x => x.CurrentAge, out _currentAge)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.MaxAgeBracket)
                    .ToProperty(this, x => x.MaxAgeBracket, out _maxAgeBracket)
                    .DisposeWith(disposables);
            });

            this.WhenAnyValue(
                    x => x.GivenName,
                    x => x.Surname)
                .Where(x => !string.IsNullOrWhiteSpace(x.Item1) && !string.IsNullOrWhiteSpace(x.Item2))
                .Select(x => $"{x.Item1} {x.Item2}")
                .ToProperty(this, x => x.FullName, out _fullName);

            this.WhenAnyValue(
                    x => x.EmailAddress,
                    x => x.FullName)
                .Where(x => !string.IsNullOrWhiteSpace(x.Item1))
                .Select(x => new MailAddress(x.Item1, x.Item2))
                .ToProperty(this, x => x.FullEmail, out _fullEmail);

            this.WhenAnyValue(x => x.CurrentAge)
                .Select(AgeToAgeRange)
                .ToProperty(this, x => x.AgeRange, out _ageRange);

            EditContactInfo = ReactiveCommand.CreateFromObservable(
                                () => HostScreen
                                        .Router
                                        .Navigate
                                        .Execute(new ContactInfoVM(
                                                        RefereeId,
                                                        FullName,
                                                        bus,
                                                        HostScreen)));

            EditCredentials = ReactiveCommand.CreateFromObservable(
                                () => HostScreen
                                        .Router
                                        .Navigate
                                        .Execute(new CredentialsVM(
                                                        RefereeId,
                                                        FullName,
                                                        bus,
                                                        HostScreen)));
        }

        public static string AgeToAgeRange(ushort age)
        {
            if (age < 18)
                return "Under 18";
            if (age == 18)
                return "Age 18";
            if (age < 25)
                return "Age 19-24";
            if (age < 40)
                return "Age 25-39";
            if (age < 50)
                return "Age 40-49";
            if (age < 60)
                return "Age 50-59";
            if (age < 70)
                return "Age 60-69";
            return "Age 70+";
        }

        public Guid RefereeId { get; }

        public string Surname => _surname?.Value ?? string.Empty;
        private ObservableAsPropertyHelper<string?> _surname = ObservableAsPropertyHelper<string?>.Default();

        public string GivenName => _givenName?.Value ?? string.Empty;
        private ObservableAsPropertyHelper<string?> _givenName = ObservableAsPropertyHelper<string?>.Default();

        public string FullName => _fullName?.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _fullName;

        public string EmailAddress => _emailAddress.Value ?? string.Empty;
        private ObservableAsPropertyHelper<string?> _emailAddress = ObservableAsPropertyHelper<string?>.Default();

        public MailAddress FullEmail => _fullEmail?.Value ?? new MailAddress("invalid@email");
        private readonly ObservableAsPropertyHelper<MailAddress?> _fullEmail;

        public RefereeMsgs.Grade RefereeGrade => _refereeGrade.Value;
        private ObservableAsPropertyHelper<RefereeMsgs.Grade> _refereeGrade = ObservableAsPropertyHelper<RefereeMsgs.Grade>.Default();

        private ushort CurrentAge => _currentAge.Value;
        private ObservableAsPropertyHelper<ushort> _currentAge = ObservableAsPropertyHelper<ushort>.Default();

        public string AgeRange => _ageRange.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _ageRange;

        public TournamentMsgs.AgeBracket MaxAgeBracket => _maxAgeBracket.Value;
        private ObservableAsPropertyHelper<TournamentMsgs.AgeBracket> _maxAgeBracket = ObservableAsPropertyHelper<TournamentMsgs.AgeBracket>.Default();

        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }
    }
}
