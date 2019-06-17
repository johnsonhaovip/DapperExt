﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace XNY.DataAccess.Utils
{
    /// <summary>
    /// DataReader Extensions
    /// </summary>
    public static class DataReaderExtensions
    {

        #region Static Readonly Fields
        private static readonly MethodInfo DataRecord_ItemGetter_String =
            typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(string) });
        private static readonly MethodInfo DataRecord_ItemGetter_Int =
            typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo DataRecord_GetOrdinal =
            typeof(IDataRecord).GetMethod("GetOrdinal");
        private static readonly MethodInfo DataReader_Read =
            typeof(IDataReader).GetMethod("Read");
        private static readonly MethodInfo Convert_IsDBNull =
            typeof(Convert).GetMethod("IsDBNull");
        private static readonly MethodInfo DataRecord_GetDateTime =
            typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecord_GetDecimal =
            typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecord_GetDouble =
            typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecord_GetInt32 =
            typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecord_GetInt64 =
            typeof(IDataRecord).GetMethod("GetInt64");
        private static readonly MethodInfo DataRecord_GetString =
            typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecord_IsDBNull =
            typeof(IDataRecord).GetMethod("IsDBNull");
        #endregion

        #region Public Static Methods

        /// <summary>
        /// 把结果集流转换成数据实体列表
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="reader">结果集流</param>
        /// <returns>数据实体列表</returns>
        public static IEnumerable<T> DataReaderToList<T>(this IDataReader reader)
            where T : class, new()
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            return EntityConverter<T>.DataReaderToList(reader);
        }

        /// <summary>
        /// 把结果集流转换成数据实体序列（延迟）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="reader">结果集流</param>
        /// <returns>数据实体序列（延迟）</returns>
        public static IEnumerable<T> DataReaderToListLazy<T>(this IDataReader reader)
            where T : class, new()
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            return EntityConverter<T>.DataReaderToListDelay(reader);
        }

        #endregion

        #region Class: EntityConverter<T>

        /// <summary>
        /// 实体转换器
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        private class EntityConverter<T>
            where T : class, new()
        {

            #region Struct: DbColumnInfo

            private struct DbColumnInfo
            {
                public readonly string PropertyName;
                public readonly string ColumnName;
                public readonly Type Type;
                public readonly MethodInfo SetMethod;
                public readonly bool IsOptional;

                public DbColumnInfo(PropertyInfo prop, DbColumnAttribute attr)
                {
                    PropertyName = prop.Name;
                    ColumnName = attr.ColumnName ?? prop.Name;
                    Type = prop.PropertyType;
                    SetMethod = prop.GetSetMethod(false);
                    IsOptional = attr.IsOptional;
                }
            }

            #endregion

            #region Fields
            private static Converter<IDataReader, T> dataLoader;
            private static Converter<IDataReader, List<T>> batchDataLoader;
            #endregion

            #region Properties

            private static Converter<IDataReader, T> DataLoader
            {
                get
                {
                    if (dataLoader == null)
                        dataLoader = CreateDataLoader(new List<DbColumnInfo>(GetProperties()));
                    return dataLoader;
                }
            }

            private static Converter<IDataReader, List<T>> BatchDataLoader
            {
                get
                {
                    if (batchDataLoader == null)
                        batchDataLoader = CreateBatchDataLoader(new List<DbColumnInfo>(GetProperties()));
                    return batchDataLoader;
                }
            }

            #endregion

            #region Init Methods

            private static IEnumerable<DbColumnInfo> GetProperties()
            {
                DbResultAttribute dbResult = Attribute.GetCustomAttribute(typeof(T), typeof(DbResultAttribute), true) as DbResultAttribute;
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (prop.GetIndexParameters().Length > 0)
                        continue;
                    var setMethod = prop.GetSetMethod(false);
                    if (setMethod == null)
                        continue;
                    var attr = Attribute.GetCustomAttribute(prop, typeof(DbColumnAttribute), true) as DbColumnAttribute;
                    if (dbResult != null && dbResult.DefaultAsDbColumn)
                        if (attr != null && attr.Ignore)
                            continue;
                        else
                            attr = attr ?? new DbColumnAttribute();
                    else
                        if (attr == null || attr.Ignore)
                        continue;
                    yield return new DbColumnInfo(prop, attr);
                }
            }

            //private static Converter<IDataReader, T> CreateDataLoaderOld(List<DbColumnInfo> columnInfoes)
            //{
            //    DynamicMethod dm = new DynamicMethod(string.Empty, typeof(T),
            //        new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
            //    ILGenerator il = dm.GetILGenerator();
            //    il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            //    foreach (var column in columnInfoes)
            //    {
            //        Label common = il.DefineLabel();
            //        il.Emit(OpCodes.Dup);
            //        il.Emit(OpCodes.Ldarg_0);
            //        il.Emit(OpCodes.Dup);
            //        il.Emit(OpCodes.Ldstr, column.ColumnName);
            //        il.Emit(OpCodes.Callvirt, DataRecord_GetOrdinal);
            //        il.Emit(OpCodes.Callvirt, DataRecord_ItemGetter_Int);
            //        il.Emit(OpCodes.Dup);
            //        il.Emit(OpCodes.Call, Convert_IsDBNull);
            //        il.Emit(OpCodes.Brfalse_S, common);
            //        il.Emit(OpCodes.Pop);
            //        il.Emit(OpCodes.Ldnull);
            //        il.MarkLabel(common);
            //        il.Emit(OpCodes.Unbox_Any, column.Type);
            //        il.Emit(OpCodes.Callvirt, column.SetMethod);
            //    }
            //    il.Emit(OpCodes.Ret);
            //    return (Converter<IDataReader, T>)dm.CreateDelegate(typeof(Converter<IDataReader, T>));
            //}

            private static Converter<IDataReader, T> CreateDataLoader(List<DbColumnInfo> columnInfoes)
            {
                DynamicMethod dm = new DynamicMethod(string.Empty, typeof(T),
                    new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                ILGenerator il = dm.GetILGenerator();
                LocalBuilder item = il.DeclareLocal(typeof(T));
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                LocalBuilder[] colIndices = GetColumnIndices(il, columnInfoes);
                // T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                // return item;
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, T>)dm.CreateDelegate(typeof(Converter<IDataReader, T>));
            }

            //private static Converter<IDataReader, List<T>> CreateBatchDataLoaderOld(List<DbColumnInfo> columnInfoes)
            //{
            //    DynamicMethod dm = new DynamicMethod(string.Empty, typeof(List<T>),
            //        new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
            //    ILGenerator il = dm.GetILGenerator();
            //    LocalBuilder list = il.DeclareLocal(typeof(List<T>));
            //    LocalBuilder item = il.DeclareLocal(typeof(T));
            //    Label exit = il.DefineLabel();
            //    Label loop = il.DefineLabel();
            //    // List<T> list = new List<T>();
            //    il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
            //    il.Emit(OpCodes.Stloc_S, list);
            //    // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
            //    LocalBuilder[] colIndices = new LocalBuilder[columnInfoes.Count];
            //    for (int i = 0; i < colIndices.Length; i++)
            //    {
            //        colIndices[i] = il.DeclareLocal(typeof(int));
            //        il.Emit(OpCodes.Ldarg_0);
            //        il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
            //        il.Emit(OpCodes.Callvirt, DataRecord_GetOrdinal);
            //        il.Emit(OpCodes.Stloc_S, colIndices[i]);
            //    }
            //    // while (arg.Read()) {
            //    il.MarkLabel(loop);
            //    il.Emit(OpCodes.Ldarg_0);
            //    il.Emit(OpCodes.Callvirt, DataReader_Read);
            //    il.Emit(OpCodes.Brfalse, exit);
            //    // T item = new T();
            //    il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            //    il.Emit(OpCodes.Stloc_S, item);
            //    for (int i = 0; i < colIndices.Length; i++)
            //    {
            //        // item.%Property% = (%PropertyType%)arg[%index%];
            //        Label common = il.DefineLabel();
            //        il.Emit(OpCodes.Ldloc_S, item);
            //        il.Emit(OpCodes.Ldarg_0);
            //        il.Emit(OpCodes.Ldloc_S, colIndices[i]);
            //        il.Emit(OpCodes.Callvirt, DataRecord_ItemGetter_Int);
            //        il.Emit(OpCodes.Dup);
            //        il.Emit(OpCodes.Call, Convert_IsDBNull);
            //        il.Emit(OpCodes.Brfalse_S, common);
            //        il.Emit(OpCodes.Pop);
            //        il.Emit(OpCodes.Ldnull);
            //        il.MarkLabel(common);
            //        il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
            //        il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            //    }
            //    // list.Add(item);
            //    il.Emit(OpCodes.Ldloc_S, list);
            //    il.Emit(OpCodes.Ldloc_S, item);
            //    il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
            //    // }
            //    il.Emit(OpCodes.Br, loop);
            //    // return list;
            //    il.MarkLabel(exit);
            //    il.Emit(OpCodes.Ldloc_S, list);
            //    il.Emit(OpCodes.Ret);
            //    return (Converter<IDataReader, List<T>>)dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
            //}

            private static Converter<IDataReader, List<T>> CreateBatchDataLoader(List<DbColumnInfo> columnInfoes)
            {
                DynamicMethod dm = new DynamicMethod(string.Empty, typeof(List<T>),
                    new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                ILGenerator il = dm.GetILGenerator();
                LocalBuilder list = il.DeclareLocal(typeof(List<T>));
                LocalBuilder item = il.DeclareLocal(typeof(T));
                Label exit = il.DefineLabel();
                Label loop = il.DefineLabel();
                // List<T> list = new List<T>();
                il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, list);
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                LocalBuilder[] colIndices = GetColumnIndices(il, columnInfoes);
                // while (arg.Read()) {
                il.MarkLabel(loop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, DataReader_Read);
                il.Emit(OpCodes.Brfalse, exit);
                //      T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                //      list.Add(item);
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
                // }
                il.Emit(OpCodes.Br, loop);
                il.MarkLabel(exit);
                // return list;
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, List<T>>)dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
            }

            private static LocalBuilder[] GetColumnIndices(ILGenerator il, List<DbColumnInfo> columnInfoes)
            {
                LocalBuilder[] colIndices = new LocalBuilder[columnInfoes.Count];
                for (int i = 0; i < colIndices.Length; i++)
                {
                    // int %index% = arg.GetOrdinal(%ColumnName%);
                    colIndices[i] = il.DeclareLocal(typeof(int));
                    if (columnInfoes[i].IsOptional)
                    {
                        // try {
                        il.BeginExceptionBlock();
                    }
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
                    il.Emit(OpCodes.Callvirt, DataRecord_GetOrdinal);
                    il.Emit(OpCodes.Stloc_S, colIndices[i]);
                    if (columnInfoes[i].IsOptional)
                    {
                        Label exit = il.DefineLabel();
                        il.Emit(OpCodes.Leave_S, exit);
                        // } catch (IndexOutOfRangeException) {
                        il.BeginCatchBlock(typeof(IndexOutOfRangeException));
                        // //forget the exception
                        il.Emit(OpCodes.Pop);
                        // int %index% = -1; // if not found, -1
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Stloc_S, colIndices[i]);
                        il.Emit(OpCodes.Leave_S, exit);
                        // } catch (ArgumentException) {
                        il.BeginCatchBlock(typeof(ArgumentException));
                        // forget the exception
                        il.Emit(OpCodes.Pop);
                        // int %index% = -1; // if not found, -1
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Stloc_S, colIndices[i]);
                        il.Emit(OpCodes.Leave_S, exit);
                        // }
                        il.EndExceptionBlock();
                        il.MarkLabel(exit);
                    }
                }
                return colIndices;
            }

            private static void BuildItem(ILGenerator il, List<DbColumnInfo> columnInfoes,
                LocalBuilder item, LocalBuilder[] colIndices)
            {
                // T item = new T();
                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, item);
                Label skip = new Label();
                for (int i = 0; i < colIndices.Length; i++)
                {
                    if (columnInfoes[i].IsOptional)
                    {
                        // if %index% == -1 then goto skip;
                        skip = il.DefineLabel();
                        il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Beq, skip);
                    }
                    if (IsCompatibleType(columnInfoes[i].Type, typeof(int)))
                    {
                        // item.%Property% = arg.GetInt32(%index%);
                        ReadInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(int?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(int?) : (int?)arg.GetInt32(%index%);
                        ReadNullableInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long)))
                    {
                        // item.%Property% = arg.GetInt64(%index%);
                        ReadInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(long?) : (long?)arg.GetInt64(%index%);
                        ReadNullableInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(decimal)))
                    {
                        // item.%Property% = arg.GetDecimal(%index%);
                        ReadDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(decimal?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(decimal?) : (int?)arg.GetDecimal(%index%);
                        ReadNullableDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime))
                    {
                        // item.%Property% = arg.GetDateTime(%index%);
                        ReadDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(DateTime?) : (int?)arg.GetDateTime(%index%);
                        ReadNullableDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else
                    {
                        // item.%Property% = (%PropertyType%)arg[%index%];
                        ReadObject(il, item, columnInfoes, colIndices, i);
                    }
                    if (columnInfoes[i].IsOptional)
                    {
                        // :skip
                        il.MarkLabel(skip);
                    }
                }
            }

            private static bool IsCompatibleType(Type t1, Type t2)
            {
                if (t1 == t2)
                    return true;
                if (t1.IsEnum && Enum.GetUnderlyingType(t1) == t2)
                    return true;
                var u1 = Nullable.GetUnderlyingType(t1);
                var u2 = Nullable.GetUnderlyingType(t2);
                if (u1 != null && u2 != null)
                    return IsCompatibleType(u1, u2);
                return false;
            }

            private static void ReadInt32(ILGenerator il, LocalBuilder item,
                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt32(ILGenerator il, LocalBuilder item,
                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                Label intNull = il.DefineLabel();
                Label intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(
                    new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadInt64(ILGenerator il, LocalBuilder item,
                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt64(ILGenerator il, LocalBuilder item,
                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                Label intNull = il.DefineLabel();
                Label intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(
                    new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadDecimal(ILGenerator il, LocalBuilder item,
                MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDecimal(ILGenerator il, LocalBuilder item,
                MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(decimal?));
                Label decimalNull = il.DefineLabel();
                Label decimalCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, decimalNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
                il.Emit(OpCodes.Call, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
                il.Emit(OpCodes.Br_S, decimalCommon);
                il.MarkLabel(decimalNull);
                il.Emit(OpCodes.Initobj, typeof(decimal?));
                il.MarkLabel(decimalCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadDateTime(ILGenerator il, LocalBuilder item,
                MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDateTime(ILGenerator il, LocalBuilder item,
                MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(DateTime?));
                Label dtNull = il.DefineLabel();
                Label dtCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, dtNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
                il.Emit(OpCodes.Call, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
                il.Emit(OpCodes.Br_S, dtCommon);
                il.MarkLabel(dtNull);
                il.Emit(OpCodes.Initobj, typeof(DateTime?));
                il.MarkLabel(dtCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadObject(ILGenerator il, LocalBuilder item,
                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                Label common = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_ItemGetter_Int);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Call, Convert_IsDBNull);
                il.Emit(OpCodes.Brfalse_S, common);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldnull);
                il.MarkLabel(common);
                il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            #endregion

            #region Internal Methods

            internal static IEnumerable<T> DataReaderToListDelay(IDataReader reader)
            {
                while (reader.Read())
                    yield return DataLoader(reader);
            }

            internal static IEnumerable<T> DataReaderToList(IDataReader reader)
            {
                return BatchDataLoader(reader);
            }

            #endregion

        }

        #endregion

    }
}
