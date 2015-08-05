// <copyright file="slaveexecutor.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the slaveexecutor class</summary>
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Markup;

namespace JenkinsSlaveRunner
{
    /// <summary>
    /// Logs message delegate.
    /// </summary>
    /// <param name="s">The string.</param>
    public delegate void LogMessageDelegate(string s);

    /// <summary>
    /// A slave executor.
    /// </summary>
    internal class SlaveExecutor
    {
        /// <summary>
        /// Message describing the on log.
        /// </summary>
        public LogMessageDelegate OnLogMessage;

        /// <summary>
        /// The jenkins process.
        /// </summary>
        private Process _jenkinsProcess;

        /// <summary>
        /// The jenkins slave configuration.
        /// </summary>
        private JenkinsSlaveConfiguration _jenkinsSlaveConfiguration;

        /// <summary>
        /// Initializes a new instance of the JenkinsSlaveRunner.SlaveExecutor class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public SlaveExecutor(JenkinsSlaveConfiguration config)
        {
            _jenkinsSlaveConfiguration = config;
        }

        /// <summary>
        /// Executes the jenkins slave operation.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void RunJenkinsSlave(object args)
        {
            if (_jenkinsProcess != null)
            {
                Stop();
            }

            var slaveParams = (object[]) args;
            string filename = slaveParams[0].ToString();
            JenkinsSlaveConfiguration config = slaveParams[1] as JenkinsSlaveConfiguration;

            string jnlpUrl = config.JenkinsUrl.Trim(new[] {'/'}).Trim() + "/computer/" + config.SlaveName.Trim() + "/slave-agent.jnlp";
            string arguments = "-jar slave.jar "+ config.Arguments + " -jnlpUrl " + jnlpUrl + " -secret " + config.Secret;

            _jenkinsProcess = new Process();

            _jenkinsProcess.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                _jenkinsProcess.StartInfo.Arguments = arguments;
            }

            _jenkinsProcess.StartInfo.CreateNoWindow = true;
            _jenkinsProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _jenkinsProcess.StartInfo.UseShellExecute = false;

            _jenkinsProcess.StartInfo.RedirectStandardError = true;
            _jenkinsProcess.StartInfo.RedirectStandardOutput = true;
            _jenkinsProcess.OutputDataReceived += JenkinsProcessOnDataReceived;
            _jenkinsProcess.ErrorDataReceived += JenkinsProcessOnDataReceived;
            _jenkinsProcess.Start();
            _jenkinsProcess.BeginOutputReadLine();
            _jenkinsProcess.BeginErrorReadLine();
            _jenkinsProcess.WaitForExit();
        }

        /// <summary>
        /// Jenkins process on data received.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="dataReceivedEventArgs">Data received event information.</param>
        private void JenkinsProcessOnDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            // raise an event here that can be subscribed to
            if (OnLogMessage != null)
            {
                OnLogMessage(dataReceivedEventArgs.Data);
            }
        }

        /// <summary>
        /// Goes this object.
        /// </summary>
        internal void Go()
        {
            var thread = new Thread(RunJenkinsSlave);
            thread.Start(new object[] {@"C:\ProgramData\Oracle\Java\javapath\java.exe", _jenkinsSlaveConfiguration});
        }

        /// <summary>
        /// Cancel execution.
        /// </summary>
        internal void Stop()
        {
            if (_jenkinsProcess != null)
            {
                try
                {
                    _jenkinsProcess.Kill();
                }
                catch (Win32Exception w32e)
                {
                    if (OnLogMessage != null)
                    {
                        OnLogMessage(w32e.Message);
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    if (OnLogMessage != null)
                    {
                        OnLogMessage(ioe.Message);
                    }
                }
                
                _jenkinsProcess = null;
            }
        }
    }
}