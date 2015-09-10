// <copyright file="app.xaml.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the app.xaml class</summary>

using System;
using System.Text;
using System.Windows;

namespace JenkinsSlaveRunner
{
    /// <content>
    /// Interaction logic for App.xaml.
    /// </content>
    public partial class App : Application
    {
        /// <summary>
        /// Event handler. Called by App for session ending events.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Session ending cancel event information.</param>
        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Ask the user if they want to allow the session to end 
            string msg =
                string.Format(
                    "{0}. Are you sure you wish to log off/shutdown? This will stop the (interactive) Jenkins Slave on this machine and you will need to log in again to restart it.",
                    e.ReasonSessionEnding);
            if (Interaction.ConfirmStopJenkinsSlave(msg) == false)
            {
                e.Cancel = true;
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool autoStart = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/Autostart")
                {
                    autoStart = true;
                }

                if (e.Args[i] == "/?")
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("Usage:" + Environment.NewLine);
                    message.AppendLine("JenkinsSlaveRunner.exe [/Autostart]" + Environment.NewLine);
                    message.AppendLine("/Autostart - automatically starts the Jenkins process based on the stored previously configuration values");
                    MessageBox.Show(message.ToString(), "Usage", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(0);
                }
            }

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            if (autoStart)
            {
                mainWindow.Start();
            }
        }
    }
}