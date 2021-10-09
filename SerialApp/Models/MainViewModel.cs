using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using SerialApp.Models;
using System.IO.Ports;
using System.Threading;
namespace SerialApp.Models
{
    public class MainViewModel: BaseDependencyObject
    {
        private string _messageToSend = "";
        public string MessageToSend
        {
            get { return _messageToSend; }
            set { SetProperty(ref _messageToSend, value, "MessageToSend"); }
        }
        private string _messageReceived = "";

        public string MessageReceived
        {
            get { return _messageReceived; }
            set { SetProperty(ref _messageReceived, value, "MessageReceived"); }
        }
        private string _status = "";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, "Status"); }
        }
        public ICommand OpenCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ReceiveCommand { get; set; }

        private static SerialPort _port;
        bool _continue = false;
        string buffer = "";
        CancellationTokenSource _ctsListen = null;

        public MainViewModel()
        {

            _port = new SerialPort();
            _port.PortName = "COM6"; //you can get available ports through SerialPort.GetPortNames()
            _port.BaudRate = 9600;
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.One;
            _port.Handshake = Handshake.None;
            _port.DataBits = 8;

            defineCommands();
        }


        private void defineCommands()
        {
            OpenCommand = new Command(execute: () => {
                if (_ctsListen != null)
                    if (!_ctsListen.IsCancellationRequested)
                        _ctsListen.Cancel();
                if (IsPortAvailable())
                {
                    if (!_port.IsOpen)
                    {
                        _port.Open();
                        Status = "Port is now open.";
                    }
                }
                else
                    Status = "Port not available.";
            });
            CloseCommand = new Command(execute: () => {
                if (_ctsListen != null)
                    if (!_ctsListen.IsCancellationRequested)
                        _ctsListen.Cancel();
                if (IsPortAvailable())
                {
                    if (_port.IsOpen)
                    {
                        _port.Close();
                        Status = "Port is now closed.";
                    }
                }
                else
                    Status = "Port not available";
            });
            SendCommand = new Command(execute: () => {
                if (_ctsListen != null)
                    if (!_ctsListen.IsCancellationRequested)
                        _ctsListen.Cancel();
                if (IsPortAvailable())
                {
                    if (_port.IsOpen)
                    {
                        _port.WriteLine(MessageToSend);
                        Status = "Message sent.";
                    }
                    else
                        Status = "Port is closed.";
                }
                else
                    Status = "Port not available";
            });
            ReceiveCommand = new Command(execute: () => {
                if (_continue)
                    return;
                if (IsPortAvailable())
                {
                    if (_port.IsOpen)
                    {
                        if (_ctsListen == null)
                            _ctsListen = new CancellationTokenSource();
                        if (_ctsListen.IsCancellationRequested)
                        {
                            _ctsListen.Cancel();
                            _ctsListen = new CancellationTokenSource();
                        }
                        Task t = Task.Factory.StartNew(async () => {
                            while (!_ctsListen.IsCancellationRequested)
                            {
                                MessageReceived += Environment.NewLine +  _port.ReadLine();
                                await Task.Delay(10);
                                _continue = true;
                            }
                            _continue = false;
                        });
                    }
                    else
                        Status = "Port is closed.";

                }
                else
                    Status = "Port not available";
            });



        }
        private bool IsPortAvailable()
        {
            return true;// SerialPort.GetPortNames().Any(p => p.ToUpper().Equals(_port.PortName.ToUpper())); 
        }

    }
}
