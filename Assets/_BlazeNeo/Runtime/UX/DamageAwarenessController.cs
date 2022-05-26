using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using System;
using Cinemachine;

namespace WizardsCode.UX
{
    /// <summary>
    /// When enabled connects to the player and looks for the AI that most recently hit the player.
    /// It is then assumed that this AI killed the player and the camera turns to show it.
    /// </summary>
    public class DamageAwarenessController : MonoBehaviour, IPlayerCharacterSubscriber
    {
        ICinemachineCamera killCam;
        Transform playerT;
        IHealthManager playerHealthManager;
        IDamageSource lastDamageSource;

        private void Awake()
        {
            FindObjectOfType<SoloPlayerCharacterEventWatcher>().AttachSubscriber(this);
        }

        private void OnDisable()
        {
            playerHealthManager.onIsAliveChanged -= OnIsAliveChaged;
            playerHealthManager.onHealthChanged -= OnHealthChanged;
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (playerHealthManager != null)
            {
                playerHealthManager.onIsAliveChanged -= OnIsAliveChaged;
                playerHealthManager.onHealthChanged -= OnHealthChanged;
            }

            if (character == null)
            {
                return;
            }

            playerT = character.transform;
            playerHealthManager = character.GetComponent<IHealthManager>();
            playerHealthManager.onIsAliveChanged += OnIsAliveChaged;
            playerHealthManager.onHealthChanged += OnHealthChanged;

            ICinemachineCamera[] candidateCameras = character.GetComponentsInChildren<ICinemachineCamera>();
            for (int i = 0; i < candidateCameras.Length; i++)
            {
                if (candidateCameras[i].Name == "KillCam")
                {
                    killCam = candidateCameras[i];
                    break;
                }
            }
            if (killCam == null)
            {
                Debug.LogError("No KillCam found on the player prefab. Please add a virtual camera with the name `KillCam`.");
            }
        }

        private void OnHealthChanged(float from, float to, bool critical, IDamageSource source)
        {
            lastDamageSource = source;
        }

        private void OnIsAliveChaged(bool alive)
        {
            if (alive) return;
            killCam.LookAt = lastDamageSource.damageSourceTransform;
            killCam.Priority = 999;
        }
    }
}
