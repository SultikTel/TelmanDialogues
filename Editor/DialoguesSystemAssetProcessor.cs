using System;
using TelmanDialogues.Dialogues;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues.Assets
{
    public class DialoguesSystemAssetProcessor : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:DialoguesSystem", new[] { assetPath });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    DialoguesSystem asset = AssetDatabase.LoadAssetAtPath<DialoguesSystem>(path);

                    if (asset == null)
                        continue;

                    string folderPath = asset.DataFolderPath;

                    if (!string.IsNullOrEmpty(folderPath) && AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.DeleteAsset(folderPath);
                        Debug.Log($"[DialogueSystem] Deleted data folder: {folderPath}");
                    }
                }

                return AssetDeleteResult.DidNotDelete;
            }

            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (type != typeof(DialoguesSystem))
                return AssetDeleteResult.DidNotDelete;

            DialoguesSystem singleAsset = AssetDatabase.LoadAssetAtPath<DialoguesSystem>(assetPath);

            if (singleAsset == null)
                return AssetDeleteResult.DidNotDelete;

            string folder = singleAsset.DataFolderPath;

            if (!string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.DeleteAsset(folder);
                Debug.Log($"[DialogueSystem] Deleted data folder: {folder}");
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}