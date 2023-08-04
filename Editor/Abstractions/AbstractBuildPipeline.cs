using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Software10101.BuildScripting.Editor {
    public abstract class AbstractBuildPipeline {
        public readonly string Name;

        private readonly IList<AbstractBuildStep> _steps = new List<AbstractBuildStep>();

        protected AbstractBuildPipeline(string name) {
            Name = name;
        }

        public void AddStep(AbstractBuildStep step) {
            _steps.Add(step);
        }

        internal void Execute(string outputDir, Action<IEnumerable<Action>> enqueueToMainThread) {
            string targetOutputDir = Path.Combine(outputDir, Name);

            try {
                for (int i = 0; i < _steps.Count; i++) {
                    AbstractBuildStep buildStep = _steps[i];

                    if (buildStep.UseMainThread) {
                        // if there are consecutive build steps which occur on the main thread, group them together

                        int uncompletedStepsCount = 0; // not atomic because these steps all execute on the main thread

                        ICollection<Action> consecutiveMainThreadSteps = new LinkedList<Action>();

                        while (i < _steps.Count && buildStep.UseMainThread) {
                            AbstractBuildStep mainThreadBuildStep = _steps[i];
                            uncompletedStepsCount++;

                            consecutiveMainThreadSteps.Add(() => {
                                Debug.Log($"Step starting on main thread: {Name}-{mainThreadBuildStep.GetType().Name}");

                                try {
                                    mainThreadBuildStep.Execute(targetOutputDir, this);
                                    Debug.Log($"Step complete: {Name}-{mainThreadBuildStep.GetType().Name}");
                                } catch (Exception e) {
                                    Debug.LogError($"Step failed: {Name}-{mainThreadBuildStep.GetType().Name}\n" +
                                                   $"{e.GetType().FullName}: {e.Message}\n{e.StackTrace}\n");
                                    throw;
                                }

                                uncompletedStepsCount--;
                            });

                            i++;
                        }

                        i--; // because we are still in a foreach

                        enqueueToMainThread(consecutiveMainThreadSteps);

                        SpinWait.SpinUntil(() => uncompletedStepsCount <= 0);
                    } else {
                        Debug.Log($"Step starting off main thread: {Name}-{buildStep.GetType().Name}");
                        buildStep.Execute(targetOutputDir, this);
                        Debug.Log($"Step complete: {Name}-{buildStep.GetType().Name}");
                    }
                }
            } catch (Exception e) {
                Debug.LogError($"Exception while executing build pipeline: {e.Message}\n{e.StackTrace}\n");
            }
        }
    }
}
