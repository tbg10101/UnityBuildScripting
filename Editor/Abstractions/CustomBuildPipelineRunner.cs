using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEditor;
using Debug = UnityEngine.Debug;
using ThreadState = System.Threading.ThreadState;

namespace Software10101.BuildScripting.Editor {
    // TODO: update this to use the Progress API when we update to 2020.x:
    // https://docs.unity3d.com/2020.3/Documentation/ScriptReference/Progress.html
    [InitializeOnLoad]
    public static class CustomBuildPipelineRunner {
        private static readonly Thread UnityMainThread = null;

        static CustomBuildPipelineRunner() {
            UnityMainThread = Thread.CurrentThread;
        }

        public static void Execute(string outputDir, params AbstractBuildPipeline[] buildPipelines) {
            Execute(outputDir, buildPipelines.AsEnumerable());
        }

        public static void Execute(string outputDir, IEnumerable<AbstractBuildPipeline> buildPipelines) {
            if (Thread.CurrentThread != UnityMainThread) {
                throw new Exception("Pipeline runner must be run from Unity's main thread!");
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            BlockingCollection<Action> mainThreadQueue = new BlockingCollection<Action>();
            ICollection<Thread> threads = buildPipelines
                .Select(bp => {
                    Thread newThread = new Thread(() => bp.Execute(outputDir, a => mainThreadQueue.Add(a))) {
                        Name = $"CustomBuildPipelineRunner-{bp.Name}"
                    };

                    newThread.Start();
                    return newThread;
                })
                .ToList();

            while (threads.Any(t => t.ThreadState == ThreadState.Running || t.ThreadState.HasFlag(ThreadState.WaitSleepJoin))) {
                if (mainThreadQueue.TryTake(out Action action, 100)) {
                    action.Invoke();
                }
            }

            sw.Stop();
            Debug.Log($"All builds finished in {sw.Elapsed.TotalSeconds} seconds.");
        }
    }
}
