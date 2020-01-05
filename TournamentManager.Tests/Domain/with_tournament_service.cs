using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;

namespace TournamentManager.Tests.Domain
{
    public abstract class with_tournament_service : IDisposable
    {
        protected readonly MockRepositorySpecification Fixture = new MockRepositorySpecification();
        private readonly TournamentSvc _tournamentSvc;

        protected readonly Guid TournamentId = Guid.NewGuid();
        protected const string TournamentName = "The Milk Cup";
        protected readonly DateTime FirstDay = new DateTime(2020, 6, 1);
        protected readonly DateTime LastDay = new DateTime(2020, 6, 2);

        protected with_tournament_service()
        {
            _tournamentSvc = new TournamentSvc(Fixture.Dispatcher, Fixture.Repository);
        }

        protected void AddTournament()
        {
            var tournament = new Tournament(
                TournamentId,
                TournamentName,
                FirstDay,
                LastDay,
                MessageBuilder.New(() => new TestCommands.Command1()));
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            repo.Save(tournament);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentAdded>(TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tournamentSvc?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
