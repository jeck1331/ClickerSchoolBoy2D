using System;

namespace _Scripts.Events
{
    public static class GameEvents
    {
        /// ------- Event Actions
        public static event Action OnGameInitialized;

        public static event Action OnUpgradePowerClick;

        /// ------- Invokes
        public static void InvokeGameInitialized() => OnGameInitialized?.Invoke();

        public static void InvokeUpgradePowerClick() => OnUpgradePowerClick?.Invoke();
    }
}