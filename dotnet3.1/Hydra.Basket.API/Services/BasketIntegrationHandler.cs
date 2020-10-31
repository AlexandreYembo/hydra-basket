using System;
using System.Threading;
using System.Threading.Tasks;
using Hydra.Basket.Domain.Repositories;
using Hydra.Core.Integration.Messages.OrderMessages;
using Hydra.Core.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hydra.Basket.API.Services
{
    public class BasketIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;

        public BasketIntegrationHandler(IMessageBus messageBus, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _messageBus = messageBus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }
        private void SetSubscribers()
        {
            _messageBus.SubscribeAsync<OrderStartedIntegrationEvent>(subscriptionId: "OrderStarted", async request =>
                    await RemoveBasket(request));
        }

        private async Task RemoveBasket(OrderStartedIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IBasketRepository>();
 
            var basket = await repository.GetByCustomerId(message.CustomerId);

            if(basket != null)
            {
                await repository.Delete(message.CustomerId);
            }
        }
    }
}