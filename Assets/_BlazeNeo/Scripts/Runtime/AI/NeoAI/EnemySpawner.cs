using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WizardsCode.BackgroundAI;
using NeoFPS;
using System;

namespace WizardsCode.BlazeNeoFPS
{
    public class EnemySpawner : Spawner
    {
        int m_ActiveSpawns = 0;

        protected override void Update()
        {
            if (m_ActiveSpawns < m_NumberOfSpawns
                && m_TimeOfNextSpawn <= Time.time)
            {
                Spawn(m_TotalSpawnedCount.ToString());
                m_TotalSpawnedCount++;
                m_TimeOfNextSpawn = Time.time + m_SpawnFrequency;
            }
        }

        protected override GameObject[] Spawn(string namePostfix)
        {
            GameObject[] spawnedObjects = base.Spawn(namePostfix);
            if (spawnedObjects != null && spawnedObjects.Length > 0)
            {
                for (int i = 0; i < spawnedObjects.Length; i++)
                {
                    BasicHealthManager health = spawnedObjects[i].GetComponentInChildren<BasicHealthManager>();
                    if (health)
                    {
                        health.onIsAliveChanged += DeSpawnOnDeath;
                    }
                    m_ActiveSpawns++;
                }
            }

            return spawnedObjects;
        }

        private void DeSpawnOnDeath(bool alive)
        {
            if (!alive)
            {
                m_ActiveSpawns--;
                GetComponent<BasicHealthManager>().onIsAliveChanged -= DeSpawnOnDeath;
            }
        }
    }
}
