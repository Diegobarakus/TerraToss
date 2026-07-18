using NUnit.Framework;
using TerraToss.Presentation.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerraToss.Presentation.Tests.EditMode
{
    [TestFixture]
    public class PrototypeSceneBuilderTests
    {
        [Test]
        public void BuildInActiveScene_RunTwice_DoesNotDuplicateMainObjects()
        {
            // EditMode tests run in a disposable untitled scene; build there and clean up afterwards
            // so Bootstrap.unity is never touched or saved.
            Scene scene = SceneManager.GetActiveScene();
            GameObject root = null;

            try
            {
                PrototypeSceneBuilder.BuildInActiveScene();
                PrototypeSceneBuilder.BuildInActiveScene();

                Assert.AreEqual(1, CountRoots(scene, PrototypeSceneBuilder.RootName),
                    "The prototype root must not be duplicated.");

                root = FindRoot(scene, PrototypeSceneBuilder.RootName);
                Assert.IsNotNull(root);

                Assert.AreEqual(1, CountChildren(root.transform, PrototypeSceneBuilder.EarthName));
                Assert.AreEqual(1, CountChildren(root.transform, PrototypeSceneBuilder.MarkersName));
                Assert.AreEqual(1, CountChildren(root.transform, PrototypeSceneBuilder.EnvironmentName));

                Transform markers = root.transform.Find(PrototypeSceneBuilder.MarkersName);
                Assert.AreEqual(1, CountChildren(markers, PrototypeSceneBuilder.OriginName));
                Assert.AreEqual(1, CountChildren(markers, PrototypeSceneBuilder.TargetName));

                Assert.IsNotNull(root.GetComponent<PrototypeSceneReferences>());

                // Shot visualization must not be duplicated either.
                Assert.AreEqual(1, CountChildren(root.transform, PrototypeSceneBuilder.ShotVisualizationName));
                Transform shotVisualization = root.transform.Find(PrototypeSceneBuilder.ShotVisualizationName);
                Assert.AreEqual(1, CountChildren(shotVisualization, PrototypeSceneBuilder.ProjectileName));
                Assert.AreEqual(1, CountChildren(shotVisualization, PrototypeSceneBuilder.TrajectoryName));

                Transform trajectory = shotVisualization.Find(PrototypeSceneBuilder.TrajectoryName);
                Assert.AreEqual(1, trajectory.GetComponents<LineRenderer>().Length,
                    "Trajectory must have exactly one LineRenderer.");
                Assert.AreEqual(1, trajectory.GetComponents<ShotTrajectoryView>().Length,
                    "Trajectory must have exactly one ShotTrajectoryView.");
                Assert.That(trajectory.GetComponent<LineRenderer>().positionCount, Is.GreaterThan(1));
            }
            finally
            {
                if (root != null)
                {
                    Object.DestroyImmediate(root);
                }
            }
        }

        private static int CountRoots(Scene scene, string name)
        {
            int count = 0;
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name == name)
                {
                    count++;
                }
            }
            return count;
        }

        private static GameObject FindRoot(Scene scene, string name)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name == name)
                {
                    return go;
                }
            }
            return null;
        }

        private static int CountChildren(Transform parent, string name)
        {
            int count = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name == name)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
