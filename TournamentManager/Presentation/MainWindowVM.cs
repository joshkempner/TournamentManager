using System.Reactive;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Helpers;

namespace TournamentManager.Presentation
{
    public class MainWindowVM
    {
        public ReactiveCommand<Unit, Unit> AddReferee { get; }

        public MainWindowVM(IDispatcher bus)
        {
            AddReferee = CommandBuilder.FromAction(() =>
            {
                Threading.RunOnUiThread(() =>
                {
                    var vm = new NewRefereeVM(bus);
                    var view = new NewReferee { ViewModel = vm };
                    view.Show();
                });
            });
        }
    }
}
