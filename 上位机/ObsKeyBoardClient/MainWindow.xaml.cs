using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ObsKeyBoardClient.Utils;
using ObsKeyBoardClient.Utils.CustomKeyBoard;
using ObsKeyBoardClient.Utils.Settings;

namespace ObsKeyBoardClient;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CustomKeyBoardHelp _kb = CustomKeyBoardHelp.Instance;
    private readonly ObsWsHelp _obs = ObsWsHelp.Instance;

    private readonly SettingFun _settingFun = new();
    private readonly SerialPortHelp _sp = SerialPortHelp.Instance;
    private string _sceneCollection;

    private Settings? _settings = new();

    public MainWindow()
    {
        InitializeComponent();
        Init();
    }

    public string[]? ObsSceneList { get; private set; }
    public string[]? ObsSourceList { get; private set; }
    public string[]? ComList { get; private set; }

    /// <summary>
    ///     初始化
    /// </summary>
    private void Init()
    {
        //刷新串口列表
        RefreshComListComboBox();

        _ = ReadSettingsAsync();


        //设置串口错误事件
        _sp.OnErrorReceived += (o, args) => { SetComStatus(_sp.IsOpen); };

        //收到串口消息
        _sp.OnDataReceived += bytes => { _kb.DataReceived(bytes); };

        //OBS连接和断开
        _obs.Obs.Connected += (o, args) =>
        {
            SetObsServerStatus(_obs.Obs.IsConnected);
            SceneCollectionChangedEvent();
        };
        _obs.Obs.Disconnected += (o, args) => { SetObsServerStatus(_obs.Obs.IsConnected); };

        //OBS场景列表切换时
        _obs.Obs.SceneCollectionChanged += (sender, args) => { SceneCollectionChangedEvent(); };


        //按键事件
        _kb.OnCustomKey += KeyEvent;

        //编码器事件
        _kb.OnCustomEncoder += EncoderEvent;

        //旋钮电位器事件
        _kb.OnCustomPotentiometer += (p, f) => { };
    }


    /// <summary>
    ///     加载设置
    /// </summary>
    /// <returns></returns>
    private async Task ReadSettingsAsync()
    {
        _settings = await _settingFun.ReadSettingsAsync();

        if (_settings == null) return;

        //设置串口
        ComListComboBox.SelectedItem = _settings.Com;

        //设置OBS相关
        ObsServerIpTextBox.Text = _settings.ObsServerIp;
        ObsServerPortTextBox.Text = _settings.ObsServerPort.ToString();
        ObsServerPasswordPasswordBox.Password = _settings.ObsServerPassword;

        //设置键盘相关
        SetKeyScene(_settings.KeyBoardBinding?[_sceneCollection]?.KeyBinding);
        SetEncoderSource(_settings.KeyBoardBinding?[_sceneCollection]?.EncoderBinding);
    }

    /// <summary>
    ///     刷新串口列表
    /// </summary>
    private void RefreshComListComboBox()
    {
        ComList = _sp.SerialPortList;
        ComListComboBox.ItemsSource = ComList;
    }

    /// <summary>
    ///     设置串口状态显示
    /// </summary>
    /// <param name="isActive"></param>
    private void SetComStatus(bool isActive)
    {
        if (isActive)
        {
            ComKeyBoardStatusLabel.Content = "已连接";
            ComKeyBoardStatusColor.Background = new SolidColorBrush(Color.FromRgb(0, 0xFF, 0));
        }
        else
        {
            ComKeyBoardStatusLabel.Content = "未连接";
            ComKeyBoardStatusColor.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0, 0));
        }
    }

    /// <summary>
    ///     设置OBS服务状态
    /// </summary>
    /// <param name="isActive"></param>
    private void SetObsServerStatus(bool isActive)
    {
        if (isActive)
        {
            ObsServerStatusLabel.Content = "已连接";
            ObsServerStatusColor.Background = new SolidColorBrush(Color.FromRgb(0, 0xFF, 0));
        }
        else
        {
            ObsServerStatusLabel.Content = "未连接";
            ObsServerStatusColor.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0, 0));
        }
    }

    /// <summary>
    ///     OBS场景源切换时
    /// </summary>
    private void SceneCollectionChangedEvent()
    {
        if (_obs.Obs.IsConnected)
        {
            var sceneCollection = _obs.Obs.GetCurrentSceneCollection();
            _sceneCollection = sceneCollection;
            Dispatcher.Invoke(delegate
            {
                RefreshObsSceneAndSource();
                _ = ReadSettingsAsync();
            });
        }
    }

    /// <summary>
    ///     刷新OBS源
    /// </summary>
    private void RefreshObsSceneAndSource()
    {
        if (_obs.Obs.IsConnected)
        {
            ObsSceneList = _obs.Obs.GetSceneList().Scenes.Select(s => s.Name).ToArray();
            ObsSourceList = _obs.Obs.GetSourcesList().Select(s => s.Name).ToArray();
        }
        else
        {
            ObsSceneList = null;
            ObsSourceList = null;
        }

        RefreshKeyScene();
        RefreshEncoderSource();
    }

    /// <summary>
    ///     刷新编码器绑定源
    /// </summary>
    private void RefreshEncoderSource()
    {
        E00.ItemsSource = ObsSourceList;
        E01.ItemsSource = ObsSourceList;
        E02.ItemsSource = ObsSourceList;
        E03.ItemsSource = ObsSourceList;
        E04.ItemsSource = ObsSourceList;
        E05.ItemsSource = ObsSourceList;
        E06.ItemsSource = ObsSourceList;
        E07.ItemsSource = ObsSourceList;
        E08.ItemsSource = ObsSourceList;
        E09.ItemsSource = ObsSourceList;
    }

    /// <summary>
    ///     绑定编码器源
    /// </summary>
    private void SetEncoderSource(Dictionary<CustomEncoder, string?>? encoderBinding)
    {
        if (encoderBinding == null) return;

        E00.SelectedItem = encoderBinding[CustomEncoder.E00];
        E01.SelectedItem = encoderBinding[CustomEncoder.E01];
        E02.SelectedItem = encoderBinding[CustomEncoder.E02];
        E03.SelectedItem = encoderBinding[CustomEncoder.E03];
        E04.SelectedItem = encoderBinding[CustomEncoder.E04];
        E05.SelectedItem = encoderBinding[CustomEncoder.E05];
        E06.SelectedItem = encoderBinding[CustomEncoder.E06];
        E07.SelectedItem = encoderBinding[CustomEncoder.E07];
        E08.SelectedItem = encoderBinding[CustomEncoder.E08];
        E09.SelectedItem = encoderBinding[CustomEncoder.E09];
    }

    /// <summary>
    ///     刷新绑定键盘源
    /// </summary>
    private void RefreshKeyScene()
    {
        K00.ItemsSource = ObsSceneList;
        K01.ItemsSource = ObsSceneList;
        K02.ItemsSource = ObsSceneList;
        K03.ItemsSource = ObsSceneList;
        K04.ItemsSource = ObsSceneList;
        K10.ItemsSource = ObsSceneList;
        K11.ItemsSource = ObsSceneList;
        K12.ItemsSource = ObsSceneList;
        K13.ItemsSource = ObsSceneList;
        K14.ItemsSource = ObsSceneList;
        K20.ItemsSource = ObsSceneList;
        K21.ItemsSource = ObsSceneList;
        K22.ItemsSource = ObsSceneList;
        K23.ItemsSource = ObsSceneList;
    }

    /// <summary>
    ///     绑定键盘源
    /// </summary>
    private void SetKeyScene(Dictionary<CustomKey, string?>? keyBinding)
    {
        if (keyBinding == null) return;
        K00.SelectedItem = keyBinding[CustomKey.K00];
        K01.SelectedItem = keyBinding[CustomKey.K01];
        K02.SelectedItem = keyBinding[CustomKey.K02];
        K03.SelectedItem = keyBinding[CustomKey.K03];
        K04.SelectedItem = keyBinding[CustomKey.K04];
        K10.SelectedItem = keyBinding[CustomKey.K10];
        K11.SelectedItem = keyBinding[CustomKey.K11];
        K12.SelectedItem = keyBinding[CustomKey.K12];
        K13.SelectedItem = keyBinding[CustomKey.K13];
        K14.SelectedItem = keyBinding[CustomKey.K14];
        K20.SelectedItem = keyBinding[CustomKey.K20];
        K21.SelectedItem = keyBinding[CustomKey.K21];
        K22.SelectedItem = keyBinding[CustomKey.K22];
        K23.SelectedItem = keyBinding[CustomKey.K23];
    }

    /// <summary>
    ///     保存设置
    /// </summary>
    private void Save()
    {
        _settings ??= new Settings();
        //保存串口
        _settings.Com = ComListComboBox.SelectedValue.ToString();

        //保存OBS相关
        if (int.TryParse(ObsServerPortTextBox.Text, out var port) &&
            IPAddress.TryParse(ObsServerIpTextBox.Text, out var ip))
        {
            _settings.ObsServerIp = ip.ToString();
            _settings.ObsServerPort = port;
            _settings.ObsServerPassword = ObsServerPasswordPasswordBox.Password;
        }

        //保存键盘相关
        var keyBinding = new Dictionary<CustomKey, string?>
        {
            {CustomKey.K00, K00.Text},
            {CustomKey.K01, K01.Text},
            {CustomKey.K02, K02.Text},
            {CustomKey.K03, K03.Text},
            {CustomKey.K04, K04.Text},
            {CustomKey.K10, K10.Text},
            {CustomKey.K11, K11.Text},
            {CustomKey.K12, K12.Text},
            {CustomKey.K13, K13.Text},
            {CustomKey.K14, K14.Text},
            {CustomKey.K20, K20.Text},
            {CustomKey.K21, K21.Text},
            {CustomKey.K22, K22.Text},
            {CustomKey.K23, K23.Text}
        };

        //保存编码器
        var encoderBinding = new Dictionary<CustomEncoder, string?>
        {
            {CustomEncoder.E00, E00.Text},
            {CustomEncoder.E01, E01.Text},
            {CustomEncoder.E02, E02.Text},
            {CustomEncoder.E03, E03.Text},
            {CustomEncoder.E04, E04.Text},
            {CustomEncoder.E05, E05.Text},
            {CustomEncoder.E06, E06.Text},
            {CustomEncoder.E07, E07.Text},
            {CustomEncoder.E08, E08.Text},
            {CustomEncoder.E09, E09.Text}
        };


        var keyBoardBinding = new KeyBoardBinding
        {
            KeyBinding = keyBinding,
            EncoderBinding = encoderBinding
        };

        _settings.KeyBoardBinding ??= new Dictionary<string, KeyBoardBinding?>();
        _settings.KeyBoardBinding[_sceneCollection] = keyBoardBinding;

        //保存文件
        _ = _settingFun.SaveAsync(_settings);
    }

    /// <summary>
    ///     按键抬起时触发事件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="e"></param>
    private void KeyEvent(CustomKey key, CustomKeyEvent e)
    {
        if (e != CustomKeyEvent.Down) return;
        if (_obs.Obs.IsConnected && ObsSceneList != null)
            switch (key)
            {
                case CustomKey.K24:
                    _obs.Obs.TransitionToProgram();
                    return;
                default:
                    var sceneName = _settings?.KeyBoardBinding?[_sceneCollection]?.KeyBinding?[key];
                    if (!string.IsNullOrWhiteSpace(sceneName) &&
                        Array.IndexOf(ObsSceneList, sceneName) > -1)
                        if (_obs.Obs.GetStudioModeStatus())
                            _obs.Obs.SetPreviewScene(sceneName);
                        else
                            _obs.Obs.SetCurrentScene(sceneName);
                    return;
            }
    }

    private void EncoderEvent(CustomEncoder encoder, CustomEncoderEvent e)
    {
        var sourceName = _settings?.KeyBoardBinding?[_sceneCollection]?.EncoderBinding?[encoder];
        if (_obs.Obs.IsConnected && ObsSourceList != null && !string.IsNullOrWhiteSpace(sourceName) &&
            Array.IndexOf(ObsSourceList, sourceName) > -1)
            switch (e)
            {
                case CustomEncoderEvent.Add:
                    var v0 = _obs.Obs.GetVolume(sourceName).Volume + 0.02f;
                    v0 = v0 >2 ? 2 : v0;
                    _obs.Obs.SetVolume(sourceName, v0);
                    return;
                case CustomEncoderEvent.Reduce:
                    var v1 = _obs.Obs.GetVolume(sourceName).Volume - 0.02f;
                    v1 = v1 < 0 ? 0 : v1;
                    _obs.Obs.SetVolume(sourceName, v1);
                    return;
                case CustomEncoderEvent.Down:
                    _obs.Obs.SetMute(sourceName, !_obs.Obs.GetVolume(sourceName).Muted);
                    return;
            }
    }


    /// <summary>
    ///     串口列表刷新按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComListRefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshComListComboBox();
    }

    /// <summary>
    ///     连接串口按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(ComListComboBox.SelectedValue.ToString()))
        {
            _sp.Init(ComListComboBox.SelectedValue.ToString()!);
            _sp.Open();
            SetComStatus(_sp.IsOpen);
        }
    }

    /// <summary>
    ///     断开串口按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComDisConnectButton_Click(object sender, RoutedEventArgs e)
    {
        _sp.Close();
        SetComStatus(_sp.IsOpen);
    }

    /// <summary>
    ///     连接OBS按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ObsServerConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ObsServerPortTextBox.Text, out var port) &&
            IPAddress.TryParse(ObsServerIpTextBox.Text, out var ip))
            _obs.Connect(port, ip.ToString(), ObsServerPasswordPasswordBox.Password);
    }


    /// <summary>
    ///     断开OBS连接按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ObsServerDisConnectButton_Click(object sender, RoutedEventArgs e)
    {
        _obs.DisConnect();
    }

    /// <summary>
    ///     刷新按钮 刷新OBS源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ObsRefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshObsSceneAndSource();
    }


    /// <summary>
    ///     保存按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Save();
    }
}