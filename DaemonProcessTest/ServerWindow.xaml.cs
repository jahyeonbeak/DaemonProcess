using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DaemonProcess;

namespace DaemonProcessTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        private NamedPipeServerStream _pipe;

        private const string PipeName = "PipeSample";

        private const int PipeInBufferSize = 4096;

        private const int PipeOutBufferSize = 65535;

        private Encoding encoding = Encoding.UTF8;
        PipeServer ps;



        public ServerWindow()
        {
            InitializeComponent();

            ps = new PipeServer(PipeName, encoding);
            //ps.Connect();

            //PipeCheckerThread = new Thread(new ParameterizedThreadStart(CheckPipe));
            //PipeCheckerThread.Priority = ThreadPriority.Normal;
            //PipeCheckerThread.Start(ps);
            ps.WaitForConnectionCallback1 += WaitForConnectionCallback;
        }

        public void WaitForConnectionCallback(IAsyncResult ar)
        {
            PipeCheckerThread = new Thread(new ParameterizedThreadStart(CheckPipe));
            PipeCheckerThread.Priority = ThreadPriority.Normal;
            PipeCheckerThread.Start(ps);


            //var pipeServer = (NamedPipeServerStream)ar.AsyncState;

            //pipeServer.EndWaitForConnection(ar);
            //var data = new byte[PipeInBufferSize];

            //var count = pipeServer.Read(data, 0, PipeInBufferSize);

            //if (count > 0)
            //{
            //    // 通信双方可以约定好传输内容的形式，例子中我们传输简单文本信息。

            //    string message = encoding.GetString(data, 0, count);

            //    Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        tblRecMsg.Text = message;
            //    }));
            //}

        }
        public Thread PipeCheckerThread { set; get; }

        private void OnSend(object sender, RoutedEventArgs e)
        {
            ps.WriteToPipe(txtSendMsg.Text);
        }

        public void CheckPipe(object ar)
        {
            var test = ar as PipeServer;
            Console.WriteLine("要开始Read了 client");
            while (true)
            {
                string message = test.ReadFromPipe();
                if (message != null)

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tblRecMsg.Text = message;
                    }));
            }
            Thread.Sleep(1000);
        }

    }
}

