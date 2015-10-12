CmisCmdlets
===========
Cmdlets to use CMIS from Powershell and Pash


# Compilation

If you want to compile against Pash, make sure you add a compiled Pash version
to the project. To do this, clone Pash (https://github.com/Pash-Project/Pash)
and invoke `update_pash.sh /path/to/cloned/Pash`

Then, regardless if you are compiling it against Pash or Powershell, you need
to init the git submodules via `git submodule init; git submodule update`.

For the tests to work, you need to add a `TestConfig.config` file to the project
directory with the following contents:
```
<?xml version="1.0"?>
<configuration>
    <appSettings>
	    <add key="user" value="cmisTestUser" />
	    <add key="password" value="testUserPassword" />
	    <add key="url" value="urlToYourTestCMISRepo" />
	    <add key="repository" value="nameOfYourTestCmisRepo" />
	    <add key="repository_alt" value="nameOfYourSecondTestCmisRepo" />
	</appSettings>
</configuration>
```

Then build the project as usual (with your IDE, xbuild or msbuild).



# Cmdlets

The following cmdlets are provided:

| **Cmdlet Name**       | **Description**                                           |
|:----------------------|:----------------------------------------------------------|
| Connect-Cmis          | Connects to a CMIS repository                             |
| Disconnect-Cmis       | Disconnects and clears the session                        |
| Get-CmisRepository    | Lists all available repositories                          |
| Set-CmisRepository    | Sets the current repository                               |
| Set-CmisWorkingFolder | Sets the folder in the repository where you operate in    |
| Get-CmisWorkingFolder | Gets in which folder you are currently in                 |
| Get-CmisObject        | Gets information about all/specific CMIS objects          |
| New-CmisFolder        | Creates a new folder                                      |
| New-CmisDocument      | Creates a new (empty) document/file                       |
| Update-CmisObject     | Updates an object											|
| Read-CmisDocument     | Reads the contents of a document                          |
| Remove-CmisObject     | Removes an object                                         |
| Get-CmisProperty      | Get the available properties, or a specific one           |



# Usage

First you need to import the CmisCmdlets.dll file as a module (Powershell) or PSSnapin (Pash) in your session.


## Connecting
To do something you first need to connect to a CMIS repository by calling
```PowerShell
Connect-Cmis -URL 'https://youhost.com/cmis/binding' -Username 'user' -Password 'pw' -Repository 'repo'
```
If you want to do some advaned things, you can find the CMIS session object in `$_CMIS_SESSION`.
To get rid of the connection, simply call
```PowerShell
Disconnect-Cmis
```


## Changing the Repository
You can get a list of available repositories with
```PowerShell
Get-CmisRepository
```
and look for a speicfic one by passing a wildcard to the `-Name` parameter.

Then you can change to a specific repository with `Set-CmisRepository`. This cmdlet can either take the internal
repository ID, or, by default, a repository name.

## Working Folder
To make usage easier, you can operate from a specific folder, e.g. by calling
```PowerShell
Set-CmisWorkingFolder /path/to/remotedir
```
From that point on, relative paths passed to any of these cmdlets will refer to that directory.
With `Get-CmisWorkingFolder` or you can find out the current working folder.


## Getting CMIS objects
You can get a specific CMIS object with
```PowerShell
Get-CmisObject path/to/object
```

To get all objects from a folder, simply call 
```PowerShell
Get-CmisObject myfolder/
```
(note the slash at the end).

You can also use that cmdlet to look for a specific object recursively. For example if you want to get
all Objects which names contain "test" in next 3 recursion levels, you'd need to call
```PowerShell
Get-CmisObject -RecursionDepth 3 -Name '*test*'
```


## Creating new CMIS objects
Currently the creation of folders and documents is supported. To create a folder, simply use
```PowerShell
New-CmisFolder newfolder
```
You can use the `-Recursive` flag to create a folder with all its parents.

Creating a document can be done in two ways: By using a local file and with content from the pipeline.
```PowerShell
New-CmisDocument -Path folder/remoteFile.html -LocalFile localFile.html
```
This command would create a document called `remoteFile.html` in the existing remote folder `folder` with the
contents of the local file `localFile.html`.

You can also directly create a document with contents by pipeline (or passed as the `-Content` parameter)
```PowerShell
'foo','bar' | New-CmisDocument -Path output.txt -Mimetype 'text/plain'
```
This command would create the remote document `output.txt` and write each input object in it with a newline at the end.
Note that for this method you explicitly need to specify the `-Mimetype`.


## Updating an existing object
Updating an object works similar to creating a CMIS document. When dealing with a document, the usage for updating the
content is the same. One important difference is that you can also directly pass the CMIS object to be updated
instead of the `-Path` can be easier and is faster.

Also, you can rename an arbitrary CMIS object by using `-Name` parameter. E.g. you can also rename a folder
```PowerShell
Update-CmisObject -Object $folder -Name 'newName'
```
The passed object would then get renamed to `newName`.


## Reading a document
If you want to read a document, you can call
```PowerShell
Read-CmisDocument path/to/document.txt
```
This call would pass pack the contents as a string to the pipeline. However, you can also download the contents to
a file:
```PowerShell
CmisDocument-CmisObject -Path path/to/document.png -Destination localFile.png
```
Feel free to leave out the explicit parameter names, it also works by position as
`Read-CmisDocument remote.txt local.txt`.

You can also pass a document object instead of the path:
```PowerShell
Read-CmisDocument -Document $doc local.txt
```

## Removing objects
To remove objects, pass their paths or objects to the `Remove-CmisObject` cmdlet. You can also specify the
`-Recursive` parameter to remove folders recursively (but be careful!).
```PowerShell
Remove-CmisObject 'path/to/folderWithContents','mydoc.txt' -Recursive
```

## Object properties
Last but not least, you can query common CMIS properties, or the properties of a specific objects. Use the
`-Name` parameter to look out for specific properties.
```PowerShell
$cmisObj | Get-CmisProperty -Name "*time*"
```


# Provider
A Powershell/Pash provider is in development. It's currently developed in the providerImplementation branch.
First things already work. Check the tests for example usages.

# Bug reports
Please feel free to submit request and bug reports!
