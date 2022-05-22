using NeoFPS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardsCode.AI;
using Random = UnityEngine.Random;

namespace WizardsCode.Items.Loot
{
    /// <summary>
    /// Drop this onto anything that will drop loot and call the OnDropLoot method.
    /// </summary>
    public class LootDrop : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField, Tooltip("A list of items that may be dropped in this loot drop.")]
        LootDropItem[] m_RandomItems;
        #endregion

        #region Variables
        internal static AIManager m_aiManager;
        #endregion

        #region Lifecycle Management
        private void Start()
        {
            if (m_aiManager == null)
            {
                m_aiManager = FindObjectOfType<AIManager>();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// A method to call when an AI is killed. A loot drop will be created at the currently location.
        /// </summary>
        public void OnDropLoot()
        {
            float chance;
            if (m_RandomItems.Length > 0)
            {
                for (int i = 0; i < m_RandomItems.Length; i++)
                {
                    if (m_RandomItems[i].m_LootValue <= m_aiManager.lootValue)
                    {
                        chance = m_RandomItems[i].m_Chance;
                    } 
                    else
                    {
                        chance = m_RandomItems[i].m_Chance * 0.25f;
                    }
                    if (Random.value <= chance)
                    {
                        SpawnLoot(m_RandomItems[i]);
                        if (m_RandomItems[i].m_StopSpawning)
                        {
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Loot Spawning
        private void SpawnLoot(LootDropItem item)
        {
            Vector3 pos = item.m_MinSpawnOffset + (Random.value * (item.m_MaxSpawnOffset - item.m_MinSpawnOffset));

            // OPTIMIZATION: use a pool
            GameObject go = Instantiate(item.m_Prefab);
            if (item.m_SpawnAsChild)
            {
                go.transform.position = pos;
                go.transform.SetParent(transform, false);
            }
            else
            {
                go.transform.position = transform.position + pos;
            }
        }
        #endregion
    }

    [Serializable]
    internal class LootDropItem
    {
        [SerializeField, Tooltip("The item that this drop describes.")]
        internal GameObject m_Prefab;
        [SerializeField, Tooltip("Should the loot spawn as a child of the object spawning it, or as a separate game object.")]
        internal bool m_SpawnAsChild = false;
        [SerializeField, Tooltip("The minimum spawn offset from the spawn transform (the pivot of the transform of this component).")]
        internal Vector3 m_MinSpawnOffset = new Vector3(-0.5f, 0.5f, -0.5f);
        [SerializeField, Tooltip("The maximum spawn offset from the spawn transform (the pivot of the transform of this component).")]
        internal Vector3 m_MaxSpawnOffset = new Vector3(0.5f, 0.75f, 0.5f);
        [SerializeField, Tooltip("If the current lootValue in the AI Manager is above or equal to this value then the drop is a valid" +
            " candidate. However, it may not be spawned, see Chance. Even when the Loot Value in the AI Manager is lower than this value" +
            " there is a Chance * 0.25 chance of it dropping."),
            Range(0f, 1f)]
        internal float m_LootValue = 0.5f;
        [SerializeField, Tooltip("Chance of dropping. When selecting an item at random each item is checked in order." +
            " For each valid drop item, if a random number is below or equal to this value then it will be spawned and the search will stop."),
            Range(0f,1f)]
        internal float m_Chance;
        [SerializeField, Tooltip("Shuold following items be considered if this item is spawned." +
            " If set to true, the next item in the list will be tested for spawning regardless of whether this " +
            " item is spawned or not. If set to false, the next item will only be checked if this item does not" +
            " spawn.")]
        internal bool m_StopSpawning = true;

    }
}
