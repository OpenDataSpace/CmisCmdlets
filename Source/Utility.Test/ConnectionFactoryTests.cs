// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using NUnit.Framework;
using System;
using System.Linq;
using Cmis.Utility;

namespace CmisUtility.Test
{
    [TestFixture]
    public class ConnectionFactoryTests : CmisTestBase
    {
 
        [Test]
        public void GetRepositories()
        {
            var repos = ConnectionFactory.GetAtomPubRepositories(TestURL, TestUser, TestPassword);
            Assert.NotNull(repos);
            var repoNames = from rep in repos select rep.Name;
            Assert.That(repoNames, Contains.Item(TestRepository));
        }

        [Test]
        public void GetRepositoriesWithFeaturedUrl()
        {
            var url = BuildFeaturedUrl(TestURL, TestUser, TestPassword);
            var repos = ConnectionFactory.GetAtomPubRepositories(url);
            Assert.NotNull(repos);
            var repoNames = from rep in repos select rep.Name;
            Assert.That(repoNames, Contains.Item(TestRepository));
        }

        [Test]
        public void ConnectWithRepoId()
        {
            var parameters = ConnectionFactory.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = ConnectionFactory.GetRepositoryByName(parameters, TestRepository);
            var session = ConnectionFactory.ConnectAtomPubById(TestURL, TestUser, TestPassword,
                                                         repo.Id);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }

        [Test]
        public void ConnectWithRepoName()
        {
            var session = ConnectionFactory.ConnectAtomPub(TestURL, TestUser, TestPassword,
                                                         TestRepository);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }

        [Test]
        public void ConnectWithFeaturedURL()
        {
            var url = BuildFeaturedUrl(TestURL, TestUser, TestPassword);
            var session = ConnectionFactory.ConnectAtomPub(url, TestRepository);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }
    }
}

