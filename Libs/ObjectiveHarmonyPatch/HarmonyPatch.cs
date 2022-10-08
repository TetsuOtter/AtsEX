﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HarmonyLib;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// オブジェクト (インスタンス) として扱える Harmony パッチのラッパーを提供します。
    /// </summary>
    public sealed partial class HarmonyPatch : IDisposable
    {
        private static readonly Harmony Harmony = new Harmony("com.objective-harmony-patch");
        private static readonly SortedList<MethodBase, List<HarmonyPatch>> Patches = new SortedList<MethodBase, List<HarmonyPatch>>(new MethodComaparer());

        private readonly MethodBase Original;
        private readonly HarmonyMethod PrefixHarmonyMethod = null;
        private readonly HarmonyMethod PostfixHarmonyMethod = null;

        private PatchInvokedEventHandler _Prefix;
        /// <summary>
        /// Harmony パッチの Prefix メソッドが実行されたときに発生します。
        /// </summary>
        public event PatchInvokedEventHandler Prefix
        {
            add
            {
                if (PrefixHarmonyMethod is null) throw new InvalidOperationException();

                PatchInvokedEventHandler x2;
                PatchInvokedEventHandler x1 = _Prefix;
                do
                {
                    x2 = x1;
                    PatchInvokedEventHandler x3 = (PatchInvokedEventHandler)Delegate.Combine(x2, value);
                    x1 = Interlocked.CompareExchange(ref _Prefix, x3, x2);
                }
                while (x1 != x2);
            }
            remove
            {
                PatchInvokedEventHandler x2;
                PatchInvokedEventHandler x1 = _Prefix;
                do
                {
                    x2 = x1;
                    PatchInvokedEventHandler x3 = (PatchInvokedEventHandler)Delegate.Remove(x2, value);
                    x1 = Interlocked.CompareExchange(ref _Prefix, x3, x2);
                }
                while (x1 != x2);
            }
        }

        private PatchInvokedEventHandler _Postfix;
        /// <summary>
        /// Harmony パッチの Postfix メソッドが実行されたときに発生します。
        /// </summary>
        public event PatchInvokedEventHandler Postfix
        {
            add
            {
                if (PostfixHarmonyMethod is null) throw new InvalidOperationException();

                PatchInvokedEventHandler x2;
                PatchInvokedEventHandler x1 = _Postfix;
                do
                {
                    x2 = x1;
                    PatchInvokedEventHandler x3 = (PatchInvokedEventHandler)Delegate.Combine(x2, value);
                    x1 = Interlocked.CompareExchange(ref _Postfix, x3, x2);
                }
                while (x1 != x2);
            }
            remove
            {
                PatchInvokedEventHandler x2;
                PatchInvokedEventHandler x1 = _Postfix;
                do
                {
                    x2 = x1;
                    PatchInvokedEventHandler x3 = (PatchInvokedEventHandler)Delegate.Remove(x2, value);
                    x1 = Interlocked.CompareExchange(ref _Postfix, x3, x2);
                }
                while (x1 != x2);
            }
        }

        private HarmonyPatch(MethodBase original, PatchTypes patchTypes)
        {
            Original = original;

            bool isStatic = original.IsStatic;
            bool hasReturnValue = Original is MethodInfo method && method.ReturnType != typeof(void);

            Type[] patchMethodArgs = GetValidPatchMethodArgumentList(isStatic, hasReturnValue);

            if (patchTypes.HasFlag(PatchTypes.Prefix)) PrefixHarmonyMethod = new HarmonyMethod(typeof(HarmonyPatch), nameof(PrefixMethod), patchMethodArgs);
            if (patchTypes.HasFlag(PatchTypes.Postfix)) PostfixHarmonyMethod = new HarmonyMethod(typeof(HarmonyPatch), nameof(PostfixMethod), patchMethodArgs);

            _ = Harmony.Patch(Original, PrefixHarmonyMethod, PostfixHarmonyMethod);

            if (!Patches.ContainsKey(Original)) Patches[Original] = new List<HarmonyPatch>();
            Patches[original].Add(this);
        }

        private static Type[] GetValidPatchMethodArgumentList(bool isStatic, bool hasReturnValue)
        {
            List<Type> args = new List<Type>();

            if (!isStatic) args.Add(typeof(object));
            if (hasReturnValue) args.Add(typeof(object).MakeByRefType());
            args.AddRange(new Type[]
            {
                typeof(object[]),
                typeof(MethodBase),
                typeof(bool),
            });

            return args.ToArray();
        }

        /// <summary>
        /// 指定したメソッドに Harmony パッチを適用します。
        /// </summary>
        /// <param name="original">パッチを適用するメソッド。</param>
        /// <param name="patchTypes">使用するパッチの種類。ここで指定されていないパッチを参照しようとした場合、例外が発生します。</param>
        /// <returns>パッチを表す <see cref="HarmonyPatch"/>。</returns>
        public static async Task<HarmonyPatch> PatchAsync(MethodBase original, PatchTypes patchTypes)
            => await Task.Run(() => Patch(original, patchTypes)).ConfigureAwait(false);

        /// <summary>
        /// 指定したメソッドに Harmony パッチを適用します。
        /// </summary>
        /// <param name="original">パッチを適用するメソッド。</param>
        /// <param name="patchTypes">使用するパッチの種類。ここで指定されていないパッチを参照しようとした場合、例外が発生します。</param>
        /// <returns>パッチを表す <see cref="HarmonyPatch"/>。</returns>
        public static HarmonyPatch Patch(MethodBase original, PatchTypes patchTypes) => new HarmonyPatch(original, patchTypes);

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!(PrefixHarmonyMethod is null)) Harmony.Unpatch(Original, PrefixHarmonyMethod.method);
            if (!(PostfixHarmonyMethod is null)) Harmony.Unpatch(Original, PostfixHarmonyMethod.method);
        }

#pragma warning disable IDE1006 // 命名スタイル
        private static bool PrefixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patch => patch._Prefix);

        private static bool PrefixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patch => patch._Prefix);
        }

        private static bool PrefixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patch => patch._Prefix);

        private static bool PrefixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patch => patch._Prefix);
        }

        private static void PostfixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patch => patch._Postfix);

        private static void PostfixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patch => patch._Postfix);
        }

        private static void PostfixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patch => patch._Postfix);

        private static void PostfixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patch => patch._Postfix);
        }

        private static bool InvokePatches(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal,
            Func<HarmonyPatch, PatchInvokedEventHandler> eventSelector)
        {
            bool cancel = false;

            PatchInvokedEventArgs e = new PatchInvokedEventArgs(__instance, __result, __args, __runOriginal);
            foreach (HarmonyPatch patch in Patches[__originalMethod])
            {
                PatchInvokationResult result = eventSelector(patch)?.Invoke(patch, e);
                if (result is null) continue;

                if (result.ChangeReturnValue) __result = result.ReturnValue;
                cancel = result.Cancel;
            }

            return !cancel;
        }
#pragma warning restore IDE1006 // 命名スタイル
    }
}
