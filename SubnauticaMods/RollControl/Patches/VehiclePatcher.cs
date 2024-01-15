﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace RollControl
{
    [HarmonyPatch(typeof(Vehicle))]
    public class VehiclePatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix(Vehicle __instance)
        {
            var src = __instance.gameObject.EnsureComponent<VehicleRollController>();
            src.myVehicle = __instance;
            src.myVehicle.stabilizeRoll = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("EnterVehicle")]
        public static bool EnterVehiclePrefix(Vehicle __instance)
        {
            // ensure we enter vehicles correctly
            Player.main.GetComponent<ScubaRollController>().GetReadyToStopRolling();
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        public static void FixedUpdatePostfix(Vehicle __instance)
        {
            // ensure we're rotated correctly
            if (Player.main.GetMode() == Player.Mode.LockedPiloting)
            {
                Player.main.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
