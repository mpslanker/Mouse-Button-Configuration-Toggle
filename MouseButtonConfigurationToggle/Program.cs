using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ManagedWinapi;

namespace MouseButtonConfigurationToggle
{
    class Program
    {
        private static NotifyIcon icon = new NotifyIcon();
        private static int currentState = new int();

        static void Main(string[] args)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Exit", null, exit_click);
            //if (Environment.OSVersion.Version.Major >= 6 && (args.Length == 0 || args[0] != "runas"))
            //{
            //    ToolStripMenuItem menuItem = new ToolStripMenuItem("Run as Administrator", null, uac_click);
            //    menu.Items.Add(menuItem);

            //}

            Hotkey swapMoustButtons = new Hotkey();
            swapMoustButtons.WindowsKey = true;
            swapMoustButtons.KeyCode = Keys.Oemtilde;
            swapMoustButtons.Enabled = true;
            swapMoustButtons.HotkeyPressed += new EventHandler(swapMouseButtons_HotkeyPressed);

            currentState = NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_SWAPBUTTON);

            if (currentState == 0)
            {
                icon.Icon = Properties.Resources.right_handed;
                icon.Text = "Right-Handed";
                icon.Visible = true;
                icon.ShowBalloonTip(2, "Mouse Button Configuration Toggle", "Currently your mouse is configured for right-handed users.", ToolTipIcon.Info);
            }
            else if (currentState == 1)
            {
                icon.Icon = Properties.Resources.left_handed;
                icon.Text = "Left-Handed";
                icon.Visible = true;
                icon.ShowBalloonTip(2, "Mouse Button Configuration Toggle", "Currently your mouse is configured for left-handed users.", ToolTipIcon.Info);
            }
            else
            {
                // we should probably do somethign here.
            }
            icon.ContextMenuStrip = menu;

            Application.Run();

            icon.Visible = false;
        }

        static void swapMouseButtons_HotkeyPressed (object sender, EventArgs e)
        {
            ToggleMouseButtons();
        }

        public static void ToggleMouseButtons()
        {
            if (currentState == 0)
            {
                NativeMethods.SwapMouseButton(1);         // Swap the mouse buttons (Make mouse left handed)
                icon.Icon = Properties.Resources.left_handed;
                icon.Text = "Left-Handed";
                icon.ShowBalloonTip(1, "Mouse Buttons Swapped", "Now configured for left-handed users.", ToolTipIcon.Info);
                icon.Visible = true;

                // Update the current state
                currentState = 1;

            }
            else if (currentState == 1)
            {
                NativeMethods.SwapMouseButton(0);         // UnSwap the mouse buttons (Make mouse right handed)
                icon.Icon = Properties.Resources.right_handed;
                icon.Text = "Right-Handed";
                icon.ShowBalloonTip(1, "Mouse Buttons Swapped", "Now configured for right-handed users.", ToolTipIcon.Info);
                icon.Visible = true;

                // Update the current state
                currentState = 0;

            }
            else
            {
                // Again we should probably do something here.
            }
        }

        static void exit_click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        static void uac_click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Arguments = "runas";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                p.StartInfo.Verb = "runas";
            }
            try
            {
                p.Start();
                Application.Exit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Do nothing if UAC prompt declined
            }

        }
    }
}
