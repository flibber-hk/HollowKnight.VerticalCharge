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

        private bool _verticalCharging;

        public bool VerticalCharging
        {
            get => _verticalCharging;

            set
            {
                if (value && !_verticalCharging)
                {
                    HeroController.instance.transform.Rotate(0, 0, -90 * HeroController.instance.transform.localScale.x);
                }
                else if (!value && _verticalCharging)
                {
                    // We need to set the SD Burst inactive before un-rotating the hero,
                    // so it doesn't rotate with it
                    if (GameObject.Find("SD Burst") is GameObject burst)
                    {
                        burst.transform.parent = HeroController.instance.gameObject.transform;
                        burst.SetActive(false);
                    }
                    HeroController.instance.transform.Rotate(0, 0, 90 * HeroController.instance.transform.localScale.x);
                }
                _verticalCharging = value;
            }
        }

        internal static VerticalCharge instance;

        public override void Initialize()
        {
            instance = this;

            _verticalCharging = false;

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
                    if (VerticalCharging)     // if vertical cdash
                    {
                        self.cameraCtrl.lookOffset += Math.Abs(self.dashOffset);
                        self.dashOffset = 0;
                    }
                }
            }
        }

        private void ResetVerticalCharge(Scene arg0, Scene arg1)
        {
            VerticalCharging = false;
        }
    }
}
