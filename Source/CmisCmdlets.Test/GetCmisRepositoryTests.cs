// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using System;
using System.Linq;
using NUnit.Framework;
using CmisCmdlets;
using DotCMIS.Client;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class GetCmisRepositoryTests : TestBase
    {
        [Test]
        public void GetRepositoriesAtomPubWorks()
        {
            CmisCommandBase.ConnectionParameters = ConnectionFactory.CreateAtomPubParams(TestURL,
                                                                           TestUser, TestPassword);
            var results = Shell.Execute(CmdletName(typeof(GetCmisRepositoryCommand)));
            Assert.NotNull(results);
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public void GetRepositoriesAtomPubWorksWithName()
        {
            CmisCommandBase.ConnectionParameters = ConnectionFactory.CreateAtomPubParams(TestURL,
                                                                            TestUser, TestPassword);
            var cmd = String.Format("{0} -name {1}", CmdletName(typeof(GetCmisRepositoryCommand)),
                                    TestRepository.Substring(0,1));
            var results = Shell.Execute(cmd);
            Assert.NotNull(results);
            var reponames = from res in results select ((IRepository)res).Name;
            Assert.That(reponames, Contains.Item(TestRepository));
        }

        [Test]
        public void GetRepositoriesAtomPubWorksAfterConnectCmdlet()
        {
            var cmd = String.Format("{0} -url {1} -user {2} -password {3}; {4} -name {5}",
                                    CmdletName(typeof(ConnectCmisCommand)), TestURL, TestUser,
                                    TestPassword, CmdletName(typeof(GetCmisRepositoryCommand)),
                                    TestRepository);
            var results = Shell.Execute(cmd);
            Assert.NotNull(results);
            var reponames = from res in results select ((IRepository)res).Name;
            Assert.That(reponames, Contains.Item(TestRepository));
        }
    }
}

