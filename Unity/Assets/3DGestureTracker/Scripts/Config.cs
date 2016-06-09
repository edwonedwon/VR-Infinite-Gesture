﻿namespace WinterMute
{
    public enum GestureHand { Left, Right };
    public static class Config
    {
        public const string SAVE_FILE_PATH = @"Assets/3DGestureTracker/VRGestureData/";

        public const bool USE_RAW_DATA = true; // REMOVE THIS
        // how many points to use in a gesture line
        public const int FIDELITY = 11; 
        // how many points captured per second
        public const int CAPTURE_RATE = 30; // SETTING
        // the threshold over witch a gesture is considered correct
        public const double CONFIDENCE_LIMIT = 0.98; // SETTING
        // whether to track when pressing trigger or all the time
        public const bool CONTINIOUS = false; // SETTING
        // which hand to track
        
        public const GestureHand gestureHand = GestureHand.Right; // the hand to track
    }
}
