using System.IO;
using TelmanDialogues.Dialogues;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues.Assets
{
    public static class DialoguesSystemCreator
    {
        private const string RootFolder = "Assets/TelmanDialogues/DialogueBlocks";

        [MenuItem("Assets/Create/Game/Dialogue System")]
        public static void Create()
        {
            DialoguesSystem asset = ScriptableObject.CreateInstance<DialoguesSystem>();

            string selectedPath = "Assets";

            if (Selection.activeObject != null)
            {
                selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (!AssetDatabase.IsValidFolder(selectedPath))
                    selectedPath = Path.GetDirectoryName(selectedPath);
            }

            string assetName = "DialoguesSystem";

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{selectedPath}/{assetName}.asset"
            );

            AssetDatabase.CreateAsset(asset, assetPath);

            string systemGuid = GUID.Generate().ToString();

            if (!AssetDatabase.IsValidFolder(RootFolder))
            {
                AssetDatabase.CreateFolder("Assets/TelmanDialogues", "DialogueBlocks");
            }

            string dataFolderPath = $"{RootFolder}/{systemGuid}";

            if (!AssetDatabase.IsValidFolder(dataFolderPath))
            {
                AssetDatabase.CreateFolder(RootFolder, systemGuid);
            }

            asset.SetDataFolderGuid($"{RootFolder}/{systemGuid}");

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
        }
    }
}