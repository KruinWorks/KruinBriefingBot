Imports System

''' <summary>
''' KruinBriefingBot's main module.
''' </summary>
Module Program
    ''' <summary>
    ''' Start KruinBriefingBot, which is also the startup object.
    ''' </summary>
    ''' <param name="args">Arguments passed to application.</param>
    Sub Main(args As String())
        AddHandler System.Runtime.Loader.AssemblyLoadContext.Default.Unloading, AddressOf OnApplicationExit
        Console.WriteLine("==> Starting KruinBriefingBot")
        Dim startupTimer As New Stopwatch
        startupTimer.Start()
        Console.WriteLine("==> Initializing configuration...")
        Try
            Methods.Initialize()
            Variables.LogWriter = New IO.StreamWriter(IO.File.Create(IO.Path.Combine(Variables.AppPath, "latest.log")), System.Text.Encoding.UTF8) With {.AutoFlush = True}
        Catch ex As Exception
            Console.WriteLine("! Error cought while initializing: " & ex.ToString())
            Environment.Exit(1)
        End Try
        Console.WriteLine("==> Testing API...")
        Try
            Variables.BotInstance = New Telegram.Bot.TelegramBotClient(Variables.currentSettings.APIKey)
            Variables.BotInstance.TestApiAsync()
        Catch ex As Exception
            Console.WriteLine("! Error cought while testing API: " & ex.ToString())
            Console.WriteLine("! Please check your API key and try again.")
            Environment.Exit(1)
        End Try
        Console.WriteLine("==> Starting message pump...")
        Try
            Variables.BotInstance.StartReceiving({Telegram.Bot.Types.Enums.UpdateType.ChannelPost, Telegram.Bot.Types.Enums.UpdateType.MessageUpdate})
        Catch ex As Exception
            Console.WriteLine("! Error cought while starting message pump: " & ex.ToString())
            Environment.Exit(1)
        End Try
        Console.WriteLine("==> Starting timeworker daemon...")
        Try
            Variables.timeWorker = New Threading.Thread(AddressOf Variables.TimeWorker_Run)
            Variables.timeWorker.Start()
            Console.WriteLine("==> Started. There'll be a response from TW now.")
        Catch ex As Exception
            Console.WriteLine("! Error cought while starting timeworker daemon: " & ex.ToString())
            Environment.Exit(1)
        End Try
        startupTimer.Stop()
        Console.WriteLine("==> OK! It took " & startupTimer.ElapsedMilliseconds & "ms to start.")
        Console.WriteLine("==> Now listening commands.")
GetCmd:
        Dim commands As String = Console.ReadLine()
        If commands.ToLower = "exit" Then
            Environment.Exit(0)
        End If
        If commands.ToLower = "addchannel" Then
            Try
                Console.WriteLine("==> Specify the channel ID: ")
                Dim id As Long = Console.ReadLine
                Console.WriteLine("==> Saving...")
                Dim obj As New Types.MemberChannelModel With {.HasPausedMessageForwarding = False, .KruinBriefingID = Variables.currentSettings.CurrentUID + 1, .MessagesBroadcasted = 0, .TelegramID = id}
                Dim text As String = Newtonsoft.Json.JsonConvert.SerializeObject(obj)
                Dim writer As New IO.StreamWriter(IO.File.Create(IO.Path.Combine(Variables.AppConfDir_Channels, Variables.currentSettings.CurrentUID + 1 & ".json")), System.Text.Encoding.UTF8)
                writer.Write(text)
                writer.Flush()
                writer.Dispose()
                Variables.currentSettings.CurrentUID += 1
                Methods.SaveSettings()
                Console.WriteLine("==> OK!")
            Catch ex As Exception
                Console.WriteLine("==> Error: " & ex.ToString)
            End Try
            GoTo GetCmd
        End If
        If commands.ToLower = "reload" Then
            Console.WriteLine("==> Initializing...")
            Methods.Initialize()
            Console.WriteLine("==> Done!")
            GoTo GetCmd
        End If
        If commands.ToLower = "save" Then
            Console.WriteLine("==> Saving...")
            Methods.SaveSettings()
            Console.WriteLine("==> Done!")
            GoTo GetCmd
        End If
        If commands.ToLower = "ping" Then
            Console.WriteLine("==> Sending....")
            Try
#Disable Warning
                Variables.BotInstance.SendTextMessageAsync(New Telegram.Bot.Types.ChatId(Variables.currentSettings.SupervisorID), "Hello, it's me.")
#Enable Warning
                Variables.currentStatistics.MessageSent += 1
                Console.WriteLine("==> Sent!")
            Catch ex As Exception
                Console.WriteLine("==> E: " & ex.ToString)
            End Try
            GoTo GetCmd
        End If
        If commands.ToLower = "help" Then
            Console.WriteLine("==== HELP ====")
            Console.WriteLine("Help - Display this message.")
            Console.WriteLine("Exit - Stops KruinBriefingBot.")
            Console.WriteLine("AddChannel - Add a channel to KruinBriefing project.")
            Console.WriteLine("Reload - Reload settings and stats.")
            Console.WriteLine("Save - Save configurations.")
            Console.WriteLine("Ping - Send a test message to the supervisor.")
            Console.WriteLine("Stats - Show statistics.")
            GoTo GetCmd
        End If
        If commands.ToLower = "stats" Then
            Console.WriteLine("==== STATISTICS ====")
            Console.WriteLine("As of " & Date.UtcNow.ToString & " (UTC): ")
            Console.WriteLine("Messages sent: " & Variables.currentStatistics.MessageSent)
            Console.WriteLine("Errors occurred: " & Variables.currentStatistics.ErrorsOccurred)
            Console.WriteLine("Total received channel posts: " & Variables.currentStatistics.TotalReceivedViaChannels)
            Console.WriteLine("Total forwarded channel posts: " & Variables.currentStatistics.MessagesForwarded)
            Console.WriteLine("Total received user commands: " & Variables.currentStatistics.TotalReceivedUserCommands)
            GoTo GetCmd
        End If
        Console.WriteLine("==> Invalid command.")
        GoTo GetCmd
    End Sub

    Sub OnApplicationExit(Of AssemblyLoadContext)(obj As AssemblyLoadContext)
        Console.WriteLine("==> Closing logger...")
        Variables.LogWriter.Close()
        Console.WriteLine("==> Goodbye.")
    End Sub
End Module
