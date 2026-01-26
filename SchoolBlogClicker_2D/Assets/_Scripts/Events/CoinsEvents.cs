using System;

namespace _Scripts.Events
{
    public static class CoinsEvents
    {
        /// ------- Event Actions

        public static event Action<ulong, bool> OnCoinsChanged;

        /// ------- Invokes

        public static void InvokeCoinsChanged(ulong coins, bool isAnimate = false) =>
            OnCoinsChanged?.Invoke(coins, isAnimate);
    }
}
