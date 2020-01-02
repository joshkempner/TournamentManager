using System.Reflection;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Logging;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Presentation;
using ILogger = ReactiveDomain.Logging.ILogger;

namespace TournamentManager
{
    internal class Bootstrap
    {
        internal const string LogName = "TournamentManager";
        private static string _assemblyName;
        private static readonly ILogger Log = LogManager.GetLogger(LogName);

        private static IStreamStoreConnection _esConnection;
        private static StreamStoreRepository _repo;
        private IDispatcher _mainBus;

        private RefereeSvc _refereeSvc;
        private MainWindowVM _mainVM;

        internal Bootstrap()
        {
            var fullName = Assembly.GetExecutingAssembly().FullName;
            Log.Info(fullName + " Created.");
            _assemblyName = fullName.Split(',')[0];
        }
        internal void Run(IStreamStoreConnection esConnection)
        {
            _mainBus = new Dispatcher("Main Bus");
            Configure(esConnection, _mainBus);

            _mainVM = new MainWindowVM(_mainBus);
            var mainWindow = new MainWindow { ViewModel = _mainVM };
            mainWindow.Show();
        }

        internal void Configure(
            IStreamStoreConnection esConnection,
            IDispatcher bus)
        {
            _esConnection = esConnection;
            _repo = new StreamStoreRepository(
                            new PrefixedCamelCaseStreamNameBuilder(),
                            esConnection,
                            new JsonMessageSerializer());
            Locator.CurrentMutable.RegisterConstant(esConnection, typeof(IStreamStoreConnection));

            _refereeSvc = new RefereeSvc(bus, _repo);

            RegisterViews();
        }

        internal void Shutdown()
        {
            _refereeSvc.Dispose();
        }

        private void RegisterViews()
        {
            // Splat uses assembly scanning here to register all views and view models.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }
}
