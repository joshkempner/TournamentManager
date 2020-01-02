using ReactiveDomain;
using ReactiveDomain.Foundation;

namespace TournamentManager.Helpers
{
    public static class StreamStoreExtensions
    {
        public static IListener GetListener(this IStreamStoreConnection connection, string name)
        {
            return new QueuedStreamListener(
                        name,
                        connection,
                        new PrefixedCamelCaseStreamNameBuilder(),
                        new JsonMessageSerializer());
        }
    }
}
