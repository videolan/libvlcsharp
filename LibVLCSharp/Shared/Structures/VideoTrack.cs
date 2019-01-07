namespace LibVLCSharp.Shared
{
    public readonly struct VideoTrack
    {
        public readonly uint Height;

        public readonly uint Width;

        public readonly uint SarNum;

        public readonly uint SarDen;

        public readonly uint FrameRateNum;

        public readonly uint FrameRateDen;

        public readonly VideoOrientation Orientation;

        public readonly VideoProjection Projection;

        public VideoViewpoint Pose;
    }
}
