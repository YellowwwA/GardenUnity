using UnityEngine;

public class PixelTo3D : MonoBehaviour
{
    public Texture2D sourceImage;
    public float desiredSize = 1.0f;
    public int targetVoxelCount = 3000;
    public Color topColor = Color.white;
    public Color bottomColor = Color.gray;

    void Start()
    {
        GenerateCubeWithFilledDepth();
    }

    void GenerateCubeWithFilledDepth()
    {
        if (sourceImage == null)
        {
            Debug.LogError("이미지를 넣어주세요.");
            return;
        }

        int imgW = sourceImage.width;
        int imgH = sourceImage.height;

        int skip = Mathf.CeilToInt(Mathf.Sqrt((imgW * imgH * 6f) / targetVoxelCount));
        int reduced = Mathf.Max(1, imgW / skip);
        float voxelSize = desiredSize / reduced;

        Vector3 centerOffset = Vector3.one * (reduced - 1) / 2f;

        // ✅ 전/후/좌/우 면 (깊이 방향으로 채움)
        for (int x = 0; x < imgW; x += skip)
        {
            for (int y = 0; y < imgH; y += skip)
            {
                Color c = sourceImage.GetPixel(x, y);
                if (c.a < 0.1f) continue;

                int px = x / skip;
                int py = y / skip;

                // FRONT (+Z)
                for (int z = 0; z < reduced; z++)
                {
                    Vector3 p = new Vector3(px, py, z);
                    CreateVoxel((p - centerOffset) * voxelSize, voxelSize, c);
                }

                // BACK (-Z) - z = reduced - 1 → 0 (정면으로 봤을 때 대칭)
                for (int z = 0; z < reduced; z++)
                {
                    Vector3 p = new Vector3(px, py, reduced - 1 - z);
                    CreateVoxel((p - centerOffset) * voxelSize, voxelSize, c);
                }

                // LEFT (-X)
                for (int dx = 0; dx < reduced; dx++)
                {
                    Vector3 p = new Vector3(dx, py, px);
                    CreateVoxel((p - centerOffset) * voxelSize, voxelSize, c);
                }

                // RIGHT (+X) - 수정됨 (반대쪽 끝)
                for (int dx = 0; dx < reduced; dx++)
                {
                    Vector3 p = new Vector3(reduced - 1 - dx, py, px);
                    CreateVoxel((p - centerOffset) * voxelSize, voxelSize, c);
                }
            }
        }

        // ✅ 위/아래 면 - 단색으로 덮기
        for (int x = 0; x < reduced; x++)
        {
            for (int z = 0; z < reduced; z++)
            {
                Vector3 top = new Vector3(x, reduced - 1, z);
                Vector3 bottom = new Vector3(x, 0, z);

                CreateVoxel((top - centerOffset) * voxelSize, voxelSize, topColor);
                CreateVoxel((bottom - centerOffset) * voxelSize, voxelSize, bottomColor);
            }
        }

        Debug.Log("✅ 정육면체 생성 완료 (6면 + 내부 + 대칭 정렬)");
    }

    void CreateVoxel(Vector3 position, float size, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(this.transform);
        cube.transform.localPosition = position;
        cube.transform.localScale = Vector3.one * size;

        var renderer = cube.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = color;
    }
}
