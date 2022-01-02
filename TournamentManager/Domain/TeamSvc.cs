using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class TeamSvc :
        QueuedSubscriber,
        IHandleCommand<TeamMsgs.CreateTeam>,
        IHandleCommand<TeamMsgs.DeleteTeam>,
        IHandleCommand<TeamMsgs.RenameTeam>
    {
        private readonly CorrelatedStreamStoreRepository _repository;

        public TeamSvc(
            IBus bus,
            IRepository repository)
            : base(bus)
        {
            _repository = new CorrelatedStreamStoreRepository(repository);

            Subscribe<TeamMsgs.CreateTeam>(this);
            Subscribe<TeamMsgs.DeleteTeam>(this);
            Subscribe<TeamMsgs.RenameTeam>(this);
        }

        public CommandResponse Handle(TeamMsgs.CreateTeam command)
        {
            if (_repository.TryGetById<Team>(command.TeamId, out _, command))
                throw new Exception($"Attempt to add a second team with ID {command.TeamId}");
            var team = new Team(
                            command.TeamId,
                            command.Name,
                            command);
            _repository.Save(team);
            return command.Succeed();
        }

        public CommandResponse Handle(TeamMsgs.DeleteTeam command)
        {
            if (!_repository.TryGetById<Team>(command.TeamId, out var team, command))
                return command.Succeed(); // attempt to delete a nonexistent team succeeds implicitly
            team.DeleteTeam();
            _repository.Save(team);
            return command.Succeed();
        }

        public CommandResponse Handle(TeamMsgs.RenameTeam command)
        {
            var team = _repository.GetById<Team>(command.TeamId, command);
            team.RenameTeam(command.Name);
            _repository.Save(team);
            return command.Succeed();
        }
    }
}
