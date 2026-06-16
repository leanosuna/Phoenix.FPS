using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Phoenix.FPS.Client;

public static class CfgFile
{
    const string _path = "CFG.json";
    public static bool Load(out CFG classFile)
    {
        if (!File.Exists(_path))
        {
            classFile = default!;
            return false;
        }
        classFile = JsonSerializer.Deserialize<CFG>(
            File.ReadAllText(_path)
        )!;
        return true;
    }

    public static void Save(CFG classFile)
    {
        File.WriteAllText(
            _path,
            JsonSerializer.Serialize(classFile, 
            new JsonSerializerOptions
            {
                WriteIndented = true
            })
        );
    }


}
