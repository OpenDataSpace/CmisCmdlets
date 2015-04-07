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

namespace Cmis.Utility
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
            return CreateDocument(path, stream, null);
        }

        public IDocument CreateDocument(CmisPath path, ContentStream stream,
                                        IDictionary<string, object> properties)
        {
            var components = path.GetComponents();
            var name = components[1];
            if (name.Length == 0)
            {
                throw new CmisNameConstraintViolationException("The document name is empty.");
            }
            var folder = GetFolder(components[0]);

            var allProps = new Dictionary<string, object>()
            {
                { PropertyIds.ObjectTypeId, "cmis:document" },
                { PropertyIds.Name, name }
            };
            Utilities.UpdateDictionary(allProps, properties);
            return folder.CreateDocument(allProps, stream, VersioningState.Major);
        }

        public IFolder CreateFolder(CmisPath path, bool recursive)
        {
            return CreateFolder(path, recursive, null);
        }

        public IFolder CreateFolder(CmisPath path, bool recursive,
                                    IDictionary<string, object> properties)
        {
            path = path.WithoutTrailingSlash();

            if (recursive)
            {
                // check if it already exists and proceed to create otherwise
                try
                {
                    return GetFolder(path);
                }
                catch (CmisObjectNotFoundException)
                {
                }
            }

            var components = path.GetComponents();
            var dirname = components[0];
            var basename = components[1];
            IFolder parent = recursive ? CreateFolder(dirname, true, null) : GetFolder(dirname);

            var allProps = new Dictionary<string, object>()
            {
                { PropertyIds.ObjectTypeId, "cmis:folder" },
                { PropertyIds.Name, basename }
            };
            Utilities.UpdateDictionary(allProps, properties);
            return parent.CreateFolder(allProps);
        }

        public bool TryGet(CmisPath cmisPath, out ICmisObject obj)
        {
            var path = _curDir.Combine(cmisPath).ToString();
            obj = null;
            try
            {
                obj = _session.GetObjectByPath(path);
            }
            catch (CmisObjectNotFoundException)
            {
                return false;
            }
            return true;
        }

        public ICmisObject Get(CmisPath cmisPath)
        {
            ICmisObject obj;
            if (!TryGet(cmisPath, out obj))
            {
                // DotCMIS is not very generous when it comes to generating error messages
                // so we create a better one
                var msg = String.Format("The path '{0}' doesn't identify a CMIS object.",
                                        cmisPath.ToString());
                throw new CmisObjectNotFoundException(msg);
            }
            return obj;
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

        public IList<string> Delete(CmisPath cmisPath, bool recursive)
        {
            ICmisObject obj;
            if (!TryGet(cmisPath, out obj))
            {
                // fail otherwise
                return new string[] { _curDir.Combine(cmisPath).ToString() };
            }
            return Delete(obj, recursive);
        }

        public IList<string> Delete(ICmisObject obj, bool recursive)
        {
            if (recursive && obj is IFolder)
            {
                return ((IFolder)obj).DeleteTree(false, UnfileObject.Delete, true);
            }
            try
            {
                obj.Delete(false);
            }
            catch (CmisBaseException)
            {
                return new string[] { GetCmisObjectPath(obj) };
            }
            return null;
        }

        private string GetCmisObjectPath(ICmisObject obj)
        {
            if (obj is IFolder)
            {
                return ((IFolder)obj).Path;
            }
            else if (obj is IDocument)
            {
                return ((IDocument)obj).Paths[0];
            }
            return "unknown";
        }
    }
}

