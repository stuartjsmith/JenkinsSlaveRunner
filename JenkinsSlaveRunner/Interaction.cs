using System.Windows;

namespace JenkinsSlaveRunner
{
    internal static class Interaction
    {
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