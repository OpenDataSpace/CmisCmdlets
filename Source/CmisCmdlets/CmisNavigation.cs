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
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Exceptions;

namespace CmisCmdlets
{
    public class CmisNavigation
    {
        private ISession _session;

        public CmisNavigation(ISession session)
        {
            _session = session;
        }

        public Folder GetFolder(string path)
        {
            Folder f = null;
            try
            {
                f = _session.GetObjectByPath(path) as Folder;
            }
            catch (CmisObjectNotFoundException)
            {
                // DotCMIS is not very generous when it comes to generating error messages
                // so we create a better one
                var msg = String.Format("The path '{0}' doesn't identify a CMIS object.", path);
                throw new CmisObjectNotFoundException(msg);
            }
            // no we still need to check if that object is a folder
            if (f == null)
            {
                var msg = String.Format("The object identified by '{0}' is not a folder.", path);
                throw new CmisObjectNotFoundException(msg);
            }
            return f;
        }
    }
}

