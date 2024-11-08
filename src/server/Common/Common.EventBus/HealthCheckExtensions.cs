using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Common.EventBus;

public static class HealthCheckExtensions
{
    /// <summary>
    /// This method register healthchecks for 
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="connectionString">The connection string</param>
    /// <returns>The updated service list with Kafka healthcheck</returns>
    public static IServiceCollection AddKafkaHealthCheck(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks().AddKafka(new ProducerConfig()
        { BootstrapServers = connectionString },
        name: "kafka", tags: ["broker"]);

        return services;
    }
}