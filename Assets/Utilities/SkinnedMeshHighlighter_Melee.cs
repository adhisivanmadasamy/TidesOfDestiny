using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshHighlighter_Melee : MonoBehaviour
{
    [SerializeField] List<SkinnedMeshRenderer> meshesToHighlight;
    [SerializeField] Material originalMat1;    
    [SerializeField] Material highlightedMat1;    

    public void HighlightMesh(bool highlight)
    {
        foreach (var mesh in meshesToHighlight)
        {
            var Materials = mesh.GetComponent<SkinnedMeshRenderer>().materials;
                   
            
            //mesh.material = (highlight) ? highlightedMat : originalMat;

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
    }

}
