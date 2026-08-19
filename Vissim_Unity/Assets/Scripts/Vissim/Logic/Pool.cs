using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Vissim.Logic
{
    public class Pool : MonoBehaviour
    {
        public string Prefab;
        public int Init_Count;
        private List<GameObject> Items;

        private GameObject Instantiate_Item()
        {
            GameObject item = (GameObject)Instantiate(Resources.Load(Prefab));
            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.useGravity = false;
            return item;
        }

        public void Start()
        {
            Items = new List<GameObject>();
            for (int i = 0; i < Init_Count; ++i)
            {
                GameObject New_Pool_Item = Instantiate_Item();
                Hide_Item(New_Pool_Item);
                Items.Add(New_Pool_Item);
            }
        }

        public GameObject GetPoolItem()
        {
            if (Items.Count > 0)
            {
                GameObject Item = Items[0];
                Items.RemoveAt(0);
                return Item;
            } else return Instantiate_Item();
        }

    
        public void Return_Pool_Item(GameObject Item)
        {
            Hide_Item(Item);
            Items.Add(Item);
        }

        private void Hide_Item (GameObject Item)
        {
            Item.transform.localPosition = new Vector3(100000, 100000, -1000);
        }

        public void OnApplicationQuit()
        {
            foreach (GameObject pi in Items)
                Destroy(pi);
        }
    };
}
