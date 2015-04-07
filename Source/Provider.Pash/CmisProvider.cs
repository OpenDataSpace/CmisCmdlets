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
using System.Management.Automation.Provider;
using System.Management.Automation;

namespace CmisProvider
{
    [CmdletProvider("Cmis", ProviderCapabilities.Credentials)]
    public class CmisProvider : NavigationCmdletProvider
    {
        #region drive related functionality

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            return base.NewDrive(drive);
        }

        #endregion

        #region item related functionality

        protected override void ClearItem(string path)
        {
            base.ClearItem(path);
        }

        protected override void SetItem(string path, object value)
        {
            base.SetItem(path, value);
        }

        protected override bool ItemExists(string path)
        {
            return base.ItemExists(path);
        }

        protected override bool IsValidPath(string path)
        {
            throw new NotImplementedException();
        }

        protected override void GetItem(string path)
        {
            base.GetItem(path);
        }

        #endregion

        #region container related functionality

        protected override void RenameItem(string path, string newName)
        {
            base.RenameItem(path, newName);
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            base.RemoveItem(path, recurse);
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            base.NewItem(path, itemTypeName, newItemValue);
        }

        protected override bool HasChildItems(string path)
        {
            return base.HasChildItems(path);
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            base.GetChildNames(path, returnContainers);
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            base.GetChildItems(path, recurse);
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }

        #endregion

        #region navigation related functionality

        protected override string NormalizeRelativePath(string path, string basePath)
        {
            return base.NormalizeRelativePath(path, basePath);
        }

        protected override void MoveItem(string path, string destination)
        {
            base.MoveItem(path, destination);
        }

        protected override string GetChildName(string path)
        {
            return base.GetChildName(path);
        }

        protected override bool IsItemContainer(string path)
        {
            return base.IsItemContainer(path);
        }

        protected override string GetParentPath(string path, string root)
        {
            return base.GetParentPath(path, root);
        }

        protected override string MakePath(string parent, string child)
        {
            return base.MakePath(parent, child);
        }

        #endregion
    }
}

