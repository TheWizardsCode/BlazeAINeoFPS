#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class PackageInstaller 
{
    private static ListRequest _listRequest;
    private static readonly string _EditorSymbol = "VSP_PACKAGES";
    private static readonly string _requiredPackage = "com.unity.jobs";
    
    [InitializeOnLoadMethod]
    static void VerifyPackages()
    {
#if !VSP_PACKAGES
        if (!Application.isPlaying)
        {
            _listRequest = Client.List(false,true);  
            EditorApplication.update += ListProgress;
        }
#endif
    }

    static void ListProgress()
    {
        if (_listRequest.IsCompleted)
        {
            bool hasCollections = false;
            if (_listRequest.Status == StatusCode.Success)
                foreach (var package in _listRequest.Result)
                {
                    if (package.name.Contains(_requiredPackage))
                    {
                        hasCollections = true;
                    }

                    //Debug.Log("Package name: " + package.name);
                }
            else if (_listRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(_listRequest.Error.message);
            }

            EditorApplication.update -= ListProgress;

            if (!hasCollections)
            {
                if (EditorUtility.DisplayDialog("Vegetation Studio Pro - Install needed packages",
                    "Vegetation Studio Pro needs Burst, Mathematics, Jobs and Collections packages installed from the package manager. Confirm to install the missing packages.",
                    "Confirm","Cancel"))
                {
                    Client.Add(_requiredPackage);
                }
            }
            else
            {
                AddCompilerDefine();
            }
        }
    }
    
    static void AddCompilerDefine()
    {
        var symbols =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!symbols.Contains(_EditorSymbol))
        {
            symbols += ";" + _EditorSymbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                symbols);
        }
    }
}
#endif
