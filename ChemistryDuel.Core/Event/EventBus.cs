namespace ChemistryDuel.Core.Event;

public class EventBus<TEvent> where TEvent : EventBase
{
    public delegate Task EvHandler(TEvent ev);

    private readonly Dictionary<Type, List<EvHandler>> _eventDic = new();


    public void Subscribe<TSubEvent>(Action<TSubEvent> eventHandler) where TSubEvent : TEvent
    {
        var eventType = typeof(TSubEvent);
        if (!_eventDic.TryGetValue(eventType, out List<EvHandler>? value))
        {
            _eventDic.Add(eventType, new List<EvHandler>
            {
                ev =>
                {
                    eventHandler((TSubEvent)ev);
                    return Task.CompletedTask;
                }
            });
        }
        else
        {
            var handlers = value;
            handlers.Add(ev =>
            {
                eventHandler((TSubEvent)ev);
                return Task.CompletedTask;
            });
        }
    }

    // public void Unsubscribe<TSubEvent>(Action<TSubEvent> eventHandler) where TSubEvent : TEvent
    // {
    //     var eventType = typeof(TSubEvent);
    //     if (_eventDic.TryGetValue(eventType, out List<EvHandler>? value))
    //     {
    //         var handlers = value;
    //         handlers.RemoveAll(ev => ev.Method == eventHandler.Method);
    //     }
    // }

    public async void Publish(TEvent ev)
    {
        var eventType = ev.GetType();
        List<Type> types = new() { eventType };
        while (types[0].BaseType != typeof(EventBase))
        {
            var baseType = types[0].BaseType;
            if (baseType != null) types.Insert(0, baseType);
        }

        List<EvHandler> handlers = new List<EvHandler>();
        foreach (var type in types)
        {
            if (_eventDic.TryGetValue(type, out List<EvHandler>? value))
            {
                handlers.AddRange(value);
            }
        }

        await Task.WhenAll(handlers.Select(handler => handler(ev)));
    }
}