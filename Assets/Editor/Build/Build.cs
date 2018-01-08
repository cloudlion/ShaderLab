using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using System.Text.RegularExpressions;
public class Build
{
	public static string folder_ios = "../ios";
	public static string folder_ios_publish = "../ios_publish";
	private static string[] levels = {"Assets/Scenes/splash.unity", "Assets/Scenes/Loading.unity", "Assets/Scenes/city.unity"};
	private static string version = "0.01";

	private const string internalAdhoc = "c52ea52b-cd13-4b0b-9a4a-7c7996951094";
	private const string internalSubmit = "2d139503-fa14-47a5-83cd-eb09b6b3fd84";
	private const string publishAdhoc = "d85ee6b1-001a-4742-b773-fe5a60947aea";
	private const string publishSubmit = "6af8538e-1a68-4976-afbf-0ed9eef724fd";

	public Build ()
	{


	}

	public static void BuildProject()
	{

		version = System.Environment.GetEnvironmentVariable("VERSION");
		string pm = System.Environment.GetEnvironmentVariable("PLATFORM");

		bool iphone, android, mac;
		ParsePlatform(pm, out iphone, out android, out mac);
		if(iphone)
			BuildIOS();
		if(android)
			ExecuteAndroid();
//		if(mac)
//			ExecuteMac();
	}
	
	

	
	public static void BuildPlayer(string[] scenes, string path, BuildTarget target, bool isDebug, string version)
	{
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		BuildOptions op = GetBuildOptions(isDebug);
		PlayerSettings.bundleVersion = version;
		string result = BuildPipeline.BuildPlayer(scenes, path, target, op);
		if(!string.IsNullOrEmpty(result)) {
			//UnityEngine.Debug.Log("warning: build player result: " + result);
		}
	}

	private static BuildOptions GetBuildOptions(bool debug)
	{
		//return BuildOptions.AutoRunPlayer;
		//return BuildOptions.AcceptExternalModificationsToPlayer;
		if(debug)
			return BuildOptions.AcceptExternalModificationsToPlayer| BuildOptions.AllowDebugging | BuildOptions.Development;
		else
			return BuildOptions.AcceptExternalModificationsToPlayer;
		
		return BuildOptions.None;
	}
	
	private static BuildTarget GetBuildTarget(string target)
	{
		BuildTarget tar = BuildTarget.StandaloneWindows;
		switch (target.Trim().ToLower())
		{
		case "mac":
			tar = BuildTarget.StandaloneOSXIntel;
			break;
		case "ios":
			tar = BuildTarget.iOS;
			break;
		case "android":
			tar = BuildTarget.Android;
			break;
		default:
			throw new Exception("unkown build target: " + target);
		}
		return tar;
	}

	static void BuildIOS(){
		// change platform
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		}

		string cfg = System.Environment.GetEnvironmentVariable("CONFIG");
		bool debug, release, submit;
		GetConfig(cfg, out debug, out release, out submit);
		


	//	BuildOptions options = (release ? BuildOptions.AcceptExternalModificationsToPlayer : BuildOptions.AcceptExternalModificationsToPlayer|BuildOptions.Development);

        BuildTarget buildTarget = BuildTarget.iOS;
		string folder = System.Environment.GetEnvironmentVariable("TUNNEL").Contains("internal")?folder_ios:folder_ios_publish;
		bool internalVersion = System.Environment.GetEnvironmentVariable ("TUNNEL").Contains ("internal");
		PlayerSettings.bundleIdentifier = internalVersion?"com.liang.aok":"com.rastargames.goc";
//		PlayerSettings.bundleIdentifier = internalVersion?"com.liang.aok":"com.teamtopgame.cryptids";

//		PlayerSettings.iOS.appleDeveloperTeamID = internalVersion ? "H3ZUW6HNPA" : "VUAVR6N5YQ";
//		if (submit)
//			PlayerSettings.iOS.iOSManualProvisioningProfileID = internalVersion ? internalSubmit : publishSubmit;
//		else
//			PlayerSettings.iOS.iOSManualProvisioningProfileID = internalVersion ? internalAdhoc : publishAdhoc;


