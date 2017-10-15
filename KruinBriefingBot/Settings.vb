Public Class Settings
    Public Property IsDebuggingMode As Boolean
    Public Property CurrentUID As Long
    Public Property CurrentSID As Long
End Class

Public Class Statistics
    Public Property MessageSent As Long
    Public Property ErrorsOccurred As Long
    Public Property TotalReceivedViaChannels As Long
    Public Property TotalReceivedUserCommands As Long
    Public Property MessagesForwarded As Long
End Class


Public NotInheritable Class Methods
    Public Shared Sub Initialize()
        If Not IO.Directory.Exists(Variables.AppConfDir_Base) Then
            IO.Directory.CreateDirectory(Variables.AppConfDir_Base)
        End If
        If Not IO.Directory.Exists(Variables.AppConfDir_Channels) Then
            IO.Directory.CreateDirectory(Variables.AppConfDir_Channels)
        End If
        If Not IO.Directory.Exists(Variables.AppConfDir_Logs) Then
            IO.Directory.CreateDirectory(Variables.AppConfDir_Logs)
        End If
        If Not IO.Directory.Exists(Variables.AppConfDir_Subscriptions) Then
            IO.Directory.CreateDirectory(Variables.AppConfDir_Subscriptions)
        End If
        If Not IO.File.Exists(Variables.AppConfFile_Conf) Then
            Variables.currentSettings = New Settings With {.CurrentSID = 0, .CurrentUID = 0, .IsDebuggingMode = False}
            Dim text As String = Newtonsoft.Json.JsonConvert.SerializeObject(Variables.currentSettings)
            Dim writer As New IO.StreamWriter(IO.File.Create(Variables.AppConfFile_Conf), System.Text.Encoding.UTF8)
            writer.Write(text)
            writer.Flush()
            writer.Dispose()
        End If
        If Not IO.File.Exists(Variables.AppConfFile_Stats) Then
            Variables.currentStatistics = New Statistics With {.ErrorsOccurred = 0, .MessageSent = 0, .MessagesForwarded = 0, .TotalReceivedUserCommands = 0, .TotalReceivedViaChannels = 0}
            Dim text As String = Newtonsoft.Json.JsonConvert.SerializeObject(Variables.currentStatistics)
            Dim writer As New IO.StreamWriter(IO.File.Create(Variables.AppConfFile_Stats), System.Text.Encoding.UTF8)
            writer.Write(text)
            writer.Flush()
            writer.Dispose()
        End If
        ReadSettings()
    End Sub

    Public Shared Sub ReadSettings()
        Dim settingsText As String = IO.File.ReadAllText(Variables.AppConfFile_Conf)
        Variables.currentSettings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Settings)(settingsText)
        Dim statsText As String = IO.File.ReadAllText(Variables.AppConfFile_Stats)
        Variables.currentStatistics = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Statistics)(statsText)
    End Sub

    Public Shared Sub SaveSettings()
        Dim text As String = Newtonsoft.Json.JsonConvert.SerializeObject(Variables.currentStatistics)
        Dim writer As New IO.StreamWriter(IO.File.Create(Variables.AppConfFile_Stats), System.Text.Encoding.UTF8)
        writer.Write(text)
        writer.Flush()
        writer.Dispose()
        Dim stext As String = Newtonsoft.Json.JsonConvert.SerializeObject(Variables.currentSettings)
        Dim swriter As New IO.StreamWriter(IO.File.Create(Variables.AppConfFile_Conf), System.Text.Encoding.UTF8)
        writer.Write(text)
        writer.Flush()
        writer.Dispose()
    End Sub
End Class