using NeoFPS;
using NeoFPS.ModularFirearms;
using System;
using UnityEngine;
using UniversalAI;

namespace WizardsCode.UniversalAIExtentions.NeoFPS
{
    /// <summary>
    /// This component will connect a Universal AI 2.0 to NeoFPS weapons to automatically enable sound detection without
    /// having to edit NeoFPS code.
    /// 
    /// To use add this component and `SoloPlayerCharacterEventWatcher` to your AI character.
    /// </summary>
    public class AISoundDetection : MonoBehaviour, IPlayerCharacterSubscriber
    {
        [SerializeField, Tooltip("The radius within which the AI may detect sound.")]
        float m_Radius = 25;

        private IPlayerCharacterWatcher m_Watcher;
        private BaseShooterBehaviour m_Shooter;
        private FpsInventoryBase inventory;

        protected void Awake()
        {
            m_Watcher = GetComponentInParent<IPlayerCharacterWatcher>();
        }

        protected void Start()
        {
            m_Watcher.AttachSubscriber(this);
        }

        private void OnDisable()
        {
            m_Watcher.ReleaseSubscriber(this);
            if (m_Shooter)
            {
                m_Shooter.onShoot -= DetectSound;
            }
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (inventory)
            {
                inventory.onSelectionChanged -= OnWieldableSelectionChanged;
                OnWieldableSelectionChanged(0, null);
            }

            if (character as Component != null)
            {
                inventory = character.GetComponent<FpsInventoryBase>();
            }
            else
            {
                inventory = null;
            }

            if (inventory != null)
            {
                inventory.onSelectionChanged += OnWieldableSelectionChanged;
                OnWieldableSelectionChanged(0, inventory.selected);
            }
        }

        private void OnWieldableSelectionChanged(int slot, IQuickSlotItem item)
        {
            if (m_Shooter)
            {
                m_Shooter.onShoot -= DetectSound;
            }

            FpsInventoryWieldable currentWieldable = item as FpsInventoryWieldable;
            if (currentWieldable as Component)
            {
                m_Shooter = currentWieldable.GetComponent<BaseShooterBehaviour>();
                if (m_Shooter)
                {
                    m_Shooter.onShoot += DetectSound;
                }
            }
        }

        private void DetectSound(IModularFirearm source)
        {
            UniversalAIManager.SoundDetection(UniversalAIEnums.SoundType.ShootSound, m_Radius, source.transform);
        }
    }
}
