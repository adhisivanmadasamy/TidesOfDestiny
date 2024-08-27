using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshHighlighter : MonoBehaviour
{
    [SerializeField] List<SkinnedMeshRenderer> meshesToHighlight;
    [SerializeField] Material originalMat1;
    [SerializeField] Material originalMat2;
    [SerializeField] Material highlightedMat1;
    [SerializeField] Material highlightedMat2;

    public void HighlightMesh(bool highlight)
    {
        foreach (var mesh in meshesToHighlight)
        {
            var Materials = mesh.GetComponent<SkinnedMeshRenderer>().materials;                  
            
            
            if(this.gameObject.tag == "EnemyMelee")
            {
                if (highlight)
                {
                    Materials[0] = highlightedMat1;                    
                    mesh.GetComponent<SkinnedMeshRenderer>().materials = Materials;
                }
                else
                {
                    Materials[0] = originalMat1;                    
                    mesh.GetComponent<SkinnedMeshRenderer>().materials = Materials;
                }
            }
            else if(this.gameObject.tag == "EnemyBox")
            {
                if (highlight)
                {
                    Materials[0] = highlightedMat1;
                    Materials[1] = highlightedMat2;
                    mesh.GetComponent<SkinnedMeshRenderer>().materials = Materials;
                }
                else
                {
                    Materials[0] = originalMat1;
                    Materials[1] = originalMat2;
                    mesh.GetComponent<SkinnedMeshRenderer>().materials = Materials;
                }
            }
            
        }
    }

}
