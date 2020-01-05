﻿using System;
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
        private HashSet<Guid> _fields = new HashSet<Guid>();
        private HashSet<Guid> _gameSlots = new HashSet<Guid>();

        public Tournament()
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
            Register<TournamentMsgs.GameSlotAdded>(e => _gameSlots.Add(e.GameSlotId));
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

        public void AddGameSlot(
            Guid gameSlotId,
            DateTime startTime,
            DateTime endTime)
        {
            Ensure.NotEmptyGuid(gameSlotId, nameof(gameSlotId));
            Ensure.True(() => startTime > _firstDay && startTime < _lastDay, "startTime > _firstDay && startTime < _lastDay");
            Ensure.True(() => endTime > _firstDay && endTime < _lastDay, "endTime > _firstDay && endTime < _lastDay");
            Ensure.True(() => endTime > startTime, "endTime > startTime");
            if (_gameSlots.Contains(gameSlotId))
                throw new ArgumentException("Cannot add a second game slot with the same ID.");
            Raise(new TournamentMsgs.GameSlotAdded(
                        Id,
                        gameSlotId,
                        startTime,
                        endTime));
        }
    }
}
