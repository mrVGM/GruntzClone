using UnityEngine;

namespace Gruntz.Navigation
{
    public interface ITravelSegmentInfo
    {
        Vector3 Pos { get; }
        Vector3 StartPos { get; }
    }
}
