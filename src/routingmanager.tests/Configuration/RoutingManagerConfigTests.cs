using Microsoft.BridgeToKubernetes.RoutingManager.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.BridgeToKubernetes.RoutingManager.Tests
{
    public class RoutingManagerConfigTests
    {
        [Fact]
        public void GetNamespace_ReturnsNamespace()
        {
            //setup
            RoutingManagerConfig routingManagerConfig = new();
            Environment.SetEnvironmentVariable("NAMESPACE", "testnamespace");

            //act
            string result = routingManagerConfig.GetNamespace();

            //assert
            Assert.Equal("testnamespace", result);
        }

        [Fact]
        public void GetNamespace_ThrowsExceptionWhenNamespaceIsNull()
        {
            //setup
            RoutingManagerConfig routingManagerConfig = new();
            Environment.SetEnvironmentVariable("NAMESPACE", "");

            //act
            var exception = Assert.Throws<ArgumentNullException>(() => routingManagerConfig.GetNamespace());

            //assert
            Assert.Equal("Value cannot be null. (Parameter '_namespaceName')", exception.Message);
        }
    }
}
