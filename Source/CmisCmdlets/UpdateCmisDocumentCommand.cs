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
using System.ComponentModel.DataAnnotations;
using DotCMIS.Client;
using DotCMIS.Exceptions;

namespace CmisCmdlets
{
    [Cmdlet(VerbsData.Update, "CmisDocument", DefaultParameterSetName = "FromFile")]
    public class UpdateCmisDocumentCommand : SetCmisDocumentBaseCommand
    {
        [Parameter(Position = 0, ParameterSetName = "FromContent")]
        [Parameter(Position = 0, ParameterSetName = "FromFile")]
        public string Path { get; set; }

        [Parameter(Position = 0, ParameterSetName = "FromFileByObject")]
        [Parameter(Position = 0, ParameterSetName = "FromContentByObject")]
        public ICmisObject Object { get; set; }

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "FromFile")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "FromFileByObject")]
        public string LocalFile {
            get { return LocalFileInternal; }
            set { LocalFileInternal = value; }
        }

        [Parameter(Position = 1, ParameterSetName = "FromContent", ValueFromPipeline = true)]
        [Parameter(Position = 1, ParameterSetName = "FromContentByObject", ValueFromPipeline = true)]
        public string Content {
            get { return ContentInternal; }
            set { ContentInternal = value; }
        }

        [Parameter(Position = 2, ParameterSetName = "FromContent")]
        [Parameter(Position = 2, ParameterSetName = "FromContentByObject")]
        public string MimeType {
            get { return MimeTypeInternal; }
            set { MimeTypeInternal = value; }
        }


        protected override void EndProcessing()
        {
            IDocument doc;
            if (Object == null)
            {
                doc = new CmisNavigation(GetCmisSession(), GetWorkingFolder()).GetDocument(Path);
            }
            else
            {
                doc = Object as IDocument;
                if (doc == null)
                {
                    var ex = new CmisObjectNotFoundException("The provided object is not a Document");
                    ThrowTerminatingError(new ErrorRecord(ex, "UpdateObjNotDocument",
                                                          ErrorCategory.InvalidArgument, Object));
                }
            }
            var stream = GetContentStream();
            try
            {
                doc.SetContentStream(stream, true);
            }
            finally
            {
                stream.Stream.Close();
            }
        }
    }
}

