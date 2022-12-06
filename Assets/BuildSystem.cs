using UnityEngine;
using System.Linq;

public class BuildSystem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Structure[] structures;

    [SerializeField]
    private Material blueMaterial;

    [SerializeField]
    private Material redMaterial;

    [SerializeField]
    private Transform rotationRef;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip buildingSound;

    [Header("UI References")]
    [SerializeField]
    private Transform buildSystemUIPanel;

    [SerializeField]
    private GameObject buildingRequiredElement;

    private Structure currentStructure;
    private bool inPlace;
    private bool canBuild;
    private Vector3 finalPosition;
    private bool systemEnabled = false;

    private void Awake()
    {
        currentStructure = structures.First();
        DisableSystem();
    }

    private void FixedUpdate()
    {
        if(!systemEnabled)
        {
            return;
        }

        canBuild = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
        finalPosition = grid.GetNearestPointOnGrid(transform.position);
        CheckPosition();
        RoundPlacementStructureRotation();
        UpdatePlacementStructureMaterial();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(currentStructure.structureType == StructureType.Stairs && systemEnabled)
            {
                DisableSystem();
            } else
            {
                ChangeStructureType(GetStructureByType(StructureType.Stairs));
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentStructure.structureType == StructureType.Wall && systemEnabled)
            {
                DisableSystem();
            }
            else
            {
                ChangeStructureType(GetStructureByType(StructureType.Wall));
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentStructure.structureType == StructureType.Floor && systemEnabled)
            {
                DisableSystem();
            }
            else
            {
                ChangeStructureType(GetStructureByType(StructureType.Floor));
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && canBuild && inPlace && systemEnabled && HasAllRessources())
        {
            BuildStructure();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateStructure();
        }
    }

    void BuildStructure()
    {
        Instantiate(currentStructure.instantiatedPrefab, currentStructure.placementPrefab.transform.position, currentStructure.placementPrefab.transform.GetChild(0).transform.rotation);

        audioSource.PlayOneShot(buildingSound);

        for (int i = 0; i < currentStructure.ressourcesCost.Length; i++)
        {
            for (int y = 0; y < currentStructure.ressourcesCost[i].count; y++)
            {
                Inventory.instance.RemoveItem(currentStructure.ressourcesCost[i].itemData);
            }
        }
    }

    bool HasAllRessources()
    {
        BuildingRequiredElement[] requiredElements = GameObject.FindObjectsOfType<BuildingRequiredElement>();

        return requiredElements.All(requiredElement => requiredElement.hasRessource);
    }

    void DisableSystem()
    {
        systemEnabled = false;

        buildSystemUIPanel.gameObject.SetActive(false);

        currentStructure.placementPrefab.SetActive(false);
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

        currentStructure.placementPrefab.transform.rotation = Quaternion.Euler(0, roundedRotation, 0);
    }

    void RotateStructure()
    {
        if(currentStructure.structureType != StructureType.Wall)
        {
            currentStructure.placementPrefab.transform.GetChild(0).transform.Rotate(0, 90, 0);
        }
    }

    void UpdatePlacementStructureMaterial()
    {
        MeshRenderer placementPrefabRenderer = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().meshRenderer;
    
        if(inPlace && canBuild && HasAllRessources())
        {
            placementPrefabRenderer.material = blueMaterial;
        } else
        {
            placementPrefabRenderer.material = redMaterial;
        }
    }

    void CheckPosition()
    {
        inPlace = currentStructure.placementPrefab.transform.position == finalPosition;

        if(!inPlace)
        {
            SetPosition(finalPosition);
        }
    }

    void SetPosition(Vector3 targetPosition)
    {
        Transform placementPrefabTransform = currentStructure.placementPrefab.transform;
        Vector3 positionVelocity = Vector3.zero;

        if(Vector3.Distance(placementPrefabTransform.position, targetPosition) > 10)
        {
            placementPrefabTransform.position = targetPosition;
        } else
        {
            Vector3 newTargetPosition = Vector3.SmoothDamp(placementPrefabTransform.position, targetPosition, ref positionVelocity, 0, 15000);
            placementPrefabTransform.position = newTargetPosition;
        }
    }

    void ChangeStructureType(Structure newStructure)
    {
        buildSystemUIPanel.gameObject.SetActive(true);

        systemEnabled = true;

        currentStructure = newStructure;

        foreach (var structure in structures)
        {
            structure.placementPrefab.SetActive(structure.structureType == currentStructure.structureType);
        }

        UpdateDisplayedCosts();
    }

    private Structure GetStructureByType(StructureType structureType)
    {
        return structures.Where(elem => elem.structureType == structureType).FirstOrDefault();
    }

    public void UpdateDisplayedCosts()
    {
        // Vider les coûts à l'écran
        foreach (Transform child in buildSystemUIPanel)
        {
            Destroy(child.gameObject);
        }

        // Ajouter les coûts de la structure à placer
        foreach (ItemInInventory requiredRessource in currentStructure.ressourcesCost)
        {
            GameObject requiredElementGO = Instantiate(buildingRequiredElement, buildSystemUIPanel);
            requiredElementGO.GetComponent<BuildingRequiredElement>().Setup(requiredRessource);
        }
    }
}

[System.Serializable]
public class Structure
{
    public GameObject placementPrefab;
    public GameObject instantiatedPrefab;
    public StructureType structureType;
    public ItemInInventory[] ressourcesCost;
}

public enum StructureType
{
    Stairs,
    Wall,
    Floor
}