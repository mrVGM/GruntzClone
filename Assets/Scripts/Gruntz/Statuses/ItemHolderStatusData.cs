using Base;
using Base.Status;
using Gruntz.Items;
using System;

namespace Gruntz.Statuses
{
    [Serializable]
    public class ItemHolderStatusData : StatusData
    {
        public DefRef<ItemDef> Item;
    }
}
