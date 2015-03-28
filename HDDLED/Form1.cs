using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;

namespace HDDLED
{
    public partial class Form1 : Form
    {
        NotifyIcon hddLedIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedWorker;
        


        #region Form Stuff
        public Form1()
        {
            InitializeComponent();

            //load icons
            activeIcon = new Icon("HDD_Busy.ico");
            idleIcon = new Icon("HDD_Idle.ico");

            //Create Notify Icon and assign default Icon
            hddLedIcon = new NotifyIcon();
            hddLedIcon.Icon = idleIcon;
            hddLedIcon.Visible = true;
            
            MenuItem quitMenuItem = new MenuItem("Quit");
            MenuItem progNameMenuItem = new MenuItem("HDD LED Indicator v. 1.0");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(progNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);

            hddLedIcon.ContextMenu = contextMenu;

            //wire up quit button
            quitMenuItem.Click += quitMenuItem_Click;

            //hide the form
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            //start workerthread
            hddLedWorker = new Thread(HDDActivityThread);
            hddLedWorker.Start();
        }

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            hddLedWorker.Abort();
            hddLedIcon.Dispose();
            this.Close();
        }
        #endregion

        #region Threads
        /// <summary>
        /// Polls the HDD for activity
        /// </summary>
        public void HDDActivityThread()
        {
            try
            {
                ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
                //main loop
                while (true)
                {
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() == "_Total")
                        {
                            if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                hddLedIcon.Icon = activeIcon;

                            }
                            else
                            {
                                hddLedIcon.Icon = idleIcon;
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException tbe)
            {

            }
        }
        #endregion
    }
}
