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
using DotCMIS.Client;
using System.Collections.Generic;
using DotCMIS.Exceptions;
using DotCMIS.Data.Impl;
using NUnit.Framework.Constraints;
using System.Text;
using System.IO;
using Cmis.Utility;
using PSTesting;
using CmisUtility.Test.Constraints;

namespace CmisUtility.Test
{
    public class CmisTestHelper : ITestHelper
    {
        private CmisNavigation _cmisNav;
        private List<object> _createdObjects;
        private ISession _session;

#region Constraint helpers
        public CmisObjectExistsConstraint Exists
        {
            get { return new CmisObjectExistsConstraint(_session, true); }
        }

        public CmisObjectExistsConstraint DoesNotExist
        {
            get { return new CmisObjectExistsConstraint(_session, false); }
        }

        
        public CmisObjectContentConstraint HasContent(string expected, string mimeType)
        {
            return HasContent(Encoding.UTF8.GetBytes(expected), mimeType);
        }

        public CmisObjectContentConstraint HasContent(byte[] content, string mimeType)
        {
            return new CmisObjectContentConstraint(content, mimeType);
        }

        public Constraint IsEqualObject(ICmisObject expected)
        {
            return new CmisObjectEqualityConstraint(expected);
        }

        public Constraint ContainsObject(ICmisObject expected)
        {
            return new CmisCollectionContainsObjectConstraint(expected);
        }

        public Constraint HasProperty(string propName, object propValue)
        {
            return new CmisObjectHasPropertyConstraint(propName, propValue);
        }
#endregion

        public CmisTestHelper(ISession session)
        {
            _session = session;
            _createdObjects = new List<object>();
            _cmisNav = new CmisNavigation(session);
        }

        public void SetUp()
        {
            // Nothing to do here
        }

        public void TearDown()
        {
            // delete in reverse order makes sure to delete hierarchies correctly
            for (int i = _createdObjects.Count -1; i >= 0; i--)
            {
                var tmpObj = _createdObjects[i];
                var obj = tmpObj as ICmisObject;
                if (obj == null)
                {
                    if (!_cmisNav.TryGet(tmpObj.ToString(), out obj))
                    {
                        continue;
                    }
                }
                try
                {
                    obj.Delete(true);
                }
                catch (CmisObjectNotFoundException) {}
            }
            _createdObjects.Clear();
        }

        public void RegisterTempObject(params ICmisObject[] objs)
        {
            _createdObjects.AddRange(objs);
        }

        public void RegisterTempObject(params string[] paths)
        {
            _createdObjects.AddRange(paths);
        }

        public void ForgetTempObjects()
        {
            _createdObjects.Clear();
        }

        public ICmisObject Get(CmisPath path)
        {
			return _cmisNav.Get(path);
        }

        public IDocument CreateTempDocument(CmisPath path)
        {
            return CreateTempDocument(path, null);
        }

        public IDocument CreateTempDocument(CmisPath path, ContentStream stream)
        {
            return CreateTempDocument(path, stream, null);
        }

        public IDocument CreateTempDocument(CmisPath path, string content, string mimeType) 
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var contentStream = new ContentStream();
            contentStream.MimeType = mimeType;
            contentStream.Length = bytes.Length;
            using (var memoryStream = new MemoryStream(bytes))
            {
                contentStream.Stream = memoryStream;
                return CreateTempDocument(path, contentStream, null);
            }
        }

        public IDocument CreateTempDocument(CmisPath path, ContentStream stream,
                                            IDictionary<string, object> properties) 
        {
            var doc = _cmisNav.CreateDocument(path, stream, properties);
            _createdObjects.Add(doc);
            return doc;
        }

        public IFolder CreateTempFolder(CmisPath path)
        {
            return CreateTempFolder(path, false);
        }

        public IFolder CreateTempFolder(CmisPath path, bool recursive)
        {
            return CreateTempFolder(path, recursive, null);
        }

        public IFolder CreateTempFolder(CmisPath path, bool recursive,
                                    IDictionary<string, object> properties) 
        {
            path = path.WithoutTrailingSlash();
            if (recursive)
            {
                try
                {
                    return _cmisNav.GetFolder(path);
                }
                catch (CmisBaseException) {}
            }
            var comps = path.GetComponents();
            if (recursive)
            {
                CreateTempFolder(comps[0], true, null);
            }
            var folder = _cmisNav.CreateFolder(path, false, properties);
            _createdObjects.Add(folder);
            return folder;
        }
    }
}

