using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public abstract class AbstractBuildPipeline {
        public readonly BuildTarget Target;

        private readonly ICollection<AbstractBuildStep> _steps = new List<AbstractBuildStep>();

        protected AbstractBuildPipeline(BuildTarget target) {
            Target = target;
        }

        public void AddStep(AbstractBuildStep step) {
            _steps.Add(step);
        }

        internal void Execute(string outputDir, Action<Action> enqueueToMainThread) {
            string targetOutputDir = Path.Combine(outputDir, Target.ToString());

            try {
                foreach (AbstractBuildStep buildStep in _steps) {
                    if (buildStep.UseMainThread) {
                        bool stepComplete = false;

                        enqueueToMainThread.Invoke(() => {
                            Debug.Log($"Step starting on main thread: {Target.ToString()}-{buildStep.GetType().Name}");
                            buildStep.Execute(targetOutputDir, this);
                            Debug.Log($"Step complete: {Target.ToString()}-{buildStep.GetType().Name}");
                            stepComplete = true;
                        });

                        SpinWait.SpinUntil(() => stepComplete);
                    } else {
                        Debug.Log($"Step starting off main thread: {Target.ToString()}-{buildStep.GetType().Name}");
                        buildStep.Execute(targetOutputDir, this);
                        Debug.Log($"Step complete: {Target.ToString()}-{buildStep.GetType().Name}");
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}
