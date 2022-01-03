using System;
using DynamicData;

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

        public IConnectableCache<GameModel, Guid> Games => _games;
        private readonly SourceCache<GameModel, Guid> _games = new SourceCache<GameModel, Guid>(x => x.GameId);

        public bool TryAddGame(GameModel game)
        {
            if (game.FieldId == FieldId && game.FieldName == FieldName)
            {
                _games.AddOrUpdate(game);
                return true;
            }
            return false;
        }
    }
}