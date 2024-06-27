namespace ChemistryDuel.Core.Mods;

public class ModException(string message) : Exception(message);

public class LoadException(string message) : ModException(message);