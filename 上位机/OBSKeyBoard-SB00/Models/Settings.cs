using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OBSKeyBoard_SB00.Models;

public class Settings
{
    /// <summary>
    ///     串口设置
    /// </summary>
    public string? Com { get; set; }

    /// <summary>
    ///     OBS IP
    /// </summary>
    public string ObsServerIp { get; set; } = "127.0.0.1";

    /// <summary>
    ///     OBS 端口
    /// </summary>
    public int ObsServerPort { get; set; } = 4444;

    /// <summary>
    ///     OBS 密码
    /// </summary>
    public string ObsServerPassword { get; set; } = "";

    /// <summary>
    ///     键盘绑定
    /// </summary>
    public Dictionary<string, KeyBoardBinding?>? KeyBoardBinding { get; set; }
}

public class KeyBoardBinding
{
    public Dictionary<CustomKey, string?>? KeyBinding { get; set; }
    public Dictionary<CustomEncoder, string?>? EncoderBinding { get; set; }
    public Dictionary<CustomPotentiometer, string?>? PotentiometerBinding { get; set; }
}

public class SettingFun
{
    /// <summary>
    ///     保存设置
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync(Settings s, string path = "./settings.json")
    {
        await using var fs = new FileStream(path, FileMode.OpenOrCreate);
        var a = JsonSerializer.SerializeToUtf8Bytes(s);
        await fs.WriteAsync(a, 0, a.Length);
    }

    public async Task<Settings?> ReadSettingsAsync(string path = "./settings.json")
    {
        using var sr = File.OpenText(path);
        var s = await sr.ReadToEndAsync();
        var settings = JsonSerializer.Deserialize<Settings>(s);
        return settings;
    }
}