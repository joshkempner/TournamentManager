using System;

namespace TournamentManager.Presentation
{
    public class FieldModel
    {
        public FieldModel(Guid fieldId, string fieldName)
        {
            FieldId = fieldId;
            FieldName = fieldName;
        }

        public Guid FieldId { get; }
        public string FieldName { get; }
    }
}