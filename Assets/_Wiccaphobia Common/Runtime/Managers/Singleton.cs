using UnityEngine;

namespace WizardsCode.Managers
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        [SerializeField, Tooltip("If set to true this instance will persist between levels.")]
        bool m_IsPersistant = false;

        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<T>();
                    if (m_Instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        m_Instance = obj.AddComponent<T>();
                    }
                }
                return m_Instance;
            }
        }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                m_Instance = this as T;
                if (m_IsPersistant)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}