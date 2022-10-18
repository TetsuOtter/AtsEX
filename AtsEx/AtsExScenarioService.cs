﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using UnembeddedResources;

using Automatic9045.AtsEx.Handles;
using Automatic9045.AtsEx.Input;
using Automatic9045.AtsEx.Plugins;
using Automatic9045.AtsEx.Plugins.Scripting.CSharp;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx
{
    internal abstract partial class AtsExScenarioService : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsExScenarioService>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> VehiclePluginTickResultTypeInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MapPluginTickResultTypeInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> UnhandledExceptionCaption { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static AtsExScenarioService()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly BveHacker BveHacker;

        private readonly Dictionary<string, PluginBase> VehiclePlugins;
        private readonly Dictionary<string, PluginBase> MapPlugins;

        protected AtsExScenarioService(AtsEx atsEx, PluginUsing vehiclePluginUsing, VehicleSpec vehicleSpec)
        {
            BveHacker = atsEx.BveHacker;

            LoadErrorResolver loadErrorResolver = new LoadErrorResolver(BveHacker);

            Plugins.PluginLoader pluginLoader = new Plugins.PluginLoader(BveHacker);
            try
            {
                {
                    VehiclePlugins = pluginLoader.LoadFromPluginUsingAsync(vehiclePluginUsing).Result;
                }

                {
                    Map map = Map.LoadAsync(BveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path, pluginLoader, loadErrorResolver).Result;
                    MapPlugins = map.LoadedPlugins;

                    IEnumerable<LoadError> removeTargetErrors = BveHacker.LoadErrorManager.Errors.Where(error =>
                    {
                        if (error.Text.Contains("[[NOMPI]]")) return true;

                        bool isMapPluginUsingError = map.MapPluginUsingErrors.Contains(error, new LoadErrorEqualityComparer());
                        return isMapPluginUsingError;
                    });
                    foreach (LoadError error in removeTargetErrors)
                    {
                        BveHacker.LoadErrorManager.Errors.Remove(error);
                    }
                }
            }
            catch (Exception ex)
            {
                loadErrorResolver.Resolve(ex);
            }
            finally
            {
                if (VehiclePlugins is null) VehiclePlugins = new Dictionary<string, PluginBase>();
                if (MapPlugins is null) MapPlugins = new Dictionary<string, PluginBase>();

                App.Instance.VehiclePlugins = VehiclePlugins;
                App.Instance.MapPlugins = MapPlugins;
            }

            App.Instance.SetScenario(vehicleSpec);
            BveHacker.SetScenario();
        }

        public void Dispose()
        {
            foreach (PluginBase plugin in VehiclePlugins.Values)
            {
                if (plugin is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            foreach (PluginBase plugin in MapPlugins.Values)
            {
                if (plugin is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            BveHacker.Dispose();
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            App.Instance.InvokeStarted(defaultBrakePosition);
        }

        public HandlePositionSet Tick(TimeSpan elapsed, VehicleState vehicleState)
        {
            App.Instance.VehicleState = vehicleState;

            BveHacker.Tick(elapsed);

            int powerNotch = App.Instance.Handles.Power.Notch;
            int brakeNotch = App.Instance.Handles.Brake.Notch;
            ReverserPosition reverserPosition = App.Instance.Handles.Reverser.Position;

            int? atsPowerNotch = null;
            int? atsBrakeNotch = null;
            ReverserPosition? atsReverserPosition = null;
            ConstantSpeedCommand? atsConstantSpeedCommand = null;

            foreach (PluginBase plugin in VehiclePlugins.Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is VehiclePluginTickResult vehiclePluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.VehiclePluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(VehiclePluginTickResult)));
                }

                HandleCommandSet commandSet = vehiclePluginTickResult.HandleCommandSet;

                if (atsPowerNotch is null) atsPowerNotch = commandSet.PowerCommand.GetOverridenNotch(powerNotch);
                if (atsBrakeNotch is null) atsBrakeNotch = commandSet.BrakeCommand.GetOverridenNotch(brakeNotch);
                if (atsReverserPosition is null) atsReverserPosition = commandSet.ReverserCommand.GetOverridenPosition(reverserPosition);
                if (atsConstantSpeedCommand is null) atsConstantSpeedCommand = commandSet.ConstantSpeedCommand;
            }

            foreach (PluginBase plugin in MapPlugins.Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is MapPluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.MapPluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(MapPluginTickResult)));
                }
            }

            return new HandlePositionSet(atsPowerNotch ?? powerNotch, atsBrakeNotch ?? brakeNotch, atsReverserPosition ?? reverserPosition, atsConstantSpeedCommand ?? ConstantSpeedCommand.Continue);
        }

        public void SetPower(int notch)
        {
            (App.Instance.Handles.Power as PowerHandle).Notch = notch;
        }

        public void SetBrake(int notch)
        {
            (App.Instance.Handles.Brake as BrakeHandle).Notch = notch;
        }

        public void SetReverser(ReverserPosition position)
        {
            (App.Instance.Handles.Reverser as Reverser).Position = position;
        }

        public void KeyDown(NativeAtsKeyName key)
        {
            (App.Instance.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyPressed();
        }

        public void KeyUp(NativeAtsKeyName key)
        {
            (App.Instance.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyReleased();
        }


        private class LoadErrorResolver : ILoadErrorResolver
        {
            private readonly BveHacker BveHacker;

            public LoadErrorResolver(BveHacker bveHacker)
            {
                BveHacker = bveHacker;
            }

            public void Resolve(Exception exception)
            {
                if (exception is AggregateException ae)
                {
                    foreach (Exception ex in ae.InnerExceptions)
                    {
                        Resolve(ex);
                    }
                    return;
                }

                if (exception is CompilationException ce)
                {
                    ce.ThrowAsLoadError(BveHacker.LoadErrorManager);
                }
                else if (exception is BveFileLoadException fe)
                {
                    BveHacker.LoadErrorManager.Throw(fe.Message, fe.SenderFileName, fe.LineIndex, fe.CharIndex);
                }
                else
                {
                    BveHacker.LoadErrorManager.Throw(exception.Message);
                    MessageBox.Show(exception.ToString(), string.Format(Resources.Value.UnhandledExceptionCaption.Value, App.Instance.ProductShortName));
                }
            }
        }


        private class LoadErrorEqualityComparer : IEqualityComparer<LoadError>
        {
            public bool Equals(LoadError x, LoadError y) => x.SenderFileName == y.SenderFileName && x.LineIndex == y.LineIndex && x.CharIndex == y.CharIndex;
            public int GetHashCode(LoadError obj) => obj.GetHashCode();
        }
    }
}
