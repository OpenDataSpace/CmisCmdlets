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

namespace CmisCmdlets
{
    public class CmisCommandBase : PSCmdlet
    {
        public const string SESSION_VAR_NAME = "_CMIS_SESSION";

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

        private ISession _session;
        public ISession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = GetSessionFromVariable();
                }
                return _session;
            }
        }

        public void SetCmisSession(ISession session)
        {
            _session = session;
            SessionState.PSVariable.Set(SESSION_VAR_NAME, session);
        }

        public ISession GetSessionFromVariable()
        {
            var session = SessionState.PSVariable.Get(SESSION_VAR_NAME).Value as ISession;
            if (session == null)
            {
                throw new RuntimeException("Session variable not set. " +
                                           "Did you forget to connect and set a repository?");
            }
            return session;
        }
    }
}

