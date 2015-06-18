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
using DotCMIS.Exceptions;
using System.Linq;
using System.Text;

namespace CmisCmdlets.Tests.Constraints
{
    public class CmisObjectExistsConstraint : CmisBaseConstraint
    {
        private readonly ISession _session;
        private readonly bool _shouldExist;

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
}

