using AHI.Infrastructure.Bus.ServiceBus.Enum;

namespace AHI.Infrastructure.Bus.ServiceBus.Abstraction
{
    public abstract class BusEvent
    {
        public abstract string TopicName { get; }
        public ActionTypeEnum ActionType { get; set; }
        public BusEvent()
        {
            ActionType = ActionTypeEnum.Updated;
        }
    }
}