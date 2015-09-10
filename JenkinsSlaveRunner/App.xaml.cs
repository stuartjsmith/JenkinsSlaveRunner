// <copyright file="app.xaml.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the app.xaml class</summary>
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
    }
}