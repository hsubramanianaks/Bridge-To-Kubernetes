﻿using System;
using System.Diagnostics;
using System.Threading;

namespace integration.tests
{
    public class IntegrationTests
    {

        public Process startBridge()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = @"/home/runner/work/Bridge-To-Kubernetes/Bridge-To-Kubernetes/src/dsc/bin/Debug/net6.0/linux-x64/dsc";
            startInfo.FileName = @"C:\Users\hsubramanian\repos\forked\Bridge-To-Kubernetes\src\dsc\bin\Debug\net6.0\dsc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.EnvironmentVariables["BRIDGE_ENVIRONMENT"] = "dev";
            startInfo.Arguments = "connect --service stats-api --namespace todo-app --local-port 3001 --control-port 51424 --use-kubernetes-service-environment-variables -y -- npm i & npm run start";

            return Process.Start(startInfo);
        }


        public Task<string> ReadOutputAsync(Process process) => process.StandardOutput.ReadLineAsync();

        [Fact]
        public void b2k_shouldConnect()
        {
            Process? process = null;
            try
            {
                // Arrange
                process = startBridge();

                //wait
                Thread.Sleep(10000);

                // Assert
                Assert.NotNull(process);
                Assert.Equal("dsc", process.ProcessName);
                Assert.Equal(0, process.ExitCode);
                // add more asserts here.
            } catch(Exception ex)
            {
                throw;
            }
            finally {
                if (process != null && process.ExitCode != 1)
                {
                    // make http call to localhost:controlport to shutdown b2k cli
                    using var httpClient = new HttpClient();
                    var stopRemotingUriBuilder = new UriBuilder();
                    stopRemotingUriBuilder.Scheme = "http";
                    stopRemotingUriBuilder.Host = "localhost";
                    stopRemotingUriBuilder.Port = 51424;
                    stopRemotingUriBuilder.Path = "api/remoting/stop/";
                    httpClient.PostAsync(stopRemotingUriBuilder.Uri, new StringContent(""));
                    process.Kill();
                }
            }
        }
    }
}