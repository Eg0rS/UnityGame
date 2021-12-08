import com.tortuga.model.PlatformType
import com.tortuga.model.PlatformType
import com.tortuga.service.DurationService

class BuildContext {
    static path
    static projectOptions
    static branch
    static version

    static job
    static nexusConfig

    static notificationService
    static checkoutService
    static cacheService
    static awxService

    static List<String> artifactIds

    static unityVersion

    static Map activeBuilds = [
            'windows': false,
            'android': false,
    ]
   
    static def isProduction() {
        return BuildContext.branch.isProduction()
    }
}

pipeline {
    agent any
    parameters {
        booleanParam(name: 'forceBuildLib', defaultValue: true, description: 'use cache lib')
        booleanParam(name: 'recreate', defaultValue: false, description: 'delete workspace and recreate it from develop')

        booleanParam(name: 'androidBuild', defaultValue: true, description: 'build android')
        booleanParam(name: 'iosBuild', defaultValue: false, description: 'build ios')
    }

    options {
        parallelsAlwaysFailFast()
        skipDefaultCheckout()
        disableConcurrentBuilds()
    }

    stages {
        stage("init-context") {
            steps {
                script {
                    configFileProvider([configFile(fileId: 'dronDonDon-project-config', variable: 'projectFilePath')]) {
                        BuildContext.projectOptions = readJSON file: projectFilePath
                    }

                    BuildContext.artifactIds = new ArrayList<>()
                    BuildContext.branch = getBranchInfo()
                    BuildContext.job = getJobInfo()
                    BuildContext.notificationService = createNotificationService(BuildContext.projectOptions['slackChannel'])
                    BuildContext.checkoutService = createCheckoutService(BuildContext.projectOptions["repoUrl"])
                    BuildContext.path = getPathOptions()

                    BuildContext.cacheService = createCacheService(BuildContext.projectOptions["cacheBranch"])

                    configFileProvider([configFile(fileId: 'tortuga-nexus-config', variable: 'nexusConfig')]) {
                        BuildContext.nexusConfig = readJSON file: nexusConfig
                    }

                    if (BuildContext.projectOptions["deletePreviousJob"]) {
                        abortPreviousBuild()
                    }

                    if (BuildContext.branch.isWip()) {
                        currentBuild.result = 'ABORTED'
                        error('Aborted wip')
                    }
                     BuildContext.awxService = createAwxService("http://awx.tortu.ga", "ava3d-stage-runner")
                     fillActiveBuilds()
                }
            }
        }

        stage("checkout") {
            steps {
                script {
                     if (!fileExists(BuildContext.path.repoPath)) {
                        echo "restore from develop cache"
                        BuildContext.cacheService.restoreRepo(BuildContext.path.repoPath)
                    }

                    BuildContext.checkoutService.checkout(BuildContext.branch, BuildContext.path.repoPath)
                    fileOperations([folderDeleteOperation(BuildContext.path.targetPath)])
                    BuildContext.version = readFile("${BuildContext.path.repoPath}/version")
                }
            }
        }

        stage("build") {
            parallel {
                
                stage('ios-client') {
                    when {
                        equals expected: true, actual: checkBuild("ios")
                    }
                    stages {
                
                        stage("build") {
                            steps {
                                buildAppClient(BuildContext, PlatformType.IOS,  params.forceBuildLib)
                            }
                        }

                        stage("deploy") {
                            steps {
                                script {
                                    deployClient(PlatformType.IOS)
                                }
                            }
                        }
                    }
                }

                stage('android') {
                    when {
                        equals expected: true, actual: checkBuild("android")
                    }
                    stages {                   
                        stage("build-client") {
                            steps {
                                buildAppClient(BuildContext, PlatformType.ANDROID, params.forceBuildLib)
                            }
                        }

                        stage("deploy-client") {
                            steps {
                                script {
                                    deployClient(PlatformType.ANDROID)
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    post {
        failure {
            script {
                BuildContext.notificationService.notifyBuildFault(BuildContext.job, BuildContext.branch.user)
            }
        }

        success {
            script {
                def nexusBranch = BuildContext.isProduction() ? BuildContext.projectOptions["nexusReleasesId"] : BuildContext.projectOptions["nexusAbyssId"]
                slackSend channel: 'drone-tech', color: 'good',
                    message: """${env.JOB_NAME} - #${env.BUILD_NUMBER}  Build success! (<${env.BUILD_URL}|Open>) @here
                    https://nexus-dev.tortugasocial.com/repository/${nexusBranch}/com/tortuga/dronDonDon/dronDonDon-client-android-${BuildContext.branch.branchName}/${BuildContext.version}/dronDonDon-client-android_${BuildContext.branch.branchName}-${BuildContext.version}.zip
                    https://nexus-dev.tortugasocial.com/repository/${nexusBranch}/com/tortuga/dronDonDon/dronDonDon-client-ios-${BuildContext.branch.branchName}/${BuildContext.version}/dronDonDon-client-ios_${BuildContext.branch.branchName}-${BuildContext.version}.zip"""
            }
        }
    }
}

private void fillActiveBuilds() {
    BuildContext.activeBuilds['android'] = params.androidBuild
    BuildContext.activeBuilds['ios'] = params.iosBuild
}

private static boolean checkBuild(String type) {
    return BuildContext.activeBuilds[type]
}

private void deployClient(PlatformType platform) {
    def clientBuildPath = BuildContext.path.buildPath + "/" + platform.name + 'client'
    def clientPom = readMavenPom file: clientBuildPath + '/pom.xml'

    def clientPackage = createPackage(
            clientPom.groupId,
            'dronDonDon-client',
            BuildContext.version,
            BuildContext.branch.branchName,
            BuildContext.isProduction(),
            platform
    )

    BuildContext.artifactIds.add(clientPackage.packageId)

    dir("${clientBuildPath}/target") {
        createNexusService(BuildContext.nexusConfig).tortugaPackage(clientPackage).upload()
    }
}

def buildAppClient(def context, PlatformType type, forceBuildLib = false) {
    def clientBuildPath = "${context.path.buildPath}/${type.name}client"
    prepareClient(context, type, forceBuildLib, "${context.path.repoPath}/client/")

    dir(clientBuildPath) {
        def prod = context.isProduction();
        
        def buildName = context.branch.safeBranchName
                .replace('feature-', '')
                .replace('release-', '')
                .replace('bugfix-', '')
                .replace('hotfix-', '')
        withCredentials([usernamePassword(credentialsId: 'unityUserPass', passwordVariable: 'unityPass', usernameVariable: 'unityUser')]) {

            createMaven()
                    .phase('clean')
                    .phase('package')
                    .param('profiler', prod ? "false" : "true")
                    .param('il2cpp', prod ? "true" : "false")
                    .param('splitApk', prod ? "true" : "false")
                    .param('platform', type.name)
                    .param('skipReplace', 'false')
                    .param('skipTest', 'true')
                    .param('buildVersion', context.version)
                    .param('buildName', buildName)
                    .param('unityUser', unityUser)
                    .param('unityPass', unityPass)
                    .param("production", "false" )
                    .run()
        }
        fileOperations([fileCopyOperation(excludes: '', flattenFiles: true, includes: "target/*.zip",
                targetLocation: context.path.targetPath)])
    }
}



