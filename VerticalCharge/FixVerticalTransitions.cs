using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SereCore;
using UnityEngine;

namespace VerticalCharge
{
    internal static class FixVerticalTransitions
    {
        public static void Hook()
        {
            UnHook();

            On.GameManager.FinishedEnteringScene += DisableUpwardOneways;
        }
        public static void UnHook()
        {
            On.GameManager.FinishedEnteringScene -= DisableUpwardOneways;
        }

        // Deactivate upward oneway transitions after spawning in so the player doesn't accidentally
        // softlock by vc-ing into them
        private static void DisableUpwardOneways(On.GameManager.orig_FinishedEnteringScene orig, GameManager self)
        {
            orig(self);

            switch (self.sceneName)
            {
                // The KP top transition is the only one that needs to be disabled; the others have collision
                case SceneNames.Tutorial_01:
                    if (GameObject.Find("top1") is GameObject topTransition)
                        topTransition.SetActive(false);
                    break;
            }
        }
    }
}
