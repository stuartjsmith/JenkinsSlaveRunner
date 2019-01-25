import jenkins.model.Jenkins

pipeline {
	agent any
    stages {
	    stage('Build') { 
		    steps {
                bat "\"${tool 'msbuild12'}\" JenkinsSlaveRunner/JenkinsSlaveRunner.sln /p:Configuration=Release /p:Platform=\"Any CPU\" /p:BuildNumber=${env.BUILD_NUMBER}"
            }
	    }
	    stage('Archive') {
		    steps {
                archiveArtifacts artifacts: 'JenkinsSlaveRunner/bin/Release/**'
            }
	    }  
	}
}