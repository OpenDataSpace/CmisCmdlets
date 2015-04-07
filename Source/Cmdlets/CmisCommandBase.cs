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
using System.Management.Automation;
using DotCMIS.Client.Impl;
using System.Collections;
using System.Collections.Generic;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Exceptions;
using Cmis.Utility;

namespace CmisCmdlets
{
    public class CmisCommandBase : PSCmdlet
    {
        public const string SESSION_VAR_NAME = "_CMIS_SESSION";
        public const string DIRECTORY_VAR_NAME = "_CMIS_DIRECTORY";

        private static IDictionary<string, string> _connectionParameters;
        internal static IDictionary<string, string> ConnectionParameters
        {
            set
            {
                _connectionParameters = value;
            }

            get
            {
                if (_connectionParameters == null)
                {
                    throw new RuntimeException("Unknown connection parameters. Please connect first");
                }
                return _connectionParameters;
            }
        }

        private ISession _cmisSession;
        public ISession CmisSession
        {
            get
            {
                if (_cmisSession == null)
                {
                    _cmisSession = GetCmisSession();
                }
                return _cmisSession;
            }
        }

        private CmisPath _workingFolder;
        public CmisPath WorkingFolder
        {
            get
            {
                if (_workingFolder == null)
                {
                    _workingFolder = GetWorkingFolder();
                }
                return _workingFolder;
            }
        }

        protected void SetWorkingFolder(CmisPath path)
        {
            _workingFolder = path;
            SessionState.PSVariable.Set(DIRECTORY_VAR_NAME, path.ToString());
        }

        protected void SetCmisSession(ISession session)
        {
            _cmisSession = session;
            SessionState.PSVariable.Set(SESSION_VAR_NAME, _cmisSession);
            SetWorkingFolder(_cmisSession == null ? "" : "/"); // reset the directory to root
        }

        private ISession GetCmisSession()
        {
            var variable = SessionState.PSVariable.Get(SESSION_VAR_NAME);
            var session = variable == null ? null : variable.Value as ISession;
            ValidateSession(session);
            return session;
        }

        private void ValidateSession(ISession session)
        {
            if (session == null)
            {
                throw new RuntimeException("Session variable not set. " +
                                           "Did you forget to connect and set a repository?");
            }
        }

        private CmisPath GetWorkingFolder()
        {
            ValidateSession(CmisSession); // just make sure we're connected
            var variable = SessionState.PSVariable.Get(DIRECTORY_VAR_NAME);
            var dir = variable == null ? "" : variable.Value as string;
            return dir == null ? "" : dir;
        }
    }
}

