using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;

namespace VerticalCharge
{
    public class VerticalCharge : Mod, ITogglableMod
    {
        internal static VerticalCharge instance;

        public override void Initialize()
        {
            instance = this;

            instance.Log("Initializing");

            Hook();
        }

        public override string GetVersion()
        {
            return "TEST";
        }

        private void Hook()
        {
            On.PlayMakerFSM.OnEnable += AllowVerticalSuperdash;
        }
        private void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= AllowVerticalSuperdash;
        }


        private void AllowVerticalSuperdash(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            if (self.FsmName == "Superdash" && self.gameObject.name == "Knight")
            {
                FsmState right = self.GetState("Right");
                right.GetActionOfType<SetFsmFloat>().setValue.Value = 90f;
                right.GetActionOfType<SetRotation>().yAngle.Value = 180f;

                FsmState dashing = self.GetState("Dashing");
                SetVelocity2d setvel = dashing.GetActionOfType<SetVelocity2d>();
                FsmFloat tmp = setvel.x;
                setvel.x = setvel.y;
                setvel.y = tmp;
                GetVelocity2d getvel = dashing.GetActionOfType<GetVelocity2d>();
                FsmFloat tmp2 = getvel.x;
                getvel.x = getvel.y;
                getvel.y = tmp2;

                FsmState cancelable = self.GetState("Cancelable");
                SetVelocity2d setvelC = cancelable.GetActionOfType<SetVelocity2d>();
                FsmFloat tmpC = setvelC.x;
                setvelC.x = setvelC.y;
                setvelC.y = tmpC;
                GetVelocity2d getvelC = cancelable.GetActionOfType<GetVelocity2d>();
                FsmFloat tmp2C = getvelC.x;
                getvelC.x = getvelC.y;
                getvelC.y = tmp2C;
            }
        }

        public void Unload()
        {
            UnHook();
        }
    }
}
