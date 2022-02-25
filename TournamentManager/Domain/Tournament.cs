using System;
using System.Collections.Generic;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Util;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class Tournament : AggregateRoot
    {
        private DateTime _firstDay;
        private DateTime _lastDay;
        private readonly HashSet<Guid> _fields = new HashSet<Guid>();
        private readonly HashSet<Guid> _teams = new HashSet<Guid>();
        /// <summary>
        /// Games, indexed by ID, with value indicating whether a referee is assigned.
        /// </summary>
        private readonly Dictionary<Guid, bool> _gameReferees = new Dictionary<Guid, bool>();
        private readonly Dictionary<Guid, (DateTime start, DateTime end)> _gameTimes = new Dictionary<Guid, (DateTime start, DateTime end)>();
        private readonly HashSet<Guid> _referees = new HashSet<Guid>();

        private Tournament()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Register<TournamentMsgs.TournamentAdded>(e =>
            {
                Id = e.TournamentId;
                _firstDay = e.FirstDay;
                _lastDay = e.LastDay;
            });
            Register<TournamentMsgs.TournamentRescheduled>(e =>
            {
                _firstDay = e.FirstDay;
                _lastDay = e.LastDay;
            });
            Register<TournamentMsgs.FieldAdded>(e => _fields.Add(e.FieldId));
            Register<TournamentMsgs.TeamAddedToTournament>(e => _teams.Add(e.TeamId));
            Register<TournamentMsgs.TeamRemovedFromTournament>(e => _teams.Remove(e.TeamId));
            Register<GameMsgs.GameAdded>(e =>
            {
                _gameTimes.Add(e.GameId, (e.StartTime, e.EndTime));
                _gameReferees.Add(e.GameId, false);
            });
            Register<GameMsgs.GameCancelled>(e =>
            {
                _gameTimes.Remove(e.GameId);
                _gameReferees.Remove(e.GameId);
            });
            Register<GameMsgs.GameRescheduled>(e => _gameTimes[e.GameId] = (e.StartTime, e.EndTime));
            Register<TournamentMsgs.RefereeAddedToTournament>(e => _referees.Add(e.RefereeId));
            Register<GameMsgs.RefereeAssigned>(e => _gameReferees[e.GameId] = true);
            Register<GameMsgs.RefereeRemoved>(e => _gameReferees[e.GameId] = false);
        }

        public Tournament(
            Guid tournamentId,
            string tournamentName,
            DateTime firstDay,
            DateTime lastDay,
            ICorrelatedMessage source)
            : this()
        {
            Ensure.NotEmptyGuid(tournamentId, nameof(tournamentId));
            Ensure.NotNullOrEmpty(tournamentName, nameof(tournamentName));
            Ensure.False(() => string.IsNullOrWhiteSpace(tournamentName), nameof(tournamentName));
            Ensure.True(() => lastDay >= firstDay, "lastDay >= firstDay");
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            if (source.CausationId == Guid.Empty)
                Ensure.NotEmptyGuid(source.MsgId, nameof(source.MsgId));
            ((ICorrelatedEventSource)this).Source = source;
            Raise(new TournamentMsgs.TournamentAdded(
                        tournamentId: tournamentId,
                        name: tournamentName,
                        firstDay: firstDay,
                        lastDay: lastDay));
        }

        #region Tournament Setup

        public void Rename(string newName)
        {
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            Ensure.False(() => string.IsNullOrWhiteSpace(newName), nameof(newName));
            Raise(new TournamentMsgs.TournamentRenamed(
                        Id,
                        newName));
        }

        public void Reschedule(
            DateTime firstDay,
            DateTime lastDay)
        {
            Ensure.True(() => lastDay >= firstDay, "lastDay >= firstDay");
            Raise(new TournamentMsgs.TournamentRescheduled(
                        tournamentId: Id,
                        firstDay: firstDay,
                        lastDay: lastDay));
        }

        public void AddField(
            Guid fieldId,
            string fieldName)
        {
            Ensure.NotEmptyGuid(fieldId, nameof(fieldId));
            Ensure.NotNullOrEmpty(fieldName, nameof(fieldName));
            if (_fields.Contains(fieldId))
                throw new ArgumentException("Cannot add a second field with the same ID.");
            Raise(new TournamentMsgs.FieldAdded(
                        Id,
                        fieldId,
                        fieldName));
        }

        public void AddReferee(Guid refereeId)
        {
            Ensure.NotEmptyGuid(refereeId, nameof(refereeId));
            if (_referees.Contains(refereeId))
                throw new ArgumentException("Cannot add the same referee twice to the same tournament.");
            Raise(new TournamentMsgs.RefereeAddedToTournament(
                        Id,
                        refereeId));
        }

        #endregion

        #region Teams

        public void AddTeamToTournament(Guid teamId, TournamentMsgs.AgeBracket ageBracket)
        {
            Ensure.NotEmptyGuid(teamId, nameof(teamId));
            if (_teams.Contains(teamId)) return; // Add is idempotent
            Raise(new TournamentMsgs.TeamAddedToTournament(
                        Id,
                        teamId,
                        ageBracket));
        }

        public void RemoveTeamFromTournament(Guid teamId)
        {
            if (!_teams.Contains(teamId)) return; // Remove is idempotent
            Raise(new TournamentMsgs.TeamRemovedFromTournament(
                        Id,
                        teamId));
        }

        public void ChangeTeamAgeBracket(Guid teamId, TournamentMsgs.AgeBracket ageBracket)
        {
            Ensure.NotEmptyGuid(teamId, nameof(teamId));
            if (!_teams.Contains(teamId))
                throw new Exception($"Cannot change the age bracket for team with ID {teamId} because they are not in the tournament.");
            Raise(new TournamentMsgs.TeamAgeBracketChanged(
                        Id,
                        teamId,
                        ageBracket));
        }

        #endregion

        #region Games

        public void AddGame(
            Guid gameId,
            Guid fieldId,
            uint tournamentDay,
            DateTime startTime,
            DateTime endTime)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            Ensure.NotEmptyGuid(fieldId, nameof(fieldId));
            var duration = _lastDay - _firstDay;
            Ensure.LessThanOrEqualTo((uint)duration.TotalDays, tournamentDay, nameof(tournamentDay));
            Ensure.False(() => startTime == endTime, "Cannot add a game with identical start and end times.");
            Ensure.True(() => startTime < endTime, "Cannot add a game with a start time before the end time.");
            if (_gameTimes.ContainsKey(gameId))
                throw new ArgumentException($"A game with ID {gameId} has already been added.");
            if (!_fields.Contains(fieldId))
                throw new ArgumentException("Cannot add a game to an invalid field.");
            Raise(new GameMsgs.GameAdded(
                        Id,
                        gameId,
                        fieldId,
                        tournamentDay,
                        startTime,
                        endTime));
        }

        public void CancelGame(Guid gameId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameTimes.ContainsKey(gameId)) return;
            Raise(new GameMsgs.GameCancelled(
                        Id,
                        gameId));
        }

        public void RescheduleGame(
            Guid gameId,
            uint tournamentDay,
            DateTime startTime,
            DateTime endTime)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameTimes.ContainsKey(gameId))
                throw new ArgumentException("Cannot reschedule a nonexistent game.");
            var duration = _lastDay - _firstDay;
            Ensure.LessThanOrEqualTo((uint)duration.TotalDays, tournamentDay, nameof(tournamentDay));
            Ensure.False(() => startTime == endTime, "Cannot reschedule a game with identical start and end times.");
            Ensure.True(() => startTime < endTime, "Cannot reschedule a game with a start time before the end time.");
            var (previousStart, previousEnd) = _gameTimes[gameId];
            if (previousStart == startTime && previousEnd == endTime) return; // reschedule is idempotent
            Raise(new GameMsgs.GameRescheduled(
                        Id,
                        gameId,
                        tournamentDay,
                        startTime,
                        endTime));
        }

        public void UpdateHomeTeam(
            Guid gameId,
            Guid homeTeamId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameTimes.ContainsKey(gameId))
                throw new ArgumentException("Cannot update the home team for a non-existent game.");
            Ensure.NotEmptyGuid(homeTeamId, nameof(homeTeamId));
            if (!_teams.Contains(homeTeamId))
                throw new ArgumentException("Cannot update the home team with a non-existent team.");
            Raise(new GameMsgs.HomeTeamUpdated(
                        Id,
                        gameId,
                        homeTeamId));
        }

        public void UpdateAwayTeam(
            Guid gameId,
            Guid awayTeamId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameTimes.ContainsKey(gameId))
                throw new ArgumentException("Cannot update the away team for a non-existent game.");
            Ensure.NotEmptyGuid(awayTeamId, nameof(awayTeamId));
            if (!_teams.Contains(awayTeamId))
                throw new ArgumentException("Cannot update the away team with a non-existent team.");
            Raise(new GameMsgs.AwayTeamUpdated(
                        Id,
                        gameId,
                        awayTeamId));
        }

        public void AssignRefereeToGame(
            Guid gameId,
            Guid refereeId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameReferees.ContainsKey(gameId))
                throw new ArgumentException("Cannot assign a referee to a non-existent game.");
            Ensure.NotEmptyGuid(refereeId, nameof(refereeId));
            if (!_referees.Contains(refereeId))
                throw new ArgumentException("Cannot assign a non-existent referee to a game.");
            Raise(new GameMsgs.RefereeAssigned(
                        Id,
                        gameId,
                        refereeId));
        }

        public void RemoveRefereeFromGame(Guid gameId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameReferees.ContainsKey(gameId))
                throw new ArgumentException("Cannot assign a referee to a non-existent game.");
            if (!_gameReferees[gameId]) return;
            Raise(new GameMsgs.RefereeRemoved(
                        Id,
                        gameId));
        }

        public void ConfirmRefereeForGame(Guid gameId)
        {
            Ensure.NotEmptyGuid(gameId, nameof(gameId));
            if (!_gameReferees.ContainsKey(gameId))
                throw new ArgumentException("Cannot confirm referee assignment for a non-existent game.");
            if (!_gameReferees[gameId])
                throw new ArgumentException("Cannot confirm referee assignment for a game with no assigned referee.");
            Raise(new GameMsgs.RefereeConfirmed(
                        Id,
                        gameId));
        }

        #endregion
    }
}
