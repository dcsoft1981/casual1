using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PlatformSpecificBuild : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPreprocessBuild(BuildReport report)
	{
		/*
		string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
		var manifestJson = JObject.Parse(File.ReadAllText(manifestPath));
		var dependencies = manifestJson["dependencies"] as JObject;

		if (report.summary.platform == BuildTarget.Android)
		{
			// Android 빌드 시 iOS 전용 패키지 제거
			dependencies.Remove("com.apple.unityplugin.core");
			dependencies.Remove("com.apple.unityplugin.gamekit");
		}
		else if (report.summary.platform == BuildTarget.iOS)
		{
			// iOS 빌드 시 iOS 전용 패키지 추가
			dependencies["com.apple.unityplugin.core"] = "file:/Users/daegyukim/Documents/GitHub/unityplugins/Build/com.apple.unityplugin.core-3.1.8.tgz";
			dependencies["com.apple.unityplugin.gamekit"] = "file:/Users/daegyukim/Documents/GitHub/unityplugins/Build/com.apple.unityplugin.gamekit-3.0.2.tgz";
		}

		File.WriteAllText(manifestPath, manifestJson.ToString());
		*/
	}
}