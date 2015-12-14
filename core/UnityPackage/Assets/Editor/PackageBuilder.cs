using UnityEditor;

public static class PackageBuilder
{
    [MenuItem("Assets/Build UnityPackage")]
    public static void BuildPackage()
    {
        var assetPaths = new string[]
        {
            "Assets/Middlewares/EntityNetwork",
        };

        var packagePath = "EntityNetwork.unitypackage";
        var options = ExportPackageOptions.Recurse;
        AssetDatabase.ExportPackage(assetPaths, packagePath, options);
    }

    [MenuItem("Assets/Build UnityPackage (Full)")]
    public static void BuildPackageFull()
    {
        var assetPaths = new string[]
        {
            "Assets/Middlewares",
        };

        var packagePath = "EntityNetwork-Full.unitypackage";
        var options = ExportPackageOptions.Recurse;
        AssetDatabase.ExportPackage(assetPaths, packagePath, options);
    }
}
