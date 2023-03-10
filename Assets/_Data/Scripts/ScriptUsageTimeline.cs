using FMODUnity;
using SOA;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

class ScriptUsageTimeline : MonoBehaviour
{
    class TimelineInfo
    {
        public int bar = 0;
        public int beat = 0;
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
    }

    [ParamRef]
    public string drops;
    public FloatVariable timeLineBar;
    public FloatVariable timeLineBeat;
    public IntVariable numDrops;
    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    private int lastDrop;

    public FMODUnity.EventReference EventName;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

#if UNITY_EDITOR
    void Reset()
    {
        EventName = FMODUnity.EventReference.Find("event:/music/music");
    }
#endif

    void Start()
    {
        numDrops.value = 0;
        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        musicInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();
    }

    public void Update()
    {
        timeLineBar.value = timelineInfo.bar;
        timeLineBeat.value = timelineInfo.beat;
        if(lastDrop != numDrops.value)
        {
            PlayMusicPressStart();
            lastDrop = numDrops.value;
        }
    }

    void OnDestroy()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("BAR = {0}, BEAT = {1}", timelineInfo.bar, timelineInfo.beat));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.bar = parameter.bar;
                        timelineInfo.beat = parameter.beat;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }

    public void PlayMusicPressStart()
    {
        RuntimeManager.StudioSystem.setParameterByName(drops, numDrops.value);
    }
}