using System.Collections.Generic;
using System.IO;
using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.VegetationSystem;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace AwesomeTechnologies
{
    public class BillboardObject
    {
        public GameObject BillboardPrefab;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public GameObject Go;
    }

    public class BillboardAtlasRenderer : MonoBehaviour
    {
        public const int BillboardVersion = 2; 
        public List<BillboardObject> BillboardObjectList = new List<BillboardObject>();

        public static Texture2D GenerateBillboardTexture(GameObject prefab, BillboardQuality billboardQuality,
            LODLevel billboardSourceLODLevel, VegetationShaderType vegetationShaderType, Quaternion rotationOffset,
            Color backgroundColor, string overrideBillboardAtlasShader, bool recalculateNormals,
            float normalBlendFactor,bool generateAlpha)
        {
            Shader diffuseShader = BillboardShaderDetector.GetDiffuceBillboardAtlasShader(prefab);

            if (overrideBillboardAtlasShader != "")
            {
                diffuseShader = Shader.Find(overrideBillboardAtlasShader);
            }

           // Debug.Log("Diffuse shader:" + diffuseShader.name);
            
            Material minPostfilter = (Material) Resources.Load("MinPostFilter/MinPostFilter", typeof(Material));
            Texture2D texture = GenerateBillboardNew(prefab, GetBillboardQualityTileWidth(billboardQuality),
                GetBillboardQualityTileWidth(billboardQuality), GetBillboardQualityColumnCount(billboardQuality),
                GetBillboardQualityRowCount(billboardQuality), diffuseShader, backgroundColor, minPostfilter,
                billboardSourceLODLevel, rotationOffset, generateAlpha, recalculateNormals, normalBlendFactor);
            return texture;
        }

        public static Texture2D GenerateBillboardNormalTexture(GameObject prefab, BillboardQuality billboardQuality,
            LODLevel billboardSourceLODLevel, Quaternion rotationOffset, string overrideBillboardAtlasNormalShader,
            bool recalculateNormals, float normalBlendFactor, bool flipBackNormals)
        {
            Shader normalShader = BillboardShaderDetector.GetNormalBillboardAtlasShader(prefab);

            if (overrideBillboardAtlasNormalShader != "")
            {
                normalShader = Shader.Find(overrideBillboardAtlasNormalShader);
            }

          //  Debug.Log("Normal shader:" + normalShader.name);

            if (flipBackNormals)
            {
                Shader.SetGlobalInt("_FlipBackNormals", 1);
            }
            else
            {
                Shader.SetGlobalInt("_FlipBackNormals", 0);
            }

            Material minPostfilter = (Material) Resources.Load("MinPostFilter/MinPostFilter", typeof(Material));
            Texture2D texture = GenerateBillboardNew(prefab, GetBillboardQualityTileWidth(billboardQuality),
                GetBillboardQualityTileWidth(billboardQuality), GetBillboardQualityColumnCount(billboardQuality),
                GetBillboardQualityRowCount(billboardQuality), normalShader, new Color(0.5f, 0.5f, 1, 0.5f), minPostfilter,
                billboardSourceLODLevel, rotationOffset, false, recalculateNormals, normalBlendFactor);
            Shader.SetGlobalInt("_FlipBackNormals", 0);
            return texture;
        }

        // public static Texture2D GenerateBillboardAlphaTexture(GameObject prefab, BillboardQuality billboardQuality,
        //     LODLevel billboardSourceLODLevel, Quaternion rotationOffset, bool recalculateNormals,
        //     float normalBlendFactor)
        // {
        //     Shader normalShader = Shader.Find("AwesomeTechnologies/Billboards/AOBackground");
        //     Material minPostfilter = (Material) Resources.Load("MinPostFilter/MinPostFilter", typeof(Material));
        //     Texture2D texture = GenerateBillboardNew(prefab, GetBillboardQualityTileWidth(billboardQuality),
        //         GetBillboardQualityTileWidth(billboardQuality), GetBillboardQualityColumnCount(billboardQuality),
        //         GetBillboardQualityRowCount(billboardQuality), normalShader, new Color(0.5f, 0.5f, 1, 0), minPostfilter,
        //         billboardSourceLODLevel, rotationOffset, true, recalculateNormals, normalBlendFactor);
        //     return texture;
        // }

        public static int GetBillboardQualityTileWidth(BillboardQuality billboardQuality)
        {
            switch (billboardQuality)
            {
                case BillboardQuality.Normal:
                case BillboardQuality.Normal3D:
                case BillboardQuality.NormalSingle:
                case BillboardQuality.NormalQuad:
                    return 128;
                case BillboardQuality.High:
                case BillboardQuality.High3D:
                case BillboardQuality.HighSingle:
                case BillboardQuality.HighQuad:
                    return 256;
                case BillboardQuality.Max:
                case BillboardQuality.Max3D:
                case BillboardQuality.MaxSingle:
                case BillboardQuality.MaxQuad:
                    return 512;
                case BillboardQuality.HighSample3D:
                case BillboardQuality.HighSample2D:
                    return 256;
                default:
                    return 128;
            }
        }

        public static int GetBillboardQualityRowCount(BillboardQuality billboardQuality)
        {
            switch (billboardQuality)
            {
                case BillboardQuality.Normal:
                case BillboardQuality.High:
                case BillboardQuality.Max:
                case BillboardQuality.HighSample2D:
                case BillboardQuality.NormalSingle:
                case BillboardQuality.HighSingle:
                case BillboardQuality.MaxSingle:
                    return 1;
                case BillboardQuality.NormalQuad:
                case BillboardQuality.HighQuad:
                case BillboardQuality.MaxQuad:
                    return 1;
                case BillboardQuality.Normal3D:
                case BillboardQuality.High3D:
                case BillboardQuality.Max3D:
                    return 8;
                case BillboardQuality.HighSample3D:
                    return 16;
            }

            return 1;
        }

        public static int GetBillboardQualityColumnCount(BillboardQuality billboardQuality)
        {
            switch (billboardQuality)
            {
                case BillboardQuality.HighSample3D:
                case BillboardQuality.HighSample2D:
                    return 16;
                case BillboardQuality.NormalSingle:
                case BillboardQuality.HighSingle:
                case BillboardQuality.MaxSingle:
                    return 1;
                case BillboardQuality.NormalQuad:
                case BillboardQuality.HighQuad:
                case BillboardQuality.MaxQuad:
                    return 4;
            }

            return 8;
        }

        public static Texture2D GenerateBillboardNew(GameObject prefab, int width, int height, int gridSizeX,
            int gridSizeY, Shader replacementShader, Color backgroundColor, Material minPostfilter,
            LODLevel billboardSourceLODLevel, Quaternion rotationOffset, bool generateAlpha, bool recalculateNormals,
            float normalBlendFactor)
        {
#if UNITY_EDITOR
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
#endif

            Vector3 renderPosition = new Vector3(0, 0, 0);

            // atlas size
            var w = width * gridSizeX;
            var h = height * gridSizeY;

            Texture2D result;
            RenderTexture frameBuffer;
            RenderTexture filteredFrameBuffer;
            
            if (generateAlpha)
            {
                result = new Texture2D(w, h, TextureFormat.RGBA32, 0, true);
                frameBuffer = new RenderTexture(width, height, 24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);
                filteredFrameBuffer = new RenderTexture(width, height, 24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);
            }
            else
            {
                result = new Texture2D(w, h);
                frameBuffer = new RenderTexture(width, height, 24);
                filteredFrameBuffer = new RenderTexture(width, height, 24);
            }

            var camGo = new GameObject("TempCamera");
            var cam = camGo.AddComponent<Camera>();

            cam.clearFlags = CameraClearFlags.Color;

            backgroundColor.a = 0;
            
            cam.backgroundColor = backgroundColor;
            if (generateAlpha)
            {
                cam.backgroundColor = new Color(0, 0, 0, 1);
            }
            
            cam.renderingPath = RenderingPath.Forward;

            var go = Instantiate(prefab, renderPosition, rotationOffset);
            SetReplacementShader(go, replacementShader,generateAlpha);

            if (recalculateNormals) RecalculateMeshNormals(go, normalBlendFactor);

            go.hideFlags = HideFlags.DontSave;
            
            var bounds = CalculateBounds(go);
            float yOffset = FindLowestMeshYposition(go);
            
            cam.orthographic = true;

            var boundsSize = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            cam.orthographicSize = boundsSize;
            cam.nearClipPlane = -boundsSize * 2;
            cam.farClipPlane = boundsSize * 2;

            SetMaterialFloat(go, "_DepthBoundsSize", boundsSize * 2);

            //Debug.Log(boundsSize * 2);
            
            
            
            cam.targetTexture = frameBuffer;

            cam.transform.position = renderPosition + new Vector3(0, bounds.extents.y - yOffset / 2, 0); // + yOffset/2

            var xAngleStep = 360f / gridSizeY / 4;
            var yAngleStep = 360f / gridSizeX;


            minPostfilter.SetInt("_UseGammaCorrection", 0);

#if UNITY_EDITOR
#if UNITY_2018_1_OR_NEWER

#else
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                minPostfilter.SetInt("_UseGammaCorrection", 1);
#endif

#endif
            
            for (int i = 0; i < gridSizeX; i++)
            for (int j = 0; j < gridSizeY; j++)
            {
                cam.transform.rotation = Quaternion.AngleAxis(yAngleStep * i, Vector3.up) *
                                         Quaternion.AngleAxis(xAngleStep * j, Vector3.right);

                 Graphics.SetRenderTarget(frameBuffer);
                 GL.Viewport(new Rect(0, 0, frameBuffer.width, frameBuffer.height));
                 GL.Clear(true, true, cam.backgroundColor, 1f);
                // Debug.Log(cam.backgroundColor);
                 GL.PushMatrix();
                 
                 GL.LoadProjectionMatrix(cam.projectionMatrix);
                 GL.modelview = cam.worldToCameraMatrix;
                 GL.PushMatrix();
                
                 RenderGameObjectNow(go,(int)billboardSourceLODLevel);
                
                 GL.PopMatrix();
                 GL.PopMatrix();
                 Graphics.ClearRandomWriteTargets();
                 
                if (!generateAlpha)
                {
                    RenderTexture.active = filteredFrameBuffer;
                    Graphics.Blit(frameBuffer, minPostfilter);
                }

                result.ReadPixels(new Rect(0, 0, frameBuffer.width, frameBuffer.height), i * width, j * height);

                RenderTexture.active = null;
            }

            DestroyImmediate(go);
            DestroyImmediate(camGo);

            result.Apply();

#if UNITY_EDITOR
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
#endif

            return result;
        }

        // static void SimpleRender(ScriptableRenderContext context, HDCamera HDcamera)
        // {
        //     //renderer stripped down to this message
        //     UnityEngine.Debug.Log("Entered custom render pipeline");
        // }

        public static void RenderGameObjectNow(GameObject go, int sourceLODLevel)
        {
            GameObject root = go;
            
            LODGroup lodGroup = go.GetComponent<LODGroup>();
            if (lodGroup && lodGroup.lodCount > 0)
            {
                if (lodGroup.fadeMode == LODFadeMode.SpeedTree)
                {
                    root = lodGroup.GetLODs()[sourceLODLevel].renderers[0].gameObject;
                }
                else
                {
                    root = lodGroup.GetLODs()[0].renderers[0].gameObject;
                }
            }
            
            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                MeshFilter meshFilter = renderers[i].gameObject.GetComponent<MeshFilter>();
                if (meshFilter)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(renderers[i].transform.position, renderers[i].transform.rotation,
                        renderers[i].transform.lossyScale);
                    Mesh mesh = meshFilter.sharedMesh;

                    for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                    {
                        Material material = renderers[i].sharedMaterials[j];
                        material.SetPass(0);
                        Graphics.DrawMeshNow(mesh, matrix,j);
                    }
                }
            }
        }

        public static Texture GetDiffuseTexture(Material material)
        {
            if (material.HasProperty("_MainTex")) return material.GetTexture("_MainTex");
            if (material.HasProperty("_BaseColorMap")) return material.GetTexture("_BaseColorMap");
            if (material.HasProperty("_TrunkBaseColorMap")) return material.GetTexture("_TrunkBaseColorMap");
            if (material.HasProperty("_MainAlbedoTex")) return material.GetTexture("_MainAlbedoTex");
            if (material.HasProperty("_BaseMap")) return material.GetTexture("_BaseMap");
            //if (material.HasProperty("_MainTexArray")) return material.GetTexture("_MainTexArray");
            
            return null;
        }
        
        public static Color GetTintColor(Material material)
        {
            if (material.HasProperty("_Color")) return material.GetColor("_Color");
            if (material.HasProperty("_BaseColor")) return material.GetColor("_BaseColor");
            if (material.HasProperty("_ColorTint")) return material.GetColor("_ColorTint");
            if (material.HasProperty("_TintColor")) return material.GetColor("_TintColor");
            if (material.HasProperty("_HueVariation")) return material.GetColor("_HueVariation");
            return Color.white;
        }
        
        public static void RecalculateMeshNormals(GameObject go, float normalBlendfactor)
        {
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();

            for (int i = 0; i <= meshFilters.Length - 1; i++)
            {
                Mesh newMesh = Instantiate(meshFilters[i].sharedMesh);
                newMesh.RecalculateNormals();


                Vector3[] originalNormals = meshFilters[i].sharedMesh.normals;
                Vector3[] newNormals = newMesh.normals;

                for (int j = 0; j <= newNormals.Length - 1; j++)
                {
                    newNormals[j] = Vector3.Slerp(originalNormals[j], newNormals[j], normalBlendfactor);
                }

                newMesh.normals = newNormals;
                newMesh.UploadMeshData(false);
                meshFilters[i].mesh = newMesh;
            }
        }

        public static void RecalculateMeshNormals(Mesh mesh, int subMeshIndex)
        {
            Vector3[] normals = mesh.normals;
            Vector3[] vertices = mesh.vertices;

            int[] indexes = mesh.GetIndices(subMeshIndex);

            Vector3 center = vertices[indexes[0]];
            for (int i = 1; i <= indexes.Length - 1; i++)
            {
                center += vertices[indexes[i]];
            }

            center = center / indexes.Length;

            for (int i = 0; i <= normals.Length - 1; i++)
            {
                Vector3 newNormal = vertices[i] - center;
                normals[i] = newNormal.normalized;
            }

            mesh.normals = normals;
        }

        public static Bounds CalculateBounds(GameObject go)
        {
            Bounds combinedbounds = new Bounds(go.transform.position, Vector3.zero);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
                    Mesh mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(mesh);
                    Vector3[] vertices = mesh.vertices;

                    for (int i = 0; i <= vertices.Length - 1; i++)
                        vertices[i] = skinnedMeshRenderer.transform.TransformPoint(vertices[i]);
                    mesh.vertices = vertices;
                    mesh.RecalculateBounds();
                    Bounds meshBounds = mesh.bounds;
                    combinedbounds.Encapsulate(meshBounds);
                }
                else
                {
                    combinedbounds.Encapsulate(renderer.bounds);
                }

            return combinedbounds;
        }

        public static float FindLowestMeshYposition(GameObject go)
        {
            float lowestY = float.PositiveInfinity;

            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                MeshFilter meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
                if (meshFilter && meshFilter.sharedMesh)
                {
                    Vector3[] vertices = meshFilter.sharedMesh.vertices;
                    for (int i = 0; i <= vertices.Length - 1; i++)
                    {
                        if (vertices[i].y < lowestY) lowestY = vertices[i].y;
                    }
                }
            }

            return lowestY;
        }

        public static void SetMaterialFloat(GameObject go, string propertyName, float value)
        {
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {

                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    if (renderers[i].sharedMaterials[j].HasProperty(propertyName))
                    {
                        renderers[i].sharedMaterials[j].SetFloat(propertyName,value);
                    }
                    
                }
            }
        }
        

        public static void SetReplacementShader(GameObject go, Shader replacementShader, bool generateAlpha)
        {
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = new Material[renderers[i].sharedMaterials.Length];
                for (int j = 0; j < materials.Length; j++)
                {
                    if (renderers[i].sharedMaterials[j] != null)
                    {
                        Texture diffuseTexture = GetDiffuseTexture(renderers[i].sharedMaterials[j]);
                        Color tintColor = GetTintColor(renderers[i].sharedMaterials[j]);
                        
                      //  Debug.Log("Texture: " + diffuseTexture);
                        
                        materials[j] = new Material(renderers[i].sharedMaterials[j]);
                        materials[j].shader = replacementShader;

                        
                        if (generateAlpha && materials[j].HasProperty("_ShowAlpha"))
                        {
                            materials[j].SetInt("_ShowAlpha",1);
                        }
                        
                        if (diffuseTexture)
                        {
                            materials[j].SetTexture("_MainTex",diffuseTexture);
                        }

                        if (materials[j].HasProperty("_Color"))
                        {
                            materials[j].SetColor("_Color",tintColor);
                        }
                    }
                }

                renderers[i].sharedMaterials = materials;
            }
        }


        public static void SaveTexture(Texture2D tex, string name)
        {
#if UNITY_EDITOR
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(name + ".png", bytes);
#endif
        }


        public static void SetTextureImportSettings(Texture2D texture, bool normalMap)
        {
#if UNITY_EDITOR
            if (null == texture) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;

                if (normalMap)
                {
                    tImporter.textureType = TextureImporterType.Default;
                    tImporter.maxTextureSize = 4096;
                    tImporter.wrapMode = TextureWrapMode.Repeat;
                    tImporter.SaveAndReimport();
                }
                else
                {
                    tImporter.mipmapEnabled = true;
                    tImporter.maxTextureSize = 4096;
                    tImporter.wrapMode = TextureWrapMode.Repeat;
                    tImporter.SaveAndReimport();
                }
            }
#endif
        }
    }

    public class Volume
    {
    }
}