using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class SceneTextureCollection {
        private SortedDictionary<string, Texture2D> textures = new SortedDictionary<string, Texture2D>();

        public void AddTexture(string textureName, Texture2D tex) {
            if (!textures.ContainsKey(textureName)) {
                textures.Add(textureName, tex);
            }
        }

        public SortedDictionary<string, Texture2D> Textures {
            get {
                return textures;
            }
        }
    }
}