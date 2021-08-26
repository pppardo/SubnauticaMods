﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using System.Runtime.CompilerServices;
using System.Collections;

namespace RollControl
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(Player __instance)
        {
            var srm = __instance.gameObject.EnsureComponent<ScubaRollController>();
            srm.player = __instance;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateRotation")]
        public static bool Prefix(Player __instance)
        {
            if (__instance.motorMode == Player.MotorMode.Dive || __instance.motorMode == Player.MotorMode.Seaglide)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMotorMode")]
        public static bool SetMotorMode(Player __instance, Player.MotorMode newMotorMode)
        {
            
            if ( // we're transitioning into swimming
                (newMotorMode == Player.MotorMode.Seaglide && __instance.motorMode != Player.MotorMode.Seaglide) ||
                (newMotorMode == Player.MotorMode.Dive     && __instance.motorMode != Player.MotorMode.Dive)
                )
            {
                __instance.gameObject.GetComponent<ScubaRollController>().OnSwimmingStarted();
            }
            else if ( // we're transitioning out of swimming
                (newMotorMode != Player.MotorMode.Seaglide && __instance.motorMode == Player.MotorMode.Seaglide) ||
                (newMotorMode != Player.MotorMode.Dive && __instance.motorMode == Player.MotorMode.Dive)
                )
            {
                __instance.gameObject.GetComponent<ScubaRollController>().GetReadyToStopRolling();
            }
            return true;
        }
    }
}

