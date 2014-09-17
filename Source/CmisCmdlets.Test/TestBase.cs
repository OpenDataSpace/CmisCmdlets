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

namespace CmisCmdlets.Test
{
    public class TestBase
    {
        private AppSettingsSection _appSettings;
        protected AppSettingsSection AppSettings
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

        protected string TestUser { get { return AppSettings.Settings["user"].Value; } }

        protected string TestPassword { get { return AppSettings.Settings["password"].Value; } }

        protected string TestURL { get { return AppSettings.Settings["url"].Value; } }

        protected string TestRepository { get { return AppSettings.Settings["repository"].Value; } }
    
        protected TestBase()
        {
            // Should avoid problems with SSL and tests systems without valid certificate
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        
        protected string BuildFeaturedUrl(string rawUrl, string user, string pw)
        {
            var parts = rawUrl.Split(new [] { @"://" }, 2, StringSplitOptions.None);
            return String.Format("{0}://{1}:{2}@{3}", parts[0], user, pw, parts[1]);
        }
    }
}

