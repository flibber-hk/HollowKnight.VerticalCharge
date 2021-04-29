using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using SereCore;

namespace VerticalCharge
{
    public class VerticalCharge : Mod, ITogglableMod
    {

        public bool VerticalCharging;

        internal static VerticalCharge instance;

        public override void Initialize()
        {
            instance = this;

            VerticalCharging = false;

            instance.Log("Initializing");

            SuperdashFsmEdit.Hook();
            On.CameraTarget.Update += FixVerticalCamera;
        }
        public void Unload()
        {
            SuperdashFsmEdit.UnHook();
            On.CameraTarget.Update -= FixVerticalCamera;
        }


        public override string GetVersion()
        {
            return "TEST";
        }

        private void FixVerticalCamera(On.CameraTarget.orig_Update orig, CameraTarget self)
        {
            orig(self);
            
            if (self.hero_ctrl != null && Ref.GM.IsGameplayScene())
            {
                if (self.superDashing)
                {
                    if (VerticalCharging)     // if vertical cdash
                    {
                        self.cameraCtrl.lookOffset += Math.Abs(self.dashOffset);
                        self.dashOffset = 0;
                    }
                }
            }
        }
    }
}
