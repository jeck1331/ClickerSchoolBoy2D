using System;

namespace _Scripts.Events
{
    public static class ClickerEvents
    {
        /// ------- Event Actions
        public static event Action OnClickOrTap;

        public static event Action<ulong, bool> OnCoinsChanged;

        /// ------- Invokes
        public static void InvokeClickOrTap() => OnClickOrTap?.Invoke();

        public static void InvokeCoinsChanged(ulong coins, bool isAnimate = false) =>
            OnCoinsChanged?.Invoke(coins, isAnimate);
    }
}