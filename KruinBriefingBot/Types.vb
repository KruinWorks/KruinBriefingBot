''' <summary>
''' Object models of KruinBriefingBot.
''' </summary>
Public NotInheritable Class Types
    ''' <summary>
    ''' Model of Telegram channels.
    ''' </summary>
    Public Class MemberChannelModel
        ''' <summary>
        ''' Initialize a MemberChannelModel object.
        ''' </summary>
        ''' <param name="Channel">Required. The member channel's Chat object.</param>
        Sub New(Channel As Telegram.Bot.Types.Chat)
            If Not Channel.Type = Telegram.Bot.Types.Enums.ChatType.Channel Then
                Throw New System.InvalidOperationException("Specified chat is not a channel.")
            End If
            TelegramID = Channel.Id
        End Sub
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
        ''' Initialize a SubscriptionModel.
        ''' 
        ''' Attention: The SubscribedChannels object will be null after initalization.
        ''' </summary>
        ''' <param name="chat">Required. The subscription [(Super)Group / Private Chat]'s Chat object.</param>
        Sub New(chat As Telegram.Bot.Types.Chat)
            ChatId = chat.Id
        End Sub
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
End Class
