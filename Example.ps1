# Powershel/Pash CMIS Cmdlet example to conenct to your Dataspace

# Set these variables for your environment/connect

# Use ./CmisCmdlets.dll, if the DLL is in the same location.
# Otherwise it will look for installed modules
$moduleLocation = './Source/Cmdlets/bin/Debug/CmisCmdlets.dll'
$cmisurl = 'http://demo.dataspace.cc/cmis/atom11'
$username = '<user>'
$password = '<password>'
$cmisrepo = 'my'
$testdir = 'cmisCmdletExampleDir' # We will operate in that folder

# First, load the CmisCmdlet.dll module
Import-Module $moduleLocation


# Now we need to establish a connection to the dataspace. We connect directly
# to the specified repository

Connect-Cmis $cmisurl $username $password $cmisrepo
# This is the short form of
# Connect-Cmis -Url '<url>' -UserName '<username>' -Password '<password>' -Repository  '<repo>'
# If you want to avoid using SSL, you can add the `-Insecure` switch to the parameter list.
# However, this is not recommended. If you have problems with mono and the secure connection,
# make sure you imported the root certificates to mono *once* with the mozroots utility.
# Check out http://www.mono-project.com/docs/faq/security/#does-ssl-work-for-smtp-like-gmail

# $_CMIS_SESSION should be set now. It holds information about your connection
if (!$_CMIS_SESSION) {
	Write-Host 'Failed to connect. Do you have the correct password and imported root certificates?' -F red
	exit 1
}

# Let's list the contents of the repository. Get-CmisObject has a little tweak right now:
# If the path ends with a slash, it will *list* the repository, otherwise you will retrieve
# the repository object

Write-Host "Look which files we have in our root folder:"
$contents = Get-CmisObject /
$contents | Format-Table Name,ContentStreamMimeType,ContentStreamLength | Out-Default
# We only select some properties to display, otherwise all properties of the object are printed

# Unfortunately, it's still a little complicated to test if something exists. This will be improved, promised!
if ($contents | where name -eq $testdir) {
	$answer = Read-Host "Test directory $testdir exists. Remove it and all its content? (y/n)"
	if ($answer -ne 'y') {
		Write-Host "Can't work with testdir. Exiting..."
		exit 1
	}
    Remove-CmisObject $testdir -Recursive
}

# Note that the following cmdlet invocations can be expressed in many forms. Here are just
# some examples to show what's possible.

# Let's create the test dir
$testdirobject = New-CmisFolder $testdir
# The $testdirobject is a DotCMIS Folder object with an API corresponding to 
# http://chemistry.apache.org/java/0.9.0/maven/apidocs/org/apache/chemistry/opencmis/client/api/Folder.html

# Because it's easier for the following operations, we set our Cmis working folder
# to the new test directory
$curdir = Set-CmisWorkingFolder $testdir
Write-Host "All operations are now relative to $curdir"


# Now we create a new file with content
$testContent = "This is a pretty test text"
# We can simply pipe content to a new cmis document.
Write-Host "Creating test document"
$testFile = $testContent | New-CmisDocument 'test.txt' -MimeType 'text/plain'
# The new Document is a DotCMIS document which has an API corresponding to
# http://chemistry.apache.org/java/0.9.0/maven/apidocs/org/apache/chemistry/opencmis/client/api/Document.html

$testPath = $testFile.Paths[0]
Write-Host "Full path of the new file is: $testPath"


# Let's try to read the content
Write-Host "Reading the content from CMIS stream:"
# Can be done with parameter -Document, by path, or if the object gets piped in the command
$readContent = $testFile | Read-CmisDocument
$readContent | Out-Default


# We can also update an existing document
Write-Host "Changing the content"
# Don't forget to set the mimetype
$testFile = "The brand new", "multiline", "content" | Update-CmisObject -Object $testFile -MimeType 'plain/text'

# Read the new content
Write-Host "New content:"
Read-CmisDocument 'test.txt' | Out-Default


# Let's remove it again
Write-Host "Removing the file"
Remove-CmisObject -Object $testFile


# Now we clean our stuff
Write-Host "Cleaning up..."
Set-CmisWorkingFolder / | Out-Null # Out-Null mutes the output
Remove-CmisObject -Object $testdirobject -Recursive
# We end the session, so a disconnect is not really necessary, but we do it for this example
Disconnect-Cmis