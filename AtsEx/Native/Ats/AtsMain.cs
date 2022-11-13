﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AtsEx.Hosting;

using AtsEx.Handles;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Input.Native;

namespace AtsEx.Native
{
    /// <summary>メインの機能をここに実装する。</summary>
    internal static class AtsMain
    {
        public static VehicleSpec VehicleSpec { get; set; }

        /// <summary>Is the Door Closed TF</summary>
        public static bool IsDoorClosed { get; set; } = false;

        private static Assembly CallerAssembly;
        private static AtsExActivator Activator;

        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static AtsEx.AsAtsPlugin AtsEx;
        private static AtsExScenarioService.AsAtsPlugin AtsExScenarioService;

        public static void Load(Assembly callerAssembly, AtsExActivator activator)
        {
            CallerAssembly = callerAssembly;
            Activator = activator;

            Version callerVersion = callerAssembly.GetName().Version;
            if (callerVersion < new Version(0, 16))
            {
                string errorMessage = $"読み込まれた AtsEX Caller (バージョン {callerVersion}) は現在の AtsEX ではサポートされていません。\nbeta0.16 (バージョン 0.16) 以降の Ats Caller をご利用下さい。";
                MessageBox.Show(errorMessage, "AtsEX Caller バージョンエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new NotSupportedException(errorMessage.Replace("\n", ""));
            }

            AtsEx = new AtsEx.AsAtsPlugin(Activator.TargetProcess, Activator.TargetAppDomain, Activator.TargetAssembly);
        }

        public static void Dispose()
        {
            AtsExScenarioService?.Dispose();
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            PluginHost.Native.VehicleSpec exVehicleSpec = new PluginHost.Native.VehicleSpec(
                vehicleSpec.BrakeNotches, vehicleSpec.PowerNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, vehicleSpec.Cars);

            AtsExScenarioService = new AtsExScenarioService.AsAtsPlugin(AtsEx, CallerAssembly, exVehicleSpec);
        }

        public static void Initialize(int defaultBrakePosition)
        {
            AtsExScenarioService?.Started((BrakePosition)defaultBrakePosition);
        }

        public static AtsHandles Elapse(VehicleState vehicleState, int[] panel, int[] sound)
        {
            PluginHost.Native.VehicleState exVehicleState = new PluginHost.Native.VehicleState(
                vehicleState.Location, vehicleState.Speed, TimeSpan.FromMilliseconds(vehicleState.Time),
                vehicleState.BcPressure, vehicleState.MrPressure, vehicleState.ErPressure, vehicleState.BpPressure, vehicleState.SapPressure, vehicleState.Current);

            HandlePositionSet handlePositionSet = AtsExScenarioService?.Tick(Stopwatch.IsRunning ? Stopwatch.Elapsed : TimeSpan.Zero, exVehicleState);

            Stopwatch.Restart();

            return new AtsHandles()
            {
                Brake = handlePositionSet.Brake,
                Power = handlePositionSet.Power,
                Reverser = (int)handlePositionSet.ReverserPosition,
                ConstantSpeed = (int)handlePositionSet.ConstantSpeed,
            };
        }

        public static void SetPower(int notch)
        {
            AtsExScenarioService?.SetPower(notch);
        }

        public static void SetBrake(int notch)
        {
            AtsExScenarioService?.SetBrake(notch);
        }

        public static void SetReverser(int position)
        {
            AtsExScenarioService?.SetReverser((ReverserPosition)position);
        }

        public static void KeyDown(int atsKeyCode)
        {
            AtsExScenarioService?.KeyDown((NativeAtsKeyName)atsKeyCode);
        }

        public static void KeyUp(int atsKeyCode)
        {
            AtsExScenarioService?.KeyUp((NativeAtsKeyName)atsKeyCode);
        }

        public static void DoorOpen()
        {

        }
        public static void DoorClose()
        {

        }
        public static void HornBlow(HornType hornType)
        {

        }
        public static void SetSignal(int signal)
        {

        }
        public static void SetBeaconData(BeaconData beaconData)
        {

        }
    }
}
