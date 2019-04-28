using System.Collections;
using System.Collections.Generic;
using ScrollRecycler;
using UnityEngine;
using UnityEngine.UI;

public class ItemTest : RecycleItemRender
{
    public Text TestContent;

    public override void RefreshView(IRecycleData data)
    {
        TestContent.text = (data as TestItemData).Value.ToString();
    }
}

public class TestItemData : IRecycleData
{
    public int Value;
}