using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ObsKeyBoardClient.Utils.Settings;

internal class SettingFun
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
        using StreamReader sr = File.OpenText(path);
        var s = await sr.ReadToEndAsync();
        var settings = JsonSerializer.Deserialize<Settings>(s);
        return  settings;
    }
}