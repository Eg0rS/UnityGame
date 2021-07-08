class BuildContext {
    static path
    static projectOptions
    static branch
    static version

    static job
    static nexusConfig

    static notificationService
    static checkoutService
    static stageService
    static oldStageService
    static deployerService
    static cacheService
    static awxService

    static List<String> artifactIds

    static unityVersion

    static Map activeBuilds = [
            'server' : false,
            'windows': false,
            'android': false,
            'ios'    : false
    ]
   
    static def isProduction() {
        return BuildContext.branch.isProduction()
    }
}

pipeline {
    agent any
    parameters {
        booleanParam(name: 'forceBuildLib', defaultValue: false, description: 'use cache lib')
        booleanParam(name: 'cleanPods', defaultValue: false, description: 'clear pods cache in ios')
        booleanParam(name: 'recreate', defaultValue: false, description: 'delete workspace and recreate it from develop')

        booleanParam(name: 'iosBuild', defaultValue: true, description: 'build ios')
        booleanParam(name: 'androidBuild', defaultValue: true, description: 'build android')
        booleanParam(name: 'windowsBuild', defaultValue: false, description: 'build windows')
        booleanParam(name: 'serverBuild', defaultValue: true, description: 'build server')
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
                    configFileProvider([configFile(fileId: 'estapp-project-config', variable: 'projectFilePath')]) {
                        BuildContext.projectOptions = readJSON file: projectFilePath
                    }

                    BuildContext.artifactIds = new ArrayList<>()
                    BuildContext.branch = getBranchInfo()
                    BuildContext.job = getJobInfo()
                    BuildContext.notificationService = createNotificationService(BuildContext.projectOptions['slackChannel'])
                    BuildContext.checkoutService = createCheckoutService(BuildContext.projectOptions["repoUrl"])
                    BuildContext.path = getPathOptions()
                    BuildContext.stageService = createNewStageService(BuildContext.projectOptions["stageUrl"],
                            BuildContext.projectOptions["projectPrefix"])

                    BuildContext.oldStageService = createStageService(BuildContext.projectOptions["projectPrefix"],
                            BuildContext.projectOptions["nexusId"], BuildContext.projectOptions["stageUrl"])

                    BuildContext.deployerService = createDeployerService(BuildContext.projectOptions["projectPrefix"],
                            BuildContext.projectOptions["nexusId"], BuildContext.projectOptions["deployerUrl"])

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
                    if (params.recreate) {
                        fileOperations([folderDeleteOperation(BuildContext.path.rootPath)])    
                    }
                    if (!fileExists(BuildContext.path.repoPath)) {
                        echo "restore from develop cache"
                        BuildContext.cacheService.restoreRepo(BuildContext.path.repoPath)
                    }

                    BuildContext.checkoutService.checkout(BuildContext.branch, BuildContext.path.repoPath)
                    fileOperations([folderDeleteOperation(BuildContext.path.targetPath)])
                    Properties properties = new Properties()
                    properties.load(new FileInputStream("${BuildContext.path.repoPath}/version.ini"))
                    BuildContext.version = properties.getProperty('build.version') + "." + properties.getProperty('major.version') + "." + properties.getProperty('minor.version')
                    def unityVersion = readYaml file: "${BuildContext.path.repoPath}/client/ProjectSettings/ProjectVersion.txt"
                    BuildContext.unityVersion = unityVersion.m_EditorVersion
                    updateUnityProductionSettings("${BuildContext.path.repoPath}/client/ProjectSettings/ProjectSettings.asset", "....", "External Services Testing App", "sergey-voyachek") // устанавливаем path, cloudProjectId, projectName, organizationId
                }
            }
        }

        stage("build") {
            parallel {
                
                stage('windows-client') {
                    when {
                        equals expected: true, actual: checkBuild("windows")
                    }
                    stages {
/*                         stage("build-windows-resources") { // раскомментировать,если нужна фаза сборки ресурсов 
                            steps {
                                buildResources(BuildContext, PlatformType.WINDOWS, params.forceBuildLib)
                            }
                        }
                        stage("deploy-windows-resources") {
                            steps {
                                script {
                                    deployResources(PlatformType.WINDOWS)
                                }
                            }
                        }   */                  
                        stage("build") {
                            steps {
                                buildAppClient(BuildContext, PlatformType.WINDOWS,  params.forceBuildLib)
                            }
                        }

                        stage("deploy") {
                            steps {
                                script {
                                    deployClient(PlatformType.WINDOWS)
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
/*                         stage("build-android-resources") { // раскомментировать,если нужна фаза сборки ресурсов 
                            steps {
                                buildResources(BuildContext, PlatformType.ANDROID, params.forceBuildLib)
                            }
                        }
                        stage("deploy-android-resources") {
                            steps {
                                script {
                                    deployResources(PlatformType.ANDROID)
                                }
                            }
                        } */                    
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

                stage('ios') {
                    when {
                        equals expected: true, actual: checkBuild("ios")
                    }
                    stages {
/*                         stage("build-ios-resources") { // раскомментировать,если нужна фаза сборки ресурсов 
                            steps {
                                buildResources(BuildContext, PlatformType.IOS, params.forceBuildLib)
                            }
                        }
                        stage("deploy-ios-resources") {
                            steps {
                                script {
                                    deployResources(PlatformType.IOS)
                                }
                            }
                        }  */                   
                        stage("build-client") {
                            steps {
                                buildAppClient(BuildContext, PlatformType.IOS, params.forceBuildLib)
                            }
                        }

                        stage("deploy-client") {
                            steps {
                                script {
                                    deployClient(PlatformType.IOS)
                                }
                            }
                        }
                    }
                }


/*                 stage('server') { // раскомментировать,если нужна фаза сборки сервера
                    stages {
                        stage("build-server") {
                            steps {
                                buildAppServer(BuildContext)
                            }
                        }

                        stage("deploy-server") {
                            steps {
                                script {
                                    def serverPom = readMavenPom file: BuildContext.path.buildPath + '/server/app-server/pom.xml'
                                    def serverPackage = createPackage(
                                            serverPom.parent.groupId,
                                            serverPom.artifactId,
                                            BuildContext.version,
                                            BuildContext.branch.branchName,
                                            BuildContext.isProduction()
                                    )

                                    BuildContext.artifactIds.add(serverPackage.packageId)

                                    dir("${BuildContext.path.buildPath}/server/app-server/target") {
                                        createNexusService(BuildContext.nexusConfig).tortugaPackage(serverPackage).upload()
                                    }
                                }
                            }
                        }
                    }
                } */

                stage("client-config") {
                    when {
                        equals expected: true, actual: checkBuild("windows") || checkBuild("ios") || checkBuild("android")
                    }

                    stages {
                        stage("build") {
                            steps {
                                script {
                                    def configBuildPath = "${BuildContext.path.buildPath}/config/"
                                    if (!fileExists(configBuildPath)) {
                                        fileOperations([folderCreateOperation(configBuildPath)])
                                    }
                                    syncDirectory("${BuildContext.path.repoPath}/config/", configBuildPath)
                                    dir(configBuildPath) {
                                        def m = createMaven()
                                        m.phase("clean").phase("package").param("buildVersion", BuildContext.version).run()
                                    }
                                }
                            }
                        }
                        stage("deploy") {
                            steps {
                                script {
                                    def configPom = readMavenPom file: BuildContext.path.buildPath + '/config/pom.xml'
                                    def configPackage = createPackage(configPom.groupId,
                                            configPom.artifactId, BuildContext.version,
                                            BuildContext.branch.branchName,
                                            BuildContext.branch.isProduction())
                                    BuildContext.artifactIds.add(configPackage.packageId)
                                    dir("${BuildContext.path.buildPath}/config/target") {
                                        createNexusService(BuildContext.nexusConfig).tortugaPackage(configPackage).upload()
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        stage("notify-ansible") {
            when {
                equals expected: true, actual: BuildContext.isProduction()
            }
            steps {
                script {
                    BuildContext.awxService.runRecipe("estapp-update-release",
                            "{\\\"version\\\": \\\"${BuildContext.version}\\\"}")
                }
            }
        }
    }

    post {
        always {
              logParser parsingRulesPath: '/Volumes/Jenkins/workdir/LogParser-Unity.rules', showGraphs: true, useProjectRule: false, projectRulePath: ''
        }
        failure {
            script {
                BuildContext.notificationService.notifyBuildFault(BuildContext.job, BuildContext.branch.user)
            }
        }

        success {
            script {
                BuildContext.notificationService.notifyBuildSuccess(BuildContext.job, BuildContext.branch.user)
            }
        }
    }
}

private void fillActiveBuilds() {
    BuildContext.activeBuilds['server'] = params.serverBuild
    BuildContext.activeBuilds['android'] = params.androidBuild
    BuildContext.activeBuilds['windows'] = params.windowsBuild
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
            'estapp-client',
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

        def buildName = prod ? "release-" + context.version : context.branch.safeBranchName
                .replace('feature-', '')
                .replace('release-', '')
                .replace('bugfix-', '')
                .replace('hotfix-', '')

        withCredentials([usernamePassword(credentialsId: 'unityUserPass', passwordVariable: 'unityPass', usernameVariable: 'unityUser')]) {
            createMaven()
                    .phase('clean')
                    .phase('package')
                    .param('profiler', prod ? "false" : "true")  // указываем profiler в зависимости от продакшена
                    .param('il2cpp', prod ? "true" : "false")   // указываем сборку в il2cpp в зависимости от продакшена
                    .param('splitApk', prod ? "true" : "false") // указываем splitApk в зависимости от продакшена
                    .param('platform', type.name)
                    .param('skipReplace', 'false')
                    .param('skipTest', 'true')
                    .param('buildVersion', context.version)
                    .param('buildName', buildName)
                    .param('unityUser', unityUser)
                    .param('unityPass', unityPass)
                    .param("production", prod ? "true" : "false")
                    .param("facebookAppId", prod ? "..." : "..")                               // указываем facebookAppId в зависимости от продакшена, тестовый или настоящий, если нет facebook - убираем
                    .param("projectName", prod ? "estapp" : "estapp-stage")                   // указываем projectName в зависимости от продакшена
                    .param("logProjectName", prod ? "estapp-client" : "estapp-client-test")  // указываем projectName для логаера в зависимости от продакшена
                    .param("configUrl", prod ? context.projectOptions["productionConfigUrl"] : context.projectOptions["stageConfigUrl"])
                    .param("aab", prod ? "true" : "false")                                  // указываем сборку в aab в зависимости от продакшена
                    .param("bigBrotherActive", "true")
                    .param('unityPath', "/Applications/Unity/Hub/Editor/${BuildContext.unityVersion}/Unity.app/Contents/MacOS/") // указываем unityPath для сборки 
                    .run()
        }
        def hashPath = "${clientBuildPath}/target/Android/hash.json"  // указываем путь до hash приложения, если у нас он вычисляется
        if (fileExists(hashPath)) {
            def hash = readFile hashPath
            echo "hash ${hash}"
        }
        fileOperations([fileCopyOperation(excludes: '', flattenFiles: true, includes: "target/*.zip",
                targetLocation: context.path.targetPath)])
    }
}

/* 
def buildAppServer(def context ) {  // раскомментировать,если нужна фаза сборки сервера 
    def serverBuildPath = "${context.path.buildPath}/server"
    if (!fileExists(serverBuildPath)) {
        fileOperations([folderCreateOperation(serverBuildPath)])
    }

    syncDirectory("${context.path.repoPath}/server/", serverBuildPath)

    dir("${context.path.buildPath}/server") {
        createMaven()
                .phase('clean')
                .phase('package')
                .param("buildVersion", context.version)
                .run();

        fileOperations([fileCopyOperation(excludes: '', flattenFiles: true, includes: "target */
/*.zip", targetLocation: context.path.targetPath)])
    }
}
 */



/* private def buildResources(def context, PlatformType type, forceBuildLib = false) { // раскомментировать,если нужна фаза сборки ресурсов 
    def resourceBuildPath = "${context.path.buildPath}/${type.name}resources"
    durationService = createDurationService()

    if (!forceBuildLib) {
        durationService.start("resourcesLibSaveLocalCache")
        saveLocalCache(resourceBuildPath, "Library", "${type.name}resources")
        durationService.end("resourcesLibSaveLocalCache")

        durationService.start("resourcesTargetSaveLocalCache")
        saveLocalCache(resourceBuildPath, "target", "${type.name}resources")
        durationService.end("resourcesTargetSaveLocalCache")
    }

    syncDirectory("${context.path.repoPath}/resources/", resourceBuildPath)

    if (!forceBuildLib) {
        durationService.start("resourcesLibRestoreLocalCache")
        restoreLocalCache(resourceBuildPath, "Library", "${type.name}resources")
        durationService.end("resourcesLibRestoreLocalCache")

        durationService.start("resourcesLibRestoreLocalCache")
        restoreLocalCache(resourceBuildPath, "target", "${type.name}resources")
        durationService.end("resourcesLibRestoreLocalCache")
    }

// retrieve  cache from master branch
    def libPath = "${resourceBuildPath}/Library"
    if (!fileExists(libPath) && !forceBuildLib) {
        durationService.start("restoreResourcesUnityLib")
        context.cacheService.restoreResourcesUnityLib(type, libPath)
        durationService.end("restoreResourcesUnityLib")
    }
    def targetPath = "${resourceBuildPath}/target"
    if (!fileExists(targetPath) && !forceBuildLib) {
        durationService.start("restoreTargetUnityLib")
        context.cacheService.restoreResourcesTarget(type, targetPath)
        durationService.end("restoreTargetUnityLib")
    }
    dir(resourceBuildPath) {
        def m = createMaven()
                .phase("clean")
                .phase("package")
                .param("platform", type.name)
                .param("zipBundles", "true")
                .param("chunkCompression", "true")
                .param("zipStreamingAssets", "true")
                .param('unityPath', "/Applications/Unity/Hub/Editor/${BuildContext.unityVersion}/Unity.app/Contents/MacOS/")
        if (context.branch.isProduction() || context.branch.isRC()) {
        }
        m.run()
        durationService.start("fileCopyOperation target *//*.zip")
        fileOperations([fileCopyOperation(excludes: '', flattenFiles: true, includes: "target *//*.zip",
                targetLocation: context.path.targetPath)])
        durationService.end("fileCopyOperation target *//*.zip")
    }
}

private void deployResources(PlatformType platform) {
    def resourceBuildPath = BuildContext.path.buildPath + "/" + platform.name + 'resources'
    def resourcePom = readMavenPom file: resourceBuildPath + '/pom.xml'
    def resourcePackage = createPackage(resourcePom.groupId,
            resourcePom.artifactId, resourcePom.version,
            BuildContext.branch.branchName,
            BuildContext.branch.isProduction(), platform)
    BuildContext.artifactIds.add(resourcePackage.packageId)

    dir("${resourceBuildPath}/target") {
        createNexusService(BuildContext.nexusConfig).tortugaPackage(resourcePackage).upload()
    }
} */


