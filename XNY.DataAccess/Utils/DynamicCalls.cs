using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace XNY.DataAccess.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate object FastInvokeHandler(object target, object[] parameters);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate object FastCreateInstanceHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public delegate object FastPropertyGetHandler(object target);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="parameter"></param>
    public delegate void FastPropertySetHandler(object target, object parameter);

    /// <summary>
    /// 
    /// </summary>
    public static class DynamicCalls
    {
        /// <summary>
        /// 用于存放GetMethodInvoker的Dictionary
        /// </summary>
        private static Dictionary<MethodInfo, FastInvokeHandler> dictInvoker = new Dictionary<MethodInfo, FastInvokeHandler>();



        /// <summary>
        /// 用于存放GetInstanceCreator的Dictionary
        /// </summary>
        private static Dictionary<Type, FastCreateInstanceHandler> dictCreator = new Dictionary<Type, FastCreateInstanceHandler>();



        /// <summary>
        /// 用于存放GetPropertyGetter的Dictionary
        /// </summary>
        private static Dictionary<PropertyInfo, FastPropertyGetHandler> dictGetter = new Dictionary<PropertyInfo, FastPropertyGetHandler>();

        public static FastPropertyGetHandler GetPropertyGetter(PropertyInfo propInfo)
        {
            lock (dictGetter)
            {
                if (dictGetter.ContainsKey(propInfo)) return (FastPropertyGetHandler)dictGetter[propInfo];

                DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, propInfo.DeclaringType.Module);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);

                ilGenerator.EmitCall(OpCodes.Callvirt, propInfo.GetGetMethod(), null);

                EmitBoxIfNeeded(ilGenerator, propInfo.PropertyType);

                ilGenerator.Emit(OpCodes.Ret);

                FastPropertyGetHandler getter = (FastPropertyGetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertyGetHandler));

                dictGetter.Add(propInfo, getter);

                return getter;
            }
        }

        /// <summary>
        /// 用于存放SetPropertySetter的Dictionary
        /// </summary>
        private static Dictionary<PropertyInfo, FastPropertySetHandler> dictSetter = new Dictionary<PropertyInfo, FastPropertySetHandler>();

        public static FastPropertySetHandler GetPropertySetter(PropertyInfo propInfo)
        {
            lock (dictSetter)
            {
                if (dictSetter.ContainsKey(propInfo)) return (FastPropertySetHandler)dictSetter[propInfo];

                DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, null, new Type[] { typeof(object), typeof(object) }, propInfo.DeclaringType.Module);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);

                ilGenerator.Emit(OpCodes.Ldarg_1);

                EmitCastToReference(ilGenerator, propInfo.PropertyType);

                ilGenerator.EmitCall(OpCodes.Callvirt, propInfo.GetSetMethod(), null);

                ilGenerator.Emit(OpCodes.Ret);

                FastPropertySetHandler setter = (FastPropertySetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertySetHandler));

                dictSetter.Add(propInfo, setter);

                return setter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitCastToReference(ILGenerator ilGenerator, System.Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitBoxIfNeeded(ILGenerator ilGenerator, System.Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="value"></param>
        private static void EmitFastInt(ILGenerator ilGenerator, int value)
        {
            switch (value)
            {
                case -1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            if (value > -129 && value < 128)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }
}
