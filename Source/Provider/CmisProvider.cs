﻿// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
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
using System.Management.Automation.Provider;
using System.Management.Automation;
using System.Collections.Generic;
using DotCMIS.Client;
using DotCMIS.Exceptions;
using Cmis.Utility;

namespace CmisProvider
{
    [CmdletProvider("Cmis", ProviderCapabilities.Credentials)]
    public class CmisProvider : NavigationCmdletProvider
    {

        #region drive related functionality

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            var parameters = DynamicParameters as CmisDriveParameters;
            return new CmisDrive(drive, parameters);
        }

        protected override object NewDriveDynamicParameters ()
        {
            return new CmisDriveParameters();
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
            var drive = CurrentDrive();
            ICmisObject obj;
            return drive.Navigation.TryGet(path, out obj);
        }

        protected override bool IsValidPath(string path)
        {
            try {
                new CmisPath(path);
            } catch (CmisPathException) {
                return false;
            }
            return true;
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
            var drive = CurrentDrive();
            var folder = drive.Navigation.GetFolder(path);
            WriteTreeList(folder.GetDescendants(recurse ? 100 : 1));
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
            var cmisPath = new CmisPath(path);
            if (cmisPath.HasTrailingSlash()) // docs say we should return the path itself if there's no item child
            {
                return path;
            }
            if (IsItemContainer(path))
            {
                return path;
            }
            var normalizedPath = cmisPath.ToString();
            return normalizedPath.Substring(normalizedPath.IndexOf(CmisPath.CorrectSlash, StringComparison.OrdinalIgnoreCase) + 1);
        }

        protected override bool IsItemContainer(string path)
        {
            return CurrentDrive().Navigation.Get(path) is IFolder;
        }

        protected override string GetParentPath(string path, string root)
        {
            var normalizedPath = new CmisPath(path).WithoutTrailingSlash().ToString();
            if (!String.IsNullOrEmpty(root) && !normalizedPath.Contains(new CmisPath(root).WithoutTrailingSlash().ToString()))
            {
                return null;
            }
            int slashIdx = normalizedPath.LastIndexOf(CmisPath.CorrectSlash, StringComparison.OrdinalIgnoreCase);
            if (slashIdx < 0)
            {
                return "";
            }
            return normalizedPath.Substring(0, slashIdx);
        }

        protected override string MakePath(string parent, string child)
        {
            var cmisPath = new CmisPath(parent);
            var cmisChild = new CmisPath(child);
            if (cmisPath.IsRoot() && cmisChild.WithoutTrailingSlash().ToString() == "..")
            {
                return "..";
            }
            return new CmisPath(parent).Combine(child).WithoutTrailingSlash().ToString();
        }

        #endregion

        private void WriteTreeList(IList<ITree<IFileableCmisObject>> treeList)
        {
            if (treeList == null)
            {
                return;
            }
            foreach (var tree in treeList)
            {
                WriteObject(tree.Item);
                if (tree.Children != null)
                {
                    WriteTreeList(tree.Children);
                }
            }
        }

        void WriteObject(IFileableCmisObject item)
        {
            WriteItemObject(item, item.Paths[0], item is IFolder);
        }

        private CmisDrive CurrentDrive() {
            var drive = PSDriveInfo as CmisDrive;
            if (drive == null)
            {
                var msg = "The current drive is not set or not a CmisDrive. " +
                    "This is an internal problem. Please report this";
                throw new InvalidOperationException(msg);
            }
            return drive;
        }
    }
}

