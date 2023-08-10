using Microsoft.BridgeToKubernetes.TestHelpers;
using Xunit;
using System.Threading;
using Microsoft.BridgeToKubernetes.Exe.Commands;
using FakeItEasy;
using Microsoft.BridgeToKubernetes.Common.Kubernetes;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Helpers;
using Microsoft.BridgeToKubernetes.Common.Commands;
using Microsoft.BridgeToKubernetes.Exe.Commands.Connect;

namespace Microsoft.BridgeToKubernetes.Exe.Tests
{
    public class CliAppTests: TestsBase
    {
        private readonly CliApp _cliApp;
        private static System.Lazy<CommandLineArgumentsManager> _commandLineArgumentsManager = new System.Lazy<CommandLineArgumentsManager>(new CommandLineArgumentsManager());
        private static System.Lazy<RootCommand> _rootCommand = new System.Lazy<RootCommand>();
        private static CommandLineApplication _commandLineApplication = new CommandLineApplication();
        private readonly CommandsConfigurator _commandsConfigurator = new CommandsConfigurator(_commandLineApplication, _rootCommand, _commandLineArgumentsManager);
        private static string[] commands = new string[] { "connect", "--service", "stats-api", "--namespace", "todo-app", "--local-port", "3001" };


        public CliAppTests()
        {
            _autoFake.Provide(_commandsConfigurator);
            _commandLineArgumentsManager.Value.ParseGlobalArgs(commands);
            _cliApp = _autoFake.Resolve<CliApp>();
        }

        [Fact]
        public void shouldFailIfNoArgumentsPassed()
        {
            int result = _cliApp.Execute(new string[0], new CancellationToken());
            Assert.Equal(1, result);
        }

        [Fact]
        public void shouldFailIfInvalidArgumentsPassed()
        {
            int result = _cliApp.Execute(new string[] { "invalid" }, new CancellationToken());
            Assert.Equal(1, result);
        }
/*
        [Fact]
        public void shouldPassIfValidArgumentsPassed()
        {
           
            int result = _cliApp.Execute(commands, new CancellationToken());
            Assert.Equal(0, result);
        }*/
    }
}
