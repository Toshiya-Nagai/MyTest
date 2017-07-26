using UnityEngine;
using System.Collections;
using System;


#region TimeControl
static public class Times{
	
	/// <summary>
	/// Gets the convert date time.
	/// </summary>
	/// <returns>The convert date time.</returns>
	/// <param name="timestamp">Timestamp.</param>
	static public System.DateTime ToDateTime(long timestamp){
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
	}
	/// <summary>
	/// Gets the convert to timestamp.
	/// </summary>
	/// <returns>The convert to timestamp.</returns>
	/// <param name="time">Time.</param>
	static public TimeSpan ToTimeSpan(DateTime time){
		return time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
	}
	
	static public TimeSpan ToTimeSpan(long timeStamp){
		var time = ToDateTime(timeStamp);
		return ToTimeSpan(time);
	}
	
	static public TimeSpan Now(){
		return ToTimeSpan(DateTime.Now);
	}
	
	
	/// <summary>
	/// Gets the remain time span.
	/// </summary>
	/// <returns>The remain time span.</returns>
	/// <param name="startTimeStamp">Start time stamp.</param>
	/// <param name="endTimeStamp">End time stamp.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	static public System.TimeSpan GetRemain<T>(T startTimeStamp,T endTimeStamp)where T : IConvertible{
		long start = 0;
		long end = 0;
		var startSuccess = long.TryParse(startTimeStamp.ToString(),out start);
		var endSuccess = long.TryParse (endTimeStamp.ToString(),out end);
		var remain = (startSuccess && endSuccess)?(ToDateTime(end)-ToDateTime (start)):System.TimeSpan.Zero;
		return remain;
	}
	
	/// <summary>
	/// Remains the end.
	/// </summary>
	/// <returns><c>true</c>, if end was remained, <c>false</c> otherwise.</returns>
	/// <param name="remain">Remain.</param>
	static public bool RemainEnd(TimeSpan remain){
		return remain < TimeSpan.Zero;
	}
}
static public class TimeEvent{
	static public long GetNextTime(long end,long interval){
		return ((long)end - (long)Times.Now ().TotalSeconds) % interval;
	}
	static public TimeSpan GetNextTimespan(long end,long interval){
		return Times.ToTimeSpan(GetNextTime(end,interval));
	}
}
#endregion
