using System;
using System.IO;
using System.Reflection;
using FluentResults;
using Newtonsoft.Json;
using NvidiaDisplayController.Objects;
using NvidiaDisplayController.Objects.Entities;

namespace NvidiaDisplayController.Global.Controllers;

public class DataController
{
    private static readonly string _location = Assembly.GetExecutingAssembly().Location;
    private static readonly string? _directory = Path.GetDirectoryName(_location);
    private static string GetUserDataFolder()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NvidiaDisplayController");
        try
        {
            Directory.CreateDirectory(folder);
        }
        catch
        {
            // best-effort; if creation fails we'll fallback to install dir below
        }

        return folder;
    }

    public string DataPath
    {
        get
        {
            var userDataPath = Path.Combine(GetUserDataFolder(), "Data.json");

            if (!File.Exists(userDataPath))
            {
                // Try to copy the installed default (if present in install folder)
                try
                {
                    if (_directory != null)
                    {
                        var installed = Path.Combine(_directory, "Data", "Data.json");
                        if (File.Exists(installed))
                        {
                            File.Copy(installed, userDataPath);
                        }
                    }
                }
                catch
                {
                    // ignore copy failures
                }

                // If still missing, create a minimal default file so app can write to it later
                if (!File.Exists(userDataPath))
                {
                    try
                    {
                        var defaultComputer = new Computer();
                        var defaultJson = JsonConvert.SerializeObject(defaultComputer, Formatting.Indented, _jsonSettings);
                        File.WriteAllText(userDataPath, defaultJson);
                    }
                    catch
                    {
                        // If this fails, fall back to the install location path which may be read-only.
                        if (_directory != null)
                            return Path.Combine(_directory, "Data", "Data.json");
                        throw;
                    }
                }
            }

            return userDataPath;
        }
    }

    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ObjectCreationHandling = ObjectCreationHandling.Replace
    };

    public void Write(Computer data)
    {
        var serializeObject = JsonConvert.SerializeObject(data, Formatting.Indented, _jsonSettings);

        File.WriteAllText(DataPath, serializeObject);
    }

    public Result<Computer> Load()
    {
        using StreamReader reader = new(DataPath);
        {
            var json = reader.ReadToEnd();

            var computer = JsonConvert.DeserializeObject<Computer>(json, _jsonSettings);
            reader.Close();

            if (computer is not null)
            {
                foreach (var monitor in computer.Monitors)
                {
                    foreach (var profile in monitor.Profiles)
                    {
                        profile.Monitor = monitor;
                    }
                }
            }

            var result = computer is null ? Result.Fail(new Error("Failed to parse data file.")) : Result.Ok(computer);
            return result;
        }
    }
}