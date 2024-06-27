using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;

namespace ChemistryDuel.Core.Mods;

public class ModManager
{
    private readonly List<ModMetadata> _mods = new();
    private readonly Dictionary<string, IModLoader> _loaders = new();

    public void LoadFromFile(string zipFile)
    {
        if (!File.Exists(zipFile))
        {
            throw new FileNotFoundException("The specified file does not exist.", zipFile);
        }

        using var archive = ZipFile.OpenRead(zipFile);

        FileModMetadata? metadata = null;
        var metadataEntry = archive.GetEntry(Const.ModMetadataFileName);
        if (metadataEntry == null)
        {
            throw new LoadException("The specified archive does not contain a metadata file.");
        }

        using var stream = metadataEntry.Open();
        using var reader = new StreamReader(stream);
        metadata = JsonConvert.DeserializeObject<FileModMetadata>(reader.ReadToEnd());
        if (metadata == null)
        {
            throw new LoadException("Failed to deserialize metadata.");
        }

        // uncompress assets
        if (metadata.Assets != null)
        {
            var assetsPath = Path.Combine(Const.ModAssetDictionaryPath, metadata.Id);
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.StartsWith(metadata.Assets))
                    {
                        var path = Path.Combine(assetsPath, entry.FullName.Replace(metadata.Assets, ""));
                        entry.ExtractToFile(path, false);
                    }
                }
            }
        }

        // load mod
        var entryDll = archive.GetEntry(metadata.EntryDll);
        if (entryDll == null)
        {
            throw new LoadException("The specified archive does not contain the entry dll.");
        }

        var assembly = Assembly.LoadFrom(entryDll.FullName);
        var type = assembly.GetType(metadata.Entry);
        if (type == null)
        {
            throw new LoadException("The specified entry type does not exist.");
        }

        if (!type.GetInterfaces().Contains(typeof(IModLoader)))
        {
            throw new LoadException("The specified entry type does not implement IModLoader.");
        }

        var loader = (IModLoader)Activator.CreateInstance(type)!;
        Load(metadata, loader);
    }

    public void Load(ModMetadata metadata, IModLoader loader)
    {
        loader.Load();
        _mods.Add(metadata);
        _loaders.Add(metadata.Id, loader);
    }

    public void Unload(string id)
    {
        if (!_loaders.TryGetValue(id, out IModLoader? loader)) return;
        loader.Unload();
        _mods.RemoveAll(mod => mod.Id == id);
        _loaders.Remove(id);
    }
}