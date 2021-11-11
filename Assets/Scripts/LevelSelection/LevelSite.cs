using Base;

namespace LevelSelection
{
    public class LevelSite : Site, ILevelProvider
    {
        public LevelDef LevelDef;
        LevelDef ILevelProvider.LevelDef => LevelDef;
    }
}
