using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonProcess;

namespace ProcessWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string address = "";
            bool isFirst = true;
            foreach(string arg in args)
            {
                if (!isFirst)
                    address += ",";
                else
                    isFirst = false;

                address += arg;
            }
            DaemonProcess.ProcessWatcher processWatcher = new DaemonProcess.ProcessWatcher(address, "");
            processWatcher.OnStart(null);
        }
    }
}
