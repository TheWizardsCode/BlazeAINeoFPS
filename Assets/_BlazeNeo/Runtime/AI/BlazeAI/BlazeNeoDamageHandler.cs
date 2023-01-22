using NeoCC;
using NeoFPS;
using UnityEngine;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// This handles player to AI damage. It operates the same way as the BasicDamageHandlers in NeoFPS.
    /// You can have multiple of these on a character to provide location based damage. See the Neo FPS documentation for more info.
    /// </summary>
    public class BlazeNeoDamageHandler : BasicDamageHandler
    {
        BlazeAI blaze;

        protected override void Awake()
        {
            blaze = GetComponent<BlazeAI>();
            base.Awake();
        }

        public override DamageResult AddDamage(float damage)
        {
            blaze.Hit();
            return base.AddDamage(damage);
        }

        public override DamageResult AddDamage(float damage, RaycastHit hit)
        {
            blaze.Hit();
            return base.AddDamage(damage, hit);
        }

        public override DamageResult AddDamage(float damage, IDamageSource source)
        {
            NotifyBlazeOfHit(source);
            return base.AddDamage(damage, source);
        }

        public override DamageResult AddDamage(float damage, RaycastHit hit, IDamageSource source)
        {
            NotifyBlazeOfHit(source);
            return base.AddDamage(damage, hit, source);
        }

        private void NotifyBlazeOfHit(IDamageSource source)
        {
            NeoCharacterController enemy = ((Component)source).GetComponentInParent<NeoCharacterController>();
            if (enemy != null)
            {
                blaze.Hit(enemy.gameObject);
            }
            else
            {
                blaze.Hit();
            }
        }
    }

}