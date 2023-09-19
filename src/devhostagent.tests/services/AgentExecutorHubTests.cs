using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Xunit;
using Microsoft.BridgeToKubernetes.DevHostAgent.Services;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("devhostagent.tests")]
public class AgentExecutorHubTests
{
    [Fact]
    public async Task RunServicePortForward_ReturnsChannelReader()
    {
        // Arrange
        var serviceDns = "my-service";
        var port = 8080;

        using var mock = AutoMock.GetLoose();
        var hub = mock.Create<AgentExecutorHub>();

        // Act
        var result = hub.RunServicePortForward(serviceDns, port);

        // Assert
        Assert.NotNull(result);
        await result.ReadAllAsync(); // Ensure the channel reader can be read from
    }
}