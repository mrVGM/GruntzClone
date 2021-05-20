using UnityEngine;

namespace Gruntz.Navigation
{
    public interface ITravelSegmentInfo
    {
        Vector3 EndPos { get; }
        Vector3 StartPos { get; }
    }

    public struct TravelSegmentInfo : ITravelSegmentInfo
    {
        public Vector3 EndPos { get; set; }
        public Vector3 StartPos { get; set; }
    }

    public static class TravelSegmentInfoUtils
    {
        public static bool AreCompatible(ITravelSegmentInfo ts1, ITravelSegmentInfo ts2)
        {
            if (AreTravelSegmentsEqual(ts1, ts2))
            {
                return true;
            }

            if ((ts1.EndPos - ts2.EndPos).sqrMagnitude < 0.00001f)
            {
                return false;
            }

            if ((ts1.StartPos - ts1.EndPos).sqrMagnitude < 0.00001f || (ts2.StartPos - ts2.EndPos).sqrMagnitude < 0.00001f)
            {
                return true;
            }

            Vector3 cross1 = Vector3.Cross(ts2.StartPos - ts1.StartPos, ts1.EndPos - ts1.StartPos);
            Vector3 cross2 = Vector3.Cross(ts2.EndPos - ts1.StartPos, ts1.EndPos - ts1.StartPos);
            if (Vector3.Dot(cross1, cross2) >= 0) 
            {
                return true;
            }

            cross1 = Vector3.Cross(ts1.StartPos - ts2.StartPos, ts2.EndPos - ts2.StartPos);
            cross2 = Vector3.Cross(ts1.EndPos - ts2.StartPos, ts2.EndPos - ts2.StartPos);
            if (Vector3.Dot(cross1, cross2) >= 0)
            {
                return true;
            }

            return false;
        }

        public static bool AreTravelSegmentsEqual(ITravelSegmentInfo ts1, ITravelSegmentInfo ts2)
        {
            if ((ts1.StartPos - ts2.StartPos).sqrMagnitude >= 0.00001f)
            {
                return false;
            }
            if ((ts1.EndPos - ts2.EndPos).sqrMagnitude >= 0.00001f)
            {
                return false;
            }
            return true;
        }
    }
}
