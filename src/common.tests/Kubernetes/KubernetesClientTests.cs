// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using k8s;
using k8s.Autorest;
using k8s.KubeConfigModels;
using k8s.Models;
using Microsoft.BridgeToKubernetes.Common.Kubernetes;
using Microsoft.BridgeToKubernetes.TestHelpers;
using Xunit;

namespace Microsoft.BridgeToKubernetes.Common.Tests.Kubernetes
{
    public class KubernetesClientTests : TestsBase
    {
        private readonly KubernetesClient _kClient;
        private readonly IK8sClientFactory _fakeFactory = A.Fake<IK8sClientFactory>();
        private readonly IKubernetes _fakeRestClient = A.Fake<IKubernetes>();

        private readonly StreamWriter _streamFake = new StreamWriter(Stream.Null);

        public KubernetesClientTests()
        {
            A.CallTo(() => _fakeFactory.CreateFromKubeConfig(A<K8SConfiguration>._)).Returns(_fakeRestClient);
            _autoFake.Provide<IK8sClientFactory>(_fakeFactory);
            _autoFake.Provide<StreamWriter>(_streamFake);
            _kClient = _autoFake.Resolve<KubernetesClient>();
        }

        public override void Dispose()
        {
            base.Dispose();
            this._streamFake.Dispose();
        }

        [Fact]
        public async Task EnsureClientInvokeWrapperHandlesIOExceptions()
        {
            Expression<Func<Task<HttpOperationResponse<V1PodList>>>> listPodsInNamespaceExpression =
                                                                            () => _fakeRestClient.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
                                                                                            A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                            A<string>._, A<int?>._, A<string>._, A<string>._,
                                                                                            A<bool?>._, A<int?>._, A<bool?>._, A<bool?>._, 
                                                                                            A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._,
                                                                                            A<CancellationToken>._);
            A.CallTo(listPodsInNamespaceExpression)
                .Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1PodList>()
                {
                    Body = new V1PodList()
                });

