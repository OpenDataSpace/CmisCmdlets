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
using System.Management.Automation;
using DotCMIS.Client;
using DotCMIS.Exceptions;
using System.Collections;
using System.Collections.Generic;
using DotCMIS;
using DotCMIS.Enums;
using Cmis.Utility;

namespace CmisCmdlets
{
    [Cmdlet(VerbsData.Update, "CmisObject", DefaultParameterSetName = "FromFile")]
    public class UpdateCmisObjectCommand : CmisContentCommandBase
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

        [Parameter(Position = 1, ParameterSetName = "FromContent", ValueFromPipeline = true, Mandatory = true)]
        [Parameter(Position = 1, ParameterSetName = "FromContentByObject", ValueFromPipeline = true, Mandatory = true)]
        public string Content {
            get { return ContentInternal; }
            set { ContentInternal = value; }
        }

        [Parameter(Position = 2, ParameterSetName = "FromContent", Mandatory = true)]
        [Parameter(Position = 2, ParameterSetName = "FromContentByObject", Mandatory = true)]
        public string MimeType {
            get { return MimeTypeInternal; }
            set { MimeTypeInternal = value; }
        }

        [Parameter(Position = 3, Mandatory = false)]
        public string Name { get; set; }

/*
 TODO: enable support for properties
        [Parameter(Position = 4, Mandatory = false)]
        public Hashtable Properties { get; set; }
*/

        protected override void EndProcessing()
        {
            var navigation = new CmisNavigation(CmisSession, WorkingFolder);
            ICmisObject obj = (Object != null) ? Object : navigation.Get(Path);

/*
            if (Properties != null || !String.IsNullOrEmpty(Name))
            {
                var props = Utilities.HashtableToDict(Properties);
                if (!String.IsNullOrEmpty(Name))
                {
                    props[PropertyIds.Name] = Name;
                }
                obj = obj.UpdateProperties(props);
            }
 */
            if (!String.IsNullOrEmpty(Name))
            {
                var props = new Dictionary<string, object>() { { PropertyIds.Name, Name } };
                obj = obj.UpdateProperties(props);
            }

            // check if we should update content
            if (LocalFile == null && !HasContent())
            {
                WriteObject(obj);
                return; 
            }

            // otherwise the object must be a document
            var doc = obj as IDocument;
            if (doc == null)
            {
                var ex = new CmisObjectNotFoundException("The provided object is not a Document");
                ThrowTerminatingError(new ErrorRecord(ex, "UpdateObjNotDocument",
                                                      ErrorCategory.InvalidArgument, Object));
            }

            var stream = GetContentStream();
            stream.FileName = obj.Name; // important, as may not set
            try
            {
                var result = doc.SetContentStream(stream, true);
                WriteObject(result == null ? doc : result);
            }
            finally
            {
                stream.Stream.Close();
            }
        }
    }
}

