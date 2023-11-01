using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private CubeSpawner cubeSpawner;
    private CubeChecker cubeChecker;
    private MeshRenderer meshRenderer;
    private int colorIndex;

    public void Setup(CubeSpawner cubeSpawner, CubeChecker cubeChecker)
    {
        this.cubeSpawner = cubeSpawner; 
        this.cubeChecker = cubeChecker;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = this.cubeSpawner.CubeColors[0];
        colorIndex = 0;
    }
    public void ChangeColor()
    {
        //���� ť�� �������� �ε����� ����
        if(colorIndex < cubeSpawner.CubeColors.Length-1)
        {
            colorIndex++;
        }
        else
        {
            colorIndex = 0;
        }

        //���� ť�� ���� ����
        meshRenderer.material.color = cubeSpawner.CubeColors[colorIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer renderer = other.GetComponent<MeshRenderer>(); 

        //��ġ ������ ť��� �ε��� ť���� ������ ���� ��
        if(meshRenderer.material.color == renderer.material.color)
        {
            cubeChecker.CorrectCount++;
        }
        else
        {
            cubeChecker.IncorrectCount++;
        }
    }

}
