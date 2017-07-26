#define DEVELOPMENT
// the preferred method of logging by the client.
// Sadly, there is currently no developer-configurable way to adjust the logging level, 
// or keep logging specific to a given group of objects.
public static class Log {
	public enum Level {
		Debug=0x01,
		Info=0x02,
		Warning=0x04,
		Error=0x08
	}
	
	public static Level ALL = Level.Error | Level.Warning | Level.Info | Level.Debug;
	
	#if DEVELOPMENT
	public static Level LevelsEnabled = ALL;
	#else
	public static Level LevelsEnabled = Level.Error | Level.Warning;
	#endif
	
	public static void Debug(string message, params object[] args) {
		Write(Level.Debug, message, args);
	}
	
	public static void Info(string message, params object[] args) {
		Write(Level.Info, message, args);
	}
	
	public static void Warning(string message, params object[] args) {
		Write(Level.Warning, message, args);
	}
	
	public static void Error(string message, params object[] args) {
		Write(Level.Error, message, args);
	}
	
	private static void Write(Level level, string message, object[] args) {
		if ((level & LevelsEnabled) != level)
			return;
		
		if (args != null && args.Length > 0)
			message = string.Format(message, args);
		
		string threadId = !string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name : System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
		
		//"yyyy'-'MM'-'dd'T'HH':'mm':'ss,fff"
		message = string.Concat(System.DateTime.UtcNow.ToString("ss,fff"), "  [", level.ToString(), "]["+ threadId + "]  ", message);
		
		#if UNITY_EDITOR
		switch (level) {
		case Level.Debug:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Info:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Warning:
			UnityEngine.Debug.LogWarning(message);
			break;
		case Level.Error:
			UnityEngine.Debug.LogError(message);
			break;
		}
		#elif UNITY_ANDROID
		switch (level) {
		case Level.Debug:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Info:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Warning:
			UnityEngine.Debug.LogWarning(message);
			break;
		case Level.Error:
			UnityEngine.Debug.LogError(message);
			break;
		}
		#elif UNITY_IPHONE
		switch (level) {
		case Level.Debug:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Info:
			UnityEngine.Debug.Log(message);
			break;
		case Level.Warning:
			UnityEngine.Debug.LogWarning(message);
			break;
		case Level.Error:
			UnityEngine.Debug.LogError(message);
			break;
		}
		#endif
	}
}
