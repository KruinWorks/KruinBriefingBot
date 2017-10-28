Public NotInheritable Class Logging
    Public Shared Function GetTimePrefix() As String
        Dim currentTime As Date = Date.UtcNow
        Return "[" & currentTime.ToString() & "]"
    End Function
    Public Shared Function GetExtendedLogString(text As String, Optional level As Types.LogEventLevel = Types.LogEventLevel.Info) As String
        Dim prefixLevel As String
        Select Case level
            Case Types.LogEventLevel.Info
                prefixLevel = "[INFO]"
            Case Types.LogEventLevel.Warning
                prefixLevel = "[WARN]"
            Case Types.LogEventLevel.Exception
                prefixLevel = "[ERROR]"
            Case Else
                prefixLevel = "[UKNN]"
        End Select
        Return (GetTimePrefix() & prefixLevel & " " & text)
    End Function
    Public Shared Sub WriteLog(text As String, Optional level As Types.LogEventLevel = Types.LogEventLevel.Info)
        Variables.LogWriter.WriteLine(GetExtendedLogString(text, level))
    End Sub
    Public Shared Sub WriteConsole(text As String, Optional level As Types.LogEventLevel = Types.LogEventLevel.Info)
        Dim originalF As ConsoleColor = Console.ForegroundColor
        Dim originalB As ConsoleColor = Console.BackgroundColor
        Select Case level
            Case Types.LogEventLevel.Info
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.BackgroundColor = originalB
            Case Types.LogEventLevel.Warning
                Console.ForegroundColor = ConsoleColor.Black
                Console.BackgroundColor = ConsoleColor.Yellow
            Case Types.LogEventLevel.Exception
                Console.ForegroundColor = ConsoleColor.White
                Console.BackgroundColor = ConsoleColor.Red
            Case Else
                Console.ForegroundColor = ConsoleColor.White
                Console.BackgroundColor = ConsoleColor.DarkGray
        End Select
        Console.WriteLine(GetExtendedLogString(text, level))
        Console.ForegroundColor = originalF
        Console.BackgroundColor = originalB
    End Sub
    Public Shared Sub WriteBoth(text As String, Optional level As Types.LogEventLevel = Types.LogEventLevel.Info)
        WriteConsole(text, level)
        WriteLog(text, level)
    End Sub
End Class
