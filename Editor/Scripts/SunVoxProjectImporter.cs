using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

namespace SunVoxIntegration
{
    [ScriptedImporter(1, "sunvox")]
    public class SunVoxProjectImporter : ScriptedImporter
    {
        //Setting asset thumbnails via GUID means we're free to change its location in the asset/package folder structure
        string thumbnailGUID = "efa0c3f16af5af54d820bd590a431648";

        string sunvoxProjectItentifier = "SunVoxProject";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            SunVoxProject newSunVoxProject = ScriptableObject.CreateInstance<SunVoxProject>();
            newSunVoxProject.projectContents = File.ReadAllBytes(ctx.assetPath);
            ctx.AddObjectToAsset(sunvoxProjectItentifier, newSunVoxProject, AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(thumbnailGUID)));
            ctx.SetMainObject(newSunVoxProject);
        }
    }
}