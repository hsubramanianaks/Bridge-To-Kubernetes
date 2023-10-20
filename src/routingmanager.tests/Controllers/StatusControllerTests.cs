

using Microsoft.AspNetCore.Mvc;
using Microsoft.BridgeToKubernetes.Common.Models;
using Microsoft.BridgeToKubernetes.RoutingManager.Controllers;
using Xunit;

namespace Microsoft.BridgeToKubernetes.RoutingManager.Tests
{
    public class StatusControllerTests
    {
        [Fact]
        public void Get_ReturnsStatus()
        {
            //setup
            RoutingManagerApp.Status.EntityTriggerNamesStatus.Add("devhostagentname", "");
            // Arrange
            var controller = new StatusController();

            // Act
            var result = controller.Get("devhostagentname");

            // Assert
            var returnValue = Assert.IsType<RoutingStatus>(result);
            Assert.True(returnValue.IsConnected);
        }

        [Fact]
        public void Get_ReturnsStatusWithErrorMessage()
        {
            //setup
            RoutingManagerApp.Status.EntityTriggerNamesStatus.Add("devhostagentname1", "Error");
            // Arrange
            var controller = new StatusController();

            // Act
            var result = controller.Get("devhostagentname1");

            // Assert
            var returnValue = Assert.IsType<RoutingStatus>(result);
            Assert.False(returnValue.IsConnected);
            Assert.Equal("Error", returnValue.ErrorMessage);
        }

        [Fact]
        public void Get_ReturnsStatusWithErrorMessageWhenNoEntryInStatusDictionary()
        {
            // Arrange
            var controller = new StatusController();

            // Act
            var result = controller.Get("devhostagentname2");

            // Assert
            var returnValue = Assert.IsType<RoutingStatus>(result);
            Assert.Null(returnValue.IsConnected);
            Assert.Equal(Common.Constants.Routing.InvalidValueOfTriggerError + "devhostagentname2", returnValue.ErrorMessage);
        }
    }
}