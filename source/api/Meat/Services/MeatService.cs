using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Services
{
    public class MeatService : BackgroundService
    {
        private readonly ILogger<MeatService> Logger;
        private readonly IMediator Mediator;
        private Timer Timer;

        public MeatService(ILogger<MeatService> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(12));

            Logger.LogInformation("Starting Task");

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            //Logger.LogInformation("Running Proceso Senia");
            //var requestSenia = new ProcesoSeniaRequest();
            //var responseSenia = await Mediator.Send(requestSenia);
            //Logger.LogInformation($"Respuesta Proceso Senia: { JsonConvert.SerializeObject(responseSenia)}");
            //Logger.LogInformation("Ending Proceso Senia");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Timer?.Change(Timeout.Infinite, 0);

            Logger.LogInformation("Finishing Task");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Timer != null)
            {
                Timer.Dispose();
                Timer = null;
            }
        }
    }
}