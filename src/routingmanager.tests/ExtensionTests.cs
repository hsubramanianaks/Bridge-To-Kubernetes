using FakeItEasy;
using Microsoft.BridgeToKubernetes.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.BridgeToKubernetes.RoutingManager.Tests
{
    public class ExtensionTests
    {
        [Fact]
        public void IsRoutingTrigger_Works()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-on-header", "" }
            };
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-from", "" }
            };
            //assert
            Assert.True(Extensions.IsRoutingTrigger(meta));
        }

        [Fact]
        public void IsRoutingTrigger_ReturnsFalseWhenNoAnnotation()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-on-header1", "" }
            };
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-from", "" }
            };
            //assert
            Assert.False(Extensions.IsRoutingTrigger(meta));
        }

        [Fact]
        public void IsRoutingTrigger_ReturnsFalseWhenNoLabel()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-on-header", "" }
            };
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-from1", "" }
            };
            //assert
            Assert.False(Extensions.IsRoutingTrigger(meta));
        }

        [Fact]
        public void IsGenerated_Works()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/generated", "" }
            };

            //assert
            Assert.True(Extensions.IsGenerated(meta));
        }

        [Fact]
        public void IsGenerated_ReturnsFalseWhenNoLabel()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/generated1", "" }
            };

            //assert
            Assert.False(Extensions.IsGenerated(meta));
        }

        [Fact]
        public void GetRouteFromServiceName_Works()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-from", "devhostagentname" }
            };

            //assert
            Assert.Equal("devhostagentname", Extensions.GetRouteFromServiceName(meta, null));
        }

        [Fact]
        public void GetRouteFromServiceName_ReturnsNullWhenNoLabel()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Labels = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-from1", "devhostagentname" }
            };

            //act
            var exception = Assert.Throws<RoutingException>(() => Extensions.GetRouteFromServiceName(meta, A.Fake<ILog>()));

            //assert
            Assert.Equal("Failed to read label value 'routing.visualstudio.io/route-from' from object 'null'. ", exception.Message);
        }

        [Fact]
        public void GetRouteOnHeader_Works()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-on-header", "x-ms-routing=name" }
            };

            var (headerName, headerValue) = Extensions.GetRouteOnHeader(meta, null);

            //assert
            Assert.Equal("x-ms-routing", headerName);
            Assert.Equal("name", headerValue);
        }

        [Fact]
        public void GetRouteOnHeader_ReturnsNullWhenNoAnnotation()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "routing.visualstudio.io/route-on-header1", "x-ms-routing=name" }
            };

            //act
            var exception = Assert.Throws<RoutingException>(() => Extensions.GetRouteOnHeader(meta, A.Fake<ILog>()));

            //assert
            Assert.Equal("Failed to read annotation value 'routing.visualstudio.io/route-on-header' from object 'null'. ", exception.Message);
        }

        [Fact]
        public void GetCorrelationId_Works()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "mindaro.io/correlation-id", "correlationid" }
            };

            //assert
            Assert.Equal("correlationid", Extensions.GetCorrelationId(meta));
        }

        [Fact]
        public void GetCorrelationId_ReturnsNullWhenNoAnnotation()
        {
            k8s.Models.V1ObjectMeta meta = new();
            meta.Annotations = new Dictionary<string, string>
            {
                { "mindaro.io/correlation-id1", "correlationid" }
            };

            //assert
            Assert.Empty(Extensions.GetCorrelationId(meta));
        }
    }
}
