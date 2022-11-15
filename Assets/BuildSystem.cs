using UnityEngine;
using System.Linq;

public class BuildSystem : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Structure[] structures;

    [SerializeField]
    private Material blueMaterial;

    [SerializeField]
    private Material redMaterial;

    private StructureType currentStructureType;

    private bool inPlace;
    private bool canBuild;
    private Vector3 finalPosition;

    [SerializeField]
    private Transform rotationRef;

    private void FixedUpdate()
    {
        canBuild = GetCurrentStructure().placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
        finalPosition = GetNearestPoint(transform.position);
        CheckPosition();
        RoundPlacementStructureRotation();
        UpdatePlacementStructureMaterial();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeStructureType(StructureType.Stairs);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeStructureType(StructureType.Wall);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeStructureType(StructureType.Floor);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && canBuild && inPlace)
        {
            Instantiate(GetCurrentStructure().instantiatedPrefab, GetCurrentStructure().placementPrefab.transform.position, GetCurrentStructure().placementPrefab.transform.GetChild(0).transform.rotation);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateStructure();
        }
    }

    void RoundPlacementStructureRotation()
    {
        float Yangle = rotationRef.localEulerAngles.y;
        int roundedRotation = 0;

        if(Yangle > -45 && Yangle <= 45)
        {
            roundedRotation = 0;
        }else if (Yangle > 45 && Yangle <= 135)
        {
            roundedRotation = 90;
        }
        else if (Yangle > 135 && Yangle <= 225)
        {
            roundedRotation = 180;
        }
        else if (Yangle > 225 && Yangle <= 315)
        {
            roundedRotation = 270;
        }

        GetCurrentStructure().placementPrefab.transform.rotation = Quaternion.Euler(0, roundedRotation, 0);
    }

    void RotateStructure()
    {
        if(currentStructureType != StructureType.Wall)
        {
            GetCurrentStructure().placementPrefab.transform.GetChild(0).transform.Rotate(0, 90, 0);
        }
    }

    void UpdatePlacementStructureMaterial()
    {
        MeshRenderer placementPrefabRenderer = GetCurrentStructure().placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().meshRenderer;
    
        if(inPlace && canBuild)
        {
            placementPrefabRenderer.material = blueMaterial;
        } else
        {
            placementPrefabRenderer.material = redMaterial;
        }
    }

    void CheckPosition()
    {
        inPlace = GetCurrentStructure().placementPrefab.transform.position == finalPosition;

        if(!inPlace)
        {
            SetPosition(finalPosition);
        }
    }

    void SetPosition(Vector3 targetPosition)
    {
        Transform placementPrefabTransform = GetCurrentStructure().placementPrefab.transform;
        Vector3 positionVelocity = Vector3.zero;
        Vector3 newTargetPosition = Vector3.SmoothDamp(placementPrefabTransform.position, targetPosition, ref positionVelocity, 0, 15000);
        placementPrefabTransform.position = newTargetPosition;
    }

    Vector3 GetNearestPoint(Vector3 referencePoint)
    {
        return grid.GetNearestPointOnGrid(referencePoint);
    }

    void ChangeStructureType(StructureType newType)
    {
        currentStructureType = newType;

        foreach (var structure in structures)
        {
            structure.placementPrefab.SetActive(structure.structureType == newType);
        }
    }

    private Structure GetCurrentStructure()
    {
        return structures.Where(elem => elem.structureType == currentStructureType).FirstOrDefault();
    }
}

[System.Serializable]
public class Structure
{
    public GameObject placementPrefab;
    public GameObject instantiatedPrefab;
    public StructureType structureType;
}

public enum StructureType
{
    Stairs,
    Wall,
    Floor
}