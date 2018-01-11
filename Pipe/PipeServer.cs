using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace Pipe
{
    public class PipeServer : IPipe
    {
        private NamedPipeServerStream _pipe;
        private PipeMessage _pipeMessage;

        public PipeServer(string pipeName = "Pipe.Server")
        {
            PipeName = pipeName;
            Connected = false;
            NewPipeServer();
        }

        public string PipeName { get; set; }
        public bool Connected { get; set; }

        public void Open()
        {
            BeginWaitForConnection();
        }


        public void Close()
        {
            if (_pipe.IsConnected)
            {
                _pipe.WaitForPipeDrain();
                _pipe.Disconnect();
            }

            NewPipeServer();
            Connected = false;
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
                throw new Exception("No client connected to pipe.");
            }
        }

        private void NewPipeServer()
        {
            _pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough, PipeMessage.MessageBufferSize,
                PipeMessage.MessageBufferSize);
        }

        private void BeginWaitForConnection()
        {
            try
            {
                if (!_pipe.IsConnected) _pipe.BeginWaitForConnection(AsyncConnectCallback, _pipe);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                if (e.Message.Contains("Pipe is broken"))
                {
                    Close();
                    Open();
                }
                else
                {
                    throw;
                }
            }
        }

        private void AsyncConnectCallback(IAsyncResult result)
        {
            try
            {
                _pipe.EndWaitForConnection(result);
                Connected = true;
                OnConnect();
                BeginRead();
            }
            catch (IOException ioe)
            {
                Debug.WriteLine(ioe.Message);
                if (ioe.Message.Contains("No process"))
                {
                    Close();
                    Open();
                }
                else
                {
                    throw;
                }
            }
        }

        private void BeginRead()
        {
            _pipeMessage = new PipeMessage();
            try
            {
                _pipe.BeginRead(_pipeMessage.MessageBytes, 0, PipeMessage.MessageBufferSize, AsyncReadMessageCallback,
                    _pipe);
            }
            catch (IOException ioe)
            {
                Debug.WriteLine(ioe.Message);
                if (ioe.Message.Contains("Waiting for"))
                {
                    Close();
                    Open();
                }
                else
                {
                    throw;
                }
            }
        }

        private void AsyncReadMessageCallback(IAsyncResult result)
        {
            _pipe.EndRead(result);
            if (!_pipeMessage.IsNullOrEmpty())
                OnMessageReceived(_pipeMessage.Message);
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
            _pipe.EndWrite(result);
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