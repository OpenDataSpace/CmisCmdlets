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
using System.Collections.Generic;
using DotCMIS;
using DotCMIS.Data.Impl;
using DotCMIS.Enums;

namespace CmisCmdlets
{
    public class CmisNavigation
    {
        private ISession _session;
        private CmisPath _curDir;

        public CmisNavigation(ISession session)
            : this(session, CmisPath.CorrectSlash)
        {
        }

        public CmisNavigation(ISession session, CmisPath path)
        {
            _session = session;
            _curDir = path;
        }

        public IDocument CreateDocument(CmisPath path, ContentStream stream)
        {
            var components = path.GetComponents();
            var name = components[1];
            if (name.Length == 0)
            {
                throw new CmisNameConstraintViolationException("The document name is empty.");
            }
            var folder = GetFolder(components[0]);

            var props = new Dictionary<string, object>()
            {
                { PropertyIds.ObjectTypeId, "cmis:document" },
                { PropertyIds.Name, name }
            };
            return folder.CreateDocument(props, stream, VersioningState.Major);
        }

        public IFolder CreateFolder(CmisPath path, bool recursive)
        {
            path = path.WithoutTrailingSlash();

            if (recursive)
            {
                // check if it already exists and proceed to create otherwise
                try
                {
                    return GetFolder(path);
                }
                catch (CmisConnectionException)
                {
                }
            }

            var components = path.GetComponents();
            var dirname = components[0];
            var basename = components[1];
            IFolder parent = recursive ? CreateFolder(dirname) : GetFolder(dirname);

            var props = new Dictionary<string, object>()
            {
                { PropertyIds.ObjectTypeId, "cmis:folder" },
                { PropertyIds.Name, basename }
            };
            return GetFolder(dirname).CreateFolder(props);
        }

        public ICmisObject Get(CmisPath cmisPath)
        {
            var path = _curDir.Combine(cmisPath).ToString();
            try
            {
                return _session.GetObjectByPath(path);
            }
            catch (CmisObjectNotFoundException)
            {
                ThrowObjectNotFound(path);
            }
        }

        public IDocument GetDocument(CmisPath cmisPath)
        {
            IDocument d = Get(cmisPath) as IDocument;
            if (d == null)
            {
                var path = _curDir.Combine(cmisPath).ToString();
                var msg = String.Format("The object identified by '{0}' is not a document.", path);
                throw new CmisObjectNotFoundException(msg);
            }
            return d;
        }

        public IFolder GetFolder(CmisPath cmisPath)
        {
            IFolder f = Get(cmisPath) as IFolder;
            // we need to check if that object is a folder
            if (f == null)
            {
                var path = _curDir.Combine(cmisPath).ToString();
                var msg = String.Format("The object identified by '{0}' is not a folder.", path);
                throw new CmisObjectNotFoundException(msg);
            }
            return f;
        }

        public void Delete(CmisPath cmisPath, bool allVersions, bool recursive)
        {
            var obj = Get(cmisPath);
            var isfolder = obj is IFolder;
            if (!recursive || !isfolder)
            {
                obj.Delete(allVersions);
                return;
            }
            var folder = obj as IFolder;
            folder.DeleteTree(allVersions, UnfileObject.Delete, false);
        }

        // DotCMIS is not very generous when it comes to generating error messages
        // so we create a better one
        private void ThrowObjectNotFound(string path)
        {
            var msg = String.Format("The path '{0}' doesn't identify a CMIS object.", path);
            throw new CmisObjectNotFoundException(msg);
        }
    }
}