            var pods = await _kClient.ListPodsInNamespaceAsync("default");
            Assert.Empty(pods.Items);
            A.CallTo(listPodsInNamespaceExpression).MustHaveHappenedANumberOfTimesMatching(n => n == 3);
        }

        [Fact]
        public async Task EnsureClientInvokeWrapperHandlesNotFound()
        {
            Expression<Func<Task<HttpOperationResponse<V1PodList>>>> listPodsInNamespaceExpression =
                                                                            () => _fakeRestClient.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
                                                                                            A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                            A<string>._, A<int?>._, A<string>._, A<string>._,
                                                                                            A<bool?>._, A<int?>._, A<bool?>._, A<bool?>._, 
                                                                                            A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._,
                                                                                            A<CancellationToken>._);
            var ex = new HttpOperationException();
            ex.Response = new HttpResponseMessageWrapper(new HttpResponseMessage(HttpStatusCode.NotFound), string.Empty);
            A.CallTo(listPodsInNamespaceExpression).Throws(ex);

            var pods = await _kClient.ListPodsInNamespaceAsync("default");
            Assert.Empty(pods.Items);
            A.CallTo(listPodsInNamespaceExpression).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ListPodsInNamespaceTest()
        {
            Expression<Func<Task<HttpOperationResponse<V1PodList>>>> listAllPodsInNamespaceExpression =
                                                                            () => _fakeRestClient.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
                                                                                            A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                            A<string>._, A<int?>._, A<string>._, A<string>._,
                                                                                            A<bool?>._, A<int?>._, A<bool?>._, A<bool?>._, 
                                                                                            A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._,
                                                                                            A<CancellationToken>._);

            A.CallTo(listAllPodsInNamespaceExpression)
                .Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1PodList>()
                {
                    Body = new V1PodList()
                });

            var pods = await _kClient.ListPodsInNamespaceAsync("default");

            Assert.Empty(pods.Items);
            A.CallTo(listAllPodsInNamespaceExpression).MustHaveHappenedANumberOfTimesMatching(n => n == 3);
        }

        [Fact]
        public async Task ListServicesInNamespace()
        {
            Expression<Func<Task<HttpOperationResponse<V1ServiceList>>>> listServiceForAllNamespacesExpression =
                                                                                () => _fakeRestClient.CoreV1.ListServiceForAllNamespacesWithHttpMessagesAsync(
                                                                                                                        A<bool?>._, A<string>._, A<string>._, A<string>._,
                                                                                                                        A<int?>._, A<bool?>._, A<string>._, A<string>._, A<bool?>._, A<int?>._, A<bool?>._,
                                                                                                                        A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            Expression<Func<Task<HttpOperationResponse<V1ServiceList>>>> listNamespacedServiceExpression =
                                                                                () => _fakeRestClient.CoreV1.ListNamespacedServiceWithHttpMessagesAsync(
                                                                                                                        A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                                                        A<string>._, A<int?>._, A<string>._, A<string>._, A<bool?>._, A<int?>._,
                                                                                                                        A<bool?>._, A<bool?>._, A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            A.CallTo(listNamespacedServiceExpression)
                .Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1ServiceList>()
                {
                    Body = new V1ServiceList()
                });

            var services = await _kClient.ListServicesInNamespaceAsync("default", null, new CancellationToken());

            Assert.Empty(services.Items);
            A.CallTo(listNamespacedServiceExpression).MustHaveHappenedANumberOfTimesMatching(n => n == 3);
            A.CallTo(listServiceForAllNamespacesExpression).MustNotHaveHappened();
        }

        [Fact]
        public async Task ListServicesInAllNamespaces()
        {
            Expression<Func<Task<HttpOperationResponse<V1ServiceList>>>> listServiceForAllNamespacesExpression =
                                                                                () => _fakeRestClient.CoreV1.ListServiceForAllNamespacesWithHttpMessagesAsync(
                                                                                                                        A<bool?>._, A<string>._, A<string>._, A<string>._,
                                                                                                                        A<int?>._, A<bool?>._, A<string>._, A<string>._, A<bool?>._, A<int?>._, A<bool?>._,
                                                                                                                        A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            Expression<Func<Task<HttpOperationResponse<V1ServiceList>>>> listNamespacedServiceExpression =
                                                                                () => _fakeRestClient.CoreV1.ListNamespacedServiceWithHttpMessagesAsync(
                                                                                                                        A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                                                        A<string>._, A<int?>._, A<string>._, A<string>._, A<bool?>._, A<int?>._,
                                                                                                                        A<bool?>._, A<bool?>._, A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            A.CallTo(listServiceForAllNamespacesExpression)
                .Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1ServiceList>()
                {
                    Body = new V1ServiceList()
                });

            var services = await _kClient.ListServicesInNamespaceAsync(null, null, new CancellationToken());

            Assert.Empty(services.Items);
            A.CallTo(listServiceForAllNamespacesExpression).MustHaveHappenedANumberOfTimesMatching(n => n == 3);
            A.CallTo(listNamespacedServiceExpression).MustNotHaveHappened();
        }

        /*[Fact]
        public async Task ListDeploymentsInNamespaceAsync()
        {
            Expression<Func<Task<HttpOperationResponse<V1DeploymentList>>>> listDeploymentsInNamespaceAsync =
                                                                                () => _fakeRestClient.AppsV1.ListNamespacedDeploymentWithHttpMessagesAsync(A<string>._, A<bool?>._, A<string>._, A<string>._,
                                                                                A<string>._, A<int>._, A<string>._, A<string>._, A<bool?>._, A<int>._, A<bool?>._, A<bool?>._,
                                                                                A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            Expression<Func<Task<HttpOperationResponse<V1DeploymentList>>>> listDeploymentsAllNamespaceAsync =
                                                                                () => _fakeRestClient.AppsV1.ListDeploymentForAllNamespacesWithHttpMessagesAsync( A<bool?>._, A<string>._, A<string>._,
                                                                                A<string>._, A<int>._, A<bool?>._, A<string>._, A<string>._, A<bool?>._, A<int?>._, A<bool?>._,
                                                                                A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._, A<CancellationToken>._);

            A.CallTo(listDeploymentsAllNamespaceAsync).Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1DeploymentList>()
            {
                Body = new V1DeploymentList()
            });
            var deploymentList = await _kClient.ListDeploymentsInNamespaceAsync(null, new CancellationToken());
            Assert.Empty(deploymentList.Items);
            A.CallTo(listDeploymentsInNamespaceAsync).MustHaveHappened();
        }*/

        [Fact]
        public async Task GetV1ServiceAsync()
        {
            Expression<Func<Task<HttpOperationResponse<V1Service>>>> readNamespacedServiceWithHttpMessagesAsync =
                                                                                () => _fakeRestClient.CoreV1.ReadNamespacedServiceWithHttpMessagesAsync(
                                                                                                                        A<string>._, A<string>._, A<bool?>._, A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._,
                                                                                                                        A<CancellationToken>._);
            A.CallTo(readNamespacedServiceWithHttpMessagesAsync)
                .Throws<IOException>()
                .Once()
                .Then
                .Throws(new HttpRequestException(string.Empty, new IOException()))
                .Once()
                .Then
                .Returns(new HttpOperationResponse<V1Service>()
                {
                    Body = new V1Service()
                });
            var result = await _kClient.GetV1ServiceAsync(null, null, new CancellationToken());
            Assert.NotNull(result);
            A.CallTo(readNamespacedServiceWithHttpMessagesAsync).MustHaveHappenedANumberOfTimesMatching(n => n == 3);

        }

        /*[Fact]
        public async Task CreateOrReplaceV1ServiceAsync()
        {
            Expression<Func<Task<HttpOperationResponse<V1Service>>>> CreateNamespacedServiceWithHttpMessagesAsync =
                                                                                () => _fakeRestClient.CoreV1.CreateNamespacedServiceWithHttpMessagesAsync(A<V1Service>._,A<string>._,null,null,n);

            Expression<Func<Task<HttpOperationResponse<V1Service>>>> DeleteNamespacedServiceWithHttpMessagesAsync =
                                                                                () => _fakeRestClient.CoreV1.DeleteNamespacedServiceWithHttpMessagesAsync(
                                                                                                                        A<string>._, A<string>._, A<V1DeleteOptions>._, A<string>._, A<int?>._,
                                                                                                                        A<bool?>._,
                                                                                                                        A<string>._,
                                                                                                                         A<bool?>._,
                                                                                                                        A<IReadOnlyDictionary<string, IReadOnlyList<String>>>._,
                                                                                                                        A<CancellationToken>._);

            A.CallTo(CreateNamespacedServiceWithHttpMessagesAsync)
                .Throws(new HttpOperationException("Conflict", new Exception()))
                .Once()
                .Then
                .Throws(new HttpOperationException("Conflict", new Exception()))
                .Once()
                .Then
                .Throws(new HttpOperationException("Conflict", new Exception()))

            var result = await _kClient.CreateOrReplaceV1ServiceAsync("test", new V1Service(), new CancellationToken());
            Assert.NotNull(result);
            A.CallTo(CreateNamespacedServiceWithHttpMessagesAsync).MustHaveHappenedANumberOfTimesMatching(n => n == 3);
        }*/

        private V1PodStatus GetPodStatus(string userContainerStatus, string initContainerStatus)
        {
            IList<V1ContainerStatus> userContainerStatuses = new List<V1ContainerStatus>() { GetV1ContainerStatus("userContainer", userContainerStatus) };
            IList<V1ContainerStatus> initContainerStatuses = new List<V1ContainerStatus>() { GetV1ContainerStatus("buildContainer", initContainerStatus) };
            V1PodStatus status = new V1PodStatus(containerStatuses: userContainerStatuses, initContainerStatuses: initContainerStatuses, message: "pod-message");
            return status;
        }

        private V1ContainerStatus GetV1ContainerStatus(string containerName, string containerState)
        {
            bool ready = string.Equals(containerState, "running");
            return new V1ContainerStatus($"{containerName}-imagename", $"{containerName}-imageId", containerName, ready, 0, null, $"{containerName}-ContainerId", state: GetV1ContainerState(containerState));
        }

        private V1ContainerState GetV1ContainerState(string status)
        {
            V1ContainerStateRunning running = new V1ContainerStateRunning(new DateTime(2017, 1, 18));
            V1ContainerStateTerminated terminated = new V1ContainerStateTerminated(255, message: "terminated-message", reason: "terminated-reason");
            V1ContainerStateWaiting waiting = new V1ContainerStateWaiting(message: "waiting-message", reason: "waiting-reason");

            switch (status)
            {
                case "terminated":
                    return new V1ContainerState(terminated: new V1ContainerStateTerminated(255, message: "terminated-message", reason: "terminated-reason"));

                case "waiting":
                    return new V1ContainerState(waiting: new V1ContainerStateWaiting(message: "waiting-message", reason: "waiting-reason"));

                default:
                case "running":
                    return new V1ContainerState(running: new V1ContainerStateRunning(new DateTime(2017, 1, 18)));
            }
        }

        private V1Container GetV1Container(string containerName, int port)
        {
            return new V1Container(containerName, ports: new List<V1ContainerPort>() { new V1ContainerPort(port, hostIP: "10.10.10.10") });
        }
    }
}