using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Material material;
    public RectTransform panel;

    private void OnDrawGizmos()
    {
        DrawLines();
    }

    private void OnRenderObject()
    {
        Debug.Log("render object please");
        DrawLines();
    }    
    static Material _lineMaterial;
    static void CreateLineMaterial()
    {
        if (!_lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void DrawLines()
    {
        CreateLineMaterial();
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        _lineMaterial.SetPass(0);
        GL.Color(Color.black);
        var rect = panel.rect;
        var position = panel.transform.position;
        Vector3[] corners = new Vector3[4];
        panel.GetWorldCorners(corners);
        float maxX = corners[2].x, maxY = corners[1].y;
        Debug.Log(maxX + " " + rect.xMax );
        float minX = corners[0].x, minY = corners[0].y;
        GL.Color(new Color(0.8f, 1f, 0.86f));
        GL.Vertex3(minX + 10, minY + 10, 0);
        GL.Vertex3(minX + 100, minY + 100, 0);
        GL.Vertex3(minX + 100, minY + 100, 0);
        GL.Vertex3(minX + 150, minY + 200, 0);
        GL.Color(Color.red);
        GL.Vertex3(minX, minY, 0);
        GL.Vertex3(maxX, maxY, 0);
        GL.Color(Color.red);
        GL.Vertex3(20, 0, 0);
        GL.Vertex3(100, 0, 0);
        
        GL.End();
        GL.PopMatrix();
        
    }
    
}