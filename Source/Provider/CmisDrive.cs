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
using System.Management.Automation;
using Cmis.Utility;
using System.Security;
using System.Runtime.InteropServices;
using DotCMIS.Client;

namespace CmisProvider
{
    public class CmisDrive : PSDriveInfo
    {
        public string Repository { get; private set; }
        public ISession Connection { get; private set; }
        public CmisNavigation Navigation { get; private set; }

        public CmisDrive(PSDriveInfo drive, CmisDriveParameters driveParameters) : base(drive)
        {
            if (driveParameters == null || String.IsNullOrEmpty(driveParameters.Host))
            {
                throw new ArgumentException("No host was specified");
            }

            var unsecurePassword = ConvertToUnsecureString(Credential.Password);
            var connectionParams = ConnectionFactory.CreateAtomPubParams(driveParameters.Host,
                Credential.UserName, unsecurePassword);
            var Repository = Root.Split(new [] { ":" }, 2, StringSplitOptions.None)[0];

            Connection = ConnectionFactory.Connect(connectionParams, Repository);
            Navigation = new CmisNavigation(Connection, NormalizePath(Root));
        }

        public string NormalizePath(string path)
        {
            if (path.StartsWith(Repository + ":"))
            {
                path = path.Substring(Repository.Length + 1);
            }
            if (path.Length == 0)
            {
                path = "/";
            }
            return path;
        }

        private static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}

