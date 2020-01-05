using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class TournamentSvc :
        QueuedSubscriber,
        IHandleCommand<TournamentMsgs.AddTournament>,
        IHandleCommand<TournamentMsgs.RenameTournament>,
        IHandleCommand<TournamentMsgs.RescheduleTournament>,
        IHandleCommand<TournamentMsgs.AddField>,
        IHandleCommand<TournamentMsgs.AddGameSlot>
    {
        private readonly CorrelatedStreamStoreRepository _repository;

        public TournamentSvc(
            IBus bus,
            IRepository repository)
            : base(bus)
        {
            _repository = new CorrelatedStreamStoreRepository(repository);
            Subscribe<TournamentMsgs.AddTournament>(this);
            Subscribe<TournamentMsgs.RenameTournament>(this);
            Subscribe<TournamentMsgs.RescheduleTournament>(this);
            Subscribe<TournamentMsgs.AddField>(this);
            Subscribe<TournamentMsgs.AddGameSlot>(this);
        }

        public CommandResponse Handle(TournamentMsgs.AddTournament command)
        {
            if (_repository.TryGetById<Tournament>(command.TournamentId, out _, command))
                throw new AggregateException("Cannot add two tournaments with the same ID.");
            var tournament = new Tournament(
                                    command.TournamentId,
                                    command.Name,
                                    command.FirstDay,
                                    command.LastDay,
                                    command);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.RenameTournament command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.Rename(command.Name);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.RescheduleTournament command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.Reschedule(
                command.FirstDay,
                command.LastDay);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.AddField command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AddField(
                command.FieldId,
                command.FieldName);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.AddGameSlot command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AddGameSlot(
                command.GameSlotId,
                command.StartTime,
                command.EndTime);
            _repository.Save(tournament);
            return command.Succeed();
        }
    }
}
