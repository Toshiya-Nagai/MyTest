using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using N_NGUIUtil;

public class AutoTimeEvent{
	private TimeSpan _end;
	private TimeSpan _interval;
	private UnityAction _intervalEvent;
	private Func<bool> _abortEvent;
	private bool _active;
	private TimeSpan _prevInterval;
	
	public AutoTimeEvent(TimeSpan end,TimeSpan interval,UnityAction intervalEvent,Func<bool> abortEvent){
		this._end = end;
		this._interval = interval;
		this._intervalEvent = intervalEvent;
		this._abortEvent = abortEvent;
		this._active = true;
		this._prevInterval = Times.Now();
	}
	private long calcNext(){
		return TimeEvent.GetNextTime((long)_end.TotalSeconds,(long)_interval.TotalSeconds);
	}
	private bool endTime(){return ((long)Times.Now ().Subtract(_end).TotalSeconds > 0);}
	private bool isEvent{get{return ((calcNext () == 0) && ((long)_prevInterval.TotalSeconds != (long)Times.Now().TotalSeconds));}}
	public TimeSpan GetRemain(){return Times.GetRemain<long>((long)Times.Now().TotalSeconds,(long)_end.TotalSeconds);}
	public void update(){
		if(_active){
			if((_abortEvent != null && _abortEvent()) || endTime ()){
				_active = false;
			}
			if(isEvent){
				_prevInterval = Times.Now();
				_intervalEvent.notNull(x=>{x();});
			}
		}
	}
}

public class AutoTimeDataModel{
	public long endTime;
	public long interval;
}

