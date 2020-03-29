using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class GameSlotSummaryVM : ReactiveObject
    {
        private readonly Guid _gameSlotId;

        public GameSlotSummaryVM(GameSlotModel model)
        {
            _gameSlotId = model.Id;
            StartTime = model.StartTime;
            EndTime = model.EndTime;

            model.Games
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Sort(SortExpressionComparer<GameModel>.Ascending(x => x.FieldName))
                .Transform(x => new GameSummaryVM(x))
                .DisposeMany()
                .Subscribe();
        }

        public IObservableCollection<GameSummaryVM> Games { get; } = new ObservableCollectionExtended<GameSummaryVM>();

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
    }
}
