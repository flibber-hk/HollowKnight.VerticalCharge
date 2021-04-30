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

            FsmState upDirectionCheck = new FsmState(self.GetState("Direction"))
            {
                Name = "Up Direction Check VC"
            };
            upDirectionCheck.ClearTransitions();
            self.AddState(upDirectionCheck);

            FsmState upStateR = new FsmState(self.GetState("Right"))
            {
                Name = "Up Right VC"
            };
            upStateR.ClearTransitions();
            upStateR.AddAction(new ExecuteLambda(() => VerticalCharge.instance.VerticalCharging = true));
            self.AddState(upStateR);

            FsmState upStateL = new FsmState(self.GetState("Right"))
            {
                Name = "Up Left VC"
            };
            upStateL.ClearTransitions();
            upStateL.AddAction(new ExecuteLambda(() => VerticalCharge.instance.VerticalCharging = true));
            self.AddState(upStateL);

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
            directionCheck.AddTransition("BUTTON UP", upDirectionCheck.Name);
            upDirectionCheck.AddTransition("LEFT", upStateL.Name);
            upDirectionCheck.AddTransition("RIGHT", upStateR.Name);
            upStateR.AddTransition("FINISHED", upDashStart.Name);
            upStateL.AddTransition("FINISHED", upDashStart.Name);
            upDashStart.AddTransition("FINISHED", upDashing.Name);
            upDashing.AddTransition("WAIT", upCancelable.Name);

            // Reset Vertical Charge variable
            self.GetState("Air Cancel").AddFirstAction(new ExecuteLambda(() => 
            {
                VerticalCharge.instance.VerticalCharging = false;
            }));
            self.GetState("Cancel").AddFirstAction(new ExecuteLambda(() =>
            {
                VerticalCharge.instance.VerticalCharging = false;
            }));
            self.GetState("Hit Wall").AddFirstAction(new ExecuteLambda(() =>
            {
                VerticalCharge.instance.VerticalCharging = false;
            }));

            // Logging
            //foreach (FsmState state in self.FsmStates)
            //{
            //    state.AddFirstAction(new ExecuteLambda(() =>
            //    {
            //        VerticalCharge.instance.Log("VC STATELOG " + state.Name);
            //    }));
            //}
        }
    }
}
