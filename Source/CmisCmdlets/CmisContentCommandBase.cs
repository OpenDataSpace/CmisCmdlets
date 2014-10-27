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
using System.Text;
using System.IO;
using DotCMIS.Data.Impl;
using DotCMIS.Exceptions;

namespace CmisCmdlets
{
    public class CmisContentCommandBase : CmisCommandBase
    {
        protected string ContentInternal;
        protected string LocalFileInternal;
        protected string MimeTypeInternal;

        private StringBuilder _allContent;

        protected override void BeginProcessing()
        {
            _allContent = new StringBuilder();
        }

        protected override void ProcessRecord()
        {
            if (!String.IsNullOrEmpty(ContentInternal))
            {
                _allContent.AppendLine(ContentInternal);
            }
        }

        protected ContentStream GetContentStream()
        {
            ContentStream stream = null;
            if (_allContent.Length > 0)
            {
                var content = UTF8Encoding.UTF8.GetBytes(_allContent.ToString());
                stream = new ContentStream();
                stream.Stream = new MemoryStream(content);
                stream.Length = content.LongLength;
                stream.MimeType = MimeTypeInternal;
            }
            else if (LocalFileInternal != null)
            {
                var fileStream = new FileStream(LocalFileInternal, FileMode.Open, FileAccess.Read,
                                                FileShare.Read);
                var ext = System.IO.Path.GetExtension(LocalFileInternal);
                stream.Stream = fileStream;
                stream.MimeType = MimeTypeMap.MimeTypeMap.GetMimeType(ext);
                stream.Length = fileStream.Length;
                stream.FileName = fileStream.Name;
            }
            return stream;
        }
    }
}

