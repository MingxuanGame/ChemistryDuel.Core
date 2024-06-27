namespace ChemistryDuel.Core.Mods;

public record ModMetadata(
    string Name,
    string Id,
    string Version, // Semver
    string[] Authors,
    string Description,
    string? Website = null,
    string? Repository = null,
    string? License = null
);

public record FileModMetadata(
    string Name,
    string Id,
    string Version,
    string[] Authors,
    string Description,
    string Entry,
    string EntryDll,
    string? Assets = null,
    string? Website = null,
    string? Repository = null,
    string? License = null)
    : ModMetadata(Name, Id, Version, Authors, Description, Website, Repository, License)
{
    public readonly string EntryDll = EntryDll;
    public readonly string Entry = Entry;
    public readonly string? Assets = Assets;
}