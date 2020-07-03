﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GUID = System.String;

namespace OpenRT {
    public class CustomShaderDatabase {

        private static CustomShaderDatabase sharedInstance;

        public static CustomShaderDatabase Instance {
            get {
                if (sharedInstance == null) {
                    sharedInstance = new CustomShaderDatabase();
                }
                return sharedInstance;
            }
        }

        private ClosestHitDataTable closestHitDataTable;
        private IntersectDataTable intersectDataTable;

        private CustomShaderDatabaseFile databaseFile;
        private CustomShaderDatabaseFileIO fileIO;

        public CustomShaderDatabase() {
            closestHitDataTable = new ClosestHitDataTable();
            intersectDataTable = new IntersectDataTable();

            fileIO = new CustomShaderDatabaseFileIO();
            databaseFile = fileIO.ReadDatabaseFromFile();

            closestHitDataTable.Populate(databaseFile.closetHit);
            intersectDataTable.Populate(databaseFile.intersect);
        }

        public string[] ShaderNameList(EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return closestHitDataTable.ShaderNameList;

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderNameList;

                default:
                    return new string[0];
            }
        }

        public SortedList<GUID, CustomShaderMeta> ShaderMetaList(EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return closestHitDataTable.ShaderMetaList;

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderMetaList;

                default:
                    return new SortedList<GUID, CustomShaderMeta>();
            }
        }

        public void Add(CustomShaderMeta meta) {

            switch (meta.shaderType) {
                case OpenRT.EShaderType.CloestHit:
                    _Add(meta, closestHitDataTable);
                    break;

                case OpenRT.EShaderType.Intersect:
                    _Add(meta, intersectDataTable);
                    break;

                default:
                    // TODO: Support adding shaders of type {meta.shaderType}
                    Debug.LogWarning($"TODO: Support adding shaders of type {meta.shaderType}");
                    break;
            }
        }

        private void _Add(CustomShaderMeta meta, ICustomShaderDatabaseDataTable datatable) {
            if (datatable.Contains(meta.name)) {
                // TODO: Support Update
                Debug.LogWarning("TODO: Support shader update");
            } else {
                datatable.AddShader(meta, databaseFile, fileIO);
            }

        }

        public int GUIDToShaderIndex(GUID guid, EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return closestHitDataTable.GUIDToShaderIndex(guid);

                case EShaderType.Intersect:
                    return intersectDataTable.GUIDToShaderIndex(guid);

                default:
                    return -1;
            }
        }

        public void PopulateShaderDatabase(CustomShaderDatabaseFile databaseFile) {
            
        }

        public string ShaderNameToGUID(string shaderName, EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return closestHitDataTable.ShaderNameToGUID(shaderName);

                case EShaderType.Intersect:
                    return intersectDataTable.ShaderNameToGUID(shaderName);

                default:
                    return string.Empty;
            }
        }
    }
}