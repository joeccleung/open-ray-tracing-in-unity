﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GUID = System.String;

namespace OpenRT
{
    public class CustomShaderDatabase
    {

        private static CustomShaderDatabase sharedInstance;

        public static CustomShaderDatabase Instance
        {
            get
            {
                if (sharedInstance == null)
                {
                    sharedInstance = new CustomShaderDatabase();
                }
                return sharedInstance;
            }
        }

        private ClosestHitDataTable closestHitDataTable;
        private IntersectDataTable intersectDataTable;

        private CustomShaderDatabaseFile databaseFile;
        private CustomShaderDatabaseFileIO fileIO;

        public CustomShaderDatabase()
        {
            LoadShaderDatabase();
        }

        public bool IsShaderTableDirty(EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.IsDirty();

                case EShaderType.Intersect:
                    return intersectDataTable.IsDirty();

                default:
                    Debug.LogWarning($"TODO: Support adding shaders of type {shaderType}");
                    return false;
            }
        }

        public void LoadShaderDatabase()
        {
            closestHitDataTable = new ClosestHitDataTable();
            intersectDataTable = new IntersectDataTable();

            fileIO = new CustomShaderDatabaseFileIO();
            databaseFile = fileIO.ReadDatabaseFromFile();

            closestHitDataTable.Populate(databaseFile.closetHit);
            intersectDataTable.Populate(databaseFile.intersect);
        }


        public string[] ShaderNameList(EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.ShaderNameList;

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderNameList;

                default:
                    return new string[0];
            }
        }

        public SortedList<GUID, CustomShaderMeta> ShaderMetaList(EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.ShaderMetaList;

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderMetaList;

                default:
                    return new SortedList<GUID, CustomShaderMeta>();
            }
        }

        public SortedList<string, GUID> ShaderSortedByName(EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.ShaderSortByName;

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderSortByName;

                default:
                    return new SortedList<string, GUID>();
            }
        }

        public void Add(CustomShaderMeta meta)
        {

            switch (meta.shaderType)
            {
                case EShaderType.ClosestHit:
                    _Add(meta, closestHitDataTable);
                    break;

                case EShaderType.Intersect:
                    _Add(meta, intersectDataTable);
                    break;

                default:
                    // TODO: Support adding shaders of type {meta.shaderType}
                    Debug.LogWarning($"TODO: Support adding shaders of type {meta.shaderType}");
                    break;
            }
        }

        private void _Add(CustomShaderMeta meta, ICustomShaderDatabaseDataTable datatable)
        {
            if (datatable.Contains(meta.name))
            {
                datatable.MoveShader(meta, meta, databaseFile, fileIO);
            }
            else
            {
                datatable.AddShader(meta, databaseFile, fileIO);
            }

        }

        public int GUIDToShaderIndex(GUID guid, EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.GUIDToShaderIndex(guid);

                case EShaderType.Intersect:
                    return intersectDataTable.GUIDToShaderIndex(guid);

                default:
                    return -1;
            }
        }

        public void Move(CustomShaderMeta meta, CustomShaderMeta previous)
        {
            switch (meta.shaderType)
            {
                case OpenRT.EShaderType.ClosestHit:
                    _Move(meta, previous, closestHitDataTable);
                    break;

                case OpenRT.EShaderType.Intersect:
                    _Move(meta, previous, intersectDataTable);
                    break;

                default:
                    // TODO: Support adding shaders of type {meta.shaderType}
                    Debug.LogWarning($"TODO: Support adding shaders of type {meta.shaderType}");
                    break;
            }
        }

        private void _Move(CustomShaderMeta meta, CustomShaderMeta previous, ICustomShaderDatabaseDataTable table)
        {
            table.MoveShader(meta, previous, databaseFile, fileIO);
        }

        public void Remove(CustomShaderMeta meta)
        {
            switch (meta.shaderType)
            {
                case OpenRT.EShaderType.ClosestHit:
                    _Remove(meta, closestHitDataTable);
                    break;

                case OpenRT.EShaderType.Intersect:
                    _Remove(meta, intersectDataTable);
                    break;

                default:
                    // TODO: Support adding shaders of type {meta.shaderType}
                    Debug.LogWarning($"TODO: Support adding shaders of type {meta.shaderType}");
                    break;
            }
        }

        private void _Remove(CustomShaderMeta meta, ICustomShaderDatabaseDataTable table)
        {
            table.RemoveShader(meta, databaseFile, fileIO);
        }

        public void SetShaderTableClean(EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    closestHitDataTable.Clean();
                    break;

                case EShaderType.Intersect:
                    intersectDataTable.Clean();
                    break;

                default:
                    Debug.LogWarning($"TODO: Support adding shaders of type {shaderType}");
                    break;
            }
        }

        public string ShaderNameToGUID(string shaderName, EShaderType shaderType)
        {
            switch (shaderType)
            {
                case EShaderType.ClosestHit:
                    return closestHitDataTable.ShaderNameToGUID(shaderName);

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderNameToGUID(shaderName);

                default:
                    return string.Empty;
            }
        }
    }
}