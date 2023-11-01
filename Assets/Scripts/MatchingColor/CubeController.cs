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
        //다음 큐브 색상으로 인데스를 변경
        if(colorIndex < cubeSpawner.CubeColors.Length-1)
        {
            colorIndex++;
        }
        else
        {
            colorIndex = 0;
        }

        //실제 큐브 색상 변경
        meshRenderer.material.color = cubeSpawner.CubeColors[colorIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer renderer = other.GetComponent<MeshRenderer>(); 

        //터치 가능한 큐브와 부딪힌 큐브의 색상이 같을 때
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
