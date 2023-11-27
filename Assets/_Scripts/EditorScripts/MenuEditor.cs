#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace _Scripts.EditorScripts
{
    public static class MenuEditor
    {
        [MenuItem("PlayerPrefs/Clear PlayerPrefs", false, 1)]
        private static void NewMenuOption()
        {
            PlayerPrefs.SetInt("level" , 1);
            LevelManager.Instance.currentLevelNumber = PlayerPrefs.GetInt("level");
            UIManager.Instance.levelIndex.text = "Level " + LevelManager.Instance.currentLevelNumber;
        }

        [MenuItem("LevelDesignHelper/Create LevelPrefab", false, 0)]
        private static void LevelPrefabAdd()
        {
            EditorUtility.CreateEmptyPrefab("Assets/_Levels/" + "BossyLevel_1"+".prefab");
            EditorUtility.CreateEmptyPrefab("Assets/_Levels/" + "NormalLevel_1"+".prefab");
        }
        
        
        [MenuItem("LevelDesignHelper/Add 'LevelDesignScript' to all 'Level' prefabs", false, 1)]
        private static void LevelDesign()
        {
            var leveling = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            
            string[] normalLevels = AssetDatabase.FindAssets("NormalLevel_", new[] { "Assets/_Levels" });

            var countNormals = normalLevels.Length;

            for (int i = 1; i != countNormals + 1; i++)
            {
                string c = "Assets/_Levels/NormalLevel_" + i + ".prefab";
                var assetPath = c;
            
                using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
                {
                    var prefabRoot = editingScope.prefabContentsRoot;
                    prefabRoot.AddComponent<LevelDesignScript>().levelType = LevelDesignScript.LevelType.Normal;
                }
            }

            string[] bossyLevels = AssetDatabase.FindAssets("BossyLevel_", new[] { "Assets/_Levels" });

            var countBossy = bossyLevels.Length;

            for (int i = 1; i != countBossy + 1; i++)
            {
                string c = "Assets/_Levels/BossyLevel_" + i + ".prefab";
                var assetPath = c;
                
                using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
                {
                    var prefabRoot = editingScope.prefabContentsRoot;
                    prefabRoot.AddComponent<LevelDesignScript>().levelType = LevelDesignScript.LevelType.WithTheBoss;
                }
            }
        }
    }
}

#endif