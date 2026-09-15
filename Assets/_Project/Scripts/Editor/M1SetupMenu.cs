using BoomPG.Core.Logging;
using BoomPG.Gameplay.Camera;
using BoomPG.Gameplay.Combat;
using BoomPG.Gameplay.Config;
using BoomPG.Gameplay.Player;
using BoomPG.Gameplay.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoomPG.Editor
{
    /// <summary>M1 검증에 필요한 에셋과 씬을 재현한다.</summary>
    public static class M1SetupMenu
    {
        private const string MovementPath = "Assets/_Project/Data/SO_MovementConfig.asset";
        private const string CombatPath = "Assets/_Project/Data/SO_CombatConfig.asset";
        private const string JetpackPath = "Assets/_Project/Data/SO_JetpackConfig.asset";
        private const string RocketPath = "Assets/_Project/Prefabs/P_Rocket.prefab";
        private const string InputPath = "Assets/_Project/Settings/InputSystem_Actions.inputactions";
        private const string ScenePath = "Assets/_Project/Scenes/M1_Greybox.unity";

        [MenuItem("BoomPG/Setup/1. 기본 에셋 생성")]
        private static void CreateBaseAssets()
        {
            if (!ValidateLayers())
            {
                return;
            }

            CreateConfig<MovementConfig>(MovementPath);
            CreateConfig<CombatConfig>(CombatPath);
            CreateConfig<JetpackConfig>(JetpackPath);
            if (AssetDatabase.LoadMainAssetAtPath(RocketPath) != null)
            {
                GameLog.Warn("Editor", $"{RocketPath} 이미 존재하여 건너뜀");
            }
            else
            {
                GameObject rocket = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rocket.name = "P_Rocket";
                rocket.transform.localScale = Vector3.one * 0.3f;
                rocket.layer = LayerMask.NameToLayer("Rocket");
                Object.DestroyImmediate(rocket.GetComponent<SphereCollider>());
                rocket.AddComponent<RocketProjectile>();
                PrefabUtility.SaveAsPrefabAsset(rocket, RocketPath);
                Object.DestroyImmediate(rocket);
            }

            AssetDatabase.SaveAssets();
            GameLog.Core("M1 기본 에셋 생성 완료");
        }

        [MenuItem("BoomPG/Setup/2. M1 그레이박스 씬 생성")]
        private static void CreateGreyboxScene()
        {
            MovementConfig movement = AssetDatabase.LoadAssetAtPath<MovementConfig>(MovementPath);
            CombatConfig combat = AssetDatabase.LoadAssetAtPath<CombatConfig>(CombatPath);
            JetpackConfig jetpack = AssetDatabase.LoadAssetAtPath<JetpackConfig>(JetpackPath);
            GameObject rocket = AssetDatabase.LoadAssetAtPath<GameObject>(RocketPath);
            // 직렬화 참조만 주입하므로 Editor 어셈블리에 입력 패키지 참조를 추가하지 않는다.
            Object actions = AssetDatabase.LoadAssetAtPath<Object>(InputPath);
            if (movement == null || combat == null || jetpack == null || rocket == null || actions == null)
            {
                GameLog.Error("Editor", "필수 에셋 누락. 먼저 기본 에셋 생성 메뉴를 실행하십시오.");
                return;
            }

            if (!ValidateLayers())
            {
                return;
            }

            if (AssetDatabase.LoadMainAssetAtPath(ScenePath) != null)
            {
                GameLog.Warn("Editor", $"{ScenePath} 이미 존재하여 건너뜀");
                return;
            }

            // 현재 편집 중인 씬을 버리지 않고 새 씬에만 생성한다.
            Scene previous = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            CreatePlatform("MainPlatform", Vector3.zero, new Vector3(40f, 1f, 40f));
            CreatePlatform("SidePlatform1", new Vector3(26f, 0f, 0f), new Vector3(8f, 1f, 8f));
            CreatePlatform("SidePlatform2", new Vector3(-26f, 0f, 0f), new Vector3(8f, 1f, 8f));
            CreatePlatform("SidePlatform3", new Vector3(0f, 0f, 26f), new Vector3(8f, 1f, 8f));
            CreatePlatform("Wall", new Vector3(0f, 2f, 10f), new Vector3(6f, 4f, 1f));

            var killZone = new GameObject("KillZone");
            killZone.layer = LayerMask.NameToLayer("KillZone");
            killZone.transform.position = new Vector3(0f, -20f, 0f);
            killZone.transform.localScale = new Vector3(400f, 1f, 400f);
            killZone.AddComponent<BoxCollider>().isTrigger = true;
            killZone.AddComponent<KillZoneTrigger>();

            bool valid = true;
            GameObject player = CreateCharacter("Player", new Vector3(0f, 1f, -8f), movement, combat, ref valid);
            JetpackController thrust = player.AddComponent<JetpackController>();
            RpgLauncher launcher = player.AddComponent<RpgLauncher>();
            PlayerInputRelay input = player.AddComponent<PlayerInputRelay>();
            valid &= Inject(thrust, "_jetpackConfig", jetpack);
            valid &= Inject(launcher, "_combatConfig", combat);
            valid &= Inject(launcher, "_rocketPrefab", rocket);
            valid &= Inject(input, "_actions", actions);

            CreateCharacter("Dummy1", new Vector3(4f, 1f, 4f), movement, combat, ref valid);
            CreateCharacter("Dummy2", new Vector3(-4f, 1f, 4f), movement, combat, ref valid);
            CreateCharacter("Dummy3", new Vector3(8f, 1f, 8f), movement, combat, ref valid);
            CreateCharacter("Dummy4", new Vector3(-8f, 1f, 8f), movement, combat, ref valid);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<UnityEngine.Camera>();
            ThirdPersonCamera camera = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraObject.transform.position = player.transform.position + Vector3.up * 1.6f - Vector3.forward * 5f;
            valid &= Inject(camera, "_target", player.transform);
            valid &= Inject(input, "_camera", camera);

            var lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            if (!valid || !EditorSceneManager.SaveScene(scene, ScenePath))
            {
                GameLog.Error("Editor", "필드 주입 또는 씬 저장 실패. 미완성 씬을 닫습니다.");
                SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            GameLog.Core("M1 그레이박스 씬 생성 완료");
        }

        private static void CreateConfig<T>(string path) where T : ScriptableObject
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
            {
                GameLog.Warn("Editor", $"{path} 이미 존재하여 건너뜀");
                return;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
        }

        private static void CreatePlatform(string name, Vector3 position, Vector3 scale)
        {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = name;
            platform.layer = LayerMask.NameToLayer("Platform");
            platform.transform.position = position;
            platform.transform.localScale = scale;
        }

        private static GameObject CreateCharacter(string name, Vector3 position,
            MovementConfig movement, CombatConfig combat, ref bool valid)
        {
            GameObject character = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            character.name = name;
            character.transform.position = position;
            character.layer = LayerMask.NameToLayer("Player");
            // 기본 캡슐과 CharacterController가 중복 충돌하지 않도록 하나만 유지한다.
            Object.DestroyImmediate(character.GetComponent<CapsuleCollider>());
            CharacterController controller = character.AddComponent<CharacterController>();
            controller.radius = movement.CapsuleRadius;
            controller.height = movement.CapsuleHeight;
            PlayerMotor motor = character.AddComponent<PlayerMotor>();
            character.AddComponent<HealthComponent>();
            valid &= Inject(motor, "_movementConfig", movement);
            valid &= Inject(motor, "_combatConfig", combat);
            return character;
        }

        private static bool Inject(Object component, string field, Object value)
        {
            var serialized = new SerializedObject(component);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null)
            {
                GameLog.Error("Editor", $"{component.name} 직렬화 필드 누락: {field}");
                return false;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return true;
        }

        private static bool ValidateLayers()
        {
            string[] names = { "Player", "Platform", "Rocket", "Pickup", "KillZone", "Grapple" };
            for (int i = 0; i < names.Length; i++)
            {
                if (LayerMask.NameToLayer(names[i]) != 8 + i)
                {
                    GameLog.Error("Editor", $"레이어 등록 불일치: {names[i]}");
                    return false;
                }
            }

            return true;
        }
    }
}
