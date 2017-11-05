Imports Telegram.Bot.Args
''' <summary>
''' Global variables.
''' </summary>
Public NotInheritable Class Variables
    Public Shared ReadOnly Property AppPath As String = AppContext.BaseDirectory
    Public Shared ReadOnly Property AppConfDir_Base As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing")
    Public Shared ReadOnly Property AppConfDir_Channels As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "channels")
    'Public Shared ReadOnly Property AppConfDir_Subscriptions As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "subscriptions")
    Public Shared ReadOnly Property AppConfDir_Logs As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "logs")
    Public Shared ReadOnly Property AppConfFile_Stats As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "stats.json")
    Public Shared ReadOnly Property AppConfFile_Conf As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "config.json")
    Public Shared ReadOnly Property AppVersion As String = "1.0.2-rs-b12"

    Public NotInheritable Class Templates
        Public Shared ReadOnly Property ConfigTemplate As String = "{
	" & ChrW(34) & "debug" & ChrW(34) & ": false,
	" & ChrW(34) & "currentUID" & ChrW(34) & ": 0,
	" & ChrW(34) & "currentSID" & ChrW(34) & ": 0
}
"

        Public Shared ReadOnly Property StatsTemplate As String = "{
	" & ChrW(34) & "messagesSent" & ChrW(34) & ": 0,
	" & ChrW(34) & "errorsOccurred" & ChrW(34) & ": 0,
	" & ChrW(34) & "totalReceivedViaChannels" & ChrW(34) & ": 0,
	" & ChrW(34) & "totalReceivedUserCommands" & ChrW(34) & ": 0,
	" & ChrW(34) & "messagesForwarded" & ChrW(34) & ": 0
}
"
        Public NotInheritable Class ErrMsg
            Public Shared ReadOnly Property IllegalInput As String = "Error: Illegal message."
            Public Shared ReadOnly Property BotException As String = "Error: Unknown error."
            Public Shared ReadOnly Property PermissionDenied As String = "Error: You're not permitted to do this."
        End Class

    End Class

    Public Shared currentSettings As Settings
    Public Shared currentStatistics As Statistics

    Public Shared timeWorker As Threading.Thread

    Public Shared LogWriter As IO.StreamWriter

    Public Shared WithEvents BotInstance As Telegram.Bot.TelegramBotClient

    Public Shared Sub TimeWorker_Run()
        'Entrance
        Logging.WriteBoth("[TW] Starting...")
        Logging.WriteConsole("[TW] Entering loop...")
        Do Until 233 = 2333
            Logging.WriteConsole("[TW] Saving statistics...")
            Try
                Methods.SaveStatsOnly()
                Logging.WriteBoth("[TW] Statistics saved.")
            Catch ex As Exception
                Logging.WriteBoth("[TW] Boom: " & ex.ToString, Types.LogEventLevel.Exception)
                Variables.currentStatistics.ErrorsOccurred += 1
            End Try
            Threading.Thread.Sleep(currentSettings.TWInterval)
        Loop
    End Sub

    Public Shared Async Sub BotInstance_OnUpdate(sender As Object, e As UpdateEventArgs) Handles BotInstance.OnUpdate
        Logging.WriteLog("Type: " & e.Update.Type.ToString)
        If e.Update.Type = Telegram.Bot.Types.Enums.UpdateType.ChannelPost Then
            Logging.WriteBoth("[C] Update: via " & e.Update.ChannelPost.Chat.Id)
            Logging.WriteLog("Broadcast: " & e.Update.ChannelPost.Text)
            Variables.currentStatistics.TotalReceivedViaChannels += 1
            If Await Methods.CheckIfInSubs(e.Update.ChannelPost.Chat.Id) Then
                Dim mainChannel As Telegram.Bot.Types.Chat = Await BotInstance.GetChatAsync(New Telegram.Bot.Types.ChatId(currentSettings.ChannelID))
                If Await Methods.CheckIfPaused(e.Update.ChannelPost.Chat.Id) Then
                    Logging.WriteBoth("Paused, ignoring.")
                    Exit Sub
                End If
                Logging.WriteBoth("Forwarding...")
                Try
                    Await BotInstance.ForwardMessageAsync(mainChannel.Id, e.Update.ChannelPost.Chat.Id, e.Update.ChannelPost.MessageId)
                Catch ex As Exception
                    Logging.WriteBoth(ex.ToString, Types.LogEventLevel.Exception)
                    Variables.currentStatistics.ErrorsOccurred += 1
                    Exit Sub
                End Try
                Variables.currentStatistics.MessageSent += 1
                Variables.currentStatistics.MessagesForwarded += 1
                Logging.WriteBoth("Succeeded.")
            End If
            ElseIf e.Update.Type = Telegram.Bot.Types.Enums.UpdateType.MessageUpdate Then
            Logging.WriteBoth("[P] Update: via " & e.Update.Message.Chat.Id)
            Logging.WriteLog("PMsg: " & e.Update.Message.Text)
            Variables.currentStatistics.TotalReceivedUserCommands += 1
            If e.Update.Message.Text.ToLower().IndexOf("/") = 0 Then
                If e.Update.Message.Text.ToLower.IndexOf("listsubs") = 1 Then
                    Dim fList() As IO.FileInfo = (New IO.DirectoryInfo(Variables.AppConfDir_Channels)).GetFiles
                    Dim text As String = "Joined channels:" & vbCrLf
                    For Each f In fList
                        Dim channel As Types.MemberChannelModel = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Types.MemberChannelModel)(IO.File.ReadAllText(f.FullName))
                        Dim channelObj As Telegram.Bot.Types.Chat = Await BotInstance.GetChatAsync(channel.TelegramID)
                        'text &= "Name: " & channelObj.FirstName & " " & channelObj.LastName & vbCrLf
                        text &= "Username: @" & channelObj.Username & vbCrLf
                        text &= "Telegram ID: " & channel.TelegramID & vbCrLf
                        text &= "KruinBriefing ID: " & channel.KruinBriefingID & vbCrLf & vbCrLf
                    Next
                    Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, text)
                    Variables.currentStatistics.MessageSent += 1
                    Logging.WriteBoth("[P] Reply sent.")
                ElseIf e.Update.Message.Text.ToLower.IndexOf("help") = 1 Then
                    Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, "Check out the available commands.")
                    Variables.currentStatistics.MessageSent += 1
                    Logging.WriteBoth("[P] Reply sent.")
                ElseIf e.Update.Message.Text.ToLower.IndexOf("start") = 1 Then
                    Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, "Welcome to KruinBriefing! Check out the available commands, or goto @KruinBriefing .")
                    Variables.currentStatistics.MessageSent += 1
                    Logging.WriteBoth("[P] Reply sent.")
                ElseIf e.Update.Message.Text.ToLower.IndexOf("status") = 1 Then
                    If e.Update.Message.Chat.Id = Variables.currentSettings.SupervisorID Then
                        Try
                            Dim status As String = ""
                            Dim memobj As Process = Process.GetCurrentProcess
                            status &= "Bot Status: " & vbCrLf
                            status &= "Device: " & Environment.MachineName & vbCrLf
                            status &= "OS: " & Runtime.InteropServices.RuntimeInformation.OSDescription & vbCrLf
                            status &= "Uptime: " & New TimeSpan(0, 0, 0, 0, Environment.TickCount).ToString() & vbCrLf
                            status &= "Process Memory Usage: " & Math.Round(memobj.WorkingSet64 / 1024 / 1024, 4) & "MiB." & vbCrLf
                            status &= "Remote Time(UTC): " & Date.UtcNow.ToString() & vbCrLf
                            Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, status)
                            Variables.currentStatistics.MessageSent += 1
                            Logging.WriteBoth("[P] Reply sent.")
                        Catch ex As Exception
#Disable Warning
                            BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, Templates.ErrMsg.BotException)
#Enable Warning
                            Variables.currentStatistics.ErrorsOccurred += 1
                            Variables.currentStatistics.MessageSent += 1
                            Logging.WriteLog(ex.ToString, Types.LogEventLevel.Exception)
                            Logging.WriteConsole(ex.ToString, Types.LogEventLevel.Exception)
                        End Try
                    Else
                        Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, Templates.ErrMsg.PermissionDenied)
                        Variables.currentStatistics.MessageSent += 1
                        Logging.WriteBoth("[P] Denial reply sent.", Types.LogEventLevel.Warning)
                    End If
                ElseIf e.Update.Message.Text.ToLower.IndexOf("stats") = 1 Then
                    If e.Update.Message.Chat.Id = Variables.currentSettings.SupervisorID Then
                        Try
                            Dim stats As String = ""
                            stats &= "As of " & Date.UtcNow.ToString & " (UTC): " & vbCrLf
                            stats &= "Messages sent: " & Variables.currentStatistics.MessageSent & vbCrLf
                            stats &= "Errors occurred: " & Variables.currentStatistics.ErrorsOccurred & vbCrLf
                            stats &= "Total received channel posts: " & Variables.currentStatistics.TotalReceivedViaChannels & vbCrLf
                            stats &= "Total forwarded channel posts: " & Variables.currentStatistics.MessagesForwarded & vbCrLf
                            stats &= "Total received user commands: " & Variables.currentStatistics.TotalReceivedUserCommands & vbCrLf
                            Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, stats)
                            Variables.currentStatistics.MessageSent += 1
                            Logging.WriteBoth("[P] Reply sent.")
                        Catch ex As Exception
#Disable Warning
                            BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, Templates.ErrMsg.BotException)
#Enable Warning
                            Variables.currentStatistics.ErrorsOccurred += 1
                            Variables.currentStatistics.MessageSent += 1
                            Logging.WriteBoth(ex.ToString, Types.LogEventLevel.Exception)
                        End Try
                    Else
                        Await BotInstance.SendTextMessageAsync(e.Update.Message.Chat.Id, Templates.ErrMsg.PermissionDenied)
                        Variables.currentStatistics.MessageSent += 1
                        Logging.WriteBoth("[P] Denial reply sent.", Types.LogEventLevel.Warning)
                    End If
                End If
            End If
        End If
    End Sub


    Public Shared Sub BotInstance_OnReceiveError(sender As Object, e As ReceiveErrorEventArgs) Handles BotInstance.OnReceiveError
        Logging.WriteBoth(e.ApiRequestException.ToString(), Types.LogEventLevel.Exception)
        Variables.currentStatistics.ErrorsOccurred += 1
    End Sub
End Class
