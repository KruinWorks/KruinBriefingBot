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
        Console.WriteLine("==> Starting KruinBriefingBot")
        Dim startupTimer As New Stopwatch
        startupTimer.Start()
        Console.WriteLine("==> Testing API...")
        Variables.BotInstance = New Telegram.Bot.TelegramBotClient(Variables.TELEGRAM_API_KEY)
        Try
            Variables.BotInstance.TestApiAsync()
        Catch ex As Exception
            Console.WriteLine("! Error cought while testing API: " & ex.ToString())
            Environment.Exit(1)
        End Try
        Console.WriteLine("==> Move on, initializing configuration...")
        Try
            Methods.Initialize()
        Catch ex As Exception
            Console.WriteLine("! Error cought while initializing: " & ex.ToString())
            Environment.Exit(1)
        End Try
        Console.WriteLine("==> Starting message pump...")
        Try
            Variables.BotInstance.StartReceiving()
        Catch ex As Exception
            Console.WriteLine("! Error cought while starting message pump: " & ex.ToString())
            Environment.Exit(1)
        End Try
        startupTimer.Stop()
        Console.WriteLine("==> OK! It took " & startupTimer.ElapsedMilliseconds & "ms to start.")
        Console.Write(">")
        Dim commands As String = Console.ReadLine()
    End Sub
End Module
