using System;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class GameSummaryVM : ReactiveObject
    {
        private readonly Guid _fieldId;

        public GameSummaryVM(GameModel model)
        {
            _fieldId = model.FieldId;
        }
    }
}
