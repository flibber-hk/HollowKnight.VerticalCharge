using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VerticalCharge
{
    internal static class FixHorizontalTransitions
    {
        public static void Hook()
        {
            UnHook();

            On.HeroController.EnterScene += DisableHorizontalQuakeEntry;
        }
        public static void UnHook()
        {
            On.HeroController.EnterScene -= DisableHorizontalQuakeEntry;
        }

        private static IEnumerator DisableHorizontalQuakeEntry(On.HeroController.orig_EnterScene orig, HeroController self, TransitionPoint enterGate, float delayBeforeEnter)
        {
            GlobalEnums.GatePosition gatePosition = enterGate.GetGatePosition();
            if (gatePosition == GlobalEnums.GatePosition.left || gatePosition == GlobalEnums.GatePosition.right || gatePosition == GlobalEnums.GatePosition.door)
            {
                self.exitedQuake = false;
            }

            return orig(self, enterGate, delayBeforeEnter);
        }
    }
}
