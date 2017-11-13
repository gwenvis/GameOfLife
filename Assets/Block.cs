using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool _dead = true;
    private MeshRenderer renderer;
    private static Material offMat;
    private static Material onMat;
    
    private void Start()
    {
        if (offMat == null)
            CreateMaterial();

        renderer = GetComponent<MeshRenderer>();
        renderer.material = offMat;
    }

    private void CreateMaterial()
    {
        offMat = new Material(Shader.Find("Unlit/Color"));
        onMat = new Material(Shader.Find("Unlit/Color"));
        onMat.color = Color.red;
        offMat.color = Color.white;
    }

    public void SetDead(bool value)
    {
        renderer.material = value ? offMat : onMat;
        _dead = value;
    }

    public bool GetDead()
    {
        return _dead;
    }

    public bool Dead
    {
        get { return _dead; }
        set
        {
            renderer.material = value ? offMat : onMat;
            _dead = value;
        }
    }
}