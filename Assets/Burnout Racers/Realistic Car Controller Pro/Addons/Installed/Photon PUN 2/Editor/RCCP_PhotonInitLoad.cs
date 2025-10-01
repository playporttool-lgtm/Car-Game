//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class RCCP_PhotonInitLoad : MonoBehaviour {

    [InitializeOnLoadMethod]
    static void InitOnLoad() {

        EditorApplication.delayCall += EditorUpdate;

    }

    public static void EditorUpdate() {

        bool hasKey = false;

#if RCCP_PHOTON
        hasKey = true;
#endif

        if (!hasKey) {

            RCCP_SetScriptingSymbol.SetEnabled("RCCP_PHOTON", true);
            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Photon PUN 2 For Realistic Car Controller Pro", "Be sure you have imported latest Photon PUN 2 to your project. Pass in your AppID to Photon, and run the RCCP_Scene_Blank_Photon demo scene. You can find more detailed info in documentation.", "Close");

            RCCP_SceneUpdater.Check();

            RenderPipelineAsset rp = GraphicsSettings.currentRenderPipeline;

            if (rp == null)   // Built-in → nothing to convert
                return;

            bool isURP = rp.GetType().ToString().Contains("Universal");
            bool isHDRP = rp.GetType().ToString().Contains("HD");

            if (!isURP && !isHDRP)
                return;

            string rpName = isURP ? "URP" : "HDRP";
            bool ok = EditorUtility.DisplayDialog(
                "Convert Materials",
                $"Your project is using {rpName}.\n\n" +
                $"You'll need to convert the imported assets to be working with {rpName}.?\n\n" +
                $"You can open the RCCP Render Pipeline Converter Window and proceed.",
                "Yes, open converter",
                "No thanks"
            );

            if (!ok)
                return;

            RCCP_RenderPipelineConverterWindow.Init();

        }


    }

}
