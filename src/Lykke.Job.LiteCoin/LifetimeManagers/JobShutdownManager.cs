using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.LiteCoin.PeriodicalHandlers;
using Lykke.Service.LiteCoin.API.Core.Services;

namespace Lykke.Job.LiteCoin.LifetimeManagers
{
    public class JobShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly PeriodicalHandlerHost _periodicalHandlerHost;

        public JobShutdownManager(ILog log, PeriodicalHandlerHost periodicalHandlerHost)
        {
            _log = log;
            _periodicalHandlerHost = periodicalHandlerHost;
        }

        public  Task StopAsync()
        {
            _periodicalHandlerHost.Stop();

            return Task.CompletedTask;
        }
    }
}
