//
// HCharaAdjustmentX entry point
//
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using UnityEngine.SceneManagement;

using KKAPI;
using KKAPI.Chara;
using KKAPI.Utilities;

//using AnimationLoader;

using IDHIUtils;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    /// <summary>
    /// This plug-in allows the position adjustment of the female character closer/apart, up/down
    /// or left/right. The movement are in discrete steps and characters maintain alignment no
    /// matter the orientation. They don't move left or right from each other.  This helps in some
    /// scenes to avoid or lessen the clipping.
    /// </summary>
    /// <remarks>
    ///
    /// The default shortcuts are:
    ///
    ///          L Show buttons menu for Female
    /// RigthAlt+L Show buttons for Player
    ///
    /// This plug-in is base on code from: 
    ///     KK_HCharaAdjustment (@DeathWeasel1337)
    ///     JetPack (@Madevil)
    ///
    /// </remarks>
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(IDHIUtils.Info.GUID, IDHIUtils.Info.Version)]
    [BepInDependency("essuhauled.animationloader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, PluginDisplayName, Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    public partial class HCharaAdjustmentX : BaseUnityPlugin
    {
        internal static bool _registered = false;
        internal static bool _showGroupGuide = false;
        internal static string _sceneName;
        internal static string _activeScene;
        internal static Logg _Log = new();
        internal static SvgColor _SvgColor = new();
        internal static IDHIUtils.AnimationLoader _animationLoader = new();

        internal static bool IsAibu { get; set; } = false;
        internal static bool IsHoushi { get; set; } = false;
        internal static bool IsSonyu { get; set; } = false;
        internal static bool IsSupportedScene { get; set; } = false;

        private void Awake()
        {
            _Log.LogSource = base.Logger;

            ConfigDebugEnntry();
            _Log.Enabled = DebugInfo.Value;
#if DEBUG
            _Log.Info($"[Awake] Log.Enabled set to {_Log.Enabled}");
#endif
            _Log.Info($"[Awake] HCharaAdjustmentX Loaded.");
            if (!_animationLoader.Installed)
            {
                _Log.Message("Cannot locate AnimationLoader saving movement is disabled.");
            }
            CharacterApi.RegisterExtraBehaviour<CTRL>(GUID);

            // Monitor loaded scenes
            SceneManager.sceneLoaded += MonitorHProc;
            SceneManager.activeSceneChanged += Utils.SceneChanged;
        }

#if DEBUG
        private void OnEnable()
        {
            _Log.Info($"1313: [OnEnabled] Called.");
        }

        private void OnDisable()
        {
            _Log.Info("1314: [OnDisabled] Called.");
        }
#endif

        /// <summary>
        /// Check for KKS_HCharaAdjustment loaded and adjust the configuration file accordingly
        /// Initialized CharHScene
        /// </summary>
        private void Start()
        {
            _Log.Info("1315: Start Called.");

            // Configuration entries
            // TODO: Check for KKS_HCharaAdjustment
#if DEBUG
            ConfigEntries(false);
#else
            ConfigEntries(true);
#endif
            // Initializing HProcScene
            HProcScene.Init();            
            CTRL.RegisterMovementEvents();

            // Start in disabled mode
            enabled = false;
        }

        /// <summary>
        /// Get controller for characters
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        public static HCharaAdjusmentXController GetController(ChaControl chaControl)
        {
            return ((chaControl == null) || (chaControl.gameObject == null))
                ? null : chaControl.GetComponent<HCharaAdjusmentXController>();
        }

        /// <summary>
        /// Get controller for character by CharacterType
        /// </summary>
        /// <param name="chaType"></param>
        /// <returns></returns>
        internal static HCharaAdjusmentXController GetControllerByType(CTRL.CharacterType chaType)
        {
            ChaControl chaControl = null;

            if (chaType == CTRL.CharacterType.Player)
            {
                chaControl = _hprocInstance.flags.player?.chaCtrl;
            }
            else
            {
                chaControl = _hprocInstance.flags.lstHeroine[(int)chaType]?.chaCtrl;
            }

            return ((chaControl == null) || (chaControl.gameObject == null))
                ? null : chaControl.GetComponent<HCharaAdjusmentXController>();
        }
    }
}
