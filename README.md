CmisCmdlets
===========
Cmdlets to use CMIS from Powershell and Pash

# Compilation

The solution contains multiple configurations: For using Pash and for using Powershell.
Both targets have are split in Release and Debug.

If you want to compile against Pash, make sure you add a compiled Pash version
to the project. To do this, clone Pash (https://github.com/Pash-Project/Pash)
and invoke `update_pash.sh /path/to/cloned/Pash`

Then build the project as usual. 

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
| New-CmisFolder        | Creates a new folder                                      |
| New-CmisDocument      | Creates a new (empty) document/file                       |
| Get-CmisObject        | Gets information about all/specific CMIS objects          |
| Read-CmisObject       | Reads the contents of an object                           |
| Write-CmisObject      | Writes the content of an object by stream                 |
| Receive-CmisObject    | Downloads an object with contents                         |
| Send-CmisObject       | Uploads an object with contents                           |
| Remove-CmisObject     | Removes an object                                         |
