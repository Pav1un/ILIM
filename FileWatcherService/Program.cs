using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(String[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            /*
            if (Environment.UserInteractive)
            {
                Service1 service1 = new Service1();
                service1.TestStartupAndStop(args);
            }
            else
            {
                // Put the body of your old Main method here.
            }*/
        }
    }
}
