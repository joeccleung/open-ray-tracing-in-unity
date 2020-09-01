using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenRT {
    using ShaderName = System.String;
    using GUID = System.String;

    public abstract class BaseDataTable : ICustomShaderDatabaseDataTable {
        protected SortedList<ShaderName, GUID> shaderList;
        protected SortedList<GUID, CustomShaderMeta> shaderMetaList;
        protected bool isDirty = false;

        public virtual GUID AddShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler) {
            isDirty = true;
            return AddShaderHelper(shaderMeta);
        }

        public virtual bool Contains(string shaderName) {
            return shaderMetaList.Any((kvp) => {
                return kvp.Value.name == shaderName;
            });
        }

        public virtual void Clean() {
            isDirty = false;
        }

        public virtual int GUIDToShaderIndex(string guid) {
            var name = GUIDToShaderName(guid);
            return shaderList.IndexOfKey(name);
        }

        public virtual string GUIDToShaderName(string guid) {
            if (shaderMetaList.ContainsKey(guid)) {
                return shaderMetaList[guid].name;
            } else {
                return string.Empty;
            }
        }

        public bool IsDirty() {
            return isDirty;
        }

        public virtual GUID MoveShader(CustomShaderMeta shaderMeta, CustomShaderMeta previousShaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler) {

            isDirty = true;

            var guid = shaderList[previousShaderMeta.name];
            shaderList.Remove(previousShaderMeta.name);
            shaderMetaList.Remove(previousShaderMeta.name);
            shaderList.Add(shaderMeta.name, guid);
            shaderMetaList.Add(shaderMeta.name, shaderMeta);

            return guid;
        }

        public virtual void Populate(Dictionary<string, CustomShaderMeta> data) {
            shaderList = new SortedList<ShaderName, ShaderName>();
            shaderMetaList = new SortedList<GUID, CustomShaderMeta>(comparer: new CustomShaderMetaGUIDComparer());

            foreach (var kvp in data) {
                // Shader Name = kvp.Value.name
                // Shader GUID = kvp.Key
                shaderList.Add(kvp.Value.name, kvp.Key);
                shaderMetaList.Add(kvp.Key, kvp.Value);
            }
        }

        public virtual GUID RemoveShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler) {

            isDirty = true;

            var guid = shaderList[shaderMeta.name];
            shaderList.Remove(shaderMeta.name);
            shaderMetaList.Remove(shaderMeta.name);

            return guid;
        }

        public virtual SortedList<GUID, CustomShaderMeta> ShaderMetaList {
            get {
                return shaderMetaList;
            }
        }

        public virtual SortedList<ShaderName, GUID> ShaderSortByName {
            get {
                return shaderList;
            }
        }

        public virtual string[] ShaderNameList {
            get {
                return shaderList.Keys.ToArray();
            }
        }

        public virtual GUID ShaderNameToGUID(ShaderName shaderName) {
            return shaderList[shaderName];
        }

        protected GUID AddShaderHelper(CustomShaderMeta shaderMeta) {
            string guid;
            do {
                guid = Guid.NewGuid().ToString();
            } while (shaderMetaList.ContainsKey(guid));
            shaderList.Add(shaderMeta.name, guid);
            shaderMetaList.Add(guid, shaderMeta);

            return guid;
        }
    }
}