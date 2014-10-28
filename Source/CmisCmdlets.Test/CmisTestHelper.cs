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
using System.Collections.Generic;
using DotCMIS.Exceptions;
using DotCMIS.Data.Impl;
using NUnit.Framework.Constraints;

namespace CmisCmdlets.Test
{
    public class CmisObjectExistsConstraint : Constraint
    {
        private ISession _session;
        private bool _shouldExist;

        public CmisObjectExistsConstraint(ISession session, bool shouldExist)
        {
            _session = session;
            _shouldExist = shouldExist;
        }

        public override bool Matches(object actual)
        {
            _session.Clear();
            try
            {
                return _session.GetObjectByPath((string)actual) != null ? _shouldExist : !_shouldExist;
            }
            catch(CmisBaseException)
            {
                return !_shouldExist;
            }
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteLine("Checks if the CMIS object designated by a given path exists");
        }
    }

    public class CmisTestHelper
    {
        private CmisNavigation _cmisNav;
        private List<ICmisObject> _createdObjects;
        private ISession _session;
        
        public CmisObjectExistsConstraint Exists
        {
            get { return new CmisObjectExistsConstraint(_session, true); }
        }

        public CmisObjectExistsConstraint DoesNotExist
        {
            get { return new CmisObjectExistsConstraint(_session, true); }
        }

        public CmisTestHelper(ISession session)
        {
            _session = session;
            _createdObjects = new List<ICmisObject>();
            _cmisNav = new CmisNavigation(session);
        }

        public void CleanUp()
        {
            // delete in reverse order makes sure to delete hierarchies correctly
            for (int i = _createdObjects.Count -1; i >= 0; i--)
            {
                try
                {
                    _createdObjects[i].Delete(true);
                }
                catch (CmisObjectNotFoundException) {}
            }
            _createdObjects.Clear();
        }

        public void RegisterTempObject(params ICmisObject[] objs)
        {
            _createdObjects.AddRange(objs);
        }

        public void ForgetTempObjects()
        {
            _createdObjects.Clear();
        }

        public ICmisObject Get(CmisPath path)
        {
			return _cmisNav.Get(path);
        }

        public IDocument CreateTempDocument(CmisPath path, ContentStream stream)
        {
            return CreateTempDocument(path, stream, null);
        }

        public IDocument CreateTempDocument(CmisPath path, ContentStream stream,
                                        IDictionary<string, object> properties) 
        {
            var doc = _cmisNav.CreateDocument(path, stream, properties);
            _createdObjects.Add(doc);
            return doc;
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

