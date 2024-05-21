using ChemistryDuel.Core.Game
namespace ChemistryDuel.Core.Event;

public class ChemistryDuelEvent : EventArgs
{
    public ChemistryDuelEvent(Game game)
    {
        Game = game;
    }

    public string Game { get; set; }
}