		BuildPlayer (levels, folder, buildTarget, debug, version);
//		ExcludeFiles(BuildTarget.iOS, true);		
//		BuildPipeline.BuildPlayer(levels, currentDir + "/doam_ios", buildTarget, options);		
//		ExcludeFiles(BuildTarget.iOS, false);
	}

	static void ExecuteAndroid()
	{
		// string cfg = System.Environment.GetEnvironmentVariable("CONFIG");
		// bool debug,release;
		// GetConfig(cfg, out debug, out release);
		// if(debug)
		// {
		// 	AndroidDevelopmentETC();
		// }else {
		// 	AndroidReleaseETC();
		// }
	}

	static void buildAndroidApk()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		string mode = args[10];
		version = args[9];


//		string windowsPlugin = Application.dataPath + "/Plugins/Windows" ;
//		if (Directory.Exists (windowsPlugin)) {
//			Directory.Delete(windowsPlugin , true);
//		} 

		if(mode.Equals("debug"))
		{
			AndroidDevelopmentETC();
		}else {
			AndroidReleaseETC();
		}
	}
	
//	[MenuItem("Build/BuildProject")]
//	static void  BuildProject ()
//	{
//		BuildIOS(false);
//	}
//	
//	[MenuItem("Build/BuildReleaseProject")]
//	static void BuildReleaseProject(){
//		BuildIOS(true);
//	}


	public static void ParsePlatform(string platform, out bool iphone, out bool android, out bool mac)
	{
		iphone = android = mac = false;
		if(string.IsNullOrEmpty(platform)) {
			iphone = android = mac = true;
			return;
		}
		
		string[] pms = platform.ToLower().Trim().Split('|');
		foreach (string pm in pms)
		{
			if (pm.Trim() == "ios")
				iphone = true;
			
			if (pm.Trim() == "android")
				android = true;
			
			if (pm.Trim() == "mac")
				mac = true;
		}
		
		if(!iphone && !android && !mac) {
			iphone = true;
		}
	}
	
	private static void GetConfig(string conf, out bool debug, out bool release, out bool submit)
	{
		debug = release = submit = false;
		if(string.IsNullOrEmpty(conf)) {
			debug = true;
			release = true;
			return;
		}
		
		string[] cfgs = conf.ToLower().Trim().Split('|');
		foreach(string cfg in cfgs) {
			if(cfg.Trim() == "debug")
				debug = true;
			if(cfg.Trim() == "release")
				release = true;
			if(cfg.Trim() == "submit")
			{
				debug = false;
				submit = true;
			}
		}
		
		if(!debug && !release) {
			debug = false;
			release = true;
		}
	}
	
	static void AndroidDevelopmentDXT(){
		BuildAndroid (false, MobileTextureSubtarget.DXT, 2);		
	}
	
	
	static void AndroidDevelopmentPVRTC(){
		BuildAndroid (false, MobileTextureSubtarget.PVRTC, 3);		
	}
	
	static void AndroidDevelopmentATC(){
		BuildAndroid (false, MobileTextureSubtarget.ATC, 4);		
	}
	
	static void AndroidDevelopmentETC(){
		BuildAndroid (false, MobileTextureSubtarget.ETC, 1);	
	}
	
	static void AndroidReleaseDXT(){
		BuildAndroid (true, MobileTextureSubtarget.DXT,2);		
	}
	
	
	static void AndroidReleasePVRTC(){
		BuildAndroid (true, MobileTextureSubtarget.PVRTC,3);		
	}
	
	static void AndroidReleaseATC(){
		BuildAndroid (true, MobileTextureSubtarget.ATC,4);	
	}
	
	static void AndroidReleaseETC(){
		BuildAndroid (true, MobileTextureSubtarget.ETC,1);		
	}
	
	
	static void BuildAndroid (bool release,MobileTextureSubtarget target, int bundleVersion )
	{
		// change platform
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		}

		string currentDir = Application.dataPath;
		int index = currentDir.LastIndexOf ("/");
		currentDir = currentDir.Remove (index);
		index = currentDir.LastIndexOf ("/");
		currentDir = currentDir.Remove (index);
		PlayerSettings.Android.keystoreName = currentDir + "/tools/androidKeystore/linkage.keystore";
		PlayerSettings.Android.keyaliasPass = "aok123456";
		PlayerSettings.Android.keyaliasName = "linkage";
		PlayerSettings.Android.keystorePass = "linkageandroid";
	//	PlayerSettings.use32BitDisplayBuffer = false;
		
		EditorUserBuildSettings.androidBuildSubtarget = target;
		string time = System.DateTime.Now.ToString("yyMMddHHmm");
		string name ="aok_"+bundleVersion + "_" +time;
		if(release)
		{
			name+="_rel";
		}else {
			name+="debug";
		}
		
		// PlayerSettings.bundleVersion = Environment.GetCommandLineArgs()[Environment.GetCommandLineArgs().Length - 1];
		PlayerSettings.bundleVersion = version;
		// set refresh time 
