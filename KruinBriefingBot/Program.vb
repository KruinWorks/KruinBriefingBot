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
        Logging.WriteConsole("==> Starting KruinBriefingBot")
        Logging.WriteConsole("==> Current version: " & Variables.AppVersion)
        Dim startupTimer As New Stopwatch
        startupTimer.Start()
        Logging.WriteConsole("==> Initializing configuration...")
        Try
            Methods.Initialize()
            Variables.LogWriter = New IO.StreamWriter(IO.File.Create(IO.Path.Combine(Variables.AppPath, "latest.log")), System.Text.Encoding.UTF8) With {.AutoFlush = True}
        Catch ex As Exception
            Logging.WriteConsole("! Error cought while initializing: " & ex.ToString(), Types.LogEventLevel.Exception)
            Console.ReadLine()
            Environment.Exit(1)
        End Try
        Logging.WriteConsole("==> Testing API...")
        Try
            Variables.BotInstance = New Telegram.Bot.TelegramBotClient(Variables.currentSettings.APIKey)
            Variables.BotInstance.TestApiAsync()
        Catch ex As Exception
            Logging.WriteConsole("! Error cought while testing API: " & ex.ToString(), Types.LogEventLevel.Exception)
            Logging.WriteConsole("! Please check your API key and try again.", Types.LogEventLevel.Exception)
            Console.ReadLine()
            Environment.Exit(1)
        End Try
        Logging.WriteConsole("==> Starting message pump...")
        Try
            Variables.BotInstance.StartReceiving({Telegram.Bot.Types.Enums.UpdateType.ChannelPost, Telegram.Bot.Types.Enums.UpdateType.MessageUpdate})
        Catch ex As Exception
            Logging.WriteConsole("! Error cought while starting message pump: " & ex.ToString(), Types.LogEventLevel.Exception)
            Console.ReadLine()
            Environment.Exit(1)
        End Try
        Logging.WriteConsole("==> Starting timeworker daemon...")
        Try
            Variables.timeWorker = New Threading.Thread(AddressOf Variables.TimeWorker_Run)
            Variables.timeWorker.Start()
            Logging.WriteConsole("==> Started. There'll be a response from TW now.")
        Catch ex As Exception
            Logging.WriteConsole("! Error cought while starting timeworker daemon: " & ex.ToString(), Types.LogEventLevel.Exception)
            Console.ReadLine()
            Environment.Exit(1)
        End Try
        startupTimer.Stop()
        Logging.WriteConsole("==> OK! It took " & startupTimer.ElapsedMilliseconds & "ms to start.")
        Logging.WriteConsole("==> Now listening commands.")
GetCmd:
        Dim commands As String = Console.ReadLine()

        If commands.ToLower = "exit" Then
            Environment.Exit(0)
        End If
        If commands.ToLower = "addchannel" Then
            Try
                Logging.WriteConsole("==> Specify the channel ID: ")
                Dim id As Long = Console.ReadLine
                Logging.WriteConsole("==> Saving...")
                Dim obj As New Types.MemberChannelModel With {.HasPausedMessageForwarding = False, .KruinBriefingID = Variables.currentSettings.CurrentUID + 1, .MessagesBroadcasted = 0, .TelegramID = id}
                Dim text As String = Newtonsoft.Json.JsonConvert.SerializeObject(obj)
                Dim writer As New IO.StreamWriter(IO.File.Create(IO.Path.Combine(Variables.AppConfDir_Channels, Variables.currentSettings.CurrentUID + 1 & ".json")), System.Text.Encoding.UTF8)
                writer.Write(text)
                writer.Flush()
                writer.Dispose()
                Variables.currentSettings.CurrentUID += 1
                Methods.SaveSettings()
                Logging.WriteConsole("==> OK!")
            Catch ex As Exception
                Logging.WriteConsole("==> Error: " & ex.ToString, Types.LogEventLevel.Exception)
            End Try
            GoTo GetCmd
        End If
        If commands.ToLower = "reload" Then
            Logging.WriteConsole("==> Initializing...")
            Methods.Initialize()
            Logging.WriteConsole("==> Done!")
            GoTo GetCmd
        End If
        If commands.ToLower = "save" Then
            Logging.WriteConsole("==> Saving...")
            Methods.SaveSettings()
            Logging.WriteConsole("==> Done!")
            GoTo GetCmd
        End If
        If commands.ToLower = "ping" Then
            Logging.WriteConsole("==> Sending....")
            Try
