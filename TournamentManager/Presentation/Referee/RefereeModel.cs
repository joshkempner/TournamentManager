using System;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public sealed class RefereeModel : ReactiveObject
    {
        public RefereeModel(
            Guid refereeId,
            string givenName,
            string surname)
        {
            RefereeId = refereeId;
            GivenName = givenName;
            Surname = surname;
        }

        public Guid RefereeId { get; }

        public string GivenName
        {
            get => _givenName;
            set => this.RaiseAndSetIfChanged(ref _givenName, value);
        }
        private string _givenName = string.Empty;

        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }
        private string _surname = string.Empty;

        public string EmailAddress
        {
            get => _emailAddress;
            set => this.RaiseAndSetIfChanged(ref _emailAddress, value);
        }
        private string _emailAddress = string.Empty;

        public RefereeMsgs.Grade RefereeGrade
        {
            get => _refereeGrade;
            set => this.RaiseAndSetIfChanged(ref _refereeGrade, value);
        }
        private RefereeMsgs.Grade _refereeGrade;

        public ushort CurrentAge
        {
            get => _currentAge;
            set => this.RaiseAndSetIfChanged(ref _currentAge, value);
        }
        private ushort _currentAge;

        public TeamMsgs.AgeBracket MaxAgeBracket
        {
            get => _maxAgeBracket;
            set => this.RaiseAndSetIfChanged(ref _maxAgeBracket, value);
        }
        private TeamMsgs.AgeBracket _maxAgeBracket;
    }
}
