''' <summary>
''' Object models of KruinBriefingBot.
''' </summary>
Public NotInheritable Class Types
    ''' <summary>
    ''' Model of Telegram channels.
    ''' </summary>
    Public Class MemberChannelModel
        ''' <summary>
        ''' Indicates whether the channel owner has paused message forwarding or not.
        ''' </summary>
        ''' <returns></returns>
        Public Property HasPausedMessageForwarding As Boolean = False
        ''' <summary>
        ''' The KruinBriefing unique ID of the channel.
        ''' </summary>
        ''' <returns></returns>
        Public Property KruinBriefingID As Integer = -1
        ''' <summary>
        ''' The Telegram Chat ID of the Telegram channel.
        ''' </summary>
        ''' <returns></returns>
        Public Property TelegramID As Long = -1
        ''' <summary>
        ''' Forwarded messages count. (To KruinBriefing Channel)
        ''' </summary>
        ''' <returns></returns>
        Public Property MessagesBroadcasted As Integer = -1
    End Class
    ''' <summary>
    ''' Model of KruinBriefing subscriptions.
    ''' </summary>
    Public Class SubscriptionModel
        ''' <summary>
        ''' The Telegram Chat ID of the subscription.
        ''' </summary>
        ''' <returns></returns>
        Public Property ChatId As Long
        ''' <summary>
        ''' Indicates whether the channel owner has paused message forwarding or not.
        ''' </summary>
        ''' <returns></returns>
        Public Property HasPausedMessageForwarding As Boolean = False
        ''' <summary>
        ''' Forwarded messages count.
        ''' </summary>
        ''' <returns></returns>
        Public Property MessagesForwarded As Integer = -1
        ''' <summary>
        ''' Subscribed channels of this subscription.
        ''' </summary>
        ''' <returns></returns>
        Public Property SubscribedChannels As List(Of MemberChannelModel)
    End Class
    ''' <summary>
    ''' Indicates how serious this log entry is.
    ''' </summary>
    Public Enum LogEventLevel
        ''' <summary>
        ''' Everything goes well.
        ''' </summary>
        Info = 0
        ''' <summary>
        ''' Something wrong may happen.
        ''' </summary>
        Warning = 1
        ''' <summary>
        ''' Something REALLY wrong happened.
        ''' </summary>
        Exception = 2
    End Enum
End Class
