using Base;

namespace LevelSelection
{
    public interface ILevelProvider
    {
        LevelDef LevelDef { get; }
    }
}
