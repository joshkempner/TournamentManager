using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;

namespace TournamentManager.Tests.Domain
{
    public abstract class with_team_service : IDisposable
    {
        protected readonly MockRepositorySpecification Fixture = new MockRepositorySpecification();
        private readonly TeamSvc _teamSvc;

        protected readonly Guid TeamId = Guid.NewGuid();
        protected const string TeamName = "Springfield United";

        protected with_team_service()
        {
            _teamSvc = new TeamSvc(Fixture.Dispatcher, Fixture.Repository);
        }

        protected void AddTeam()
        {
            var team = new Team(
                            TeamId,
                            TeamName,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            repo.Save(team);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamCreated>(TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _teamSvc?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
