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
        [SerializeField, Tooltip("A list of items that Will be dropped in this loot drop.")]
        GameObject[] m_GuaranteedItems;
        [SerializeField, Tooltip("A list of items that may be dropped in this loot drop.")]
        GameObject[] m_RandomItems;
        [SerializeField, Tooltip("Should the loot spawn as a child of the object spawning it, or as a separate game object.")]
        bool m_SpawnAsChild = false;
        [SerializeField, Tooltip("The minimum spawn offset from the spawn transform (the pivot of the transform of this component).")]
        Vector3 m_MinSpawnOffset = new Vector3(-0.5f, 0.5f, -0.5f);
        [SerializeField, Tooltip("The maximum spawn offset from the spawn transform (the pivot of the transform of this component).")]
        Vector3 m_MaxSpawnOffset = new Vector3(0.5f, 0.75f, 0.5f);

        /// <summary>
        /// A method to call when an AI is killed. A loot drop will be created at the currently location.
        /// </summary>
        public void OnDropLoot()
        {   
            for (int i = 0; i < m_GuaranteedItems.Length; i++)
            {
                SpawnLoot(m_GuaranteedItems[i]);
            }

            if (m_RandomItems.Length > 0)
            {
                SpawnLoot(m_RandomItems[Random.Range(0, m_RandomItems.Length)]);
            }
        }

        private void SpawnLoot(GameObject item)
        {
            Vector3 pos = m_MinSpawnOffset + (Random.value * (m_MaxSpawnOffset - m_MinSpawnOffset));

            // OPTIMIZATION: use a pool
            GameObject go = Instantiate(item);
            if (m_SpawnAsChild)
            {
                go.transform.position = pos;
                go.transform.SetParent(transform, false);
            }
            else
            {
                go.transform.position = transform.position + pos;
            }
        }
    }
}
