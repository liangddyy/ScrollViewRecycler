using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ScrollRecycler
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollRecycleBase : MonoBehaviour
    {
        public ItemRender PrefItem; //克隆体

        [SerializeField] protected int itemWidth = 50; //单元格宽
        [SerializeField] protected int itemHeight = 50; //单元格高
        [Header("显示行列数")] [SerializeField] protected int rowCount = 2;
        [SerializeField] protected int columnCount = 2;
        [Header("横纵间距")] [SerializeField] protected int offsetX = 10;
        [SerializeField] protected int offsetY = 10;

        private ScrollRect scrollRect;
        protected RectTransform itemParent;

        // 计算数据
        protected int createCount; //创建的ITEM数量
        protected int rectWidth; //列表宽度
        protected int rectHeight; //列表高度
        protected int listCount;
        private int showCount;
        private int lastStartIndex;
        private int startIndex;
        private int endIndex;

        private Dictionary<int, Transform> dicItems = new Dictionary<int, Transform>(); //item对应的序号
        private Vector3 curItemParentPos = Vector3.zero;

        private RecycleGetItem onRecycleGetItem;
        private bool isInited;

        private List<int> _newIndexList = new List<int>();
        private List<int> _changeIndexList = new List<int>();

        /// <summary>
        /// 重置列表,回滚到顶部
        /// </summary>
        public void InitList(int count, RecycleGetItem recycleGetAction)
        {
            if (!isInited)
            {
                InitData();
            }

            listCount = count;
            onRecycleGetItem = recycleGetAction;
            itemParent.transform.localPosition = Vector2.zero;

            SetVertHeightOrHorWid(listCount);

            itemParent.sizeDelta = new Vector2(rectWidth, rectHeight);
            showCount = Mathf.Min(listCount, createCount);
            startIndex = 0;
            dicItems.Clear();

            for (int i = 0; i < showCount; i++)
            {
                Transform item = GetItem(i);
                SetItem(item, i);
            }

            ShowListCount(itemParent, showCount);
        }

        /// <summary>
        /// 重新刷新列表,不回滚
        /// </summary>
        /// <param name="count">列表的元素的最大个数</param>
        /// <param name="updateItem">单个元素的赋值</param>
        public void RefreshList(int count, RecycleGetItem updateItem)
        {
            onRecycleGetItem = updateItem;
            SetVertHeightOrHorWid(count);

            itemParent.sizeDelta = new Vector2(rectWidth, rectHeight);
            listCount = count;
            showCount = Mathf.Min(count, createCount); //显示item的数量
            dicItems.Clear();
            if (count == 0)
            {
                ShowListCount(itemParent, showCount);
                return;
            }

            if (count <= createCount)
            {
                startIndex = 0;
                endIndex = count - 1;
            }
            else
            {
                startIndex = GetStartIndex(itemParent.localPosition);
                if (startIndex + createCount >= count)
                {
                    startIndex = count - createCount;
                    endIndex = count - 1;
                }
                else
                {
                    endIndex = startIndex + createCount - 1;
                }
            }

            lastStartIndex = startIndex;
            if (endIndex < startIndex)
            {
                Debug.LogError("列表有问题！");
                return;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                Transform item = GetItem(i - startIndex);
                SetItem(item, i);
            }

            ShowListCount(itemParent, showCount);
        }


        private void OnValueChange(Vector2 pos)
        {
            curItemParentPos = itemParent.localPosition;
            if (listCount <= createCount)
                return;
            // 当前起始索引
            startIndex = GetStartIndex(itemParent.localPosition);
            if (startIndex + createCount >= listCount)
            {
                startIndex = listCount - createCount;
                endIndex = listCount - 1;
            }
            else
            {
                endIndex = startIndex + createCount - 1;
            }

            // 不需要刷新
            if (startIndex == lastStartIndex)
                return;
            lastStartIndex = startIndex;
            _newIndexList.Clear();
            _changeIndexList.Clear();
            for (int i = startIndex; i <= endIndex; i++)
            {
                _newIndexList.Add(i);
            }

            // ReSharper disable once GenericEnumeratorNotDisposed
            var e = dicItems.GetEnumerator();
            while (e.MoveNext())
            {
                int index = e.Current.Key;
                if (index >= startIndex && index <= endIndex)
                {
                    if (_newIndexList.Contains(index))
                        _newIndexList.Remove(index);
                }
                else
                {
                    _changeIndexList.Add(e.Current.Key);
                }
            }

            // 刷新新界面和索引
            for (int i = 0; i < _newIndexList.Count && i < _changeIndexList.Count; i++)
            {
                int oldIndex = _changeIndexList[i];
                int newIndex = _newIndexList[i];
                if (newIndex >= 0 && newIndex < listCount)
                {
                    var item = dicItems[oldIndex];
                    dicItems.Remove(oldIndex);
                    SetItem(item, newIndex);
                }
            }
        }

        private Transform GetItem(int index)
        {
            Transform item;
            if (index < itemParent.childCount)
                item = itemParent.GetChild(index);
            else
                item = ((GameObject) GameObject.Instantiate(PrefItem.gameObject)).transform;
            item.name = index.ToString();
            item.SetParent(itemParent);
            item.localScale = Vector3.one;
            return item;
        }

        /// <summary>
        /// 刷新item对应数据信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private void SetItem(Transform item, int index)
        {
            dicItems[index] = item;
            item.localPosition = GetPos(index);
            item.name = index.ToString();
            item.GetComponent<ItemRender>().RefreshView(onRecycleGetItem(index));
        }

        /// <summary>
        /// 显示子物体的数量
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="num"></param>
        private void ShowListCount(Transform trans, int num)
        {
            if (trans.childCount < num)
                return;
            for (int i = 0; i < num; i++)
            {
                trans.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = num; i < trans.childCount; i++)
            {
                trans.GetChild(i).gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置 item长宽，行列数
        /// </summary>
        /// <param name="width">item宽</param>
        /// <param name="height">item长</param>
        /// <param name="column"></param>
        /// <param name="row">一个列表最多能显示多少行（元素）</param>
        public void SetMargin(int width, int height, int column, int row)
        {
            itemWidth = width;
            itemHeight = height;
            columnCount = column;
            rowCount = row;
            InitData();
        }

        /// <summary>
        /// 设置列表的 item长宽，列数和行,初始化事件等
        /// 该设置会在下次 initList 或 refreshList 生效
        /// </summary>
        private void InitData()
        {
            if (columnCount >= rowCount)
            {
                columnCount = columnCount + 2;
            }
            else
            {
                rowCount = rowCount + 2;
            }

            createCount = columnCount * rowCount;
            if (createCount <= 0)
            {
                Debug.LogError("横纵不能为0！");
                return;
            }

            scrollRect = transform.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnValueChange);
            }

            itemParent = scrollRect.content;
            itemParent.pivot = new Vector2(0, 1);

            RectTransform rec = PrefItem.GetComponent<RectTransform>();
            rec.anchorMin = new Vector2(0, 1);
            rec.anchorMax = new Vector2(0, 1);
            rec.pivot = new Vector2(0, 1);

            isInited = true;
        }

        /// <summary>
        /// 设置元素之间的间距 spacing
        /// 该设置会在下次 initList 或 refreshList 生效
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetOffset(int x, int y)
        {
            offsetX = x;
            offsetY = y;
            SetVertWidOrHorHeightOnOffset();
        }

        #region 横向或纵向复写方法

        protected abstract void SetVertWidOrHorHeightOnOffset();

        protected abstract void SetVertHeightOrHorWid(int count);

        /// <summary>
        /// 获取当前点的位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract Vector2 GetPos(int index);

        /// <summary>
        /// 获取当前点的index索引
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract int GetStartIndex(Vector2 pos);

        #endregion
    }

    public delegate IRecycleNode RecycleGetItem(int index);
}