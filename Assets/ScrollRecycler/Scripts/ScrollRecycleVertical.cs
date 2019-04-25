using UnityEngine;

namespace ScrollRecycler
{
    public class ScrollRecycleVertical : ScrollRecycleBase
    {
        private int rowSum; //总共多少行

        protected override void SetVertWidOrHorHeightOnOffset()
        {
            rectWidth = (columnCount - 1) * (itemWidth + offsetX);
        }

        protected override void SetVertHeightOrHorWid(int count)
        {
            rowSum = count / columnCount + (count % columnCount > 0 ? 1 : 0); //计算有多少行，用于计算出总高度
            rectHeight = Mathf.Max(0, rowSum * itemHeight + (rowSum - 1) * offsetY);
        }

        /// <summary>
        ///     item对应位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Vector2 GetPos(int index)
        {
            var spread = 0;
            return new Vector2(index % columnCount * (itemWidth + offsetX),
                -index / columnCount * (itemHeight + offsetY) - spread);
        }

        protected override int GetStartIndex(Vector2 pos)
        {
            var value = pos.y;
            var _spreadHeight = 0;
            if (value <= itemHeight + _spreadHeight)
                return 0;
            var scrollHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            if (value >= itemParent.sizeDelta.y - scrollHeight - _spreadHeight) //拉到底部了
            {
                if (listCount <= createCount)
                    return 0;
                return listCount - createCount;
            }

            return ((int) ((value - _spreadHeight) / (itemHeight + offsetY)) +
                    ((value - _spreadHeight) % (itemHeight + offsetY) > 0 ? 1 : 0) - 1) * columnCount;
        }
    }
}