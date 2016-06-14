﻿namespace WinterMute
{
    public enum HandType { Left, Right };
    public enum VRTYPE { OculusTouchVR, SteamVR };
    public static class Config
    {
        public const string SAVE_FILE_PATH = @"Assets/3DGestureTracker/VRGestureData/";

        // records raw, unmodified tracking data when recording gestures
        // should always be true
        public const bool USE_RAW_DATA = true;
        // how many points to use in a gesture line
        public const int FIDELITY = 11; 
        // how many points captured per second
        public const int CAPTURE_RATE = 30;
        // whether to track when pressing trigger or all the time
        // continious mode is not supported yet, though you're welcome to try it out
        public const bool CONTINIOUS = false; 
    }
}
