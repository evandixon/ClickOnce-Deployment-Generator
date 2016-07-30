Imports System.Reflection

Module Module1

    Sub Main()
        Dim options As New Options
        If CommandLine.Parser.Default.ParseArguments(Environment.GetCommandLineArgs, options) Then
            Dim exeAssembly As Assembly = Assembly.ReflectionOnlyLoadFrom(options.Executable)
            Dim version = exeAssembly.GetName.Version.ToString

            'If the executable is in bin\Release\Something.exe, we want the working directory to be bin
            Dim originalSourceDirectory = IO.Path.GetFileName(IO.Path.GetDirectoryName(options.Executable)) 'Should be Release in this example
            Environment.CurrentDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(options.Executable), "..")

            'Copy the source files (e.g. from bin\Release to bin\Deploy\1.0.0.0)
            Dim newSourceDirectory = IO.Path.Combine("Deploy", version)
            If IO.Directory.Exists("Deploy") Then
                IO.Directory.Delete("Deploy")
            End If
            IO.Directory.CreateDirectory("Deploy")
            My.Computer.FileSystem.CopyDirectory(originalSourceDirectory, newSourceDirectory)

            Dim manifestName As String = IO.Path.Combine("Deploy", version, IO.Path.GetFileName(options.Executable) & ".manifest")
            Dim deploymentName As String = IO.Path.Combine("Deploy", IO.Path.GetFileName(options.Executable) & ".application")

            'Create the manifest
            RunProgram(options.MageFilename, $"-New Application -Processor ""{options.Architecture}"" -ToFile ""{manifestName}"" -version ""{version}"" -name ""{exeAssembly.GetName.Name}"" -FromDirectory ""{newSourceDirectory}""")

            'Todo: enable extension mapping
            'Todo: rename all files but the manifest to "*.deploy"

            'Sign the manifest
            RunProgram(options.MageFilename, $"-Sign ""{manifestName}"" -CertFile ""{options.KeyFilename}"" -Password {options.KeyPassword}")

            'Create the deployment (.application)
            RunProgram(options.MageFilename, $"-New Deployment -Processor ""{options.Architecture}"" -Install true -Publisher ""{options.Publisher}"" -version ""{version}"" -ProviderUrl ""{options.ProviderUrl}"" -AppManifest ""{manifestName}"" -UseManifestForTruest true -ToFile ""{deploymentName}""")

            'Sign the deployment
            RunProgram(options.MageFilename, $"-Sign ""{deploymentName}"" -CertFile ""{options.KeyFilename}"" -Password {options.KeyPassword}")
        End If
    End Sub

    Sub RunProgram(program As String, arguments As String)
        Dim p As New Process
        p.StartInfo.WorkingDirectory = Environment.CurrentDirectory
        p.StartInfo.FileName = program
        p.StartInfo.Arguments = arguments
        p.Start()
        p.WaitForExit()
    End Sub

End Module
