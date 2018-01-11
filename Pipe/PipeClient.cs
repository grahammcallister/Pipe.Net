using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Pipe
{
    public class PipeClient : IPipe
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private NamedPipeClientStream _pipe;
        private PipeMessage _pipeMessage;

        public PipeClient(string pipeName = "Pipe.Server")
        {
            PipeName = pipeName;
            Encoding = Encoding.UTF8;
            Connected = false;
            NewPipe();
            _backgroundWorker.DoWork += AsyncConnect;
            _backgroundWorker.RunWorkerCompleted += AsyncConnectCompleted;
        }

        public Encoding Encoding { get; set; }

        public bool Connected { get; set; }

        public string PipeName { get; set; }

        public void Open()
        {
            if (!Connected && !_backgroundWorker.IsBusy) _backgroundWorker.RunWorkerAsync();
        }

        public void Close()
        {
            Connected = false;
            if (_pipe.IsConnected)
            {
                _pipe.WaitForPipeDrain();
                _pipe.Close();
            }

            NewPipe();
            OnDisconnect();
        }

        public event ConnectEvent Connect;

        public event DisconnectEvent Disconnect;

        public event MessageEvent MessageReceived;

        public void SendMessage(string message)
        {
            if (_pipe.IsConnected)
            {
                Connected = true;
                var msg = new PipeMessage(message);
                _pipe.BeginWrite(msg.MessageBytes, 0, PipeMessage.MessageBufferSize, AsyncBeginWriteCallback, _pipe);
            }
            else
            {
                Connected = false;
                throw new Exception("Not connected to pipe.");
            }
        }

        private void NewPipe()
        {
            _pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        private void AsyncConnect(object sender, DoWorkEventArgs e)
        {
            while (!_pipe.IsConnected)
                try
                {
                    Connected = _pipe.IsConnected;
                    _pipe.Connect(200);
                }
                catch (IOException iox)
                {
                    Debug.WriteLine(iox.Message);
                    Connected = false;
                    if (iox.Message.Contains("expired")) continue;
                    throw;
                }
                catch (ObjectDisposedException ode)
                {
                    Debug.WriteLine(ode.Message);
                    Close();
                    break;
                }
        }

        private void AsyncConnectCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Connected = _pipe.IsConnected;
            if (Connected)
            {
                OnConnect();
                BeginRead();
            }
            else
            {
                Close();
            }
        }

        private void BeginRead()
        {
            _pipeMessage = new PipeMessage();
            _pipe.BeginRead(_pipeMessage.MessageBytes, 0, PipeMessage.MessageBufferSize, AsyncReadMessageCallback,
                _pipe);
        }

        private void AsyncReadMessageCallback(IAsyncResult result)
        {
            try
            {
                _pipe.EndRead(result);
            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae.Message);
            }

            if (!_pipeMessage.IsNullOrEmpty())
            {
                OnMessageReceived(_pipeMessage.Message);
                _pipeMessage = new PipeMessage();
            }

            if (_pipe.IsConnected)
            {
                BeginRead();
            }
            else
            {
                Close();
                Open();
            }
        }

        private void AsyncBeginWriteCallback(IAsyncResult result)
        {
            try
            {
                _pipe.EndWrite(result);
            }
            catch (OperationCanceledException oce)
            {
                Connected = false;
            }
        }

        protected virtual void OnConnect()
        {
            Connect?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnect()
        {
            Disconnect?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMessageReceived(string message)
        {
            var args = new MessageEventArgs {Message = message};
            MessageReceived?.Invoke(this, args);
        }
    }
}