using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;

namespace TournamentManager.Tests.Helpers
{
    public class RefereeTestHelper
    {
        private readonly MockRepositorySpecification _fixture;
        private readonly CorrelatedStreamStoreRepository _repo;

        internal const ushort IntramuralAge = 12;
        internal const TournamentMsgs.AgeBracket IntramuralMaxAgeBracket = TournamentMsgs.AgeBracket.U8;
        internal readonly DateTime TravelBirthdate = new DateTime(1990, 1, 1);
        internal const TournamentMsgs.AgeBracket TravelMaxAgeBracket = TournamentMsgs.AgeBracket.U16;
        internal const string GivenName = "John";
        internal const string Surname = "Smith";
        internal readonly string FullName = $"{GivenName} {Surname}";

        internal RefereeTestHelper(MockRepositorySpecification fixture)
        {
            _fixture = fixture;
            _repo = new CorrelatedStreamStoreRepository(fixture.Repository);
        }

        internal void AddIntramuralReferee(Guid refereeId)
        {
            var referee = new Referee(
                refereeId,
                GivenName,
                Surname,
                RefereeMsgs.Grade.Intramural,
                MessageBuilder.New(() => new TestCommands.Command1()));
            referee.AddOrUpdateAge(IntramuralAge);
            referee.AddOrUpdateMaxAgeBracket(IntramuralMaxAgeBracket);
            _repo.Save(referee);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MaxAgeBracketChanged>(TimeSpan.FromMilliseconds(200));
            _fixture.ClearQueues();
        }

        internal void AddTravelReferee(Guid refereeId)
        {
            var referee = new Referee(
                refereeId,
                GivenName,
                Surname,
                RefereeMsgs.Grade.Grassroots,
                MessageBuilder.New(() => new TestCommands.Command1()));
            referee.AddOrUpdateBirthdate(TravelBirthdate);
            referee.AddOrUpdateMaxAgeBracket(TravelMaxAgeBracket);
            _repo.Save(referee);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MaxAgeBracketChanged>(TimeSpan.FromMilliseconds(200));
            _fixture.ClearQueues();
        }
    }
}
