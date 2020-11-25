using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Util;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class Team : AggregateRoot
    {
        private bool _isDeleted;

        private Team()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Register<TeamMsgs.TeamCreated>(e => Id = e.TeamId);
            Register<TeamMsgs.TeamDeleted>(_ => _isDeleted = true);
        }

        public Team(
            Guid teamId,
            string teamName,
            TeamMsgs.AgeBracket ageBracket,
            ICorrelatedMessage source)
            : this()
        {
            Ensure.NotEmptyGuid(teamId, nameof(teamId));
            Ensure.NotNullOrEmpty(teamName, nameof(teamName));
            Ensure.False(() => string.IsNullOrWhiteSpace(teamName), nameof(teamName));
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            if (source.CausationId == Guid.Empty)
                Ensure.NotEmptyGuid(source.MsgId, nameof(source.MsgId));
            ((ICorrelatedEventSource)this).Source = source;
            Raise(new TeamMsgs.TeamCreated(
                        teamId,
                        teamName,
                        ageBracket));
        }

        public void DeleteTeam()
        {
            if (_isDeleted) return;
            Raise(new TeamMsgs.TeamDeleted(Id));
        }

        public void RenameTeam(string newName)
        {
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            Ensure.False(() => string.IsNullOrWhiteSpace(newName), nameof(newName));
            if (_isDeleted) throw new InvalidOperationException("Cannot rename a team that has been deleted.");
            Raise(new TeamMsgs.TeamRenamed(
                        Id,
                        newName));
        }

        public void UpdateAgeBracket(TeamMsgs.AgeBracket ageBracket)
        {
            if (_isDeleted) throw new InvalidOperationException("Cannot change the age bracket for a team that has been deleted.");
            Raise(new TeamMsgs.AgeBracketUpdated(
                        Id,
                        ageBracket));
        }
    }
}
