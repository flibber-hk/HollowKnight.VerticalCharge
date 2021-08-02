using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VerticalCharge
{
    public class VerticalCharge : Mod, ITogglableMod
    {
        internal static VerticalCharge instance;

        public override void Initialize()
        {
            instance = this;

            SkillStates.Initialize();

            instance.Log("Initializing");

            SuperdashFsmEdit.Hook();
            QuakeFsmEdit.Hook();
            On.CameraTarget.Update += FixVerticalCamera;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ResetCharge;
            FixVerticalTransitions.Hook();
            FixHorizontalTransitions.Hook();
        }

        public void Unload()
        {
            SuperdashFsmEdit.Disable();
            QuakeFsmEdit.Disable();
            On.CameraTarget.Update -= FixVerticalCamera;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ResetCharge;
            FixVerticalTransitions.UnHook();
            FixHorizontalTransitions.UnHook();
        }


        public override string GetVersion()
        {
            return "1.0(Quake)";
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

        private void ResetCharge(Scene arg0, Scene arg1)
        {
            SkillStates.ResetStates();
        }
    }
}
