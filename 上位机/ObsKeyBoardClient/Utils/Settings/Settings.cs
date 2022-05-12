using System.Collections.Generic;

namespace ObsKeyBoardClient.Utils.Settings;

internal class Settings
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