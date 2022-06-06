using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WizardsCode.Optimization
{
    /// <summary>
    /// The ProximityActivationManager will enable and disable objects based on their proximity to a target object.
    /// Place this component in your scene as a static manager.
    /// 
    /// Objects are automatically registered with the manager if they have a `
    /// `ProximityRegistration` behaviour attached to the object.
    /// 
    /// Objects are split into "distance bands" with the closest being validated most frequently and the furthest
    /// being validated least frequently.
    /// 
    /// OPTIMIZATION: This code could be optimized further by using `CullingGroups`
    /// </summary>
    public class ProximityActivationManager : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField, Tooltip("The target to detect proximity to.")]
        internal Transform m_ProximityTarget;

        [Header("Tick")]
        [SerializeField, Tooltip("The frequency, in seconds, at which to evaluate distances from the target and to enable/disable managed objects. Not that not every object will be evaluated on every tick. Ones that are closer to the target will be evaluated more frequently.")]
        float m_TickFrequency = 0.5f;

        HashSet<ProximityRegistration> m_FrequentlyManagedObjects = new HashSet<ProximityRegistration>();
        HashSet<ProximityRegistration> m_MidFrequencyManagedObjects = new HashSet<ProximityRegistration>();
        HashSet<ProximityRegistration> m_InfrequentManagedObjects = new HashSet<ProximityRegistration>();

        Transform ProximityTarget
        {
            get {
                return m_ProximityTarget; 
            }
        }
        internal virtual void Start()
        {
            StartCoroutine(EvalauteCo());
        }
        internal void Add(ProximityRegistration obj)
        {
            obj.Disable();
            m_FrequentlyManagedObjects.Add(obj);
        }

        private IEnumerator EvalauteCo()
        {
            while (true)
            {
                // First cycle check all
                EvaluateAll(m_FrequentlyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);

                EvaluateAll(m_MidFrequencyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);

                EvaluateAll(m_FrequentlyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);

                // Second cycle check only mid and near
                EvaluateAll(m_MidFrequencyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);

                EvaluateAll(m_FrequentlyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);

                // Third cycle check only near
                EvaluateAll(m_FrequentlyManagedObjects);
                yield return new WaitForSeconds(m_TickFrequency);
            }
        }

        private void EvaluateAll(HashSet<ProximityRegistration> set)
        {
            for (int i = set.Count - 1; i >= 0; i--)
            {
                Evaluate(set.ElementAt(i), set);
            }
        }

        private void Evaluate(ProximityRegistration obj, HashSet<ProximityRegistration> currentSet)
        {
            if (!ProximityTarget) return;

            float distance = Vector3.SqrMagnitude(ProximityTarget.position - obj.transform.position);
            if (!obj.gameObject.activeInHierarchy && obj.DisabledByProximity && distance < obj.NearDistanceSqr)
            {
                if (currentSet != null && currentSet != m_FrequentlyManagedObjects)
                {
                    currentSet.Remove(obj);
                    m_FrequentlyManagedObjects.Add(obj);
                }
                obj.Enable();
            }
            else if (obj.gameObject.activeInHierarchy && distance > obj.FarDistanceSqr)
            {
                if (currentSet != null && currentSet != m_InfrequentManagedObjects)
                {
                    currentSet.Remove(obj);
                    m_InfrequentManagedObjects.Add(obj);
                }
                obj.Disable();
            }
            else
            {
                if (currentSet != null && currentSet != m_MidFrequencyManagedObjects)
                {
                    currentSet.Remove(obj);
                    m_MidFrequencyManagedObjects.Add(obj);
                }
            }
        }
    }
}
