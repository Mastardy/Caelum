using UnityEditor;
using UnityEngine;


//Credits:
//Original hard-coded solution by NathanJSmith: https://answers.unity.com/questions/1538195/unity-lod-billboard-asset-example.html?childToView=1692072#answer-1692072
//Cutomization to script by BarShiftGames on above link, and below link
//Inspiration/Feedback/More coding optimization by SomeGuy22 : https://answers.unity.com/questions/1692296/unable-to-save-generatedrendered-texture2d-to-use.html

#if UNITY_EDITOR
public class GenerateBillboard : ScriptableWizard
{
    [Header("")]
    [Tooltip("This should be a Nature/Speedtree billboard material")]
    public Material m_material;
    [Tooltip("How much to pinch the mesh at a certain point, to cut off extra pixels off the mesh that won't be needed.")]
    [Range(0, 1)]
    public float topWidth = 1;
    [Tooltip("How much to pinch the mesh at a certain point, to cut off extra pixels off the mesh that won't be needed.")]
    [Range(0, 1)]
    public float midWidth = 1;
    [Tooltip("How much to pinch the mesh at a certain point, to cut off extra pixels off the mesh that won't be needed.")]
    [Range(0, 1)]
    public float botWidth = 1;

    [Tooltip("Units in height of the object, roughly, this can be fine-tuned later on the final asset")]
    public float objectHeight = 0;
    [Tooltip("Units in width of the object, roughly, this can be fine-tuned later on the final asset")]
    public float objectWidth = 0;
    [Tooltip("Usually negative and small, to make it sit in the ground slightly, can be modifed on final asset")]
    public float bottomOffset = 0;

    [Tooltip("The amount of rows in the texture atlas")]
    [Min(1)]
    public int atlasRowImageCount = 3;
    [Tooltip("The amount of columns in the texture atlas")]
    [Min(1)]
    public int atlasColumnImageCount = 3;
    [Tooltip("The total number of images to bake, ALSO decides how many angles to view from")]
    [Min(1)]
    public int totalImageCount = 8;

    [Header("-")]
    [Tooltip("This dictates the rotational center of the render for the billboard, and what is rotated to get different angles.\nThis also checks once for an object with named \"BillboardCameraArm\"")]
    public GameObject toRotateCamera;
    [Tooltip("This should be child of toRotateCamera, and on the local +x axis from it, facing center with a complete view of the object")]
    public Camera renderCamera;

    [Header("Dimensios of atlas")]
    public int atlasPixelWidth = 1024;
    public int atlasPixelHeight = 1024;

    [Header("Optional renderer to set in")]
    public BillboardRenderer optionalBillboardRenderer;

    private bool doOnce = true;
    private bool checkArmOnce = true;

    void OnWizardUpdate()
    {
        string helpString = "";
        bool isValid = (m_material != null && objectHeight != 0 && objectWidth != 0 && renderCamera != null && toRotateCamera != null);

        if (doOnce)
        {
            //this will get activated once
            doOnce = false;
            toRotateCamera = GameObject.Find("BillboardCameraArm");

        }

        if (toRotateCamera != null && checkArmOnce)
        {
            //this will check for a camera under toRotateCamera once
            checkArmOnce = false;
            Camera cam = toRotateCamera.GetComponentInChildren<Camera>();
            if (cam != null) { renderCamera = cam; }
        }
    }



