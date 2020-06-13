namespace OpenRT {
    public struct CustomShaderMeta {

        public readonly string name;
        public readonly string absPath;
        public readonly OpenRT.EShaderType shaderType;

        public CustomShaderMeta(string name, string absPath, OpenRT.EShaderType shaderType) {
            this.name = name;
            this.absPath = absPath;
            this.shaderType = shaderType;
        }
    }
}