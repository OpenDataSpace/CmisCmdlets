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
using Cmis.Utility;
using PSTesting;

namespace CmisCmdlets.Test
{
    public class CmisTestBase : TestBase
    {
        public string TestUser { get { return AppSettings.Settings["user"].Value; } }

        public string TestPassword { get { return AppSettings.Settings["password"].Value; } }

        public string TestURL { get { return AppSettings.Settings["url"].Value; } }

        public string TestRepository { get { return AppSettings.Settings["repository"].Value; } }

        public string TestRepositoryAlt { get { return AppSettings.Settings["repository_alt"].Value; } }
    
        private ISession _cmisSession;
        public ISession CmisSession
        {
            get
            {
                if (_cmisSession == null)
                {
                    _cmisSession = ConnectionFactory.ConnectAtomPub(TestURL, TestUser, TestPassword,
                                                                    TestRepository);
                }
                return _cmisSession;
            }

            set
            {
                _cmisSession = value;
            }
        }

        private CmisTestHelper _cmisHelper;
        public CmisTestHelper CmisHelper
        {
            get
            {
                if (_cmisHelper == null)
                {
                    _cmisHelper = new CmisTestHelper(CmisSession);
                    RegisterHelper(_cmisHelper);
                }
                return _cmisHelper;
            }
        }

        protected CmisTestBase() : base(typeof(ConnectCmisCommand), "TestConfig.config", true)
        {
        }

        protected string GetConnectToTestRepoCmd(bool insecure = false)
        {
            return String.Format("{0} -url '{1}' -user '{2}' -password '{3}' -repo '{4}' {5};",
                                 CmdletName(typeof(ConnectCmisCommand)), TestURL, TestUser,
                                 TestPassword, TestRepository, insecure ? "-Insecure" : "");
        }

        protected string BuildFeaturedUrl(string rawUrl, string user, string pw)
        {
            var parts = rawUrl.Split(new [] { @"://" }, 2, StringSplitOptions.None);
            return String.Format("{0}://{1}:{2}@{3}", parts[0], user, pw, parts[1]);
        }

        protected void ValidateSession(object sessionObject, string repoName)
        {
            var session = sessionObject as ISession;
            Assert.NotNull(session);
            Assert.AreEqual(repoName, session.RepositoryInfo.Name);
        }
    }
}

