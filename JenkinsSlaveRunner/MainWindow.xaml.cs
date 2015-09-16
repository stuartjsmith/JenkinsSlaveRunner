// <copyright file="MainWindow.xaml.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the main window.xaml class</summary>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace JenkinsSlaveRunner
{
    /// <content>
    ///     Interaction logic for MainWindow.xaml.
    /// </content>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     The log maximum lines.
        /// </summary>
        private const int LogMaxLines = 500;

        /// <summary>
        ///     The configuration file.
        /// </summary>
        private const string ConfigFile = "SlaveConfig.xml";

        /// <summary>
        ///     The slave executor.
        /// </summary>
        private SlaveExecutor _slaveExecutor;

        /// <summary>
        ///     Initializes a new instance of the JenkinsSlaveRunner.MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            PopulateJavaPath();
            DeserializeSlaveConfig();
        }

        /// <summary>
        /// Handles the existing jenkins process.
        /// </summary>
        private void HandleExistingJenkinsProcess(int previousProcessId)
        {
            try
            {
                Process process = Process.GetProcessById(previousProcessId);
                if (process.ProcessName == "java")
                {
                    // we have a previous Jenkins process
                    // for now, just kill it
                    LogMessage("Killing previous jenkins process with Process ID " + previousProcessId);
                    process.Kill();
                }
            }
            catch (ArgumentException)
            {
                // nothing to do here, the previous Jenkins session was not found, so ignore and continue
            }
        }

        /// <summary>
        ///     Determines if we can populate java path.
        /// </summary>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        private bool PopulateJavaPath()
        {
            try
            {
                JavaPath.Text = JavaInstallation.JavaExe;
                return true;
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
                return false;
            }
        }

        /// <summary>
        ///     Event handler. Called by btnStart for click events.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Routed event information.</param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        /// <summary>
        ///     Starts this object.
        /// </summary>
        public void Start()
        {
            if (String.IsNullOrEmpty(JavaPath.Text))
            {
                if (PopulateJavaPath() == false)
                {
                    return;
                }
            }
            // write out the latest settings
            JenkinsSlaveConfiguration config = CreateJenkinsSlaveConfiguration();
            OutputLog.Items.Clear();
            _slaveExecutor = new SlaveExecutor(config);
            _slaveExecutor.OnLogMessage += LogMessage;
            _slaveExecutor.OnJenkinsStarted += JenkinsStarted;
            _slaveExecutor.Go();
        }

        /// <summary>
        ///     Jenkins started.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private void JenkinsStarted(JenkinsSlaveConfiguration config)
        {
            LogMessage("The Jenkins process has started under Process ID " + config.ProcessId);
            SerializeSlaveConfig(config);
            Thread t = new Thread(PollForExit);
            t.Start(config.ProcessId);
            SetUiForRunningState(true);
        }

        private void PollForExit(object obj)
        {
            int processId = (int) obj;
            while (_slaveExecutor != null)
            {
                try
                {
                    Process.GetProcessById(processId);
                    Thread.Sleep(5000);
                }
                catch (ArgumentException)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        ///     Event handler. Called by btnStop for click events.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Routed event information.</param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Sets user interface for running state.
        /// </summary>
        /// <param name="isRunningState">true if this object is running state.</param>
        private void SetUiForRunningState(bool isRunningState)
        {
            if (!Dispatcher.CheckAccess())
            {
                // Need for invoke if called from a different thread
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, (ThreadStart)delegate { SetUiForRunningState(isRunningState); });
            }
            else
            {
                btnStop.IsEnabled = isRunningState;
                btnStart.IsEnabled = !isRunningState;
                DownloadSlaveJar.IsEnabled = !isRunningState;
            }
        }

        /// <summary>
        ///     Stops this object.
        /// </summary>
        private void Stop()
        {
            if (_slaveExecutor != null)
            {
                if (_slaveExecutor.OnLogMessage != null)
                {
                    _slaveExecutor.OnLogMessage -= LogMessage;
                }
                if (_slaveExecutor.OnJenkinsStarted != null)
                {
                    _slaveExecutor.OnJenkinsStarted -= JenkinsStarted;
                }
                _slaveExecutor.Stop();
                _slaveExecutor = null;
            }
            LogMessage("Jenkins has stopped");
            SetUiForRunningState(false);
        }

        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(String message)
        {
            if (!Dispatcher.CheckAccess())
            {
                // Need for invoke if called from a different thread
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, (ThreadStart) delegate { LogMessage(message); });
            }
            else
            {
                // add this line at the top of the log
                OutputLog.Items.Add(message);

                // keep only a few lines in the log
                while (OutputLog.Items.Count > LogMaxLines)
                {
                    OutputLog.Items.RemoveAt(0);
                }
                if (OutputLog.Items.Count > 0)
                {
                    OutputLog.SelectedIndex = OutputLog.Items.Count - 1;
                    OutputLog.ScrollIntoView(OutputLog.SelectedItem);
                }
            }
        }

        /// <summary>
        ///     Event handler. Called by Window for closing events.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Cancel event information.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_slaveExecutor != null)
            {
                string msg = "Are you sure you wish to stop the (interactive) Jenkins Slave on this machine? ";
                if (Interaction.ConfirmStopJenkinsSlave(msg) == false)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            Stop();
        }

        /// <summary>
        ///     Serialize slave configuration.
        /// </summary>
        /// <returns>
        ///     A JenkinsSlaveConfiguration.
        /// </returns>
        private void SerializeSlaveConfig(JenkinsSlaveConfiguration config)
        {
            var serializer = new XmlSerializer(typeof (JenkinsSlaveConfiguration));
            TextWriter textWriter = new StreamWriter(ConfigFile);
            serializer.Serialize(textWriter, config);
            textWriter.Close();
        }

        private JenkinsSlaveConfiguration CreateJenkinsSlaveConfiguration()
        {
            var config = new JenkinsSlaveConfiguration();
            config.JenkinsUrl = JenkinsUrl.Text;
            config.SlaveName = SlaveName.Text;
            config.Secret = Secret.Text;
            config.Arguments = Arguments.Text;
            return config;
        }

        /// <summary>
        ///     Deserialize slave configuration.
        /// </summary>
        private void DeserializeSlaveConfig()
        {
            var serializer = new XmlSerializer(typeof (JenkinsSlaveConfiguration));

            if (File.Exists(ConfigFile) == false)
            {
                // default the machine name here
                SlaveName.Text = Environment.MachineName;
                return;
            }

            // A file stream is used to read the XML file into the ObservableCollection
            using (var reader = new StreamReader(ConfigFile))
            {
                var config = serializer.Deserialize(reader) as JenkinsSlaveConfiguration;
                if (config != null)
                {
                    JenkinsUrl.Text = config.JenkinsUrl;
                    SlaveName.Text = config.SlaveName;
                    Secret.Text = config.Secret;
                    Arguments.Text = config.Arguments;
                    if (config.ProcessId > 0)
                    {
                        HandleExistingJenkinsProcess(config.ProcessId);
                    }
                }
            }
        }

        /// <summary>
        ///     Event handler. Called by DownloadSlaveJar_OnClick for click events.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Routed event information.</param>
        private void DownloadSlaveJar_OnClick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string slaveUrl = JenkinsUrl.Text.Trim(new[] {'/'}).Trim() + "/jnlpJars/slave.jar";
                Uri uri;
                bool isUri = Uri.TryCreate(slaveUrl, UriKind.Absolute, out uri) &&
                             (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                if (isUri)
                {
                    uri = new Uri(slaveUrl);
                    LogMessage("Downloading slave.jar from " + uri.AbsoluteUri);
                    string filename = Path.GetFileName(uri.LocalPath);
                    new WebClient().DownloadFile(uri.AbsoluteUri, filename);
                    LogMessage("Completed download of slave.jar from " + uri.AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
        }
    }
}