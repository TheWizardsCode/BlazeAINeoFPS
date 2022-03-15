using NeoFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardsCode.Items.Loot
{
    /// <summary>
    /// Drop this onto anything that will drop loot and call the OnDropLoot method.
    /// </summary>
    public class LootDrop : MonoBehaviour
    {
        [SerializeField, Tooltip("A list of items that can be found in this loot drop.")]
        GameObject[] m_Items;
        [SerializeField, Tooltip("Should the loot spawn as a child of the object spawning it, or as a separate game object.")]
        bool m_SpawnAsChild = false;

        /// <summary>
        /// A method to call when an AI is killed. A loot drop will be created at the currently location.
        /// </summary>
        public void OnDropLoot()
        {
            // OPTIMIZATION: use a pool
            GameObject go = Instantiate(m_Items[Random.Range(0, m_Items.Length)]);
            go.transform.position = Vector3.zero;
            if (m_SpawnAsChild)
            {
                go.transform.SetParent(transform, false);
            }
            else
            {
                go.transform.position = transform.position;
            }
        }
    }
}
