using Base;
using Base.Status;
using Gruntz.Items;

namespace Gruntz.Statuses
{
    public class ItemHolderStatusDef : StatusDef
    {
        public ItemDef DefaultItemDef;
        protected override StatusData StatusData
        {
            get
            {
                var item = default(DefRef<ItemDef>);
                if (DefaultItemDef != null) {
                    item = DefaultItemDef.ToDefRef<ItemDef>();
                }
                return new ItemHolderStatusData { Item = item };
            }
        }
    }
}
