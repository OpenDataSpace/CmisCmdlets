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
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using TestShell;
using System.Management.Automation;
using System.Text;
using System.Collections.ObjectModel;
using NUnit.Framework;
using DotCMIS.Client;

namespace CmisCmdlets.Test
{
    public class TestBase
    {
        private AppSettingsSection _appSettings;
        public AppSettingsSection AppSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                    configMap.ExeConfigFilename = @"TestConfig.config";
                    _appSettings = ConfigurationManager.OpenMappedExeConfiguration(configMap,
                                        ConfigurationUserLevel.None).AppSettings;
                }
                return _appSettings;
            }
        }

        public string TestUser { get { return AppSettings.Settings["user"].Value; } }

        public string TestPassword { get { return AppSettings.Settings["password"].Value; } }

        public string TestURL { get { return AppSettings.Settings["url"].Value; } }

        public string TestRepository { get { return AppSettings.Settings["repository"].Value; } }

        public string TestRepositoryAlt { get { return AppSettings.Settings["repository_alt"].Value; } }
    
        private TestShellInterface _shell;
        public TestShellInterface Shell
        {
            get
            {
                if (_shell == null)
                {
                    _shell = new TestShellInterface();
                }
                return _shell;
            }
        }

        protected TestBase()
        {
            // Should avoid problems with SSL and tests systems without valid certificate
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        protected string GetConnectToTestRepoCmd(bool insecure = false)
        {
            return String.Format("{0} -url {1} -user {2} -password {3} -repo {4} {5};",
                                 CmdletName(typeof(ConnectCmisCommand)), TestURL, TestUser,
                                 TestPassword, TestRepository, insecure ? "-Insecure" : "");
        }

        public static string NewlineJoin(params string[] strs)
        {
            return String.Join(Environment.NewLine, strs);
        }
        
        public static string CmdletName(Type cmdletType)
        {
            var attribute = System.Attribute.GetCustomAttribute(cmdletType, typeof(CmdletAttribute))
                as CmdletAttribute;
            return string.Format("{0}-{1}", attribute.VerbName, attribute.NounName);
        }

        public static string GetCodeForHashtableDefinition(string varname,
                                                           IDictionary<string, string> hashtable)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("${0} = @{{", varname);
            foreach (var pair in hashtable)
            {
                sb.AppendFormat(" {0} = \"{1}\";", pair.Key, pair.Value);
            }
            sb.Remove(sb.Length - 1, 1); // remove last ;
            sb.Append("}; ");
            return sb.ToString();
        }
        
        protected string BuildFeaturedUrl(string rawUrl, string user, string pw)
        {
            var parts = rawUrl.Split(new [] { @"://" }, 2, StringSplitOptions.None);
            return String.Format("{0}://{1}:{2}@{3}", parts[0], user, pw, parts[1]);
        }

        protected void ValidateSession(Collection<object> results, string repoName)
        {
            Assert.AreEqual(1, results.Count);
            ValidateSession(results[0] as ISession, repoName);
        }

        protected void ValidateSession(ISession session, string repoName)
        {
            Assert.NotNull(session);
            Assert.AreEqual(repoName, session.RepositoryInfo.Name);
        }
    }
}

