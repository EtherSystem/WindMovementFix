[assembly: MelonInfo(typeof(WindMovementFix.Core), "WindMovementFix", "1.0.0", "EtherSystem", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace WindMovementFix
{
    public sealed class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
            LoggerInstance.Msg("Initialized.");
        }
    }

    [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.GetWindMovementMultiplier))]
    internal static class PlayerMovementGetWindMovementMultiplierPatch
    {
        private const float VanillaMaximumWindMph = 65f;
        private const float ExtremeMaximumWindMph = 85f;
        private const float ExtremeHeadwindMultiplier = 0.35f;
        private const float DirectionEpsilon = 0.0001f;
        private const float HalfPi = 1.57079637f;

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(PlayerMovement __instance, ref float __result)
        {
            if (__instance == null) return;

            var weather = GameManager.GetWeatherComponent();
            var wind = GameManager.GetWindComponent();
            if (weather == null || wind == null) return;
            if (weather.IsIndoorEnvironment() || wind.PlayerShelteredFromWind()) return;

            float windSpeedMph = wind.GetSpeedMPH();
            float minimumAffectMph = __instance.m_MinWindSpeedToAffectMovement;
            if (windSpeedMph < minimumAffectMph) return;

            Vector3 velocity = __instance.GetVelocity();
            if (velocity.magnitude < 0.001f) return;

            velocity.Normalize();
            float directionDot = Vector3.Dot(velocity, wind.GetWindDirection_Base());
            if (directionDot >= -DirectionEpsilon) return;

            float headwindAngleFactor = Mathf.Clamp(HalfPi - Mathf.Acos(-directionDot), 0f, HalfPi) / HalfPi;
            float configuredMinimum = Mathf.Clamp(__instance.m_WindMovementSpeedMultiplierMin, 0f, 1f);
            float fullHeadwindMultiplier = CalculateFullHeadwindMultiplier(windSpeedMph, minimumAffectMph, configuredMinimum);
            float reduction = (1f - fullHeadwindMultiplier) * headwindAngleFactor;

            var blizzardWalker = GameManager.GetFeatBlizzardWalker();
            if (blizzardWalker != null)
            {
                reduction *= blizzardWalker.GetWalkingSpeedInWindReductionModifier();
            }

            __result = Mathf.Clamp(1f - reduction, 0f, 1f);
        }

        private static float CalculateFullHeadwindMultiplier(float windSpeedMph, float minimumAffectMph, float configuredMinimum)
        {
            float vanillaRangeEnd = Math.Max(minimumAffectMph + 0.001f, VanillaMaximumWindMph);
            if (windSpeedMph <= vanillaRangeEnd)
            {
                float vanillaIntensity = Mathf.InverseLerp(minimumAffectMph, vanillaRangeEnd, windSpeedMph);
                return Mathf.Lerp(1f, configuredMinimum, vanillaIntensity);
            }

            float extremeMinimum = Math.Min(configuredMinimum, ExtremeHeadwindMultiplier);
            float extremeIntensity = Mathf.InverseLerp(VanillaMaximumWindMph, ExtremeMaximumWindMph, windSpeedMph);
            return Mathf.Lerp(configuredMinimum, extremeMinimum, extremeIntensity);
        }
    }
}