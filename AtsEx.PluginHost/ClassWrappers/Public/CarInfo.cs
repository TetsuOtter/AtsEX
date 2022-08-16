﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 自列車の車両に関する物理演算機能を提供します。
    /// </summary>
    public class CarInfo : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<CarInfo>();

            WeightGetMethod = members.GetSourcePropertyGetterOf(nameof(Weight));
            WeightSetMethod = members.GetSourcePropertySetterOf(nameof(Weight));

            CountGetMethod = members.GetSourcePropertyGetterOf(nameof(Count));
            CountSetMethod = members.GetSourcePropertySetterOf(nameof(Count));

            InertiaFactorGetMethod = members.GetSourcePropertyGetterOf(nameof(InertiaFactor));
            InertiaFactorSetMethod = members.GetSourcePropertySetterOf(nameof(InertiaFactor));

            ShoeFrictionAGetMethod = members.GetSourcePropertyGetterOf(nameof(ShoeFrictionA));
            ShoeFrictionASetMethod = members.GetSourcePropertySetterOf(nameof(ShoeFrictionA));

            ShoeFrictionBGetMethod = members.GetSourcePropertyGetterOf(nameof(ShoeFrictionB));
            ShoeFrictionBSetMethod = members.GetSourcePropertySetterOf(nameof(ShoeFrictionB));

            ShoeFrictionCGetMethod = members.GetSourcePropertyGetterOf(nameof(ShoeFrictionC));
            ShoeFrictionCSetMethod = members.GetSourcePropertySetterOf(nameof(ShoeFrictionC));
        }

        protected CarInfo(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="CarInfo"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static CarInfo FromSource(object src) => src is null ? null : new CarInfo(src);

        private static MethodInfo WeightGetMethod;
        private static MethodInfo WeightSetMethod;
        /// <summary>
        /// 1 両当たりの重量 [kg] を取得・設定します。
        /// </summary>
        public double Weight
        {
            get => WeightGetMethod.Invoke(Src, null);
            set => WeightSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo CountGetMethod;
        private static MethodInfo CountSetMethod;
        /// <summary>
        /// 両数を取得・設定します。
        /// </summary>
        public double Count
        {
            get => CountGetMethod.Invoke(Src, null);
            set => CountSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo InertiaFactorGetMethod;
        private static MethodInfo InertiaFactorSetMethod;
        /// <summary>
        /// 慣性係数を取得・設定します。
        /// </summary>
        public double InertiaFactor
        {
            get => InertiaFactorGetMethod.Invoke(Src, null);
            set => InertiaFactorSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo ShoeFrictionAGetMethod;
        private static MethodInfo ShoeFrictionASetMethod;
        /// <summary>
        /// 制輪子摩擦係数式の係数 a を取得・設定します。
        /// </summary>
        public double ShoeFrictionA
        {
            get => ShoeFrictionAGetMethod.Invoke(Src, null);
            set => ShoeFrictionASetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo ShoeFrictionBGetMethod;
        private static MethodInfo ShoeFrictionBSetMethod;
        /// <summary>
        /// 制輪子摩擦係数式の係数 b を取得・設定します。
        /// </summary>
        public double ShoeFrictionB
        {
            get => ShoeFrictionBGetMethod.Invoke(Src, null);
            set => ShoeFrictionBSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo ShoeFrictionCGetMethod;
        private static MethodInfo ShoeFrictionCSetMethod;
        /// <summary>
        /// 制輪子摩擦係数式の係数 c を取得・設定します。
        /// </summary>
        public double ShoeFrictionC
        {
            get => ShoeFrictionCGetMethod.Invoke(Src, null);
            set => ShoeFrictionCSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
