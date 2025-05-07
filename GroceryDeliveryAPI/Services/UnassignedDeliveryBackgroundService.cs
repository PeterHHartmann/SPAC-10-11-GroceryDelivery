using GroceryDeliveryAPI.Managers;

namespace GroceryDeliveryAPI.Services
{
    public class UnassignedDeliveryBackgroundService : BackgroundService
    {
        private readonly ILogger<UnassignedDeliveryBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public UnassignedDeliveryBackgroundService(
            ILogger<UnassignedDeliveryBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Unassigned Delivery Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for unassigned deliveries at: {time}", DateTimeOffset.Now);

                try
                {
                    await ProcessUnassignedDeliveriesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing unassigned deliveries");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessUnassignedDeliveriesAsync(CancellationToken stoppingToken)
        {
            // Create a new scope for each execution
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the delivery manager from the scope
                var deliveryManager = scope.ServiceProvider.GetRequiredService<DeliveryManager>();

                // Process unassigned deliveries
                await deliveryManager.AssignDriversToUnassignedDeliveries();
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Unassigned Delivery Background Service is stopping.");
            return base.StopAsync(stoppingToken);
        }
    }
}
