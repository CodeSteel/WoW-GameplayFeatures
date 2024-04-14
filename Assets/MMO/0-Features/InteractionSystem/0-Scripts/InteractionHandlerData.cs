using System;
using SteelBox;

namespace MMO.InteractionSystem
{
    public static class InteractionHandlerData
    {
        public static event Action<IInteraction> C_OnInteractionStarted;

        public static void C_InteractionStarted(IInteraction interaction) =>
            C_OnInteractionStarted?.Invoke(interaction);

        public static event Action C_OnInteractionCanceled;
        public static void C_InteractionCanceled() => C_OnInteractionCanceled?.Invoke();

        public static event ActionRef<bool> C_OnGetMouseOverInteraction;

        public static void C_GetMouseOverInteraction(ref bool isOverInteraction) =>
            C_OnGetMouseOverInteraction?.Invoke(ref isOverInteraction);
    }
}