using System;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class GameModel : ReactiveObject
    {
        public GameModel(
            Guid fieldId,
            string fieldName)
        {
            FieldId = fieldId;
            FieldName = fieldName;
        }

        public Guid FieldId { get; }
        public string FieldName { get; }
    }
}
