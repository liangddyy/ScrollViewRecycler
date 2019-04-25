using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollRecycler
{
    public class RecycleItemBase : MonoBehaviour
    {
        public virtual void RefreshView(IRecycleNode data)
        {
            // to data as yours object.
        }
    }


    public interface IRecycleNode
    {
    }
}