using UnityEngine;

namespace ScrollRecycler
{
    public class ScrollRecycleHorizontal : ScrollRecycleBase
    {
        private int columnSum;

        protected override void SetVertWidOrHorHeightOnOffset()
        {
            rectHeight = (rowCount - 1) * (itemHeight + offsetY);
        }

        protected override void SetVertHeightOrHorWid(int count)
        {
            columnSum = count / rowCount + (count % rowCount > 0 ? 1 : 0); //计算有多少行，用于计算出总高度
            rectWidth = Mathf.Max(0, columnSum * itemWidth + (columnSum - 1) * offsetX);
        }

        protected override Vector2 GetPos(int index)
        {
            var spread = 0;
            return new Vector2(index / rowCount * (itemHeight + offsetX),
                -index % rowCount * (itemWidth + offsetY) - spread);
        }

        protected override int GetStartIndex(Vector2 pos)
        {
            var value = pos.x;
            value = -value;
            var _spreadWidth = 0;
            if (value <= itemWidth + _spreadWidth)
                return 0;
            var scrollWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            if (value >= itemParent.sizeDelta.x - scrollWidth - _spreadWidth) //拉到底部了
            {
                if (listCount <= createCount)
                    return 0;
                return listCount - createCount;
            }

            return ((int) ((value - _spreadWidth) / (itemWidth + offsetX)) +
                    ((value - _spreadWidth) % (itemWidth + offsetX) > 0 ? 1 : 0) - 1) * rowCount;
        }
    }
}