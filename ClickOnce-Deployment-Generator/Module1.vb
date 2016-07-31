Imports System.IO
Imports System.Reflection

Module Module1

    Sub Main()
        Dim options As New Options
        If CommandLine.Parser.Default.ParseArguments(Environment.GetCommandLineArgs, options) Then
            Dim exeAssembly As Assembly = Assembly.ReflectionOnlyLoadFrom(options.Executable)
            Dim version = exeAssembly.GetName.Version.ToString
            Dim architecture = exeAssembly.GetName.ProcessorArchitecture.ToString

            'If the executable is in bin\Release\Something.exe, we want the working directory to be bin
            Dim originalSourceDirectory = IO.Path.GetFileName(IO.Path.GetDirectoryName(options.Executable)) 'Should be Release in this example
            Environment.CurrentDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(options.Executable), "..")

            'Copy the source files (e.g. from bin\Release to bin\Deploy\1.0.0.0)
            Dim newSourceDirectory = IO.Path.Combine("Deploy", version)
            If IO.Directory.Exists("Deploy") Then
                IO.Directory.Delete("Deploy", True)
            End If
            IO.Directory.CreateDirectory("Deploy")
            My.Computer.FileSystem.CopyDirectory(originalSourceDirectory, newSourceDirectory)

            Dim manifestName As String = IO.Path.Combine("Deploy", version, IO.Path.GetFileName(options.Executable) & ".manifest") 'Something.exe.manifest
            Dim iconName As String = IO.Path.Combine("Deploy", version, IO.Path.GetFileName(options.Executable) & ".ico") 'Something.exe.ico
            Dim deploymentName As String = IO.Path.Combine("Deploy", IO.Path.GetFileNameWithoutExtension(options.Executable) & ".application") 'Something.application

            'Extract the icon
            Using iconStream As New IO.FileStream(iconName, FileMode.OpenOrCreate)
                System.Drawing.Icon.ExtractAssociatedIcon(options.Executable).Save(iconStream)
            End Using

            'Create the manifest
            RunProgram(options.MageFilename, $"-New Application -Processor ""{architecture}"" -ToFile ""{manifestName}"" -version ""{version}"" -name ""{exeAssembly.GetName.Name}"" -FromDirectory ""{newSourceDirectory}"" -IconFile ""{IO.Path.GetFileName(iconName)}""")

            'Todo: enable extension mapping
            'Todo: rename all files but the manifest to "*.deploy"

            'Sign the manifest
            RunProgram(options.MageFilename, $"-Sign ""{manifestName}"" -CertHash ""{options.CertificateHash}""")

            'Create the deployment (.application)
            RunProgram(options.MageFilename, $"-New Deployment -Processor ""{architecture}"" -Install true -Publisher ""{options.Publisher}"" -version ""{version}"" -ProviderUrl ""{options.ProviderUrl}"" -AppManifest ""{manifestName}"" -UseManifestForTrust true -ToFile ""{deploymentName}""")

            'Sign the deployment
            RunProgram(options.MageFilename, $"-Sign ""{deploymentName}"" -CertHash ""{options.CertificateHash}""")
        End If
    End Sub

    Sub RunProgram(program As String, arguments As String)
        Using p As New Process
            p.StartInfo.WorkingDirectory = Environment.CurrentDirectory
            p.StartInfo.FileName = program
            p.StartInfo.Arguments = arguments
            p.StartInfo.UseShellExecute = False
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.RedirectStandardError = True
            p.Start()

            AddHandler p.OutputDataReceived, AddressOf OnOutput
            AddHandler p.ErrorDataReceived, AddressOf OnOutput

            p.BeginOutputReadLine()
            p.BeginErrorReadLine()

            p.WaitForExit()

            RemoveHandler p.OutputDataReceived, AddressOf OnOutput
            RemoveHandler p.ErrorDataReceived, AddressOf OnOutput
        End Using
    End Sub

    Sub OnOutput(sender As Object, e As DataReceivedEventArgs)
        Console.WriteLine(e.Data)
    End Sub

End Module
