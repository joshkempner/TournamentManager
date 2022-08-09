using System;
using System.Reflection;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Logging;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Presentation;
using TournamentManager.Services;
using ILogger = ReactiveDomain.Logging.ILogger;

namespace TournamentManager
{
    internal class Bootstrap
    {
        private const string LogName = "TournamentManager";
        private static readonly ILogger Log = LogManager.GetLogger(LogName);

        private static IConfiguredConnection? _esConnection;
        private static IRepository? _repo;
        private IDispatcher? _mainBus;

        private RefereeSvc? _refereeSvc;
        private TournamentSvc? _tournamentSvc;
        private TeamSvc? _teamSvc;
        private HostViewSvc? _hostViewSvc;
        private MainWindowVM? _mainVM;

        internal Bootstrap()
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            var fullName = Assembly.GetExecutingAssembly().FullName ?? LogName;
            Log.Info(fullName + " Created.");
        }
        internal void Run(IConfiguredConnection esConnection)
        {
            _mainBus = new Dispatcher("Main Bus");
            Configure(esConnection, _mainBus);

            _mainVM = new MainWindowVM(_mainBus);
            _hostViewSvc = new HostViewSvc(_mainVM, _mainBus);
            var mainWindow = new MainWindow { ViewModel = _mainVM };
            mainWindow.Show();
        }

        private void Configure(
            IConfiguredConnection esConnection,
            IDispatcher bus)
        {
            _esConnection = esConnection;
            _repo = esConnection.GetRepository();
            Locator.CurrentMutable.RegisterConstant(_esConnection, typeof(IConfiguredConnection));

            _refereeSvc = new RefereeSvc(bus, _repo);
            _tournamentSvc = new TournamentSvc(bus, _repo);
            _teamSvc = new TeamSvc(bus, _repo);

            RegisterViews();
        }

        internal void Shutdown()
        {
            _refereeSvc?.Dispose();
            _teamSvc?.Dispose();
            _hostViewSvc?.Dispose();
            _tournamentSvc?.Dispose();
        }

        private void RegisterViews()
        {
            // Splat uses assembly scanning here to register all views and view models.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }
}
