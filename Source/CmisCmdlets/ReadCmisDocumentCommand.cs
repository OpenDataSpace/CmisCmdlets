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
using System.Management.Automation;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using DotCMIS.Data;

namespace CmisCmdlets
{

    [Cmdlet(VerbsCommunications.Read, "CmisDocument" , DefaultParameterSetName = "ByPath",
            SupportsShouldProcess = true)]
    public class ReadCmisDocumentCommand : CmisCommandBase
    {
        [Parameter(Position = 0, ParameterSetName = "ByPath")]
        public string Path { get; set; }

        [Parameter(Position = 0, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [Alias("Object")]
        public IDocument Document { get; set; }

        [Parameter(Position = 1, Mandatory = false)]
        public string Destination { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }


        // 0,1MB directly to pipeline okay without questions
        private static readonly long _pipelineMaxSize = 1024 * 100;

        // only content of the following mimetypes will be displayed without further questions
        private static List<string> _plainMimeTypes = new List<string>()
        {
            "text/plain",
            "text/html"
        };


        protected override void ProcessRecord()
        {
            var doc = Document;
            if (doc == null)
            {
                doc = new CmisNavigation(CmisSession, WorkingFolder).GetDocument(Path);
            }
            var writeToPipeline = String.IsNullOrEmpty(Destination);
            var size = (long) doc.ContentStreamLength;

            // Do some security checks firsat if we should write the output to pipeline
            var msg = String.Format("The content is not plain text, but of type {0}. Do you want to"
                                    + "write it to pipeline anyway?", doc.ContentStreamMimeType);
            if (writeToPipeline && !IsPlaintextType(doc.ContentStreamMimeType) && !Force &&
                !ShouldContinue(msg, "Document Content Warning"))
            {
                return;
            }

            msg = String.Format("The document is pretty big (greater than {0:0.##} MB). " +
                "Are you sure that you want to write it to the pipeline anyway?",
                ((double)_pipelineMaxSize) / (1024 * 1024));
            if (writeToPipeline && size > _pipelineMaxSize && !Force &&
                !ShouldContinue(msg, "Big Document Warning"))
            {
                return;
            }

            msg = "The destination already exists. Do you want to overwrite that file?";
            if (!writeToPipeline && File.Exists(Destination) && !Force &&
                !ShouldContinue(msg, "Destination File Exists"))
            {
                return;
            }

            // TODO: better download mechanism anywhere
            byte[] stringBuffer = null;
            Stream outputStream;
            if (writeToPipeline)
            {
                stringBuffer = new byte[size];
                outputStream = new MemoryStream(stringBuffer);
            }
            else
            {
                outputStream = new FileStream(Destination, FileMode.Create);
            }

            IContentStream stream = null;
            try
            {
                var buffer = new byte[8 * 1024];
                int offset = 0;
                int bytesRead = 0;
                stream = doc.GetContentStream();

                while ((bytesRead = stream.Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                    offset += bytesRead;
                }
                outputStream.Flush();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Stream.Close();
                }
                outputStream.Close();
            }

            if (writeToPipeline)
            {
                // TODO: support encodings
                WriteObject(Encoding.UTF8.GetString(stringBuffer));
            }
        }

        private bool IsPlaintextType(string mimetype)
        {
            foreach (var supportedType in _plainMimeTypes)
            {
                if (mimetype.ToLowerInvariant().StartsWith(supportedType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

