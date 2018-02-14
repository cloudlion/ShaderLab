using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class RemoveEmptyDirectory
{
	const string title = "Remove Empty Directory";
	[MenuItem("Tools/" + title)]
	public static void CleanEmptyDirectory()
	{
		var targetDir = new DirectoryInfo("Assets/");
		var emptyDis = new List<DirectoryInfo>();

		DoRemoveEmptyDirectory(targetDir, emptyDis);

		if (emptyDis.Count == 0)
		{
			EditorUtility.DisplayDialog(title, "No Empty Directory", "OK");
			return;
		}

		var sb = new System.Text.StringBuilder();
		for (int i = 0; i < emptyDis.Count; ++i)
		{
			int index = i + 1;
			sb.AppendLine(index.ToString() + " " + emptyDis[i].FullName.Replace(Application.dataPath + "/", string.Empty));
		}

		if (EditorUtility.DisplayDialog(title, sb.ToString(), "OK", "Cancel"))
		{
			foreach (var target in emptyDis)
			{
				if (File.Exists(target.FullName + ".meta"))
					File.Delete(target.FullName + ".meta");

				target.Delete(true);
			}
			AssetDatabase.Refresh();
		}
	}

	public static bool DoRemoveEmptyDirectory(DirectoryInfo targetDir, List<DirectoryInfo> emptyDirs)
	{
		bool hasDirOrFile = false;
		var dis = targetDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
		foreach (var di in dis)
		{
			bool result = DoRemoveEmptyDirectory(di, emptyDirs);
			if (result)
			{
				hasDirOrFile = true;
			}
		}

		if (!hasDirOrFile)
		{
			var fis = targetDir.GetFiles("*", SearchOption.TopDirectoryOnly);
			foreach(var fi in fis)
			{
				if (!fi.Name.StartsWith(".") && !fi.FullName.EndsWith(".meta"))
				{
					hasDirOrFile = true;
					break;
				}
			}
		}

		if (!hasDirOrFile)
		{
			if (!emptyDirs.Contains(targetDir))
				emptyDirs.Add(targetDir);
		}

		return hasDirOrFile;
	}
}
