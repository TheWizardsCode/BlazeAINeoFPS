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

        protected override bool ShouldSpawn()
        {
            if (Player == null || (m_Player.position - transform.position).sqrMagnitude < m_MinDistanceSqr) return false;
            if (m_ActiveSpawns >= m_NumberOfSpawns || m_TimeOfNextSpawn > Time.time) return false;

            return true;
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
            }
        }
    }
}
