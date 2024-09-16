using HuesosQuickloader.Extensions;
using HuesosQuickloader.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HuesosQuickloader.Services.Config;

public sealed class ConfigService : IConfigService
{
    private const string _folderName = "HuesosQuickloader";
    private const string _configName = "config.json";

    public string GetPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).CombineWith(_folderName, _configName);
    }

    public void Initialize()
    {
        var path = GetPath();
        var dir = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (!File.Exists(path))
        {
            CreateDefault(path);
        }
    }

    public AppConfig Read()
    {
        var path = GetPath();

        if (!File.Exists(path))
            CreateDefault(path);

        var data = File.ReadAllText(path);
        var deserialized = JsonConvert.DeserializeObject<AppConfig>(data);

        if (deserialized is null)
        {
            CreateDefault(path);
            return Read();
        }

        return deserialized;
    }

    public void Write(AppConfig config)
    {
        var path = GetPath();
        var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(path, serialized);
    }

    private void CreateDefault(string path)
    {
        var serialized = JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented);
        File.WriteAllText(path, serialized);
    }
}
