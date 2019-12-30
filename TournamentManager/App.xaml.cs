using System.IO;
using System.Windows;
using ReactiveDomain.EventStore;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Bootstrap _bootstrap;
        private readonly EventStoreLocalStartupUtils _es;

        /// <inheritdoc />
        public App()
        {
            _bootstrap = new Bootstrap();
            _es = new EventStoreLocalStartupUtils();
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            const string eventStoreLocation = "C:\\Program Files\\EventStore";
            _es.SetupEventStore(new DirectoryInfo(eventStoreLocation));
            _bootstrap.Run(_es.Connection);
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrap.Shutdown();
            _es.TeardownEventStore();
            base.OnExit(e);
        }
    }
}
