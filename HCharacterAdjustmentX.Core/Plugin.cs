﻿//
// HCharaAdjustmentX entry point
//
using BepInEx;

using KKAPI;
using KKAPI.Chara;

using IDHIUtils;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    /// <summary>
    /// This plug-in allows the position adjustment of the female character closer/apart,
    /// up/down or left/right. The movement are in discrete steps and characters maintain
    /// alignment no matter the orientation. This helps in some scenes to avoid or
    /// lessen the clipping.
    ///
    /// If AnimationLoader is available it can save the movement of the female character.
    ///
    /// TODO: Save Player movement info.
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
    [BepInDependency(
        "essuhauled.animationloader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, PluginDisplayName, Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    public partial class HCharaAdjustmentX : BaseUnityPlugin
    {
        internal static bool _registered = false;
        internal static bool _showGroupGuide = false;
        internal static Logg _Log = new();
        internal static SvgColor _SvgColor = new();
        internal static IDHIUtils.AnimationLoader _animationLoader = new();
        internal static bool _animationKeyOk = true;
        internal static bool _animationMovementOk = true;
        internal static bool IsAibu { get; set; } = false;
        internal static bool IsHoushi { get; set; } = false;
        internal static bool IsSonyu { get; set; } = false;
        internal static bool IsSupportedScene { get; set; } = false;

        #region Unity Methods
        private void Awake()
        {
            _Log.LogSource = base.Logger;

            ConfigDebugEnntry();
            _Log.Enabled = DebugInfo.Value;
#if DEBUG
            _Log.Info($"HCAX0001: Log.Enabled set to {_Log.Enabled}");
#endif
            _Log.Info($"HCAX0002: HCharaAdjustmentX Loaded.");
            if (!_animationLoader.Installed)
            {
                _Log.Message("HCAX0003: Cannot locate AnimationLoader saving movement " +
                    "is disabled.");
                _animationKeyOk = false;
                _animationMovementOk = false;
            }
            else
            {
                Utils.CheckAnimationLoader();
            }
            CharacterApi.RegisterExtraBehaviour<CTRL>(GUID);
        }

#if DEBUG
        private void OnEnable()
        {
            _Log.Info($"HCAX1313: [OnEnabled] Called.");
        }

        private void OnDisable()
        {
            _Log.Info("HCAX1314: [OnDisabled] Called.");
        }
#endif

        private void Start()
        {
#if DEBUG
            _Log.Info("HCAX1315: Start Called.");
#endif

            ConfigEntries();

            // Hook to HProcMonitor
            HProcMonitor.OnHSceneStartLoading += OnHStart;
            CTRL.RegisterMovementEvents();

            // Start in disabled mode
            enabled = false;
        }
        #endregion

        #region public classes
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
        public static HCharaAdjusmentXController GetControllerByType(
            CharacterType chaType)
        {
            ChaControl chaControl = null;

            if (chaType == CharacterType.Player)
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
        #endregion
    }
}
