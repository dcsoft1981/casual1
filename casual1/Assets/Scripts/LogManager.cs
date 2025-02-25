using UnityEngine;

public class LogManager
{
	public static void Log(object message)
	{
		if (!Define.DEBUG_LOG)
			return;
		Debug.Log(message);
	}

	public static void LogError(object message)
	{
		//if (!Define.DEBUG_LOG)
		//	return;
		Debug.LogError(message);
	}
}
