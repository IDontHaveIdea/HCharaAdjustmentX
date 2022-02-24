using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using UnityEngine;
using UnityEngine.SceneManagement;

using KKAPI;
using KKAPI.Chara;
using KKAPI.Utilities;

using IDHIUtils;

using SHCA = IDHIPlugins.HCharaterAdjustX.HCharacterAdjustXController;


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
    [BepInDependency(IDHIUtils.PInfo.GUID, IDHIUtils.PInfo.Version)]
    [BepInDependency("essuhauled.animationloader", "1.1.2")]
    //[BepInDependency("essuhauled.animationloader", AnimationLoader)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInPlugin(PInfo.GUID, PInfo.PluginDisplayName, PInfo.Version)]
    public partial class HCharaterAdjustX : BaseUnityPlugin
    {
        #region Fields definitions
        static internal ConfigEntry<bool> DebugInfo;

        static internal bool _registered = false;
        static internal bool _showGroupGuide = false;
        static internal string _sceneName;
        static internal string _activeScene;
        #endregion

        #region Properties
        static internal bool IsAibu { get; set; } = false;
        static internal bool IsHoushi { get; set; } = false;
        static internal bool IsSonyu { get; set; } = false;
        static internal bool IsSupportedScene { get; set; } = false;       
        #endregion

        #region Unity methods
        private void Awake()
        {
            Log.SetLogSource(base.Logger);

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
                Log.Enabled = DebugInfo.Value;
#if DEBUG
                Log.Level(LogLevel.Info, $"0028: Log.Enabled set to {Log.Enabled}");
#endif
            };
            Log.Enabled = DebugInfo.Value;
#if DEBUG
            Log.Level(LogLevel.Info, $"0028: Log.Enabled set to {Log.Enabled}");
#endif
            Log.Info($"SHCA0001: SimpleHCharaAdjust Loaded.");
            SvgColor.Init();
            CharacterApi.RegisterExtraBehaviour<SHCA>(PInfo.GUID);

            // Monitor loaded scenes
            SceneManager.sceneLoaded += MonitorHProc;
            SceneManager.activeSceneChanged += Utils.SceneChanged;
        }

#if DEBUG
        private void OnEnable()
        {
            Log.Info($"1313: [OnEnabled] Called.");
        }

        private void OnDisable()
        {
            Log.Info("1314: [OnDisabled] Called.");
        }
#endif

        /// <summary>
        /// Check for KKS_HCharaAdjustment loaded and adjust the configuration file accordingly
        /// Initialized CharHScene
        /// </summary>
        private void Start()
        {
#if DEBUG
            Log.Info("1315: Start Called.");
#endif

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
        #endregion

        #region public methods
        /// <summary>
        /// Get controller for characters
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        static public HCharacterAdjustXController GetController(ChaControl chaControl) =>
            chaControl == null || chaControl.gameObject == null
            ? null : chaControl.GetComponent<HCharacterAdjustXController>();

        #endregion
    }
}
