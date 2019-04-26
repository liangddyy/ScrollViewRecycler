using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollRecycler
{
    public class ItemRender : MonoBehaviour
    {
        public virtual void RefreshView(IRecycleNode data)
        {
            // data as yours object.
        }
    }


    public interface IRecycleNode
    {
    }
}