﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class ScenarioProvider : ClassWrapper
    {
        static ScenarioProvider()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<ScenarioProvider>();

            TimeManagerGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeManager));
            LocationManagerGetMethod = members.GetSourcePropertyGetterOf(nameof(LocationManager));
            RouteGetMethod = members.GetSourcePropertyGetterOf(nameof(Route));
            VehicleGetMethod = members.GetSourcePropertyGetterOf(nameof(Vehicle));
            TimeTableGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeTable));
        }

        private ScenarioProvider(object src) : base(src)
        {
        }

        public static ScenarioProvider FromSource(object src)
        {
            if (src is null) return null;
            return new ScenarioProvider(src);
        }

        private static MethodInfo TimeManagerGetMethod;
        public TimeManager TimeManager
        {
            get => ClassWrappers.TimeManager.FromSource(TimeManagerGetMethod.Invoke(Src, null));
        }

        private static MethodInfo LocationManagerGetMethod;
        public UserVehicleLocationManager LocationManager
        {
            get => ClassWrappers.UserVehicleLocationManager.FromSource(LocationManagerGetMethod.Invoke(Src, null));
        }

        private static MethodInfo RouteGetMethod;
        public Route Route
        {
            get => ClassWrappers.Route.FromSource(RouteGetMethod.Invoke(Src, null));
        }

        private static MethodInfo VehicleGetMethod;
        public Vehicle Vehicle
        {
            get => ClassWrappers.Vehicle.FromSource(VehicleGetMethod.Invoke(Src, null));
        }

        private static MethodInfo TimeTableGetMethod;
        public TimeTable TimeTable
        {
            get => ClassWrappers.TimeTable.FromSource(TimeTableGetMethod.Invoke(Src, null));
        }
    }
}
