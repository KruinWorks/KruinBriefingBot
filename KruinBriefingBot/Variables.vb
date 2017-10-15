Imports Telegram.Bot.Args
''' <summary>
''' Global variables.
''' </summary>
Public NotInheritable Class Variables
    ''' <summary>
    ''' Bot's exact Telegram API Key.
    ''' </summary>
    Public Shared ReadOnly Property TELEGRAM_API_KEY = "403774570:AAGROS4MfMiNnbELvZ6UI8wikFrlNtMNEQI"

    Public Shared ReadOnly Property AppPath As String = AppContext.BaseDirectory
    Public Shared ReadOnly Property AppConfDir_Base As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing")
    Public Shared ReadOnly Property AppConfDir_Channels As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "channels")
    Public Shared ReadOnly Property AppConfDir_Subscriptions As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "subscriptions")
    Public Shared ReadOnly Property AppConfDir_Logs As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "logs")
    Public Shared ReadOnly Property AppConfFile_Stats As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "stats.json")
    Public Shared ReadOnly Property AppConfFile_Conf As String = IO.Path.Combine(AppContext.BaseDirectory, "KruinBriefing", "config.json")

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

    Public Shared currentSettings As Settings
    Public Shared currentStatistics As Statistics


    Public Shared WithEvents BotInstance As Telegram.Bot.TelegramBotClient

    Public Shared Sub BotInstance_OnMessage(sender As Object, e As MessageEventArgs) Handles BotInstance.OnMessage

    End Sub
End Class
