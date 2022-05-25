using UnityEngine;

namespace WizardsCode.Managers
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        static T m_Instance;
        [SerializeField, Tooltip("If set to true this instance will persist between levels.")]
        bool m_IsPersistant = false;

        public virtual void OnEnable()
        {
            if (m_IsPersistant)
            {
                if (!m_Instance)
                {
                    m_Instance = this as T;
                }
                else
                {
                    Destroy(gameObject);
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                m_Instance = this as T;
            }
        }

        public static T Instance { get { return m_Instance; } }
    }
}