﻿Imports CommandLine

Public Class Options

    <[Option]("e"c, "Executable", HelpText:="The main executable of the application.  Should be in its own directory, with any other files to be included in the same deployment.", Required:=True)>
    Public Property Executable As String

    <[Option]("u"c, "ProviderUrl", HelpText:="The URI the application file is stored.", Required:=True)>
    Public Property ProviderUrl As String

    <[Option]("p"c, "Publisher", HelpText:="Name of the publisher of the deployment.", Required:=True)>
    Public Property Publisher As String

    <[Option]("h"c, "Certificate Hash", HelpText:="Hash of the certificate used for signing.", Required:=True)>
    Public Property CertificateHash As String

    <[Option]("m"c, "MageFilename", HelpText:="Path of mage.exe", Required:=True)>
    Public Property MageFilename As String

    <[Option]("s"c, "SupportUrl", HelpText:="Support URL, as shown in Add/Remove programs", Required:=True)>
    Public Property SupportUrl As String

    <HelpOption>
    Public Function GetUsage() As String
        Return "Usage documentation not added yet :("
    End Function

End Class
