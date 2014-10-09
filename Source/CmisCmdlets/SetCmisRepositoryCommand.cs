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
using DotCMIS.Client;
using DotCMIS;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Set, "CmisRepository", DefaultParameterSetName = "Name")]
    public class SetCmisRepositoryCommand : CmisCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true,
                   ParameterSetName = "Name", Position = 0)]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true,
                   ParameterSetName = "Id", Position = 0)]
        public string Id { get; set; }

        protected override void EndProcessing()
        {
            ISession session = null;
            if (!String.IsNullOrEmpty(Name))
            {
                session = ConnectionFactory.Connect(ConnectionParameters, Name);
            }
            else
            {
                ConnectionParameters[SessionParameter.RepositoryId] = Id;
                session = ConnectionFactory.Connect(ConnectionParameters);
            }
            SetCmisSession(session);
        }

    }
}

