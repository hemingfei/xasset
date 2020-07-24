using System;

namespace Hegametech.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonAttribute : System.Attribute
    {
        private string m_AbsolutePath;

        public MonoSingletonAttribute(string relativePath)
        {
            m_AbsolutePath = relativePath;
        }

        public string AbsolutePath
        {
            get { return m_AbsolutePath; }
        }
    }
}
