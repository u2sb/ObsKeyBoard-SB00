using System;
using System.Linq;
using System.Threading;
using RJCP.IO.Ports;

namespace OBSKeyBoard_SB00.Utils;

public class SerialPortHelp : Singleton<SerialPortHelp>
{
    private readonly byte[] _data = new byte[8192];
    private readonly ISerialPortStream _sp;

    private int _dataIndex;
    private int _dataIndex2;

    public Action<byte[]>? OnDataReceived;
    public Action<object?, EventArgs>? OnErrorReceived;

    public SerialPortHelp()
    {
        _sp = new SerialPortStream();

        ThreadStart ts = WatchDataChanged;
        var watchThread = new Thread(ts);
        watchThread.Start();

        _sp.DataReceived += (sender, args) =>
        {
            var size = _sp.BytesToRead;
            var buff = new byte[size];
            _ = _sp.Read(buff, 0, size);

            Array.Copy(buff, 0, _data, _dataIndex, size);
            _dataIndex += size;
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


    public void WatchDataChanged()
    {
        for (;;)
        {
            if (_dataIndex != 0)
                lock (Lock)
                {
                    if (_dataIndex2 == _dataIndex)
                    {
                        OnDataReceived?.Invoke(_data.Take(_dataIndex).ToArray());
                        _dataIndex = 0;
                        _dataIndex2 = 0;
                        Array.Clear(_data);
                    }
                    else
                    {
                        _dataIndex2 = _dataIndex;
                    }
                }

            Thread.Sleep(10);
        }
    }

    ~SerialPortHelp()
    {
        Close();
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
            _sp.WriteAsync(data, 0, data.Length);
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