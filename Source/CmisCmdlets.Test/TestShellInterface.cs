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


namespace TestShell                                                                                                                               
{                                                                                                                                            
    public class TestShellInterface                                                                                                          
    {                                                                                                                                        
        private Runspace _runspace;                                                                                                          

        public TestShellInterface()                                                                                                          
        {                                                                                                                                    
            _runspace = RunspaceFactory.CreateRunspace();                                                                                    
            _runspace.Open();                                                                                                                
            LoadCmdletBinary();                                                                                                           
        }                                                                                                                                    

        public Collection<object> Execute(params string[] commands)                                                                                 
        {                                                                                                                                    
            Collection<PSObject> results = null;                                                                                             
            Collection<object> resultObjects = new Collection<object>();                                                                     
            using (var pipeline = _runspace.CreatePipeline())                                                                                
            {
                var script = String.Join(";" + Environment.NewLine, commands);
                pipeline.Commands.AddScript(script, false);                                                                             
                results = pipeline.Invoke();                                                                                                 
                if (pipeline.Error.Count > 0)                                                                                                
                {                                                                                                                            
                    var sb = new StringBuilder();                                                                                            
                    foreach (var error in pipeline.Error.NonBlockingRead())                                                                  
                    {                                                                                                                        
                        sb.Append(error.ToString() + Environment.NewLine);                                                                   
                    }                                                                                                                        
                    throw new RuntimeException(sb.ToString());                                                                               
                }                                                                                                                            
            }                                                                                                                                
            if (results == null)                                                                                                             
            {                                                                                                                                
                return resultObjects;
            }
            foreach (var curPSObject in results)
            {
                if (curPSObject == null)
                {
                    resultObjects.Add(null);
                }
                else
                {
                    resultObjects.Add(curPSObject.BaseObject);
                }
            }
            return resultObjects;
        }

        public object GetVariableValue(string variableName)
        {
            object variable = _runspace.SessionStateProxy.GetVariable(variableName);
            if (variable is PSObject)
            {
                variable = ((PSObject)variable).BaseObject;
            }
            return variable;
        }

        private void LoadCmdletBinary()
        {
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string path = new Uri(typeof(CmisCmdlets.CmisCmdlets).Assembly.CodeBase).LocalPath;
            if (isWindows) //we are likely to run Powershell 2.0 or higher, let's import it as a module
            {
                try
                {
                    Execute(String.Format("Import-Module {0}", path));
                }
                catch (MethodInvocationException e)
                {
                    throw new RuntimeException(String.Format(
                        "Failed to import module '{0}'. Didn't you build it?", path), e);
                }                                            
            }
            else
            {
                Execute(String.Format("Add-PSSnapIn '{0}'", path));
            }
        }
    }
}
