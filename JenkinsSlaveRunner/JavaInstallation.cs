// <copyright file="JavaInstallation.cs" company="MyCompany.com">
// Copyright (c) 2015 MyCompany.com. All rights reserved.
// </copyright>
// <author>Stuart. Smith</author>
// <date>05/08/2015</date>
// <summary>Implements the java installation class</summary>

using System;
using System.IO;
using Microsoft.Win32;

namespace JenkinsSlaveRunner
{
    /// <summary>
    /// A java installation.
    /// </summary>
    internal class JavaInstallation
    {
        /// <summary>
        /// The java key.
        /// </summary>
        private const string JavaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";

        private const string JavaHomeEvarKey = "JAVA_HOME";

        /// <summary>
        /// Gets the java home.
        /// </summary>
        /// <value>
        /// The java home.
        /// </value>
        public static string JavaHome
        {
            get
            {
                string javaHome = ReadJavaHomeFromRegistry(JavaKey, RegistryView.Registry32);
                if (string.IsNullOrEmpty(javaHome))
                {
                    javaHome = ReadJavaHomeFromRegistry(JavaKey, RegistryView.Registry64);
                }

                if (string.IsNullOrEmpty(javaHome))
                {
                    javaHome = Environment.GetEnvironmentVariable(JavaHomeEvarKey);
                }

                return javaHome;
            }
        }

        /// <summary>
        /// Gets the java executable.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the requested file is not present.
        /// </exception>
        /// <value>
        /// The java executable.
        /// </value>
        public static string JavaExe
        {
            get
            {
                string javaHome = JavaHome;
                if (string.IsNullOrEmpty(javaHome))
                {
                    throw new FileNotFoundException(
                        "Java Exe cannot be found. Please check you have a valid Java installation");
                }
                string javaExePath = Path.Combine(javaHome, @"bin\java.exe");
                if (File.Exists(javaExePath) == false)
                {
                    throw new FileNotFoundException("Java Exe cannot be found at " + javaExePath +
                                                    ". Please check you have a valid Java installation");
                }
                return javaExePath;
            }
        }

        /// <summary>
        /// Reads java home from registry.
        /// </summary>
        /// <param name="jre">The jre.</param>
        /// <param name="registryView">The registry view.</param>
        /// <returns>
        /// The java home from registry.
        /// </returns>
        private static string ReadJavaHomeFromRegistry(string jre, RegistryView registryView)
        {
            string javaHome = null;
            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
            RegistryKey jreKey = baseKey.OpenSubKey(jre);
            if (jreKey != null)
            {
                string currentVersion = jreKey.GetValue("CurrentVersion").ToString();
                if (string.IsNullOrEmpty(currentVersion) == false)
                {
                    RegistryKey javaHomeKey = jreKey.OpenSubKey(currentVersion);
                    if (javaHomeKey != null)
                    {
                        javaHome = javaHomeKey.GetValue("JavaHome").ToString();
                        javaHomeKey.Close();
                    }
                }
                jreKey.Close();
            }
            return javaHome;
        }
    }
}