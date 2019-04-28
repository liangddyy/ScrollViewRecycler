using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollRecycler
{
    public class RecycleItemRender : MonoBehaviour
    {
        public virtual void RefreshView(IRecycleData data)
        {
            // data as yours object.
        }
    }


    public interface IRecycleData
    {
    }
}