using System;
using System.Linq;
using DynamicData;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using Splat;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class ManageRefereesRM :
        ReadModelBase,
        IHandle<RefereeMsgs.RefereeAdded>,
        IHandle<RefereeMsgs.GivenNameChanged>,
        IHandle<RefereeMsgs.SurnameChanged>,
        IHandle<RefereeMsgs.EmailAddressChanged>,
        IHandle<RefereeMsgs.GradeChanged>,
        IHandle<RefereeMsgs.AgeChanged>,
        IHandle<RefereeMsgs.BirthdateChanged>,
        IHandle<RefereeMsgs.MaxAgeBracketChanged>
    {
        public ManageRefereesRM()
            : base(
                nameof(ManageRefereesRM),
                () => Locator.Current.GetService<IStreamStoreConnection>()!.GetListener(nameof(ManageRefereesRM)))
        {
            EventStream.Subscribe<RefereeMsgs.RefereeAdded>(this);
            EventStream.Subscribe<RefereeMsgs.GivenNameChanged>(this);
            EventStream.Subscribe<RefereeMsgs.SurnameChanged>(this);
            EventStream.Subscribe<RefereeMsgs.EmailAddressChanged>(this);
            EventStream.Subscribe<RefereeMsgs.GradeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.AgeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.BirthdateChanged>(this);
            EventStream.Subscribe<RefereeMsgs.MaxAgeBracketChanged>(this);
            Start<Domain.Referee>();
        }

        public SourceCache<RefereeModel, Guid> Referees { get; } = new SourceCache<RefereeModel, Guid>(x => x.RefereeId);

        public void Handle(RefereeMsgs.RefereeAdded message)
        {
            var model = new RefereeModel(
                                message.RefereeId,
                                message.GivenName,
                                message.Surname)
                                {
                                    RefereeGrade = message.RefereeGrade
                                };
            Referees.AddOrUpdate(model);
        }

        public void Handle(RefereeMsgs.GivenNameChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.GivenName = message.GivenName;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.SurnameChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.Surname = message.Surname;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.EmailAddressChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.EmailAddress = message.Email;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.GradeChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.RefereeGrade = message.RefereeGrade;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.AgeChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.CurrentAge = (ushort)message.Age;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.BirthdateChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.CurrentAge = (ushort)message.Birthdate.YearsAgo();
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.MaxAgeBracketChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.MaxAgeBracket = message.MaxAgeBracket;
            Referees.AddOrUpdate(referee);
        }
    }
}
