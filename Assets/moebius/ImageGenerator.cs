using System.IO;
using UnityEngine;
using UnityEditor;

public class ImageGenerator : Editor
{
    [MenuItem("Tools/Generate 128x128 Image")]
    public static void GenerateImage()
    {
        int imgSize = 128;
        int lineInterval = 10;
        int lineWidth = 1;

        // 创建一个Texture2D来生成图像
        Texture2D texture = new Texture2D(imgSize, imgSize, TextureFormat.RGB24, false,true);
        texture.filterMode = FilterMode.Point;  // 设置为点过滤模式，避免像素混合
        
        // 初始化为黑色背景
        Color black = new Color(0, 0, 0);
        Color red = new Color(1, 0, 0);
        Color green = new Color(0, 1, 0);
        Color blue = new Color(0, 0, 1);

        for (int y = 0; y < imgSize; y++)
        {
            for (int x = 0; x < imgSize; x++)
            {
                texture.SetPixel(x, y, black);
            }
        }

        // 绘制水平红色线
        for (int y = 0; y < imgSize; y += lineInterval)
        {
            for (int dy = 0; dy < lineWidth; dy++)
            {
                if (y + dy < imgSize)
                {
                    for (int x = 0; x < imgSize; x++)
                    {
                        texture.SetPixel(x, y + dy, red);
                    }
                }
            }
        }

        // 绘制垂直绿色线
        for (int x = 0; x < imgSize; x += lineInterval)
        {
            for (int dx = 0; dx < lineWidth; dx++)
            {
                if (x + dx < imgSize)
                {
                    for (int y = 0; y < imgSize; y++)
                    {
                        Color existingColor = texture.GetPixel(x + dx, y);
                        texture.SetPixel(x + dx, y, BlendColors(existingColor, green));
                        //texture.SetPixel(x + dx, y, green);
                    }
                }
            }
        }

        // 绘制45度蓝色斜线
        /*for (int start = -imgSize; start < imgSize; start += lineInterval)
        {
            for (int offset = 0; offset < lineWidth; offset++)
            {
                for (int i = Mathf.Max(0, start); i < Mathf.Min(imgSize, imgSize + start); i++)
                {
                    int j = i - start;
                    if (i + offset < imgSize && j < imgSize)
                    {
                        Color existingColor = texture.GetPixel(i + offset, j);
                        texture.SetPixel(i + offset, j, BlendColors(existingColor, blue));
                    }
                }
            }
        }*/
        // 绘制45度蓝色斜线
        for (int startY = -imgSize; startY < imgSize; startY += lineInterval)
        {
            for (int offsetY = 0; offsetY < lineWidth; offsetY++)
            {
                for (int x = 0; x < imgSize; x++)
                {
                    int y = x + offsetY+startY;
                    if (y>=0 && y < imgSize)
                    {
                        Color existingColor = texture.GetPixel(x , y);
                        texture.SetPixel(x, y, BlendColors(existingColor, blue));
                    }
                }
            }
        }

        // 应用更改
        texture.Apply();

        // 将图像保存为PNG文件
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "GeneratedImage.png");
        File.WriteAllBytes(path, bytes);

        // 刷新AssetDatabase，以便Unity项目可以看到新生成的文件
        AssetDatabase.Refresh();

        Debug.Log("Image generated and saved to: " + path);
    }

    // 混合颜色的方法
    private static Color BlendColors(Color baseColor, Color blendColor)
    {
        float r = Mathf.Clamp01(baseColor.r + blendColor.r);
        float g = Mathf.Clamp01(baseColor.g + blendColor.g);
        float b = Mathf.Clamp01(baseColor.b + blendColor.b);
        return new Color(r, g, b);
    }
}
