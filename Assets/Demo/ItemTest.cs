using System.Collections;
using System.Collections.Generic;
using ScrollRecycler;
using UnityEngine;
using UnityEngine.UI;

public class ItemTest : RecycleItemBase
{
    public Text TestContent;

    public override void RefreshView(IRecycleNode data)
    {
        TestContent.text = (data as TestItemData).Value.ToString();
    }
}

public class TestItemData : IRecycleNode
{
    public int Value;
}