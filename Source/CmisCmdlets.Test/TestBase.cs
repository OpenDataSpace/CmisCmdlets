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
        
        public static string CmdletName(Type cmdletType)
        {
            var attribute = System.Attribute.GetCustomAttribute(cmdletType, typeof(CmdletAttribute))
                as CmdletAttribute;
            return string.Format("{0}-{1}", attribute.VerbName, attribute.NounName);
        }
        
        protected string BuildFeaturedUrl(string rawUrl, string user, string pw)
        {
            var parts = rawUrl.Split(new [] { @"://" }, 2, StringSplitOptions.None);
            return String.Format("{0}://{1}:{2}@{3}", parts[0], user, pw, parts[1]);
        }
    }
}

