using System;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TeamModel : ReactiveObject
    {
        public TeamModel(
            Guid teamId,
            string name)
        {
            TeamId = teamId;
            Name = name;
        }

        public Guid TeamId { get; }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        private string _name = string.Empty;

        public TournamentMsgs.AgeBracket AgeBracket
        {
            get => _ageBracket;
            set => this.RaiseAndSetIfChanged(ref _ageBracket, value);
        }
        private TournamentMsgs.AgeBracket _ageBracket;
    }
}