#Disable Warning
                Variables.BotInstance.SendTextMessageAsync(New Telegram.Bot.Types.ChatId(Variables.currentSettings.SupervisorID), "Hello, it's me.")
#Enable Warning
                Variables.currentStatistics.MessageSent += 1
                Logging.WriteConsole("==> Sent!")
            Catch ex As Exception
                Logging.WriteConsole(ex.ToString, Types.LogEventLevel.Exception)
            End Try
            GoTo GetCmd
        End If
        If commands.ToLower = "help" Then
            Logging.WriteConsole("==== HELP ====")
            Logging.WriteConsole("AddChannel - Add a channel to KruinBriefing project.")
            Logging.WriteConsole("Clear - Clear screen.")
            Logging.WriteConsole("Cls - Clear screen with a message.")
            Logging.WriteConsole("Color - Test colored output.")
            Logging.WriteConsole("Exit - Stops KruinBriefingBot.")
            Logging.WriteConsole("Help - Display this message.")
            Logging.WriteConsole("Ping - Send a test message to the supervisor.")
            Logging.WriteConsole("Reload - Reload settings and stats.")
            Logging.WriteConsole("Save - Save configurations.")
            Logging.WriteConsole("Stats - Show statistics.")

            GoTo GetCmd
        End If
        If commands.ToLower = "stats" Then
            Logging.WriteConsole("==== STATISTICS ====")
            Logging.WriteConsole("As of " & Date.UtcNow.ToString & " (UTC): ")
            Logging.WriteConsole("Messages sent: " & Variables.currentStatistics.MessageSent)
            Logging.WriteConsole("Errors occurred: " & Variables.currentStatistics.ErrorsOccurred)
            Logging.WriteConsole("Total received channel posts: " & Variables.currentStatistics.TotalReceivedViaChannels)
            Logging.WriteConsole("Total forwarded channel posts: " & Variables.currentStatistics.MessagesForwarded)
            Logging.WriteConsole("Total received user commands: " & Variables.currentStatistics.TotalReceivedUserCommands)
            GoTo GetCmd
        End If
        If commands.ToLower = "color" Then
            Dim txt As String = "The quick brown fox jumps over the lazy dog."
            Logging.WriteConsole("==> Begin of color test.")
            Logging.WriteConsole(txt, Types.LogEventLevel.Info)
            Logging.WriteConsole(txt, Types.LogEventLevel.Warning)
            Logging.WriteConsole(txt, Types.LogEventLevel.Exception)
            Logging.WriteConsole(txt, -1)
            Logging.WriteConsole("==> End of color test.")
            GoTo GetCmd
        End If
        If commands.ToLower = "cls" Or commands.ToLower = "clear" Then
            Console.Clear()
            If commands.ToLower = "cls" Then
                Logging.WriteConsole("Console has been cleared.")
            End If
            GoTo GetCmd
        End If
        Logging.WriteConsole("==> Invalid command.", Types.LogEventLevel.Warning)
        GoTo GetCmd
    End Sub

    Sub OnApplicationExit(Of AssemblyLoadContext)(obj As AssemblyLoadContext)
        Logging.WriteConsole("==> Closing logger...")
        Variables.LogWriter.Close()
        Logging.WriteConsole("==> Goodbye.")
    End Sub
End Module
