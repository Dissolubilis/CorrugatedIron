﻿// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Util;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.RiakClientSetBucketPropertiesTests
{
    public class MockCluster : IRiakCluster
    {
        public Mock<IRiakConnection> ConnectionMock = new Mock<IRiakConnection>();

        public void Dispose()
        {
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RiakClientSetBucketPropertiesTestBase
    {
        protected MockCluster Cluster;
        protected RiakClient Client;
        protected byte[] ClientId;

        protected RiakClientSetBucketPropertiesTestBase()
        {
            Cluster = new MockCluster();
            ClientId = System.Text.Encoding.Default.GetBytes("fadjskl").Take(4).ToArray();
            Client = new RiakClient(Cluster) {ClientId = ClientId};
        }
    }

    [TestFixture]
    public class WhenSettingBucketPropertiesWithExtendedProperties : RiakClientSetBucketPropertiesTestBase
    {
        protected RiakResult Response;
        [SetUp]
        public void SetUp()
        {
            var result = RiakResult<RiakRestResponse>.Success(new RiakRestResponse { StatusCode = System.Net.HttpStatusCode.NoContent });
            Cluster.ConnectionMock.Setup(m => m.RestRequest(It.IsAny<RiakRestRequest>())).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true).SetRVal("one"));
        }

        [Test]
        public void RestInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.RestRequest(It.Is<RiakRestRequest>(r => r.ContentType == RiakConstants.ContentTypes.ApplicationJson
                && r.Method == RiakConstants.Rest.HttpMethod.Put)), Times.Once());
        }
    }

    [TestFixture]
    public class WhenSettingBucketPropertiesWithoutExtendedProperties : RiakClientSetBucketPropertiesTestBase
    {
        protected RiakResult Response;
        [SetUp]
        public void SetUp()
        {
            var result = RiakResult<RpbSetBucketResp>.Success(new RpbSetBucketResp());
            Cluster.ConnectionMock.Setup(m => m.PbcWriteRead<RpbSetBucketReq, RpbSetBucketResp>(It.IsAny<RpbSetBucketReq>())).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true));
        }

        [Test]
        public void PbcInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.PbcWriteRead<RpbSetBucketReq, RpbSetBucketResp>(It.Is<RpbSetBucketReq>(r => r.Props.AllowMultiple)), Times.Once());
        }
    }
}
