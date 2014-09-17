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
using System.Configuration;
using DotCMIS;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class ConnectionTests : TestBase
    {
                
        [Test]
        public void GetRepositories()
        {
            var repos = CmisCommandBase.GetAtomPubRepositories(TestURL, TestUser, TestPassword);
            Assert.NotNull(repos);
            var repoNames = from rep in repos select rep.Name;
            Assert.That(repoNames, Contains.Item(TestRepository));
        }

        [Test]
        public void GetRepositoriesWithFeaturedUrl()
        {
            var url = BuildFeaturedUrl(TestURL, TestUser, TestPassword);
            var repos = CmisCommandBase.GetAtomPubRepositories(url);
            Assert.NotNull(repos);
            var repoNames = from rep in repos select rep.Name;
            Assert.That(repoNames, Contains.Item(TestRepository));
        }

        [Test]
        public void ConnectWithRepoId()
        {
            var parameters = CmisCommandBase.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = CmisCommandBase.GetRepositoryByName(parameters, TestRepository);
            var session = CmisCommandBase.ConnectAtomPubById(TestURL, TestUser, TestPassword,
                                                         repo.Id);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }

        [Test]
        public void ConnectWithRepoName()
        {
            var session = CmisCommandBase.ConnectAtomPub(TestURL, TestUser, TestPassword,
                                                         TestRepository);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }

        [Test]
        public void ConnectWithFeaturedURL()
        {
            var url = BuildFeaturedUrl(TestURL, TestUser, TestPassword);
            var session = CmisCommandBase.ConnectAtomPub(url, TestRepository);
            Assert.NotNull(session);
            Assert.NotNull(session.RepositoryInfo);
        }

        private string BuildFeaturedUrl(string rawUrl, string user, string pw)
        {
            var parts = rawUrl.Split(new [] { @"://" }, 2, StringSplitOptions.None);
            return String.Format("{0}://{1}:{2}@{3}", parts[0], user, pw, parts[1]);
        }
    }
}

