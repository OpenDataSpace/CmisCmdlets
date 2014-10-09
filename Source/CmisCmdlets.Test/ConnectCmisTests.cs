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
using NUnit.Framework;
using DotCMIS.Client;
using DotCMIS;

namespace CmisCmdlets.Test
{
    [TestFixture]
    public class ConnectCmisTests : TestBase
    {
        [Test]
        public void ConnectCmdletStoresInfo()
        {
            var cmd = String.Format("{0} -url {1} -user {2} -password {3}",
                                    CmdletName(typeof(ConnectCmisCommand)), TestURL, TestUser,
                                    TestPassword);
            Shell.Execute(cmd);
            Assert.NotNull(CmisCommandBase.ConnectionParameters);
        }

        [Test]
        public void ConnectCmdletCanConnectToRepo()
        {
            var cmd = String.Format("{0}; ${1}", GetConnectToTestRepoCmd(),
                                    CmisCommandBase.SESSION_VAR_NAME);
            var res = Shell.Execute(cmd);
            Assert.NotNull(CmisCommandBase.ConnectionParameters);
            Assert.AreEqual(1, res.Count);
            var session = res[0] as ISession;
            Assert.NotNull(session);
            Assert.AreEqual(TestRepository, session.RepositoryInfo.Name);
        }

        [Test, Ignore("Currently not parsed by Pash")]
        public void ConnectCmdletCanConnectToRepoWithParams()
        {
            var parameters = ConnectionFactory.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = ConnectionFactory.GetRepositoryByName(parameters, TestRepository);
            parameters[SessionParameter.RepositoryId] = repo.Id;
            var objCode = GetCodeForHashtableDefinition("p", parameters);
            var cmd = String.Format("{0}; {1} -parameters $p; ${2}", objCode,
                                    CmdletName(typeof(ConnectCmisCommand)), 
                                    CmisCommandBase.SESSION_VAR_NAME);
            var res = Shell.Execute(cmd);
            Assert.NotNull(CmisCommandBase.ConnectionParameters);
            Assert.AreEqual(1, res.Count);
            var session = res[0] as ISession;
            Assert.NotNull(session);
            Assert.AreEqual(TestRepository, session.RepositoryInfo.Name);
        }

    }
}

