using System.Threading.Tasks;
using ChemistryDuel.Core.Game;
namespace ChemistryDuel.Core.Event;

public EventBus{
    public delegate ValueTask EventAsyncCallBackHandler<in TEventArgs>(Game game, TEventArgs eventArgs)
        where TEventArgs : System.EventArgs;

    
}
