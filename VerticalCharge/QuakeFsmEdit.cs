using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using UnityEngine;
using VerticalCharge.Extensions;
using VerticalCharge.FsmStateActions;

namespace VerticalCharge
{
    internal static class QuakeFsmEdit
    {
        public static void Hook()
        {
            UnHook();
            On.PlayMakerFSM.OnEnable += AllowHorizontalQuake;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= AllowHorizontalQuake;
        }

        private static void AllowHorizontalQuake(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.FsmName != "Spell Control" || self.gameObject.name != "Knight")
            {
                return;
            }

            self.GetState("Quake Finish").AddFirstAction(new ExecuteLambda(() =>
            {
                SkillStates.LeftQuaking = false;
                SkillStates.RightQuaking = false;
            }));

            FsmState qOnGround = self.GetState("Q On Ground");
            FsmState qOffGround = self.GetState("Q Off Ground");
            FsmState directionCheck = new FsmState(self.GetState("Cancel"))
            {
                Name = "Direction Check HQ"
            };
            directionCheck.ClearTransitions();
            directionCheck.RemoveActionsOfType<FsmStateAction>();
            directionCheck.AddAction(new ExecuteLambda(() =>
            {
                if (InputHandler.Instance.inputActions.right.IsPressed) self.SendEvent("RIGHT");
                else if (InputHandler.Instance.inputActions.left.IsPressed) self.SendEvent("LEFT");
                else self.SendEvent("FINISHED");
            }));

            qOnGround.RemoveTransitionsTo("Quake Antic");
            qOffGround.RemoveTransitionsTo("Quake Antic");
            qOnGround.AddTransition("FINISHED", directionCheck.Name);
            qOffGround.AddTransition("FINISHED", directionCheck.Name);
            directionCheck.AddTransition("FINISHED", "Quake Antic");
            self.AddState(directionCheck);

            // Adding 12 states whee
            FsmState qaLeft = new FsmState(self.GetState("Quake Antic")) { Name = "Quake Antic HQL" };
            qaLeft.ClearTransitions();
            self.AddState(qaLeft);
            FsmState qaRight = new FsmState(self.GetState("Quake Antic")) { Name = "Quake Antic HQR" };
            qaRight.ClearTransitions();
            self.AddState(qaRight);
            FsmState lcLeft = new FsmState(self.GetState("Level Check 2")) { Name = "Level Check 2 HQL" };
            lcLeft.ClearTransitions();
            self.AddState(lcLeft);
            FsmState lcRight = new FsmState(self.GetState("Level Check 2")) { Name = "Level Check 2 HQR" };
            lcRight.ClearTransitions();
            self.AddState(lcRight);
            FsmState diveELeft = new FsmState(self.GetState("Q1 Effect")) { Name = "Q1 Effect HQL" };
            diveELeft.ClearTransitions();
            self.AddState(diveELeft);
            FsmState diveERight = new FsmState(self.GetState("Q1 Effect")) { Name = "Q1 Effect HQR" };
            diveERight.ClearTransitions();
            self.AddState(diveERight);
            FsmState darkELeft = new FsmState(self.GetState("Q2 Effect")) { Name = "Q2 Effect HQL" };
            darkELeft.ClearTransitions();
            self.AddState(darkELeft);
            FsmState darkERight = new FsmState(self.GetState("Q2 Effect")) { Name = "Q2 Effect HQR" };
            darkERight.ClearTransitions();
            self.AddState(darkERight);
            FsmState diveDLeft = new FsmState(self.GetState("Quake1 Down")) { Name = "Quake1 Down HQL" };
            self.AddState(diveDLeft);
            FsmState diveDRight = new FsmState(self.GetState("Quake1 Down")) { Name = "Quake1 Down HQR" };
            self.AddState(diveDRight);
            FsmState darkDLeft = new FsmState(self.GetState("Quake2 Down")) { Name = "Quake2 Down HQL" };
            self.AddState(darkDLeft);
            FsmState darkDRight = new FsmState(self.GetState("Quake2 Down")) { Name = "Quake2 Down HQR" };
            self.AddState(darkDRight);

            // Transitions
            directionCheck.AddTransition("LEFT", qaLeft.Name);
            directionCheck.AddTransition("RIGHT", qaRight.Name);
            qaLeft.AddTransition("ANIM END", lcLeft.Name);
            qaRight.AddTransition("ANIM END", lcRight.Name);
            lcLeft.AddTransition("LEVEL 1", diveELeft.Name);
            lcLeft.AddTransition("LEVEL 2", darkELeft.Name);
            lcRight.AddTransition("LEVEL 1", diveERight.Name);
            lcRight.AddTransition("LEVEL 2", darkERight.Name);
            diveELeft.AddTransition("FINISHED", diveDLeft.Name);
            diveERight.AddTransition("FINISHED", diveDRight.Name);
            darkELeft.AddTransition("FINISHED", darkDLeft.Name);
            darkERight.AddTransition("FINISHED", darkDRight.Name);


            // Don't need to leave the ground when horizontal quaking
            qaLeft.AddFirstAction(new ExecuteLambda(() => self.FsmVariables.FindFsmFloat("Quake Antic Speed").Value = 0f));
            qaRight.AddFirstAction(new ExecuteLambda(() => self.FsmVariables.FindFsmFloat("Quake Antic Speed").Value = 0f));

            // Set dive state
            qaLeft.AddFirstAction(new ExecuteLambda(() => SkillStates.LeftQuaking = true));
            qaRight.AddFirstAction(new ExecuteLambda(() => SkillStates.RightQuaking = true));
            

            // Set velocity
            diveDLeft.GetActionOfType<SetVelocity2d>().SwapXandY();
            diveDLeft.GetActionOfType<GetVelocity2d>().SwapXandY();
            diveDLeft.GetActionOfType<CheckCollisionSide>().SetBottomToLeft();
            
            darkDLeft.GetActionOfType<SetVelocity2d>().SwapXandY();
            darkDLeft.GetActionOfType<GetVelocity2d>().SwapXandY();
            darkDLeft.GetActionOfType<CheckCollisionSide>().SetBottomToLeft();


            diveDRight.GetActionOfType<SetVelocity2d>().SwapXandY();
            diveDRight.GetActionOfType<SetVelocity2d>().x.Value *= -1;
            diveDRight.GetActionOfType<GetVelocity2d>().SwapXandY();
            diveDRight.GetActionOfType<CheckCollisionSide>().SetBottomToRight();
            
            darkDRight.GetActionOfType<SetVelocity2d>().SwapXandY();
            darkDRight.GetActionOfType<SetVelocity2d>().x.Value *= -1;
            darkDRight.GetActionOfType<GetVelocity2d>().SwapXandY();
            darkDRight.GetActionOfType<CheckCollisionSide>().SetBottomToRight();


            // Fix hero on "swag dive"
            self.GetState("Reset Cam Zoom").AddFirstAction(new ExecuteLambda(() =>
            {
                SkillStates.LeftQuaking = false;
                SkillStates.RightQuaking = false;
            }));



            // Logging
            foreach (FsmState state in self.FsmStates)
            {
                state.AddFirstAction(new ExecuteLambda(() =>
                {
                    VerticalCharge.instance.Log("HQ STATELOG " + state.Name);
                }));
            }
        }


    }
}
