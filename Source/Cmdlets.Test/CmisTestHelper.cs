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

namespace CmisCmdlets.Test
{
    public abstract class CmisBaseConstraint : Constraint
    {
        protected string ActualValue { get; set; }
        protected string ExpectedValue { get; set; }

        
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteLine(ExpectedValue);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine(ActualValue);
        }

        protected bool Problem(string format, object expected, object actual)
        {
            return Problem(String.Format(format, expected), String.Format(format, actual));
        }

        protected bool Problem(string expected, string actual)
        {
            ExpectedValue = expected.ToString();
            ActualValue = actual.ToString();
            return false;
        }

        protected bool MatchObject(ICmisObject expected, ICmisObject actual)
        {
            if (actual == null)
            {
                return Problem("An ICmisObject", "Something else");
            }
            if (expected.BaseTypeId != actual.BaseTypeId)
            {
                return Problem("Object with BaseTypeId {0}", expected.BaseTypeId,
                               actual.BaseTypeId);
            }
            if (expected.Id != actual.Id)
            {
                return Problem("Object with Id \"{0}\"", expected.BaseTypeId,
                               actual.BaseTypeId);
            }
            /*
             * This would have to be done by hand. Skipping for now
            if (!expected.Properties.SequenceEqual(actual.Properties))
            {
                return Problem("Object with Properties \"{0}\"", expected.Properties,
                               actual.Properties);
            }
            */
            if (expected is IFolder)
            {
                return MatchFolder(expected as IFolder, actual as IFolder);
            }
            else if (expected is IDocument)
            {
                return MatchDocument(expected as IDocument, actual as IDocument);
            }
            return true;
        }

        protected bool MatchFolder(IFolder expected, IFolder actual)
        {
            if (actual == null)
            {
                return Problem("An IFolder object", "Something else");
            }
            if (!expected.Path.Equals(actual.Path))
            {
                return Problem("IFolder with Path {0}", expected.Path, actual.Path);
            }
            return true;
        }

        protected bool MatchDocument(IDocument expected, IDocument actual)
        {
            if (actual == null)
            {
                return Problem("An IDocument object", "Something else");
            }
            if (!expected.Paths.SequenceEqual(actual.Paths))
            {
                return Problem("IDocument with Paths {0}", expected.Paths, actual.Paths);
            }
            return true;
        }
    }

    public class CmisObjectExistsConstraint : CmisBaseConstraint
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
            ExpectedValue = "Object should" + (_shouldExist ? "" : " not") + " exist.";
            ActualValue = "Object does" + (_shouldExist ? " not" : "") + " exist.";

            _session.Clear();
            try
            {
                var path = (string)actual;
                path = path.StartsWith("/") ? path : "/" + path;
                return _session.GetObjectByPath(path) != null ? _shouldExist : !_shouldExist;
            }
            catch(CmisBaseException)
            {
                return !_shouldExist;
            }
        }
    }

    public class CmisObjectContentConstraint : CmisBaseConstraint
    {
        private byte[] _content;
        private string _mimetype;

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
                return Problem("An existing IDocument", "Object which is not an IDocument");
            }
            if (!doc.ContentStreamMimeType.Equals(_mimetype))
            {
                return Problem("Content with MimeType \"{0}\"", _mimetype,
                               doc.ContentStreamMimeType);
            }
            var contentStream = doc.GetContentStream();
            if (!contentStream.MimeType.Equals(_mimetype))
            {
                return Problem("ContentStream with MimeType \"{0}\"", _mimetype,
                               contentStream.MimeType);
            }
            if (doc.ContentStreamLength != _content.Length)
            {
                return Problem("Content of length {0}", _content.Length, doc.ContentStreamLength);
            }
            var buffer = new byte[_content.Length];
            contentStream.Stream.Read(buffer, 0, _content.Length);
            contentStream.Stream.Close();
            if (!_content.SequenceEqual(buffer))
            {
                return Problem("Content: \"{0}\"", Encoding.UTF8.GetString(_content),
                               Encoding.UTF8.GetString(buffer));
            }
            return true;
        }
    }

    public class CmisObjectEqualityConstraint : CmisBaseConstraint
    {
        private ICmisObject _object;

        public CmisObjectEqualityConstraint(ICmisObject obj)
        {
            _object = obj;
        }

        public override bool Matches(object actual)
        {
            return MatchObject(_object, actual as ICmisObject);
        }
    }

    public class CmisObjectHasPropertyConstraint : CmisBaseConstraint
    {
        private string _propertyName;
        private object _propertyValue;

        public CmisObjectHasPropertyConstraint(string propertyName, object propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public override bool Matches(object obj)
        {
            var cmisObj = obj as ICmisObject;
            if (cmisObj == null)
            {
                return Problem("A cmis object", "Something else");
            }
            foreach (var prop in cmisObj.Properties)
            {
                if (!prop.LocalName.Equals(_propertyName))
                {
                    continue;
                }
                if (!prop.Value.Equals(_propertyValue))
                {
                    return Problem("Object with property value {0}", _propertyValue, prop.Value);
                }
                return true;
            }
            return Problem("Object with the specified property", "Object without that property");
        }
    }

    public class CmisCollectionContainsObjectConstraint : CmisBaseConstraint
    {
        private ICmisObject _object;

        public CmisCollectionContainsObjectConstraint(ICmisObject obj)
        {
            _object = obj;
        }

        public override bool Matches(object actual)
        {
            var enumerable = actual as IEnumerable<object>;
            if (enumerable == null)
            {
                return Problem("An IEnumerable<object>", "Something else");
            }
            foreach (var obj in enumerable)
            {
                if (MatchObject(_object, obj as ICmisObject))
                {
                    return true;
                }
            }
            return Problem("An IEnumerable with " + _object.ToString(), "An IEnumerable without it");
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

