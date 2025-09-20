using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "65c47ed0144043ef811aa082136ee5f9de02d704")]
public class Tracker : Component
{
	[ShowInEditor]
	[ParameterFile]
	private string trackerWrapperUsc = "";

	[ShowInEditor]
	[ParameterFile(Filter = ".track")]
	private List<string> trackFiles = new List<string>();

	private static bool isWrapperLoaded = false;

	private static readonly Variable initFunc = new Variable("TrackerWrapper::init");
	private static readonly Variable shutdownFunc = new Variable("TrackerWrapper::shutdown");
	private static readonly Variable addTrackFunc = new Variable("TrackerWrapper::addTrack");
	private static readonly Variable removeTrackFunc = new Variable("TrackerWrapper::removeTrack");
	private static readonly Variable getMinTimeFunc = new Variable("TrackerWrapper::getMinTime");
	private static readonly Variable getMaxTimeFunc = new Variable("TrackerWrapper::getMaxTime");
	private static readonly Variable getUnitTimeFunc = new Variable("TrackerWrapper::getUnitTime");
	private static readonly Variable setFunc = new Variable("TrackerWrapper::set");

	private static Dictionary<string, int> trackIDs = new Dictionary<string, int>();

	private static Variable trackFileVar = new Variable("");
	private static Variable trackIdVar = new Variable(0);
	private static Variable timeVar = new Variable(0.0f);

	public static bool IsInitialized { get; private set; } = false;

	public static int AddTrack(string trackFile)
	{
		if (IsInitialized && FileSystem.IsFileExist(trackFile) && FileSystem.GetExtension(trackFile) == "track")
		{
			string trackName = trackFile.Split('.')[0];
			string[] parts = trackName.Split('/');
			trackName = parts[parts.Length - 1];

			if (!trackIDs.ContainsKey(trackName))
			{
				trackFileVar.String = trackFile;
				int trackID = Engine.RunWorldFunction(addTrackFunc, trackFileVar).Int;
				trackIDs[trackName] = trackID;

				return trackID;
			}
			else
			{
				Log.Warning($"Tracker::AddTrack: {trackFile} already added\n");
				return trackIDs[trackName];
			}
		}

		return -1;
	}

	public static void RemoveTrack(int trackID)
	{
		if (IsInitialized && trackIDs.ContainsValue(trackID))
		{
			trackIdVar.Int = trackID;
			Engine.RunWorldFunction(removeTrackFunc, trackIdVar);

			string itemToRemove = "";
			foreach (var pair in trackIDs)
			{
				if (pair.Value.Equals(trackID))
					itemToRemove = pair.Key;
			}

			trackIDs.Remove(itemToRemove);
		}
	}

	public static void RemoveTrack(string trackName)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			RemoveTrack(trackIDs[trackName]);
	}

	public static bool ContainsTrack(string trackName)
	{
		if (IsInitialized)
			return trackIDs.ContainsKey(trackName);

		return false;
	}

	public static int GetTrackID(string trackName)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			return trackIDs[trackName];

		return -1;
	}

	public static float GetMinTime(int trackID)
	{
		if (IsInitialized)
		{
			trackIdVar.Int = trackID;
			return Engine.RunWorldFunction(getMinTimeFunc, trackIdVar).Float;
		}

		return 0.0f;
	}

	public static float GetMinTime(string trackName)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			return GetMinTime(trackIDs[trackName]);

		return 0.0f;
	}

	public static float GetMaxTime(int trackID)
	{
		if (IsInitialized)
		{
			trackIdVar.Int = trackID;
			return Engine.RunWorldFunction(getMaxTimeFunc, trackIdVar).Float;
		}

		return 0.0f;
	}

	public static float GetMaxTime(string trackName)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			return GetMaxTime(trackIDs[trackName]);

		return 0.0f;
	}

	public static float GetUnitTime(int trackID)
	{
		if (IsInitialized)
		{
			trackIdVar.Int = trackID;
			return Engine.RunWorldFunction(getUnitTimeFunc, trackIdVar).Float;
		}

		return 0.0f;
	}

	public static float GetUnitTime(string trackName)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			return GetUnitTime(trackIDs[trackName]);

		return 0.0f;
	}

	public static void SetTime(int trackID, float time)
	{
		if (IsInitialized)
		{
			timeVar.Float = time;
			trackIdVar.Int = trackID;
			Engine.RunWorldFunction(setFunc, trackIdVar, timeVar);
		}
	}

	public static void SetTime(string trackName, float time)
	{
		if (IsInitialized && trackIDs.ContainsKey(trackName))
			SetTime(trackIDs[trackName], time);
	}

	// initialize tracker before all components
	[MethodInit(Order = int.MinValue)]
	private void Init()
	{
		// check tracker wrapper
		string trackerWrapperGUID = FileSystem.GuidToPath(FileSystem.GetGUID(trackerWrapperUsc));
		isWrapperLoaded = (World.ScriptName == trackerWrapperGUID);

		// add tracker_wrapper to world logic if not set in editor
		if (!isWrapperLoaded)
		{
			World.ScriptName = trackerWrapperGUID;
			World.ScriptExecute = true;
			World.SaveWorld();
			World.ReloadWorld();
			return;
		}

		// init tracker in wrapper
		Engine.RunWorldFunction(initFunc);
		IsInitialized = true;

		// add tracks
		if (trackFiles != null)
		{
			foreach (var track in trackFiles)
				AddTrack(track);
		}
	}

	// shutdown tracker after all components
	[MethodShutdown(Order = int.MaxValue)]
	private void Shutdown()
	{
		if (IsInitialized)
		{
			// shutdown tracker in wrapper
			Engine.RunWorldFunction(shutdownFunc);

			trackIDs.Clear();
			isWrapperLoaded = false;
			IsInitialized = false;
		}
	}
}
