using System;
using System.Net;
using System.Windows;
using Microsoft.Extensions.Configuration;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using ES = EventStore.ClientAPI;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IConfiguredConnection? _esConnection;
        private static readonly IConfigurationRoot _appConfig;
        private readonly Bootstrap _bootstrap;

        static App()
        {
            _appConfig = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("es_settings.json")
                                .Build();
        }

        /// <inheritdoc />
        public App()
        {
            _bootstrap = new Bootstrap();
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConnectToEs();
            if (_esConnection == null) throw new Exception("Failed to connect to ESDB");
            _bootstrap.Run(_esConnection);
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrap.Shutdown();
            _esConnection?.Connection?.Close();;
            base.OnExit(e);
        }

        private static void ConnectToEs()
        {
            _esConnection = BuildConnection();
            Console.WriteLine("Connected to ESDB");
        }

        private static IConfiguredConnection BuildConnection()
        {
            string esUser = _appConfig.GetValue<string>("EventStoreUserName");
            string esPwd = _appConfig.GetValue<string>("EventStorePassword");
            string esIpAddress = _appConfig.GetValue<string>("EventStoreIPAddress");
            int esPort = _appConfig.GetValue<int>("EventStorePort");
            string schema = _appConfig.GetValue<string>("Schema");
            var tcpEndpoint = new IPEndPoint(IPAddress.Parse(esIpAddress), esPort);

            var settings = ES.ConnectionSettings.Create()
                            .SetDefaultUserCredentials(new ES.SystemData.UserCredentials(esUser, esPwd))
                            .KeepReconnecting()
                            .KeepRetrying()
                            .UseConsoleLogger()
                            .DisableTls()
                            .DisableServerCertificateValidation()
                            .WithConnectionTimeoutOf(TimeSpan.FromSeconds(15))
                            .Build();

            // TODO: Start ES or connect to it if already running
            var conn = ES.EventStoreConnection.Create(settings, tcpEndpoint, "PolicyLoader");
            conn.ConnectAsync().Wait();
            //todo: confirm connected
            // conn.AppendToStreamAsync("test",ES.ExpectedVersion.Any, 
            //    new ES.EventData(Guid.NewGuid(),"TestEvent",false,new byte[]{5,5,5},new byte[]{5,5,5 })).Wait();
            //Console.WriteLine("written");
            return new ConfiguredConnection(
                            new EventStoreConnectionWrapper(conn),
                            new PrefixedCamelCaseStreamNameBuilder(schema),
                            new JsonMessageSerializer());
        }
    }
}
