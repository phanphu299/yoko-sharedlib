namespace AHI.Infrastructure.AzureMqttTriggerExtension.Messaging
{
    public enum MqttRetainHandling
    {
        SendAtSubscribe = 0,
        SendAtSubscribeIfNewSubscriptionOnly = 1,
        DoNotSendOnSubscribe = 2,
        NotSet
    }
}
