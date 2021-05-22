using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VerticalCharge
{
    public class VerticalCharge : Mod
    {
        internal static VerticalCharge instance;

        public override void Initialize()
        {
            instance = this;

            SkillStates.Initialize();

            instance.Log("Initializing");

            SuperdashFsmEdit.Hook();
            On.CameraTarget.Update += FixVerticalCamera;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ResetVerticalCharge;
            FixVerticalTransitions.Hook();
        }

        /* There's no easy way to reset the superdash FSM on unload, so it's not Toggleable
        public void Unload()
        {
            SuperdashFsmEdit.UnHook();
            On.CameraTarget.Update -= FixVerticalCamera;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ResetVerticalCharge;
            FixVerticalTransitions.UnHook();
        }
        */


        public override string GetVersion()
        {
            return "1.0";
        }

        private void FixVerticalCamera(On.CameraTarget.orig_Update orig, CameraTarget self)
        {
            orig(self);
            
            if (self.hero_ctrl != null && GameManager.instance.IsGameplayScene())
            {
                if (self.superDashing)
                {
                    if (SkillStates.VerticalCharging)     // if vertical cdash
                    {
                        self.cameraCtrl.lookOffset += Math.Abs(self.dashOffset);
                        self.dashOffset = 0;
                    }
                }
            }
        }

        private void ResetVerticalCharge(Scene arg0, Scene arg1)
        {
            SkillStates.VerticalCharging = false;
        }
    }
}
