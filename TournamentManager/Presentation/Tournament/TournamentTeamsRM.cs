using System;
using System.Linq;
using DynamicData;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using TournamentManager.Domain;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TournamentTeamsRM :
        ReadModelBase,
        IHandle<TeamMsgs.TeamCreated>,
        IHandle<TeamMsgs.TeamRenamed>,
        IHandle<TeamMsgs.TeamDeleted>,
        IHandle<TournamentMsgs.TeamAddedToTournament>,
        IHandle<TournamentMsgs.TeamRemovedFromTournament>,
        IHandle<TournamentMsgs.TeamAgeBracketChanged>
    {
        public TournamentTeamsRM(Guid tournamentId)
            : base(
                nameof(TournamentTeamsRM),
                () => Bootstrap.GetListener(nameof(TournamentTeamsRM)))
        {
            EventStream.Subscribe<TeamMsgs.TeamCreated>(this);
            EventStream.Subscribe<TeamMsgs.TeamRenamed>(this);
            EventStream.Subscribe<TeamMsgs.TeamDeleted>(this);
            EventStream.Subscribe<TournamentMsgs.TeamAddedToTournament>(this);
            EventStream.Subscribe<TournamentMsgs.TeamRemovedFromTournament>(this);
            EventStream.Subscribe<TournamentMsgs.TeamAgeBracketChanged>(this);
            // NB: blockUntilLive to ensure we know about all teams before reading tournament events
            Start<Team>(blockUntilLive: true);
            Start<Domain.Tournament>(tournamentId, blockUntilLive: true);
        }

        private readonly SourceCache<TeamModel, Guid> _allTeams = new SourceCache<TeamModel, Guid>(x => x.TeamId);

        public IConnectableCache<TeamModel, Guid> TournamentTeams => _tournamentTeams;
        private readonly SourceCache<TeamModel, Guid> _tournamentTeams = new SourceCache<TeamModel, Guid>(x => x.TeamId);

        public bool IsTeamInTournament(string teamName)
        {
            return _tournamentTeams.Items.Any(x => string.Compare(x.Name, teamName, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        public TeamModel? GetTeam(string teamName)
        {
            return _allTeams.Items.FirstOrDefault(x => x.Name == teamName);
        }

        public void Handle(TeamMsgs.TeamCreated message)
        {
            _allTeams.AddOrUpdate(new TeamModel(
                                        message.TeamId,
                                        message.Name));
        }

        public void Handle(TeamMsgs.TeamRenamed message)
        {
            var team = _allTeams.Lookup(message.TeamId);
            if (!team.HasValue) return;
            team.Value.Name = message.Name;
        }

        public void Handle(TeamMsgs.TeamDeleted message)
        {
            _allTeams.Remove(message.TeamId);
        }

        public void Handle(TournamentMsgs.TeamAddedToTournament message)
        {
            var team = _allTeams.Lookup(message.TeamId);
            if (!team.HasValue) return;
            team.Value.AgeBracket = message.AgeBracket;
            _tournamentTeams.AddOrUpdate(team.Value);
        }

        public void Handle(TournamentMsgs.TeamRemovedFromTournament message)
        {
            _tournamentTeams.Remove(message.TeamId);
        }

        public void Handle(TournamentMsgs.TeamAgeBracketChanged message)
        {
            var team = _tournamentTeams.Lookup(message.TeamId);
            if (!team.HasValue) return;
            team.Value.AgeBracket = message.AgeBracket;
        }
    }
}
