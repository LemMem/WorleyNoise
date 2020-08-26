using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class WorleyNoise : MonoBehaviour
{
    public int size;
    public Gradient gradient;
    public int numPoints = 7;
    public int z;
    public int n;
    public string ExportPath;
    public bool evolve;
    private int width;
    private int height;
    private Texture2D tex;
    private MeshRenderer render;
    private Vector3Int[] points;
    private int framecount;

    void Start()
    {
        width = size;
        height = size;
        render = GetComponent<MeshRenderer>();
        tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        points = new Vector3Int[numPoints];
        render.material.SetTexture("_MainTex", tex);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3Int(Random.Range(0, width), Random.Range(0, height), Random.Range(0, size));
        }
    }

    private void FixedUpdate()
    {
        framecount++;
        z = Mathf.Clamp(z, 0, size);
        Debug.Log(framecount % size);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float[] distances = new float[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    Vector3Int v = points[i];
                    if(evolve)
                    {
                        z = framecount % size;
                    }
                    float d = Vector3Int.Distance(new Vector3Int(x, y, z), v);
                    distances[i] = d;
                }
                System.Array.Sort(distances);
                float noise = Math.Remap(distances[n], 0, width / 2, 1, 0);
                
                tex.SetPixel(x, y, gradient.Evaluate(noise));
            }
        }
        tex.Apply();
    }
    public void ExportToImage()
    {
        byte[] img = tex.EncodeToPNG();
        try
        {
        File.WriteAllBytes(ExportPath + "/output.png", img);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