    void OnWizardCreate()
    {
        //function to execute on submit

        BillboardAsset billboard = new BillboardAsset();

        billboard.material = m_material;
        Vector4[] texCoords = new Vector4[totalImageCount];

        ushort[] indices = new ushort[12];
        Vector2[] vertices = new Vector2[6];

        //make texture to save at end
        var texture = new Texture2D(atlasPixelWidth, atlasPixelHeight, TextureFormat.ARGB32, false);
        //make render texture to copy to texture and assign it to camera
        //renderCamera.targetTexture = RenderTexture.GetTemporary(atlasPixelWidth / atlasColumnImageCount, atlasPixelHeight / atlasRowImageCount, 16);
        renderCamera.targetTexture = RenderTexture.GetTemporary(atlasPixelWidth, atlasPixelHeight, 16);
        var renderTex = renderCamera.targetTexture;
        renderCamera.targetTexture = renderTex;

        //reset rotation, but camera should be on local +x axis from rotating object
        toRotateCamera.transform.eulerAngles = Vector3.zero;
        int imageAt = 0;
        for (int j = 0; j < atlasRowImageCount; j++)
        {
            for (int i = 0; i < atlasColumnImageCount; i++)
            {
                //i is x, j is y
                if (imageAt < totalImageCount)
                {
                    //atla them left-right, top-bottom, 0,0 is bottom left
                    float xRatio = (float)i / atlasColumnImageCount;
                    float yRatio = (float)(atlasRowImageCount - j - 1) / atlasRowImageCount;

                    //starts at viewing from +x, and rotates camera clockwise around object, uses amount of vertices set (later down) to tell how many angles to view from
                    texCoords[imageAt].Set(xRatio, yRatio, 1f / atlasColumnImageCount, 1f / atlasRowImageCount);
                    imageAt++;

                    //set rect of where to render texture to
                    renderCamera.rect = new Rect(xRatio, yRatio, 1f / atlasColumnImageCount, 1f / atlasRowImageCount);
                    renderCamera.Render();

                    //read pixels on rec
                    //Rect rec = new Rect(xRatio * atlasPixelWidth, yRatio * atlasPixelHeight, (float)1 / atlasColumnImageCount * atlasPixelWidth, (float)1 / atlasRowImageCount * atlasPixelHeight);
                    //texture.ReadPixels(rec, i / atlasColumnImageCount * atlasPixelWidth, (atlasRowImageCount - j - 1) / atlasRowImageCount * atlasPixelHeight);

                    toRotateCamera.transform.eulerAngles -= Vector3.up * (360 / totalImageCount);
                }
            }
        }
        toRotateCamera.transform.eulerAngles = Vector3.zero;
        renderCamera.rect = new Rect(0, 0, 1, 1);

        RenderTexture pastActive = RenderTexture.active;
        RenderTexture.active = renderTex;
        texture.ReadPixels(new Rect(0, 0, atlasPixelWidth, atlasPixelHeight), 0, 0);
        RenderTexture.active = pastActive;
        texture.Apply();

        //texCoords[0].Set(0.230981f, 0.33333302f, 0.230981f, -0.33333302f);
        //texCoords[1].Set(0.230981f, 0.66666603f, 0.230981f, -0.33333302f);
        //texCoords[2].Set(0.33333302f, 0.0f, 0.33333302f, 0.23098099f);
        //texCoords[3].Set(0.564314f, 0.23098099f, 0.23098099f, -0.33333302f);
        //texCoords[4].Set(0.564314f, 0.564314f, 0.23098099f, -0.33333403f);
        //texCoords[5].Set(0.66666603f, 0.0f, 0.33333302f, 0.23098099f);
        //texCoords[6].Set(0.89764804f, 0.23098099f, 0.230982f, -0.33333302f);
        //texCoords[7].Set(0.89764804f, 0.564314f, 0.230982f, -0.33333403f);

        //make basic box out of four trinagles, to be able to pinch the top/bottom/middle to cut extra transparent pixels
        //still not sure how this works but it connects vertices to make the mesh
        indices[0] = 4;
        indices[1] = 3;
        indices[2] = 0;
        indices[3] = 1;
        indices[4] = 4;
        indices[5] = 0;
        indices[6] = 5;
        indices[7] = 4;
        indices[8] = 1;
        indices[9] = 2;
        indices[10] = 5;
        indices[11] = 1;

        //set vertices positions on mesh
        vertices[0].Set(-botWidth / 2 + 0.5f, 0);
        vertices[1].Set(-midWidth / 2 + 0.5f, 0.5f);
        vertices[2].Set(-topWidth / 2 + 0.5f, 1);
        vertices[3].Set(botWidth / 2 + 0.5f, 0);
        vertices[4].Set(midWidth / 2 + 0.5f, 0.5f);
        vertices[5].Set(topWidth / 2 + 0.5f, 1);

        //assign data
        billboard.SetImageTexCoords(texCoords);
        billboard.SetIndices(indices);
        billboard.SetVertices(vertices);

        billboard.width = objectWidth;
        billboard.height = objectHeight;
        billboard.bottom = bottomOffset;

        //save assets
        string path;
        int nameLength = AssetDatabase.GetAssetPath(m_material).Length;
        //take out ".mat" prefix
        path = AssetDatabase.GetAssetPath(m_material).Substring(0, nameLength - 4) + ".asset";
        AssetDatabase.CreateAsset(billboard, path);
        path = AssetDatabase.GetAssetPath(m_material).Substring(0, nameLength - 4) + ".png";
        byte[] byteArray = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, byteArray);
        Debug.Log("BILLBOARD ASSET COMPLETED: File saved to " + path + ",\n if pressing save in editor breaks billboard, manually assign texture to material");

        if (optionalBillboardRenderer != null)
        {
            optionalBillboardRenderer.billboard = billboard;
        }

        //cleanup / qol things
        RenderTexture.ReleaseTemporary(renderTex);
        renderCamera.targetTexture = null;
        m_material.SetTexture("_MainTex", texture);

        AssetDatabase.Refresh();
    }

    [MenuItem("Window/Rendering/Generate Billboard of Object")]
    static void MakeBillboard()
    {
        ScriptableWizard.DisplayWizard<GenerateBillboard>(
            "Make Billboard from object", "Create");
    }
}
#endif