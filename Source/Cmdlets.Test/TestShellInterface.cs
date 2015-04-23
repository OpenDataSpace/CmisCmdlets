// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System;
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework;
using System.Net.Sockets;

namespace TestShell
{
    public class ShellExecutionHasErrorsException : Exception
    {
        public Collection<ErrorRecord> Errors { get; private set; }

        public ShellExecutionHasErrorsException(Collection<ErrorRecord> errors) : base()
        {
            Errors = errors;
        }
    }

    public class TestShellInterface
    {
        public Runspace LastRunspace { get; set; }
        public Collection<object> LastResults { get; set; }
        public Collection<ErrorRecord> LastErrors { get; set; }

        private string[] _preExecutionCmds;
        private string[] _postExecutionCmds;

        public void SetPreExecutionCommands(params string[] commands)
        {
            _preExecutionCmds = commands;
        }

        public void SetPostExecutionCommands(params string[] commands)
        {
            _postExecutionCmds = commands;
        }

        public Collection<object> Execute(params string[] commands)
        {
            if (LastRunspace != null)
            {
                LastRunspace.Close();
            }
            LastRunspace = RunspaceFactory.CreateRunspace();
            LastRunspace.Open();
            LoadCmdletBinary();

            return ExecuteInExistingRunspace(commands);
        }

        public Collection<object> ExecuteInExistingRunspace(params string[] commands)
        {
            Collection<PSObject> results = null;
            LastResults = new Collection<object>();
            LastErrors = new Collection<ErrorRecord>();

            using (var pipeline = LastRunspace.CreatePipeline())
            {
                var script = JoinCommands(_preExecutionCmds) +
                    JoinCommands(commands) +
                        JoinCommands(_postExecutionCmds);
                pipeline.Commands.AddScript(script, false);
                results = pipeline.Invoke();
                LastErrors = new Collection<ErrorRecord>(
                    (from errObj in pipeline.Error.NonBlockingRead()
                                where errObj is PSObject
                                select ((PSObject) errObj).BaseObject as ErrorRecord
                    ).ToList()
                );
            }

            foreach (var curPSObject in results)
            {
                if (curPSObject == null)
                {
                    LastResults.Add(null);
                }
                else
                {
                    LastResults.Add(curPSObject.BaseObject);
                }
            }

            if (LastErrors.Count > 0)
            {
                throw new ShellExecutionHasErrorsException(LastErrors);
            }
            return LastResults;
        }

        public object GetVariableValue(string variableName)
        {
            object variable = LastRunspace.SessionStateProxy.GetVariable(variableName);
            if (variable is PSObject)
            {
                variable = ((PSObject)variable).BaseObject;
            }
            return variable;
        }

#region Constraints for Nunit
        public EqualConstraint IsValueOfVariable(string variableName)
        {
            return Is.EqualTo(GetVariableValue(variableName));
        }
#endregion

        private string JoinCommands(string[] cmds)
        {
            if (cmds == null)
            {
                return "";
            }
            return String.Join(";" + Environment.NewLine, cmds);
        }

        private void LoadCmdletBinary()
        {
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string path = new Uri(typeof(CmisCmdlets.CmisCommandBase).Assembly.CodeBase).LocalPath;

            // with real Powershell we use Import-Module to avoid installation of the SnapIn
            string cmd = isWindows ? "Import-Module '{0}'" : "Add-PSSnapIn '{0}'";
            using (var pipeline = LastRunspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(String.Format(cmd, path));
                try
                {
                    pipeline.Invoke();
                }
                catch (MethodInvocationException e)
                {
                    throw new RuntimeException(String.Format(
                        "Failed to import module '{0}'. Didn't you build it?", path), e);
                }
            }
        }
    }
}
