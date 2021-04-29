using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SereCore;
using UnityEngine;
using VerticalCharge.FsmStateActions;
using VerticalCharge.Extensions;

namespace VerticalCharge
{
    internal static class SuperdashFsmEdit
    {
        public static void Hook()
        {
            UnHook();
            On.PlayMakerFSM.OnEnable += AllowVerticalSuperdash;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= AllowVerticalSuperdash;
        }

        private static void AllowVerticalSuperdash(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.FsmName != "Superdash" || self.gameObject.name != "Knight")
            {
                return;
            }

            FsmState upState = new FsmState(self.GetState("Right"))
            {
                Name = "Up VC"
            };
            upState.ClearTransitions();
            upState.GetActionOfType<SetFsmFloat>().setValue.Value = 90f;
            upState.GetActionOfType<SetRotation>().yAngle.Value = 0f;
            upState.AddFirstAction(new ExecuteLambda(() => VerticalCharge.instance.VerticalCharging = true));
            self.AddState(upState);

            FsmState directionCheck = self.GetState("Direction");
            directionCheck.AddFirstAction(new ExecuteLambda(() =>
            {
                if (Ref.Input.inputActions.up.IsPressed)
                {
                    self.SendEvent("BUTTON UP"); // This should be the "UP PRESSED" event, but IDK if we can use events not in the list
                }
            }));

            // Start dashing up
            FsmState upDashStart = new FsmState(self.GetState("Dash Start"))
            {
                Name = "Up Dash Start VC"
            };
            upDashStart.ClearTransitions();
            upDashStart.AddFirstAction(new ExecuteLambda(() => 
            {
                // Rotate sprite
                Ref.Hero.transform.Rotate(0, 0, -90 * Ref.Hero.transform.localScale.x);
            }));
            self.AddState(upDashStart);

            // Dashing Up
            FsmState upDashing = new FsmState(self.GetState("Dashing"))
            {
                Name = "Up Dashing VC"
            };
            SetVelocity2d setvel = upDashing.GetActionOfType<SetVelocity2d>();
            (setvel.x, setvel.y) = (setvel.y, setvel.x);
            GetVelocity2d getvel = upDashing.GetActionOfType<GetVelocity2d>();
            (getvel.x, getvel.y) = (getvel.y, getvel.x);
            upDashing.RemoveTransitionsTo("Cancelable");
            self.AddState(upDashing);

            // Cancelable dashing up
            FsmState upCancelable = new FsmState(self.GetState("Cancelable"))
            {
                Name = "Up Cancelable VC"
            };
            SetVelocity2d setvelC = upCancelable.GetActionOfType<SetVelocity2d>();
            (setvelC.x, setvelC.y) = (setvelC.y, setvelC.x);
            GetVelocity2d getvelC = upCancelable.GetActionOfType<GetVelocity2d>();
            (getvelC.x, getvelC.y) = (getvelC.y, getvelC.x);
            self.AddState(upCancelable);

            // Adding transitions
            directionCheck.AddTransition("BUTTON UP", upState.Name);
            upState.AddTransition("FINISHED", upDashStart.Name);
            upDashStart.AddTransition("FINISHED", upDashing.Name);
            upDashing.AddTransition("WAIT", upCancelable.Name);

            // Reset Vertical Charge variable
            self.GetState("Air Cancel").AddFirstAction(new ExecuteLambda(() => 
            {
                if (VerticalCharge.instance.VerticalCharging) Ref.Hero.transform.Rotate(0, 0, 90 * Ref.Hero.transform.localScale.x);
                VerticalCharge.instance.VerticalCharging = false;
            }));
            self.GetState("Hit Wall").AddFirstAction(new ExecuteLambda(() =>
            {
                if (VerticalCharge.instance.VerticalCharging) Ref.Hero.transform.Rotate(0, 0, 90 * Ref.Hero.transform.localScale.x);
                VerticalCharge.instance.VerticalCharging = false;
            }));

            // Logging
            foreach (FsmState state in self.FsmStates)
            {
                state.AddFirstAction(new ExecuteLambda(() =>
                {
                    VerticalCharge.instance.Log("VC STATELOG " + state.Name);
                }));
            }
        }
    }
}
