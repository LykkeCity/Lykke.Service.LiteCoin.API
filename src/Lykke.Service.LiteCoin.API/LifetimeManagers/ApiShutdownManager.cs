using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Services;

namespace Lykke.Service.LiteCoin.API.LifetimeManagers
{
    // NOTE: Sometimes, shutdown process should be expressed explicitly. 
    // If this is your case, use this class to manage shutdown.
    // For example, sometimes some state should be saved only after all incoming message processing and 
    // all periodical handler was stopped, and so on.

    public class ApiShutdownManager : IShutdownManager
    {
        private readonly ILog _log;

        public ApiShutdownManager(ILog log)
        {
            _log = log;
        }

        public async Task StopAsync()
        {
            // TODO: Implement your shutdown logic here. Good idea is to log every step

            await Task.CompletedTask;
        }
    }
}
