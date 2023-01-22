using NeoFPS;
using UnityEngine;
using UnityEngine.UI;
using WizardsCode.Common;

namespace WizardsCode.FPS
{
    public class NeoGameManager : AbstractGameManager, IPlayerCharacterSubscriber
    {
        [Header("NeoFPS")]
        [SerializeField, Tooltip("The game mode for this mission.")]
        private BlazeNeoMinimalGame m_NeoGame;

        private IInventory inventory;
        private IHealthManager playerHealthManager;
        private ICharacter m_Player;

        protected override void Awake()
        {
            base.Awake();
            FpsGameMode.onInGameChanged += OnInNeoGameChanged;
        }

        private void OnInNeoGameChanged(bool inGame)
        {
            if (inGame)
            {
                m_GameState = GameState.InitializingGame;
            }
        }

        protected override void StartGame()
        {
            base.StartGame();

            m_NeoGame.Respawn(m_NeoGame.player);
        }

        protected override void InitializeLoadout()
        {
            if (mission.m_InventoryLoadout != null)
            {
                inventory.ApplyLoadout(mission.m_InventoryLoadout);
            }

            // TODO: this is a workaround for a bug in NeoFPS that is due to be fixed 6/12 - see also InitializeMission
            for (int i = 0; i < SpawnManager.spawnPoints.Count; i++)
            {
                SpawnManager.spawnPoints[i].onSpawn -= InitializeLoadout;
            }
        }

        protected override void InitializeMission()
        {
            base.InitializeMission();
            FindObjectOfType<SoloPlayerCharacterEventWatcher>().AttachSubscriber(this);

            // TODO: this is a workaround for a bug in NeoFPS that is due to be fixed 6/12 - see also InitializeMission
            for (int i = 0; i < SpawnManager.spawnPoints.Count; i++)
            {
                SpawnManager.spawnPoints[i].onSpawn += InitializeLoadout;
            }
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (playerHealthManager != null)
            {
                playerHealthManager.onIsAliveChanged -= OnIsAliveChaged;
            }

            if (character == null)
            {
                return;
            }

            m_Player = character;
            playerHealthManager = character.GetComponent<IHealthManager>();
            playerHealthManager.onIsAliveChanged += OnIsAliveChaged;

            inventory = character.GetComponent<IInventory>();
            inventory.ApplyLoadout(mission.m_InventoryLoadout);

            deathCanvasGroup.alpha = 0f;
        }

        protected override void OnIsAliveChaged(bool alive)
        {
            base.OnIsAliveChaged(alive);

            if (!alive)
            {
                int timeshiftsLeft = mission.m_LivesAvailable - livesLost;
                deathCanvasGroup.GetComponentInChildren<Text>().text = $"{UIStrings.DeathScreeMessage.Replace("{AvailableTimeshifts}", timeshiftsLeft.ToString())}";
                deathCanvasGroup.alpha = 1f;

                var items = inventory.GetItems();
                foreach (var item in items)
                {
                    FpsInventoryWieldable wieldable = item.GetComponent<FpsInventoryWieldable>();
                    if (wieldable != null)
                    {
                        bool dropIt = false;
                        for (int i = 0; i < mission.m_InventoryLoadout.items.Length; i++)
                        {
                            if (mission.m_InventoryLoadout.items[i].itemIdentifier == wieldable.itemIdentifier)
                            {
                                dropIt = true;
                                break;
                            }
                        }
                        if (dropIt)
                        {
                            wieldable.DropItem(wieldable.transform.position, transform.forward, new Vector3(Random.value, Random.value, Random.value));
                        }
                    }
                    else
                    {
                        FpsInventoryAmmo ammo = item.GetComponent<FpsInventoryAmmo>();
                        if (ammo != null)
                        {
                            SharedAmmoTypeExtended sharedAmmoType = ammo.m_AmmoType as SharedAmmoTypeExtended; // TODO: this requires FPSInventoryAmmo to have the m_AmmoType marked public. Need an accessor in that class. Proposed to Chris.
                            if (sharedAmmoType != null)
                            {
                                InventoryItemPickup[] drops = sharedAmmoType.GetDefaultPickups(ammo.quantity);
                                foreach (var drop in drops)
                                {
                                    drop.transform.position = ammo.transform.position;

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
