using System;
using System.Collections.Generic;
using DynamicData;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleRM :
        ReadModelBase,
        IHandle<TournamentMsgs.TournamentAdded>,
        IHandle<TournamentMsgs.TournamentRescheduled>,
        IHandle<TournamentMsgs.FieldAdded>,
        IHandle<GameMsgs.GameAdded>,
        IHandle<GameMsgs.GameRescheduled>
    {
        public TournamentScheduleRM(
            Guid tournamentId)
            : base(
                nameof(TournamentScheduleRM),
                () => Bootstrap.GetListener(nameof(TournamentScheduleRM)))
        {
            EventStream.Subscribe<TournamentMsgs.TournamentAdded>(this);
            EventStream.Subscribe<TournamentMsgs.TournamentRescheduled>(this);
            EventStream.Subscribe<TournamentMsgs.FieldAdded>(this);
            EventStream.Subscribe<GameMsgs.GameAdded>(this);
            EventStream.Subscribe<GameMsgs.GameRescheduled>(this);
            Start<Domain.Tournament>(tournamentId, blockUntilLive: true);
        }

        public IObservable<DateTime> FirstDay => _firstDay;
        private readonly ReadModelProperty<DateTime> _firstDay = new ReadModelProperty<DateTime>(default);

        public IObservable<DateTime> LastDay => _lastDay;
        private readonly ReadModelProperty<DateTime> _lastDay = new ReadModelProperty<DateTime>(default);

        public IConnectableCache<FieldModel, Guid> Fields => _fields;
        private readonly SourceCache<FieldModel, Guid> _fields = new SourceCache<FieldModel, Guid>(x => x.FieldId);

        private readonly Dictionary<Guid, GameModel> _games = new Dictionary<Guid, GameModel>();

        public void Handle(TournamentMsgs.TournamentAdded message)
        {
            _firstDay.Update(message.FirstDay);
            _lastDay.Update(message.LastDay);
        }

        public void Handle(TournamentMsgs.TournamentRescheduled message)
        {
            _firstDay.Update(message.FirstDay);
            _lastDay.Update(message.LastDay);
        }

        public void Handle(TournamentMsgs.FieldAdded message)
        {
            _fields.AddOrUpdate(new FieldModel(message.FieldId, message.FieldName));
        }

        public void Handle(GameMsgs.GameAdded message)
        {
            var field = _fields.Lookup(message.FieldId);
            if (!field.HasValue) return;
            var game = new GameModel(
                            message.GameId,
                            message.StartTime,
                            message.EndTime,
                            field.Value.FieldId,
                            field.Value.FieldName);
            field.Value.TryAddGame(game);
            _games[message.GameId] = game;
        }

        public void Handle(GameMsgs.GameRescheduled message)
        {
            if (!_games.TryGetValue(message.GameId, out var game)) return;
            game.StartTime = message.StartTime;
            game.EndTime = message.EndTime;
        }
    }
}
