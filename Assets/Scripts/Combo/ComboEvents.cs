using System;

namespace MahjongGame.Combo
{
    public static class ComboEvents
    {
        public static event Action<ComboChangedContext> ComboChanged;

        public static event Action<ComboIncreasedContext> ComboIncreased;

        public static event Action ComboExpired;

        internal static void RaiseComboChanged(ComboChangedContext context)
        {
            if (context == null)
            {
                return;
            }

            ComboChanged?.Invoke(context);
        }

        internal static void RaiseComboIncreased(ComboIncreasedContext context)
        {
            if (context == null)
            {
                return;
            }

            ComboIncreased?.Invoke(context);
        }

        internal static void RaiseComboExpired()
        {
            ComboExpired?.Invoke();
        }
    }
}
