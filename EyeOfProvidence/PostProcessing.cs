using System.Collections.Generic;
using UnityEngine;
using BepInExHelpers.Extensions;

namespace EyeOfProvidence;

class PostProcessing : MonoBehaviour {
    enum CameraFace : int {
        Front = 0,
        Back = 1,
        Left = 2,
        Right = 3,
        Up = 4,
        Down = 5
    };

    static float FOV = 360;
    static float Quality = 9;

    public Camera mainCam;
    public Material postEffectMaterial;
    public Camera[] cams;
    List<RenderTexture> camTexs = new();

    readonly FilterMode filterMode = FilterMode.Point; // TODO: test static, maybe make it a setting?

    // Shader postShader;

    void Start() {
        mainCam = GetComponent<Camera>();
        mainCam.depthTextureMode = mainCam.depthTextureMode | DepthTextureMode.DepthNormals;

        Settings.Debug.SetOnSettingChanged((v) => postEffectMaterial.SetFloat("_DEBUG", v ? 1 : 0));

        Settings.Grid.SetOnSettingChanged((v) => postEffectMaterial.SetFloat("_GRID", v ? 1 : 0));
        Settings.GridOpacity.SetOnSettingChanged((v) => postEffectMaterial.SetFloat("_GRID_FACTOR", v));

        Settings.FOV.SetOnSettingChanged((v) => {
            FOV = v;
            postEffectMaterial.SetFloat("_FOV", FOV);
        });
        Settings.Perspective.SetOnSettingChanged((v) => {
            postEffectMaterial.SetFloat("_MODE", (int)v);
            cams[(int)CameraFace.Back]?.gameObject.SetActive(v != Settings.PerspectiveMode.Panini);
            RefreshRenderTextures();
        });
        Settings.Quality.SetOnSettingChanged((v) => {
            Quality = Mathf.Clamp(v, 0, 10);
            RefreshRenderTextures();
        });
        Settings.Stretch.SetOnSettingChanged((v) => {
            postEffectMaterial.SetFloat("_STRETCH", v ? 1 : 0);
            postEffectMaterial.SetFloat("_ASPECT", v ? 1 : ((float)Screen.width / (float)Screen.height));
        });

        Settings.StereoFactor.SetOnSettingChanged((v) => postEffectMaterial.SetFloat("_FISHEYE_STEREO_FACTOR", v));
        Settings.PaniniFactor.SetOnSettingChanged((v) => postEffectMaterial.SetFloat("_PANINI_FACTOR", v));

        RefreshRenderTextures(); // TODO: necessary?
    }

    void Update() {
        float newFOV = FOV;
        CameraController camController = CameraController.Instance;
        if (camController) {
            float ratio = camController.cam.fieldOfView / camController.defaultFov;
            newFOV *= ratio;
            UnityEngine.Debug.LogError("Updated FOV: " + newFOV + " (Ratio: " + ratio + ")");
        }
        postEffectMaterial.SetFloat("_FOV", newFOV);
    }

    public void RefreshRenderTextures() {
        int qualityPixel = (int)Mathf.Pow(2, Quality);

        foreach (Camera cam in cams) {
            if (cam == null) continue;
            cam.targetTexture?.Release();
            RenderTexture rendTex = new(qualityPixel, qualityPixel, 24);
            rendTex.filterMode = filterMode;
            cam.targetTexture = rendTex;
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        // if (postEffectMaterial == null) {
        //     postEffectMaterial = new Material(postShader);
        // }
        // RenderTexture rendTex = RenderTexture.GetTemporary(
        // src.width,
        // src.height,
        // src.depth,
        // src.format);

        // Graphics.Blit(cam1.targetTexture, cam1Tex);
        Matrix4x4 viewToWorld = mainCam.cameraToWorldMatrix;

        postEffectMaterial.SetMatrix("_viewToWorld", viewToWorld);

        postEffectMaterial.SetTexture("_Cam_Front", cams[(int)CameraFace.Front].targetTexture);
        postEffectMaterial.SetTexture("_Cam_Back", cams[(int)CameraFace.Back].targetTexture);
        postEffectMaterial.SetTexture("_Cam_Left", cams[(int)CameraFace.Left].targetTexture);
        postEffectMaterial.SetTexture("_Cam_Right", cams[(int)CameraFace.Right].targetTexture);
        postEffectMaterial.SetTexture("_Cam_Up", cams[(int)CameraFace.Up].targetTexture);
        postEffectMaterial.SetTexture("_Cam_Down", cams[(int)CameraFace.Down].targetTexture);

        Graphics.Blit(src, dest, postEffectMaterial);
        // RenderTexture.ReleaseTemporary(rendTex);
    }
}