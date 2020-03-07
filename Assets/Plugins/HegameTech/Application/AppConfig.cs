using UnityEngine;

namespace Hegametech.Framework
{
    public class AppConfig : ScriptableObject
    {

        // 下面为自动代码填充区域，请勿在此写内容，会被覆盖掉
        #region AUTO FLUSH AREA
        ///<<< BEGIN FLUSH

		#region 版本号
		/// <summary>
		/// 打包时候的主版本号
		/// </summary>
		public const string MAIN_VERSION = "0.0.0";
		/// <summary>
		/// 打包时候的应用版本号
		/// </summary>
		public const string APP_VERSION_BUILD = "0.0.0.1.beta.202002182133";
		/// <summary>
		/// 打包时候的资源版本号
		/// </summary>
		public const string RES_VERSION_BUILD = "0.0.0.0..202002182133";
		#endregion

		#region 产品名称
		/// <summary>
		/// PlayerSettings 填写的 英文 ProductName ， 用于更新资源的子文件夹名称等
		/// </summary>
		public const string PRODUCT_NAME = "LinkedCube";
		#endregion

        ///<<< END FLUSH
        #endregion
        // 上面为自动代码填充区域，请勿在此写内容，会被覆盖掉

        #region 字段
        public const string APP_CONFIG_DIR_PATH = "Assets/Resources/";
        public const string APP_CONFIG_FILE = "AppConfig";
        private static AppConfig m_instance;
        #endregion

        #region 序列化区域

        [SerializeField] private string m_appVersion;
        [SerializeField] private string m_resVersion;

        #region get set
        public static string S_AppVersion
        {
            get { return Instance.m_appVersion; }
        }

        public static string S_ResVersion
        {
            get { return Instance.m_resVersion; }
        }
        #endregion

        #region EDITOR 模式下 GET SET
#if UNITY_EDITOR
        public string AppVersion { get { return m_appVersion; } set { m_appVersion = value; } }
        public string ResVersion { get { return m_resVersion; } set { m_resVersion = value; } }
#endif
        #endregion

        #endregion

        #region 创建销毁 ScriptableObject

        private void Awake()
        {
            if (string.IsNullOrEmpty(m_appVersion))
            {
                m_appVersion = string.Empty;
                Log.Warning("m_appVersion is null");
            }
            if (string.IsNullOrEmpty(m_resVersion))
            {
                m_resVersion = string.Empty;
                Log.Warning("m_resVersion is null");
            }
        }

        private static string AssetsResourcesPath2Path(string path)
        {
            return path.Substring(17);
        }

        private static AppConfig LoadInstance()
        {
            UnityEngine.Object obj = Resources.Load(AssetsResourcesPath2Path(APP_CONFIG_DIR_PATH + APP_CONFIG_FILE));
            if (obj == null)
            {
                Log.Error("Not Find AppConfig File.");
            }
            Log.Debug("Success Load AppConfig Config.");
            m_instance = obj as AppConfig;
            return m_instance;
        }

        public static AppConfig Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = LoadInstance();
                }
                return m_instance;
            }
        }

        public static void Unload()
        {
            Resources.UnloadAsset(m_instance);
            m_instance = null;
        }
        #endregion

        #region 获取版本号
        /// <summary>
        /// 获取 APP Version 的所有序号
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        /// <param name="build"></param>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        public void GetAppVersionInfo(ref string major, ref string minor, ref string patch, ref string build, ref string type, ref string desc)
        {
            var split = m_appVersion.Split('.');
            if (split.Length == 6)
            {
                major = split[0];
                minor = split[1];
                patch = split[2];
                build = split[3];
                type = split[4];
                desc = split[5];
            }
            else
            {
                major = string.Empty;
                minor = string.Empty;
                patch = string.Empty;
                build = string.Empty;
                type = string.Empty;
                desc = string.Empty;
            }
        }

        /// <summary>
        /// 获取 Res Version 的除主版本号的序号
        /// </summary>
        /// <param name="build"></param>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        public void GetResVersionInfo(ref string build, ref string type, ref string desc)
        {
            var split = m_resVersion.Split('.');
            if (split.Length == 6)
            {
                build = split[3];
                type = split[4];
                desc = split[5];
            }
            else
            {
                build = 0.ToString();
                type = string.Empty;
                desc = string.Empty;
            }
        }

        public string GetMainVersion()
        {
            var split = m_appVersion.Split('.');
            if (split.Length == 6)
            {
                return split[0] + "." + split[1] + "." + split[2];
            }
            return string.Empty;
        }
        #endregion

        #region 设置版本号

        #region EDITOR下的完全控制权
#if UNITY_EDITOR
        /// <summary>
        /// EDITOR下的
        /// </summary>
        /// <param name="ss"></param>
        public void SetAppVersionInfo(string ss)
        {
            m_appVersion = ss;
        }

        /// <summary>
        /// EDITOR下的
        /// </summary>
        /// <param name="ss"></param>
        public void SetResVersionInfo(string ss)
        {
            m_resVersion = ss;
        }

        /// <summary>
        /// EDITOR下的 资源版本号自动更新
        /// </summary>
        public void ResVersionAutoUpdate()
        {
            string m_resbuildNum = string.Empty;
            string m_restypeTxt = string.Empty;
            string m_resdescTxt = string.Empty;
            GetResVersionInfo(ref m_resbuildNum, ref m_restypeTxt, ref m_resdescTxt);
            m_resbuildNum = (int.Parse(m_resbuildNum) + 1).ToString();
            m_resdescTxt = System.DateTime.Now.ToString("yyyyMMddHHmm");
            int m4 = int.Parse(m_resbuildNum.Trim());
            string m5 = m_restypeTxt.Trim();
            string m6 = m_resdescTxt.Trim();
            if (m_restypeTxt.Contains(".") || m_resdescTxt.Contains("."))
            {
                Hegametech.Framework.Log.Error("不能包含 . 这个符号"); 
            }
            string merge = (MAIN_VERSION + "." + m4.ToString() + "." + m5 + "." + m6).Trim();
            SetResVersionInfo(merge);
        }
#endif
        #endregion

        #region RUNTIME下只能设置ResVersion的非主版本序号
        /// <summary>
        /// 设置更新的 ResVersion, 传入 后三位序号内容 格式 X.X.X
        /// </summary>
        /// <param name="lastThreeIndexInfo">后三位序号内容 格式 X.X.X  不包含主版本号</param>
        public void SetUpdatedResVersionInfo(string lastThreeIndexInfo)
        {
            m_resVersion = GetMainVersion() + "." + lastThreeIndexInfo;
        }
        #endregion

        #endregion
    }
}
