using System.Collections.Generic;
using ScrollRecycler;
using UnityEngine;

public class UIListTest : MonoBehaviour
{
    public int Count = 100;
    private ScrollRecycleBase scrollList1;
    private ScrollRecycleBase scrollList2;

    private List<TestItemData> intlist;

    void Start()
    {
        intlist = new List<TestItemData>();
        for (var i = 0; i < Count; i++)
        {
            var a = new TestItemData();
            a.Value = i;
            intlist.Add(a);
        }

        scrollList1 = transform.Find("VerticalScroll").GetComponent<ScrollRecycleVertical>();
        scrollList1.InitList(intlist.Count, (index) => { return intlist[index]; });

        scrollList2 = transform.Find("HorizontalScroll").GetComponent<ScrollRecycleHorizontal>();
        scrollList2.InitList(intlist.Count, (index) => { return intlist[index]; });
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Add"))
        {
            intlist.Add(new TestItemData() {Value = intlist.Count});
            scrollList1.RefreshList(intlist.Count, (index) => { return intlist[index]; });
        }

        if (GUILayout.Button("remove"))
        {
            intlist.RemoveAt(intlist.Count - 1);
            scrollList1.RefreshList(intlist.Count, (index) => { return intlist[index]; });
        }

        if (GUILayout.Button("init"))
        {
            scrollList1.InitList(intlist.Count, (index) => { return intlist[index]; });
        }
    }
}