﻿using System;
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
        IHandleCommand<TournamentMsgs.AddRefereeToTournament>,
        IHandleCommand<TournamentMsgs.AddTeamToTournament>,
        IHandleCommand<TournamentMsgs.RemoveTeamFromTournament>,
        IHandleCommand<TournamentMsgs.ChangeTeamAgeBracket>,
        IHandleCommand<GameMsgs.AddGame>,
        IHandleCommand<GameMsgs.CancelGame>,
        IHandleCommand<GameMsgs.RescheduleGame>,
        IHandleCommand<GameMsgs.UpdateHomeTeam>,
        IHandleCommand<GameMsgs.UpdateAwayTeam>,
        IHandleCommand<GameMsgs.AssignReferee>,
        IHandleCommand<GameMsgs.ConfirmReferee>,
        IHandleCommand<GameMsgs.RemoveReferee>
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
            Subscribe<TournamentMsgs.AddRefereeToTournament>(this);
            Subscribe<TournamentMsgs.AddTeamToTournament>(this);
            Subscribe<TournamentMsgs.RemoveTeamFromTournament>(this);
            Subscribe<TournamentMsgs.ChangeTeamAgeBracket>(this);

            Subscribe<GameMsgs.AddGame>(this);
            Subscribe<GameMsgs.CancelGame>(this);
            Subscribe<GameMsgs.RescheduleGame>(this);
            Subscribe<GameMsgs.UpdateHomeTeam>(this);
            Subscribe<GameMsgs.UpdateAwayTeam>(this);
            Subscribe<GameMsgs.AssignReferee>(this);
            Subscribe<GameMsgs.ConfirmReferee>(this);
            Subscribe<GameMsgs.RemoveReferee>(this);
        }

        #region Core TournamentMsgs

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

        public CommandResponse Handle(TournamentMsgs.AddRefereeToTournament command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AddReferee(command.RefereeId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        #endregion

        #region TeamMsgs

        public CommandResponse Handle(TournamentMsgs.AddTeamToTournament command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AddTeamToTournament(command.TeamId, command.AgeBracket);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.RemoveTeamFromTournament command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.RemoveTeamFromTournament(command.TeamId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(TournamentMsgs.ChangeTeamAgeBracket command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.ChangeTeamAgeBracket(command.TeamId, command.AgeBracket);
            _repository.Save(tournament);
            return command.Succeed();
        }

        #endregion

        #region GameMsgs

        public CommandResponse Handle(GameMsgs.AddGame command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AddGame(
                command.GameId,
                command.FieldId,
                command.TournamentDay,
                command.StartTime,
                command.EndTime);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.CancelGame command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.CancelGame(command.GameId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.RescheduleGame command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.RescheduleGame(
                command.GameId,
                command.TournamentDay,
                command.StartTime,
                command.EndTime);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.UpdateHomeTeam command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.UpdateHomeTeam(
                command.GameId,
                command.HomeTeamId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.UpdateAwayTeam command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.UpdateAwayTeam(
                command.GameId,
                command.AwayTeamId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.AssignReferee command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.AssignRefereeToGame(
                command.GameId,
                command.RefereeId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.ConfirmReferee command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.ConfirmRefereeForGame(command.GameId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        public CommandResponse Handle(GameMsgs.RemoveReferee command)
        {
            var tournament = _repository.GetById<Tournament>(command.TournamentId, command);
            tournament.RemoveRefereeFromGame(command.GameId);
            _repository.Save(tournament);
            return command.Succeed();
        }

        #endregion
    }
}
