// <copyright file="JenkinsSlaveConfiguration.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the jenkins slave configuration class</summary>
using System;

namespace JenkinsSlaveRunner
{
    /// <summary>
    /// (Serializable)a jenkins slave configuration.
    /// </summary>
    [Serializable]
    public class JenkinsSlaveConfiguration
    {
        /// <summary>
        /// Gets or sets URL of the jenkins.
        /// </summary>
        /// <value>
        /// The jenkins URL.
        /// </value>
        public string JenkinsUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the slave.
        /// </summary>
        /// <value>
        /// The name of the slave.
        /// </value>
        public string SlaveName { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>
        /// The secret.
        /// </value>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the process.
        /// </summary>
        /// <value>
        /// The identifier of the process.
        /// </value>
        public int ProcessId { get; set; }
    }
}