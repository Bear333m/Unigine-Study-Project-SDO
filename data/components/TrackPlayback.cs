using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ffeea9d63e6d3b347b52141a40884907670027a7")]
public class TrackPlayback : Component
{
	private int animBaseTrackID = -1;
	private int animDiskTrackID = -1;

	private float animBaseTrackTime = 0.0f;
	private float animDiskTrackTime = 0.0f;

	private bool isPlayingAnimBase = false;
	private bool isPlayingAnimDisk = false;


	void Init()
	{
		// write here code to be called on component initialization
		animBaseTrackID = Tracker.GetTrackID("animBase");			//получаем ID для анимации с таким названием из компонента Tracker
		animBaseTrackTime = Tracker.GetMinTime("animBase");			//получаем время начала анимации с таким названием из компонента Tracker

		animDiskTrackID = Tracker.GetTrackID("animDisk");
		animDiskTrackTime = Tracker.GetMinTime("animDisk");
	}
	
	void Update()
	{
		// write here code to be called before updating each render frame
		if (isPlayingAnimBase)
		{
			animBaseTrackTime += Game.IFps;
			if (animBaseTrackTime >= Tracker.GetMaxTime(animBaseTrackID))		//если время анимации дошло до конца, то записываем его время начала
			{
				animBaseTrackTime = Tracker.GetMinTime(animBaseTrackID);
			}
			Tracker.SetTime(animBaseTrackID, animBaseTrackTime);				//после чего ставим анимацию в трекере на полученное время
		}

		if (isPlayingAnimDisk)
		{
			animDiskTrackTime += Game.IFps;
			if (animDiskTrackTime >= Tracker.GetMaxTime(animDiskTrackID))		//если время анимации дошло до конца, то записываем его время начала
			{
				animDiskTrackTime = Tracker.GetMinTime(animDiskTrackID);
			}
			Tracker.SetTime(animDiskTrackID, animDiskTrackTime);				//после чего ставим анимацию в трекере на полученное время
		}

		if (Input.IsKeyDown(Input.KEY.K))		//для проверок
		{
			StartAnim("animBase");
		}

		if (Input.IsKeyDown(Input.KEY.L))		//для проверок
		{
			StartAnim("animDisk");
		}

		if (Input.IsKeyDown(Input.KEY.J))		//для проверок
		{
			StopAnim("animBase");
			StopAnim("animDisk");
		}
	}


	public void StartAnim(string trackName)			//поменял с private на public
	{
		switch (trackName)
		{
			case "animBase":
				isPlayingAnimBase = true;
				Tracker.SetTime(animBaseTrackID, animBaseTrackTime);
				break;
			case "animDisk":
				isPlayingAnimDisk = true;
				Tracker.SetTime(animDiskTrackID, animDiskTrackTime);
				break;
			default:
				return;
		}
	}

	public void StopAnim(string trackName)			//поменял с private на public
	{
		switch (trackName)
		{
			case "animBase":
				isPlayingAnimBase = false;
				animBaseTrackTime = 0.0f;
				Tracker.SetTime(animBaseTrackID, animBaseTrackTime);
				break;
			case "animDisk":
				isPlayingAnimDisk = false;
				animDiskTrackTime = 0.0f;
				Tracker.SetTime(animDiskTrackID, animDiskTrackTime);
				break;
			default:
				return;
		}
	}

	public float GetAnimBaseTrackTime()			//дописал сам
	{
		return animBaseTrackTime;
	}
}