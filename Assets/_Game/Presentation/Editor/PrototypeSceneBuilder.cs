using System;
using TerraToss.Geo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerraToss.Presentation.Editor
{
    /// <summary>
    /// Idempotently builds the static prototype hierarchy into Bootstrap.unity using Editor code.
    /// Running it multiple times reuses objects by stable name and never duplicates them.
    /// It never edits scene YAML directly.
    /// </summary>
    public static class PrototypeSceneBuilder
    {
        public const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";

        public const string RootName = "TerraToss_Prototype";
        public const string EarthName = "Earth";
        public const string MarkersName = "Markers";
        public const string OriginName = "Origin_Mainz";
        public const string TargetName = "Target_Helsinki";
        public const string EnvironmentName = "Environment";
        public const string DirectionalLightName = "Directional Light";
        public const string ShotVisualizationName = "ShotVisualization";
        public const string ProjectileName = "Projectile";
        public const string TrajectoryName = "Trajectory";

        // Documented approximate coordinates.
        public static readonly GeoCoordinate MainzCoordinate = new GeoCoordinate(49.9929, 8.2473);
        public static readonly GeoCoordinate HelsinkiCoordinate = new GeoCoordinate(60.1699, 24.9384);

        // ---- Centralized demonstration shot parameters (easy to locate and change) ----
        public const double DemoHeadingDegrees = 20.0;
        public const double DemoLaunchAngleDegrees = 45.0;
        public const double DemoPower = 1.0;
        public const double DemoMaximumRangeKm = 2000.0;

        // ---- Trajectory visualization parameters ----
        public const int TrajectorySampleCount = 48;
        public const float TrajectoryArcHeight = PrototypeSceneReferences.EarthRadiusUnits * 0.35f;
        public const float TrajectoryLineWidth = 0.08f;
        public const float ProjectileDiameterFactor = 0.1f;

        // ---- Flight animation parameters (compressed to the 6-10 s design window) ----
        public const float DemoFlightDurationSeconds = 8f;

        [MenuItem("TerraToss/Build Prototype Scene")]
        public static void BuildPrototypeSceneMenu()
        {
            try
            {
                Scene scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
                BuildInActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);

                if (EditorSceneManager.SaveScene(scene))
                {
                    Debug.Log($"[PrototypeSceneBuilder] Prototype scene built and saved at {BootstrapScenePath}.");
                }
                else
                {
                    Debug.LogError($"[PrototypeSceneBuilder] Failed to save scene at {BootstrapScenePath}.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrototypeSceneBuilder] Build failed: {e}");
                throw;
            }
        }

        /// <summary>
        /// Idempotently builds/updates the prototype hierarchy in the currently active scene.
        /// Does not open or save any scene. Returns the root object.
        /// </summary>
        public static GameObject BuildInActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            float radius = PrototypeSceneReferences.EarthRadiusUnits;

            GameObject root = GetOrCreateRoot(scene, RootName);
            root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Earth: a unit sphere scaled to diameter = 2 * radius.
            GameObject earth = GetOrCreatePrimitive(root.transform, EarthName, PrimitiveType.Sphere);
            earth.transform.localPosition = Vector3.zero;
            earth.transform.localRotation = Quaternion.identity;
            earth.transform.localScale = Vector3.one * (radius * 2f);
            ApplyMaterial(earth, "Earth", new Color(0.20f, 0.45f, 0.80f));

            // Markers container.
            GameObject markers = GetOrCreateChild(root.transform, MarkersName);
            markers.transform.localPosition = Vector3.zero;
            markers.transform.localRotation = Quaternion.identity;

            GameObject origin = BuildMarker(markers.transform, OriginName, MainzCoordinate, radius,
                new Color(0.85f, 0.15f, 0.15f));
            GameObject target = BuildMarker(markers.transform, TargetName, HelsinkiCoordinate, radius,
                new Color(0.15f, 0.75f, 0.30f));

            // Static shot visualization (projectile + trajectory line).
            GameObject shotVisualization = BuildShotVisualization(root.transform, radius);

            // Environment + directional light (reuse the existing one when present).
            GameObject environment = GetOrCreateChild(root.transform, EnvironmentName);
            environment.transform.localPosition = Vector3.zero;
            environment.transform.localRotation = Quaternion.identity;
            ReuseOrCreateDirectionalLight(scene, environment.transform);

            // Camera (reuse an existing one when present).
            Camera camera = ConfigureCamera(scene, origin.transform.position, target.transform.position, radius);

            // Explicit references, so nothing needs a runtime scene search.
            var references = GetOrAddComponent<PrototypeSceneReferences>(root);
            references.SetReferences(earth.transform, origin.transform, target.transform, camera);

            // Deterministic sibling order matching the documented hierarchy.
            earth.transform.SetSiblingIndex(0);
            markers.transform.SetSiblingIndex(1);
            shotVisualization.transform.SetSiblingIndex(2);
            environment.transform.SetSiblingIndex(3);

            return root;
        }

        private static GameObject BuildMarker(Transform parent, string name, GeoCoordinate coordinate,
            float radius, Color color)
        {
            GameObject marker = GetOrCreatePrimitive(parent, name, PrimitiveType.Capsule);

            Vector3 surface = GeoSphereProjection.ToLocalPosition(coordinate, radius);
            Vector3 normal = GeoSphereProjection.SurfaceNormal(coordinate);

            // A default capsule is 2 units tall (half-height 1) along local up before scaling.
            float thickness = radius * 0.06f;
            float halfHeight = radius * 0.125f;
            marker.transform.localScale = new Vector3(thickness, halfHeight, thickness);
            marker.transform.localRotation = Quaternion.FromToRotation(Vector3.up, normal);
            // Place the base on the surface: centre = surface + normal * worldHalfHeight (= scale.y here).
            marker.transform.localPosition = surface + normal * halfHeight;

            ApplyMaterial(marker, name, color);
            return marker;
        }

        private static GameObject BuildShotVisualization(Transform rootTransform, float radius)
        {
            GameObject shotVisualization = GetOrCreateChild(rootTransform, ShotVisualizationName);
            shotVisualization.transform.localPosition = Vector3.zero;
            shotVisualization.transform.localRotation = Quaternion.identity;

            // Projectile placeholder sphere; its position is set by the director when a shot is fired.
            GameObject projectile = GetOrCreatePrimitive(shotVisualization.transform, ProjectileName, PrimitiveType.Sphere);
            projectile.transform.localScale = Vector3.one * (radius * ProjectileDiameterFactor);
            projectile.transform.localRotation = Quaternion.identity;
            ApplyMaterial(projectile, "Projectile", new Color(0.95f, 0.75f, 0.10f));

            // Trajectory: a LineRenderer driven by the presentation-only view.
            GameObject trajectory = GetOrCreateChild(shotVisualization.transform, TrajectoryName);
            trajectory.transform.localPosition = Vector3.zero;
            trajectory.transform.localRotation = Quaternion.identity;

            LineRenderer line = GetOrAddComponent<LineRenderer>(trajectory);
            ConfigureLineRenderer(line);
            ApplyLineMaterial(line, "Trajectory", new Color(0.95f, 0.55f, 0.10f));

            var view = GetOrAddComponent<ShotTrajectoryView>(trajectory);
            view.SetLineRenderer(line);

            var animator = GetOrAddComponent<ShotFlightAnimator>(shotVisualization);

            // Director orchestrates a shot: compute -> sample -> project -> view + projectile + flight.
            var director = GetOrAddComponent<ShotVisualizationDirector>(shotVisualization);
            director.Configure(view, animator, projectile.transform,
                MainzCoordinate, HelsinkiCoordinate, DemoMaximumRangeKm,
                radius, TrajectoryArcHeight, TrajectorySampleCount, DemoFlightDurationSeconds);

            // Desktop aiming controls + minimal on-screen readout (fire with Space in Play Mode).
            var aim = GetOrAddComponent<ShotAimController>(shotVisualization);
            aim.Configure(director, DemoHeadingDegrees, DemoLaunchAngleDegrees, DemoPower);

            var readout = GetOrAddComponent<ShotAimReadout>(shotVisualization);
            readout.Configure(aim, director);

            // Populate the initial static trajectory (without playing the flight).
            director.Fire(DemoHeadingDegrees, DemoLaunchAngleDegrees, DemoPower, play: false);

            return shotVisualization;
        }

        private static void ConfigureLineRenderer(LineRenderer line)
        {
            // Positions are supplied in the LineRenderer's local space (see ShotTrajectoryView).
            line.useWorldSpace = false;
            line.widthMultiplier = TrajectoryLineWidth;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
        }

        private static Camera ConfigureCamera(Scene scene, Vector3 originPosition, Vector3 targetPosition, float radius)
        {
            Camera camera = FindFirstInScene<Camera>(scene);
            if (camera == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                SceneManager.MoveGameObjectToScene(go, scene);
                camera = go.AddComponent<Camera>();
            }

            Vector3 mid = originPosition + targetPosition;
            Vector3 direction = mid.sqrMagnitude > 1e-6f ? mid.normalized : Vector3.forward;
            Vector3 up = Mathf.Abs(Vector3.Dot(direction, Vector3.up)) > 0.9f ? Vector3.forward : Vector3.up;

            float distance = radius * 3f;
            camera.transform.position = direction * distance;
            camera.transform.rotation = Quaternion.LookRotation(-direction, up);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = distance + radius * 3f;
            return camera;
        }

        private static void ReuseOrCreateDirectionalLight(Scene scene, Transform environment)
        {
            Light directional = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Light light in root.GetComponentsInChildren<Light>(true))
                {
                    if (light.type == LightType.Directional)
                    {
                        directional = light;
                        break;
                    }
                }
                if (directional != null)
                {
                    break;
                }
            }

            if (directional != null)
            {
                if (directional.transform.parent != environment)
                {
                    directional.transform.SetParent(environment, true);
                }
                return;
            }

            var go = new GameObject(DirectionalLightName);
            go.transform.SetParent(environment, false);
            Light created = go.AddComponent<Light>();
            created.type = LightType.Directional;
            go.transform.rotation = Quaternion.Euler(50f, 330f, 0f);
        }

        // ---- Idempotent helpers ----

        private static GameObject GetOrCreateRoot(Scene scene, string name)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name == name)
                {
                    return go;
                }
            }

            var created = new GameObject(name);
            SceneManager.MoveGameObjectToScene(created, scene);
            return created;
        }

        private static GameObject GetOrCreateChild(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject GetOrCreatePrimitive(Transform parent, string name, PrimitiveType type)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }

            return go;
        }

        private static void ApplyMaterial(GameObject go, string materialName, Color color)
        {
            ApplyMaterial(go.GetComponent<Renderer>(), materialName, color, "Universal Render Pipeline/Lit");
        }

        private static void ApplyLineMaterial(LineRenderer line, string materialName, Color color)
        {
            ApplyMaterial(line, materialName, color, "Universal Render Pipeline/Unlit");
        }

        private static void ApplyMaterial(Renderer renderer, string materialName, Color color, string preferredShader)
        {
            if (renderer == null)
            {
                return;
            }

            Material material = renderer.sharedMaterial;
            if (material == null || material.name != materialName)
            {
                Shader shader = Shader.Find(preferredShader);
                if (shader == null)
                {
                    shader = Shader.Find("Universal Render Pipeline/Lit");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                material = new Material(shader) { name = materialName };
                renderer.sharedMaterial = material;
            }

            material.color = color;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }

        private static T FindFirstInScene<T>(Scene scene) where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
