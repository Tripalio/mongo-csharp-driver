﻿/* Copyright 2010-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;

namespace MongoDB.Driver.Tests.Communication.Security
{
    [TestFixture]
    [Explicit]
    [Category("Authentication")]
    [Category("GssapiMechanism")]
    public class GssapiAuthenticationTests
    {
        private static readonly string __collectionName = "test";

        private MongoClientSettings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = Configuration.TestClient.Settings.Clone();
        }

        [Test]
        public void TestNoCredentials()
        {
            _settings.Credentials = Enumerable.Empty<MongoCredential>();
            var client = new MongoClient(_settings);

            Assert.Throws<MongoQueryException>(() =>
            {
#pragma warning disable 618
                client.GetServer()
#pragma warning restore
                    .GetDatabase(Configuration.TestDatabase.Name)
                    .GetCollection(__collectionName)
                    .FindOne();
            });
        }

        [Test]
        public void TestSuccessfulAuthentication()
        {
            var client = new MongoClient(_settings);

#pragma warning disable 618
            var result = client.GetServer()
#pragma warning restore
                .GetDatabase(Configuration.TestDatabase.Name)
                .GetCollection(__collectionName)
                .FindOne();

            Assert.IsNotNull(result);
        }

        [Test]
        public void TestBadPassword()
        {
            var currentCredentialUsername = _settings.Credentials.Single().Username;
            _settings.Credentials = new[] 
            {
                MongoCredential.CreateGssapiCredential(currentCredentialUsername, "wrongPassword")
            };

            var client = new MongoClient(_settings);

            Assert.Throws<TimeoutException>(() =>
            {
#pragma warning disable 618
                client.GetServer()
#pragma warning restore
                    .GetDatabase(Configuration.TestDatabase.Name)
                    .GetCollection(__collectionName)
                    .FindOne();
            });
        }
    }
}