using UnityEngine;
using NeoFPS.WieldableTools;
using NeoSaveGames.Serialization;
using NeoSaveGames;
using NeoFPS;

namespace WizardsCode.Weapons
{
    public class WieldableTriggerModule : BaseWieldableToolModule, INeoSerializableComponent
    {
        [SerializeField, Tooltip("The range within which bombs will be set off when the trigger is pressed. Bomb objects must have the `TriggerableBomb` component attached."), Range(10f, 100f)]
        float m_Range = 50;
        [SerializeField, Tooltip("The layers on which to look for bombs within the range of this trigger.")]
        LayerMask m_Layers;

        Collider[] m_PossibleBombs = new Collider[100];

        public override bool isValid
        {
            get { return true; }
        }

        public override WieldableToolActionTiming timing
        {
            get { return (WieldableToolActionTiming)WieldableToolOneShotTiming.Start; }
        }

        public override void FireStart()
        {
            int candidates = Physics.OverlapSphereNonAlloc(transform.position, m_Range, m_PossibleBombs, m_Layers);
            TriggerableBomb bomb;
            for (int i = 0; i < candidates; i++)
            {
                bomb = m_PossibleBombs[i].GetComponent<TriggerableBomb>();
                if (bomb != null)
                {
                    bomb.AddDamage(float.MaxValue);
                }
            }
        }
        
        public override void FireEnd(bool success)
        {   
        }
        
        public override bool TickContinuous()
        {
            return true;
        }

        #region Serialization
        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
        }
        #endregion
    }
}
