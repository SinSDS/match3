using System;
using UnityEngine;

public class BuildNumber
{
	private static string svnVersion = string.Empty;

	private static string svnBuildDate = string.Empty;

	private static string jenkinsBuildVersion = string.Empty;

	private static string gitLastHash = string.Empty;

	public static string GetSVNRevision()
	{
		if (string.IsNullOrEmpty(svnVersion))
		{
			TextAsset textAsset = Resources.Load<TextAsset>("revision");
			if (!string.IsNullOrEmpty(textAsset.text))
			{
				int num = textAsset.text.IndexOf("Last Changed Rev: ") + 18;
				int num2 = textAsset.text.IndexOf('\n');
				svnVersion = textAsset.text.Substring(num, num2 - num);
				svnVersion = svnVersion.Replace("\r", string.Empty);
			}
		}
		return svnVersion;
	}

	public static string GetSVNRevisionDate()
	{
		if (string.IsNullOrEmpty(svnBuildDate))
		{
			TextAsset textAsset = Resources.Load<TextAsset>("revision");
			if (textAsset != null)
			{
				int num = textAsset.text.IndexOf("Last Changed Date: ") + 19;
				int num2 = textAsset.text.IndexOf(" +");
				string s = textAsset.text.Substring(num, num2 - num);
				DateTime result = DateTime.Now;
				DateTime.TryParse(s, out result);
				svnBuildDate = result.ToString();
			}
		}
		return svnBuildDate;
	}

	public static string GetJenkinsBuildVersion()
	{
		if (string.IsNullOrEmpty(jenkinsBuildVersion))
		{
			TextAsset textAsset = Resources.Load<TextAsset>("buildversion");
			if (!string.IsNullOrEmpty(textAsset.text))
			{
				jenkinsBuildVersion = textAsset.text.Replace("\r", string.Empty).Replace("\n", string.Empty);
			}
		}
		return jenkinsBuildVersion;
	}

	public static string GetGitLashHash()
	{
		if (string.IsNullOrEmpty(gitLastHash))
		{
			TextAsset textAsset = Resources.Load<TextAsset>("gitlasthash");
			if (!string.IsNullOrEmpty(textAsset.text))
			{
				gitLastHash = textAsset.text.Replace("\r", string.Empty).Replace("\n", string.Empty);
			}
		}
		return gitLastHash;
	}
}
