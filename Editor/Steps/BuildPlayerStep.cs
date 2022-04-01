using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Software10101.BuildScripting.Editor {
    public class BuildPlayerStep : AbstractBuildStep {
        private readonly string[] _scenes;
        private readonly string _outputPath;
        private readonly BuildOptions _options;
        private readonly BuildTarget _target;
        private readonly string[] _scriptingDefineSymbols;

        /// <summary>
        /// Note that this runs on the main thread.
        /// </summary>
        /// <param name="scenes">List of scenes to include in the build.</param>
        /// <param name="outputPath">Relative to [workingDir]/[outputDir]/[targetDir].</param>
        /// <param name="target">The platform to build for.</param>
        /// <param name="options">Build options.</param>
        /// <param name="scriptingDefineSymbols">Scripting define symbols which should be added for this build.</param>
        public BuildPlayerStep(
            string[] scenes,
            string outputPath,
            BuildTarget target,
            BuildOptions options,
            params string[] scriptingDefineSymbols
        ) : base(true) {
            _scenes = scenes;
            _outputPath = outputPath;
            _options = options;
            _target = target;
            _scriptingDefineSymbols = scriptingDefineSymbols ?? Array.Empty<string>();
        }

        public override void Execute(string outputDir, AbstractBuildPipeline pipeline) {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(_target);

            string backupDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            foreach (string scriptingDefineSymbol in _scriptingDefineSymbols) {
                AddDefine(buildTargetGroup, scriptingDefineSymbol);
            }

            string outputPath = Path.Combine(outputDir, _outputPath);
            BuildPipeline.BuildPlayer(_scenes, outputPath, _target, _options);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, backupDefines);
        }

        private static void AddDefine(BuildTargetGroup group, string define) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            HashSet<string> definesCollection = new HashSet<string>(defines.Split(';').ToList());
            definesCollection.Add(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", definesCollection));
        }

        private static void RemoveDefine(BuildTargetGroup group, string define) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            HashSet<string> definesCollection = new HashSet<string>(defines.Split(';').ToList());
            definesCollection.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", definesCollection));
        }
    }
}
