using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VerticalCharge
{
    public static class SkillStates
    {
        private static bool _verticalCharging;
        private static bool _leftQuaking;
        private static bool _rightQuaking;

        public static void Initialize()
        {
            _verticalCharging = false;
            _leftQuaking = false;
            _rightQuaking = false;
        }
        public static void ResetStates()
        {
            VerticalCharging = false;
            LeftQuaking = false;
            RightQuaking = false;
        }

        public static bool VerticalCharging
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

        public static bool LeftQuaking
        {
            get => _leftQuaking;

            set
            {
                if (value && !_leftQuaking)
                {
                    HeroController.instance.transform.Rotate(0, 0, -90);
                }
                else if (!value && _leftQuaking)
                {
                    HeroController.instance.transform.Rotate(0, 0, 90);
                }

                _leftQuaking = value;
            }
        }

        public static bool RightQuaking
        {
            get => _rightQuaking;

            set
            {
                if (value && !_rightQuaking)
                {
                    HeroController.instance.transform.Rotate(0, 0, 90);
                }
                else if (!value && _rightQuaking)
                {
                    HeroController.instance.transform.Rotate(0, 0, -90);
                }

                _rightQuaking = value;
            }
        }

    }
}
