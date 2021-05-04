using UnityEngine;

namespace Gruntz.Navigation
{
    public interface IOccupiedPosition
    {
        bool IsValid { get; }
        Vector3 Pos { get; }
    }
}
