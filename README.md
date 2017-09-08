# JenkinsSlaveRunner
Windows application for running a Jenkins Slave in an interactive session

This application replaces the need to use a DOS window to launch a Jenkins slave from Windows in an interactive session. 

It is a WPF application (requires .Net 4.5) and has a simple UI.

Usage
=====
* Copy the application into the directory that is going to be your Jenkins root e.g. c:\Jenkins
* Double Click to start it
* Fill out all the necessary fields
 * The Java path is detected automatically on application start-up based on your registry settings - this is currently non-editable.
 * The Jenkins Master URL should be of the form e.g. http://ci.jenkins-ci.org or http://jenkins.mycompany.com:8080
 * The Slave Name should be the name of the slave that you have configured in the Jenkins Web UI
 * The Secret should be the secret key that is generated - this can also be found in the Web UI under the Slave Configuration
 * Arguments are any additional parameters that you wish to pass to Java

Once you have configured it, click on 'Download Slave.jar' to obtain the Jar file

You can now click on Start and Stop to start your Jenkins Slave

Command Line Parameters
=======================
The only currently available command line parameter is /Autostart e.g. 

JenkinsSlaveRunner.exe /Autostart

Providing you have succesfully started your slave process using this tool previously, and therefore have a slave.jar downloaded and an exe.config file, then the Jenkins slave will start automatically using the previous settings on application startup.

Other Info
==========
Your settings are serialized out into a SlaveConfig.xml file alongside the application and will be loaded in on application start.
This file is written to every time you attempt to start your slave.

The application also has basic functionality built in to warn the user if they are logging off or shutting down the machine that Jenkins is running, and it gives the option to prevent shutdown at this point.
This was the initial reason for writing this application as people would forget that Jenkins was running interactively and close the machine down, or log off, meaning the slave stopped.

