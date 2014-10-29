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
using DotCMIS.Exceptions;
using System.Text;
using System.Collections;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.New, "CmisDocument", DefaultParameterSetName = "FromFile")]
    public class NewCmisDocumentCommand : CmisContentCommandBase
    {
        [Parameter(Position = 0)]
        public string Path { get; set; }

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "FromFile")]
        public string LocalFile {
            get { return LocalFileInternal; }
            set { LocalFileInternal = value; }
        }

        [Parameter(Position = 1, ParameterSetName = "FromContent", ValueFromPipeline = true)]
        public string Content {
            get { return ContentInternal; }
            set { ContentInternal = value; }
        }

        [Parameter(Position = 2, ParameterSetName = "FromContent")]
        public string MimeType {
            get { return MimeTypeInternal; }
            set { MimeTypeInternal = value; }
        }

        [Parameter(Position = 3, Mandatory = false)]
        public Hashtable Properties { get; set; }

        protected override void EndProcessing()
        {
            var path = new CmisPath(Path);
            if (LocalFile != null && path.HasTrailingSlash())
            {
                path = path.Combine(System.IO.Path.GetFileName(LocalFile));
            }
            var nav = new CmisNavigation(CmisSession, WorkingFolder);
            var props = Utilities.HashtableToDict(Properties);
            var stream = GetContentStream();
            try
            {
                WriteObject(nav.CreateDocument(path, stream, props));
            }
            catch (CmisBaseException e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "CreateDocumentFailed",
                                                      ErrorCategory.WriteError, path));
            }
            finally
            {
                if (stream != null)
                {
                    stream.Stream.Close();
                }
            }
        }
    }
}

