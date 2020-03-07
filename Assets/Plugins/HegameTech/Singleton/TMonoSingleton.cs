namespace Hegametech.Framework
{
    public abstract class TMonoSingleton<T> : MonoSingleton, ISingleton where T : TMonoSingleton<T>
    {
        private static T m_instance = null;
        private static readonly object s_lock = new object();

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (s_lock)
                    {
                        if (m_instance == null)
                        {
                            m_instance = CreateMonoSingleton<T>();
                        }
                    }
                }

                return m_instance;
            }
        }

        public virtual void OnSingletonInit() { }

    }
}
