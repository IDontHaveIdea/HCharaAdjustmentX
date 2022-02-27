using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using UnityEngine.SceneManagement;

using KKAPI;
using KKAPI.Chara;
using KKAPI.Utilities;

using AnimationLoader;

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
    [BepInDependency(Utilities.GUID, Utilities.Version)]
    [BepInDependency(SwapAnim.GUID, SwapAnim.Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInPlugin(GUID, PluginDisplayName, Version)]
    public partial class HCharaAdjustmentX : BaseUnityPlugin
    {
        static internal ConfigEntry<bool> DebugInfo;

        static internal bool _registered = false;
        static internal bool _showGroupGuide = false;
        static internal string _sceneName;
        static internal string _activeScene;
        static internal Logg _Log = new();
        static internal SvgColor _SvgColor = new();

        static internal bool IsAibu { get; set; } = false;
        static internal bool IsHoushi { get; set; } = false;
        static internal bool IsSonyu { get; set; } = false;
        static internal bool IsSupportedScene { get; set; } = false;       

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
                _Log.Level(LogLevel.Info, $"0028: Log.Enabled set to {_Log.Enabled}");
            };
            _Log.Enabled = DebugInfo.Value;
            _Log.Level(LogLevel.Info, $"0028: Log.Enabled set to {_Log.Enabled}");
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
        static public HCharaAdjusmentXController GetController(ChaControl chaControl)
        {
            return ((chaControl == null) || (chaControl.gameObject == null))
                ? null : chaControl.GetComponent<HCharaAdjusmentXController>();
        }
    }
}
