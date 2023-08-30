using Microsoft.BridgeToKubernetes.RoutingManager.Controllers;
using Xunit;

namespace Microsoft.BridgeToKubernetes.RoutingManager.Tests
{
    public class PingControllerTests
    {
        [Fact]
        public void Get_ReturnsStatus()
        {
           PingController pingController = new();
           string result = pingController.Get();

            //assert
            Assert.Equal("Ping!!", result);
        }
    }
}
