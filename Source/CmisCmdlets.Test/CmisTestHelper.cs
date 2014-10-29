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
            writer.WriteLine("Object should" + (_shouldExist ? "" : " not") + " exist.");
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine("Object does" + (_shouldExist ? " not" : "") + " exist.");
        }
    }

    public class CmisObjectContentConstraint : Constraint
    {
        private byte[] _content;
        private string _mimetype;
        private string _actual;
        private string _expected;

        public CmisObjectContentConstraint(byte[] content, string mimetype)
        {
            _content = content;
            _mimetype = mimetype;
        }

        public override bool Matches(object actual)
        {
            var doc = actual as IDocument;
            if (doc == null)
            {
                _expected = "An existing IDocument";
                _actual = "Object which is not an> IDocument";
                return false;
            }
            if (doc.ContentStreamLength != _content.Length)
            {
                _actual = "Content of length " + doc.ContentStreamLength.ToString();
                _expected = "Content of length " + _content.Length.ToString();
                return false;
            }
            if (!doc.ContentStreamMimeType.Equals(_mimetype))
            {
                _actual = "Content with MimeType \"" + doc.ContentStreamMimeType + "\"";
                _expected = "Content with MimeType \"" + _mimetype + "\"";
                return false;
            }
            var contentStream = doc.GetContentStream();
            if (!contentStream.MimeType.Equals(_mimetype))
            {
                _actual = "ContentStream with MimeType \"" + contentStream.MimeType + "\"";
                _expected = "ContentStream with MimeType \"" + _mimetype + "\"";
                return false;
            }
            var buffer = new byte[_content.Length];
            contentStream.Stream.Read(buffer, 0, _content.Length);
            _actual = "Content: \"" + Encoding.UTF8.GetString(_content) + "\""; 
            _expected = "Content: \"" + Encoding.UTF8.GetString(buffer) + "\"";
            return _content.SequenceEqual(buffer);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteLine(_expected);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine(_actual);
        }
    }

    public class CmisTestHelper
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
#endregion

        public CmisTestHelper(ISession session)
        {
            _session = session;
            _createdObjects = new List<object>();
            _cmisNav = new CmisNavigation(session);
        }

        public void CleanUp()
        {
            // delete in reverse order makes sure to delete hierarchies correctly
            for (int i = _createdObjects.Count -1; i >= 0; i--)
            {
                var tmpObj = _createdObjects[i];
                ICmisObject obj = tmpObj as ICmisObject;
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

