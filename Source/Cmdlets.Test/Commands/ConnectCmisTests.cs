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
using System.Management.Automation;
using Cmis.Utility;

namespace CmisCmdlets.Test.Commands
{
    [TestFixture]
    public class ConnectCmisTests : CmisTestBase
    {
        [Test]
        public void ConnectCmdletStoresInfo()
        {
            var connectCmd = GetConnectToTestRepoCmd();
            Shell.Execute(connectCmd);
            Assert.NotNull(CmisCommandBase.ConnectionParameters);
        }

        [Test]
        public void ConnectCmdletCanConnectToRepoWithInsecure()
        {
            Shell.Execute(GetConnectToTestRepoCmd(true));
            Assert.That(CmisCommandBase.ConnectionParameters, Is.Not.Null);
            ValidateSession(Shell.GetVariableValue(CmisCommandBase.SESSION_VAR_NAME)
                            , TestRepository);
        }

        [Test]
        public void ConnectCmdletCanConnectToRepo()
        {
            var connectCmd = GetConnectToTestRepoCmd();
            Shell.Execute(connectCmd);
            Assert.That(CmisCommandBase.ConnectionParameters, Is.Not.Null);
            ValidateSession(Shell.GetVariableValue(CmisCommandBase.SESSION_VAR_NAME)
                            , TestRepository);
        }

        [Test, Ignore("Currently not parsed by Pash")]
        public void ConnectCmdletCanConnectToRepoWithParams()
        {
            var parameters = ConnectionFactory.CreateAtomPubParams(TestURL, TestUser, TestPassword);
            var repo = ConnectionFactory.GetRepositoryByName(parameters, TestRepository);
            parameters[SessionParameter.RepositoryId] = repo.Id;
            var objCode = "$p = " + HashtableDefinition(parameters);
            var cmd = String.Format("{0}; {1} -parameters $p; ${2}", objCode,
                                    CmdletName(typeof(ConnectCmisCommand)));
           Shell.Execute(cmd);
            Assert.That(CmisCommandBase.ConnectionParameters, Is.Not.Null);
            ValidateSession(Shell.GetVariableValue(CmisCommandBase.SESSION_VAR_NAME)
                            , TestRepository);
        }

        [Test]
        public void DisconnectCmdletClearsSessionAndParams()
        {
            var cmd = String.Format("{0}; {1}; ${2}", GetConnectToTestRepoCmd(),
                                    CmdletName(typeof(DisconnectCmisCommand)),
                                    CmisCommandBase.SESSION_VAR_NAME);
            var res = Shell.Execute(cmd);
            Assert.Throws<RuntimeException>(delegate { 
                Assert.Null(CmisCommandBase.ConnectionParameters);
            });
            Assert.AreEqual(1, res.Count);
            Assert.Null(res[0]);
        }

    }
}

