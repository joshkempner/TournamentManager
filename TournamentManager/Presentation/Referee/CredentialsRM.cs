using System;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class CredentialsRM :
        ReadModelBase,
        IHandle<RefereeMsgs.RefereeAdded>,
        IHandle<RefereeMsgs.GradeChanged>,
        IHandle<RefereeMsgs.AgeChanged>,
        IHandle<RefereeMsgs.BirthdateChanged>,
        IHandle<RefereeMsgs.MaxAgeBracketChanged>
    {
        public CredentialsRM(Guid refereeId)
            : base(
                nameof(CredentialsRM),
                () => Locator.Current.GetService<IStreamStoreConnection>()!.GetListener(nameof(CredentialsRM)))
        {
            EventStream.Subscribe<RefereeMsgs.RefereeAdded>(this);
            EventStream.Subscribe<RefereeMsgs.GradeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.AgeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.BirthdateChanged>(this);
            EventStream.Subscribe<RefereeMsgs.MaxAgeBracketChanged>(this);
            Start<Referee>(refereeId, blockUntilLive: true);
        }

        public IObservable<RefereeMsgs.Grade?> RefereeGrade => _refereeGrade;
        private readonly ReadModelProperty<RefereeMsgs.Grade?> _refereeGrade = new ReadModelProperty<RefereeMsgs.Grade?>(null);

        public IObservable<DateTime> Birthdate => _birthdate;
        private readonly ReadModelProperty<DateTime> _birthdate = new ReadModelProperty<DateTime>(default);

        public IObservable<ushort> CurrentAge => _currentAge;
        private readonly ReadModelProperty<ushort> _currentAge = new ReadModelProperty<ushort>(0);

        public IObservable<TournamentMsgs.AgeBracket> MaxAgeBracket => _maxAgeBracket;
        private readonly ReadModelProperty<TournamentMsgs.AgeBracket> _maxAgeBracket = new ReadModelProperty<TournamentMsgs.AgeBracket>(TournamentMsgs.AgeBracket.U8);

        public void Handle(RefereeMsgs.RefereeAdded message)
        {
            _refereeGrade.Update(message.RefereeGrade);
        }

        public void Handle(RefereeMsgs.GradeChanged message)
        {
            _refereeGrade.Update(message.RefereeGrade);
        }

        public void Handle(RefereeMsgs.AgeChanged message)
        {
            _currentAge.Update((ushort)message.Age);
        }

        public void Handle(RefereeMsgs.BirthdateChanged message)
        {
            _birthdate.Update(message.Birthdate);
            _currentAge.Update((ushort)message.Birthdate.YearsAgo());
        }

        public void Handle(RefereeMsgs.MaxAgeBracketChanged message)
        {
            _maxAgeBracket.Update(message.MaxAgeBracket);
        }
    }
}
