using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SereCore;

namespace VerticalCharge.Extensions
{
    public static class FsmExtensions
    {
        public static void AddPenultimateAction(this FsmState self, FsmStateAction action)
        {
            FsmStateAction[] actions = new FsmStateAction[self.Actions.Length + 1];
            Array.Copy(self.Actions, actions, self.Actions.Length - 1);
            actions[self.Actions.Length - 1] = action;
            actions[self.Actions.Length] = self.Actions[self.Actions.Length - 1];
            
            self.Actions = actions;
        }
    }
}
