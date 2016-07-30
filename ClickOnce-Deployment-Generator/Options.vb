Imports CommandLine

Public Class Options

    <[Option]("e"c, "Executable", HelpText:="The main executable of the application.  Should be in its own directory, with any other files to be included in the same deployment.", Required:=True)>
    Public Property Executable As String

    <[Option]("u"c, "ProviderUrl", HelpText:="The URI the application file is stored.", Required:=True)>
    Public Property ProviderUrl As String

    <[Option]("p"c, "Publisher", HelpText:="Name of the publisher of the deployment.", Required:=True)>
    Public Property Publisher As String

    <[Option]("a"c, "Architecture", HelpText:="Architecture of the processor.  Valid values: ""msil"", ""x86"", and ""x64"".", Required:=True)>
    Public Property Architecture As String

    <[Option]("KeyFilename", HelpText:="Filename of the key used for signing.", Required:=True)>
    Public Property KeyFilename As String

    <[Option]("KeyPassword", HelpText:="Password of the key used for signing.", Required:=True)>
    Public Property KeyPassword As String

    <[Option]("MageFilename", HelpText:="Path of mage.exe", Required:=True)>
    Public Property MageFilename As String

    <HelpOption>
    Public Function GetUsage() As String
        Return "Usage documentation not added yet :("
    End Function

End Class
