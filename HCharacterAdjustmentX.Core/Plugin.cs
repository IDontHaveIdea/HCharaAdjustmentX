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

using SHCA = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    /// <summary>
    /// This plug-in allows the position adjustment of the female character closer/apart, up/down
    /// or left/right. The movement are in discrete steps and characters maintain alignment no
    /// matter the orientation. They don't move left or right from each other.  This helps in some
    /// scenes to avoid or lesser the clipping.
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
    //[BepInDependency(SwapAnim.GUID, SwapAnim.Version)]
    [BepInDependency("essuhauled.animationloader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, PluginDisplayName, Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    public partial class HCharaAdjustmentX : BaseUnityPlugin
    {
        internal static ConfigEntry<bool> DebugInfo;

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

            DebugInfo = Config.Bind(
            section: "Debug",
            key: "Debug Information",
            defaultValue: false,
            configDescription: new ConfigDescription(
                description: "Show debug information in Console",
                acceptableValues: null,
                tags: new ConfigurationManagerAttributes { Order = 1, IsAdvanced = true }));
            DebugInfo.SettingChanged += (_sender, _args) =>
            {
                _Log.Enabled = DebugInfo.Value;
#if DEBUG
                _Log.Info($"0028: Log.Enabled set to {_Log.Enabled}");
#endif
            };
            _Log.Enabled = DebugInfo.Value;
#if DEBUG
            _Log.Info($"0028: Log.Enabled set to {_Log.Enabled}");
#endif
            _Log.Info($"SHCA0001: HCharaAdjustmentX Loaded.");
            CharacterApi.RegisterExtraBehaviour<SHCA>(GUID);

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

            // TODO: Decide if pull request or still look to see if can integrate.
            // HCAdjustment.Init();
            // DebugLog($"SHCA0004: HCAdjustment in system - HCAdjustment.Mansai");

            // Configuration entries
            ConfigEntries(false);

            // Initializing HProcScene
            HProcScene.Init();            
            SHCA.RegisterMovementEvents();

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
    }
}
