﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    /// <summary>
    /// クラスラッパーに対応する BVE の型とメンバーの情報を提供します。
    /// </summary>
    public partial class BveTypeSet
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<BveTypeSet>("PluginHost");

        protected SortedList<Type, TypeMemberSetBase> Types { get; }
        protected SortedList<Type, Type> OriginalAndWrapperTypes { get; }

        protected BveTypeSet(IEnumerable<TypeMemberSetBase> types, Type classWrapperType)
        {
            TypeMemberSetBase illegalType = types.FirstOrDefault(type => !(type.WrapperType.IsClass && type.WrapperType.IsSubclassOf(classWrapperType)) && !type.WrapperType.IsEnum);
            if (!(illegalType is null))
            {
                throw new ArgumentException(
                    string.Format(Resources.GetString("TypeNotClassWrapper").Value,
                    illegalType.WrapperType.FullName, classWrapperType.FullName));
            }

            Types = new SortedList<Type, TypeMemberSetBase>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());
        }


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパー型の情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパー型。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパー型の情報を表す <see cref="TypeMemberSetBase"/>。</returns>
        /// <seealso cref="GetClassInfoOf{TWrapper}"/>
        /// <seealso cref="GetClassInfoOf(Type)"/>
        /// <seealso cref="GetEnumInfoOf{TWrapper}"/>
        /// <seealso cref="GetEnumInfoOf(Type)"/>
        public TypeMemberSetBase GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパー型の情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパー型。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパー型の情報を表す <see cref="TypeMemberSetBase"/>。</returns>
        /// <seealso cref="GetClassInfoOf{TWrapper}"/>
        /// <seealso cref="GetClassInfoOf(Type)"/>
        /// <seealso cref="GetEnumInfoOf{TWrapper}"/>
        /// <seealso cref="GetEnumInfoOf(Type)"/>
        public TypeMemberSetBase GetTypeInfoOf(Type wrapperType) => Types[wrapperType];


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパー列挙型の情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパー列挙型。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパー列挙型の情報を表す <see cref="EnumMemberSet"/>。</returns>
        public EnumMemberSet GetEnumInfoOf<TWrapper>() => (EnumMemberSet)Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパー列挙型の情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパー列挙型。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパー列挙型の情報を表す <see cref="EnumMemberSet"/>。</returns>
        public EnumMemberSet GetEnumInfoOf(Type wrapperType) => (EnumMemberSet)Types[wrapperType];


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパークラスの情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパークラス。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパークラスの情報を表す <see cref="ClassMemberSet"/>。</returns>
        public ClassMemberSet GetClassInfoOf<TWrapper>() => (ClassMemberSet)Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパークラスの情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパークラス。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパークラスの情報を表す <see cref="ClassMemberSet"/>。</returns>
        public ClassMemberSet GetClassInfoOf(Type wrapperType) => (ClassMemberSet)Types[wrapperType];


        /// <summary>
        /// <paramref name="originalType"/> に指定したオリジナル型のラッパー型を取得します。
        /// </summary>
        /// <param name="originalType">オリジナル型。</param>
        /// <returns></returns>
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
