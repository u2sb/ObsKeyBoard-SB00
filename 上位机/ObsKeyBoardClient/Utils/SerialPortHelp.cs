using System;
using RJCP.IO.Ports;

namespace ObsKeyBoardClient.Utils;

internal class SerialPortHelp : SingletonClass<SerialPortHelp>
{
    private readonly SerialPortStream _sp;

    public Action<byte[]>? OnDataReceived;
    public Action<object, EventArgs>? OnErrorReceived;

    public SerialPortHelp()
    {
        _sp = new SerialPortStream();

        _sp.DataReceived += (sender, args) =>
        {
            var size = _sp.BytesToRead;
            var buff = new byte[size];
            _ = _sp.Read(buff, 0, size);
            OnDataReceived?.Invoke(buff);
        };

        _sp.ErrorReceived += (sender, args) => { OnErrorReceived?.Invoke(sender, args); };
    }

    /// <summary>
    ///     是否打开
    /// </summary>
    public bool IsOpen
    {
        get
        {
            lock (Lock)
            {
                return _sp.IsOpen;
            }
        }
    }

    /// <summary>
    ///     串口列表
    /// </summary>
    public string[] SerialPortList => SerialPortStream.GetPortNames();

    ~SerialPortHelp()
    {
        Dispose(false);
    }

    /// <summary>
    ///     初始化
    /// </summary>
    /// <param name="portName"></param>
    /// <param name="baudRate"></param>
    public void Init(string portName, int baudRate = 115200)
    {
        lock (Lock)
        {
            if (_sp.IsOpen) _sp.Close();

            _sp.PortName = portName;
            _sp.BaudRate = baudRate;
        }
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="data"></param>
    public void Send(byte[] data)
    {
        lock (Lock)
        {
            _sp.WriteAsync(data);
        }
    }

    /// <summary>
    ///     开启
    /// </summary>
    public void Open()
    {
        lock (Lock)
        {
            if (_sp.IsOpen) _sp.Close();

            _sp.Open();
        }
    }

    /// <summary>
    ///     关闭
    /// </summary>
    public void Close()
    {
        lock (Lock)
        {
            if (_sp.IsOpen) _sp.Close();
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        if (!IsDisposed)
        {
            if (isDisposing)
            {
                Close();
                lock (Lock)
                {
                    _sp.Dispose();
                }
            }

            IsDisposed = true;
        }
    }
}