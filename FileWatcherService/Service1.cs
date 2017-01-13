using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;

namespace FileWatcherService
{
    public partial class Service1 : ServiceBase
    {
        OutloockWather ow;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;            
        }

        protected override void OnStart(string[] args)
        {
            ow = new OutloockWather();
            ow.GetInstanceOfOutlook();
            Thread loggerThread = new Thread(new ThreadStart(ow.Start));
            loggerThread.Start();
        }

        protected override void OnStop() => ow.Stop();
    }


    class OutloockWather
    {
        Outlook.Application app = null;
        Outlook.MAPIFolder inbox = null;
        Boolean enabled = true;
        

        public void Start() {                       
            while (enabled)       
                Thread.Sleep(1000);         
        }

        public void Stop()  => enabled = false;            
        

        public Outlook.Application GetInstanceOfOutlook() {
            try {
                
                if (Process.GetProcessesByName("OUTLOOK").Count() > 0) {
                    app = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                }

                /*
                Type acType = Type.GetTypeFromProgID("Outlook.Application");
                app = (Outlook.Application)Activator.CreateInstance(acType, true);*/
            }
            catch (Exception e) {
                WriteBodyMessage(String.Format("Error {0} ", e.Message));
            }        

            return app;
        }

        public void SetAddMail() {
            inbox = app.Application.ActiveExplorer().Session.GetDefaultFolder
                (Outlook.OlDefaultFolders.olFolderInbox);
            var items = inbox.Items;
            items.ItemAdd += (item) => {
                var mail = (Outlook.MailItem)item;
                if (mail != null)
                {
                    WriteBodyMessage(mail.Body);
                }
            };
        }

        private void WriteBodyMessage(String msgBody) {
            using (StreamWriter writer = new StreamWriter(@"D:\templog.txt", true))
            {
                writer.WriteLine(String.Format(" Body: {0}  /n Date: {1} ",
                    DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), msgBody));
                writer.Flush();
            }
        }
    }

    /*
    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            watcher = new FileSystemWatcher(@"D:\Tmp");
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
    */

}
