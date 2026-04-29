using System;
using TelmanDialogues.Dialogues;
using TelmanDialogues.Windows;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues.Assets
{
    public class DialoguesSystemAssetProcessor : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            DialoguesEditorWindow[] windows = Resources.FindObjectsOfTypeAll<DialoguesEditorWindow>();
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:DialoguesSystem", new[] { assetPath });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    DialoguesSystem asset = AssetDatabase.LoadAssetAtPath<DialoguesSystem>(path);

                    if (asset == null)
                        continue;

                    foreach (var window in windows)
                    {
                        if (window.DialogueSystem == asset)
                        {
                            window.Close();
                        }
                    }

                    string folderPath = asset.DataFolderPath;

                    if (!string.IsNullOrEmpty(folderPath) && AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.DeleteAsset(folderPath);
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

            foreach (var window in windows)
            {
                if (window.DialogueSystem == singleAsset)
                {
                    window.Close();
                }
            }

            string folder = singleAsset.DataFolderPath;

            if (!string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.DeleteAsset(folder);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}