//		Time.maximumDeltaTime = 0.05f;
		
		if(release){
			PlayerSettings.Android.useAPKExpansionFiles = true;
		}
		else 
		{
			PlayerSettings.Android.useAPKExpansionFiles = false;
		}
		if(GameVersion.paymentDebug == 1)
		{
			PlayerSettings.bundleIdentifier = "com.rastargames.goc";
		}else {
			PlayerSettings.bundleIdentifier = "com.liang.aok";
		}

		currentDir = currentDir + "/" + name + ".apk";

#pragma warning disable 0162
//		if(DoamBuild.VERSION_CODE == 0){
			PlayerSettings.Android.bundleVersionCode = Convert.ToInt32((bundleVersion.ToString() + PlayerSettings.bundleVersion.Replace(".", "")));
//		}else{
//			PlayerSettings.Android.bundleVersionCode = Convert.ToInt32(bundleVersion.ToString()+DoamBuild.VERSION_CODE.ToString());
//		}
#pragma warning restore 0162
		BuildPipeline.BuildPlayer(levels,currentDir, BuildTarget.Android, release ?  BuildOptions.None : BuildOptions.Development);		

	}
	
	/* FILES EXCLUSION */
	private const string ASSETS_DIR_NAME = "Assets";
	private const string EXCLUSION_TEMP_DIR_NAME = "BuildFilesExclusionTemp";
	
	public static IDictionary<BuildTarget, IList<string>> FilesToIgnore = new Dictionary<BuildTarget, IList<string>>() {
		{BuildTarget.iOS, new List<string>(){"Resources/Font/DroidSansFallback.ttf"}}
	};
	
	private static void ExcludeFiles(BuildTarget buildTarget, bool exclude = true) {
		if(!FilesToIgnore.ContainsKey(buildTarget))
			return;
		
		if(exclude)
			AssetDatabase.CreateFolder(ASSETS_DIR_NAME, EXCLUSION_TEMP_DIR_NAME);	
		
		string tempDirPath = ASSETS_DIR_NAME + '/' + EXCLUSION_TEMP_DIR_NAME;
		
		IList<string> filesToIgnore = FilesToIgnore[buildTarget];
		
		for(int i = 0; i < filesToIgnore.Count; i++) {
			string originalPath = ASSETS_DIR_NAME + '/' + filesToIgnore[i];
			
			string originalFileName = Path.GetFileNameWithoutExtension(originalPath);
			string fileExtension = Path.GetExtension(originalPath);
			string tempFileNameExtension = i + fileExtension;
			
			string moveNewPath = exclude 
				? tempDirPath + '/' + originalFileName + fileExtension
				: Path.GetDirectoryName(originalPath) + '/' + tempFileNameExtension;			
			
			AssetDatabase.MoveAsset(
				exclude ? originalPath : tempDirPath + '/' + tempFileNameExtension,
				moveNewPath
			);
			
			AssetDatabase.RenameAsset(
				moveNewPath,
				exclude ? i.ToString() : originalFileName
			);
		}
		
		if(!exclude)
			AssetDatabase.DeleteAsset(tempDirPath);
	}
	

}


