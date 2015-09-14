// <copyright file="Interaction.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>14/09/2015</date>
// <summary>Implements the interaction class</summary>
using System.Windows;

namespace JenkinsSlaveRunner
{
    /// <summary>
    /// An interaction.
    /// </summary>
    internal static class Interaction
    {
        /// <summary>
        /// Confirm stop jenkins slave.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        internal static bool ConfirmStopJenkinsSlave(string message)
        {
            // Ask the user if they want to allow the session to end 
            MessageBoxResult result = MessageBox.Show(message, "Stop Jenkins Slave?", MessageBoxButton.YesNo);

            // End session, if specified 
            if (result == MessageBoxResult.No)
            {
                return false;
            }

            return true;
        }
    }
}