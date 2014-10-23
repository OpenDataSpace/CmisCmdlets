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
using DotCMIS.Client;
using System.Collections.Generic;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommon.Get, "CmisObject")]
    public class GetCmisObjectCommand : CmisCommandBase
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public string Path { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public string Name { get; set; }

        [Parameter]
        public SwitchParameter Exact { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public int RecursionDepth { get; set; }

        protected override void ProcessRecord()
        {
            var cmisPath = new CmisPath(Path);
            var navigation = new CmisNavigation(GetCmisSession(), GetWorkingFolder());
            ICmisObject obj = null;
            try
            {
                obj = navigation.Get(cmisPath);
            }
            catch (CmisBaseException e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "GetObjectFailed",
                                                      ErrorCategory.ResourceUnavailable, Path));
                return;
            }

            var nameIsEmpty = String.IsNullOrEmpty(Name);

            if (!(obj is IFolder) ||
                (!cmisPath.HasTrailingSlash() && nameIsEmpty))
            {
                WriteObject(obj);
                return;
            }

            WildcardPattern wildcard = new WildcardPattern("*");
            if (!nameIsEmpty)
            {
                wildcard = Exact.IsPresent ? new WildcardPattern(Name)
                                           : new WildcardPattern(Name + "*");
            }
            int depth = RecursionDepth == 0 ? 1 : RecursionDepth;

            //otherwise we want the descendants of the folder
            var folder = obj as IFolder;
            IList<ITree<IFileableCmisObject>> descendants;
            try
            {
                descendants = folder.GetDescendants(depth);
            }
            catch (CmisBaseException e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "GetDescendatnsFailed",
                                                      ErrorCategory.ResourceUnavailable, Path));
                return;
            }
            WriteTreeList(descendants, wildcard);
        }

        private void WriteTreeList(IList<ITree<IFileableCmisObject>> treeList,
                                   WildcardPattern wildcard)
        {
            foreach (var tree in treeList)
            {
                if (wildcard.IsMatch(tree.Item.Name))
                {
                    WriteObject(tree.Item);
                }
                if (tree.Children != null)
                {
                    WriteTreeList(tree.Children, wildcard);
                }
            }
        }
    }
}

