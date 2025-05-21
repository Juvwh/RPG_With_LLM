using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dice3D : MonoBehaviour
{
    #region Variables
    [Header("Dice Info")] 
    public int _nbr_faces = 6;  // Nombre de faces du d�
    public Transform[] dieFaces;  // Tableau des faces du d� (Transform de chaque face)
    private DiceManager _diceManager;

    [Header("Rigibody")]
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Camera mainCamera;  // La cam�ra principale
    public int dragMinus = 2;
    public float timeByDragMinus = 0.3f;

    private Vector3 diceVelocity;
    public float speedMin = 500;
    public float speedMax = 1500f;
    public float speedTorque = 1500f;

    [Header ("Debug")]
    public float normalLength = 1f;
    public float rayLength = 1f;  // Longueur du rayon � dessiner

    [Header("Targets Position")]
    public Vector3 targetPosition;
    [Header("Targets Rotation")]
    public Vector3[] targetRotation;
    public Vector3 startRotation;

    [Header("UI")]
    public GameObject m_Dice;

    private int _lastResult;
    #endregion
    #region Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _diceManager = FindFirstObjectByType<DiceManager>();
        mainCamera = _diceManager.mainCamera;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        m_Dice = this.gameObject;
        boxCollider = GetComponent<BoxCollider>();
    }
    public int GetResult()
    {
        return _lastResult;
    }
    public void ResetDice(bool showDice, Vector3 startPos)
    {
        _lastResult = -1;
        m_Dice.SetActive(showDice);
        targetPosition = startPos;
        transform.localPosition = startPos;
        Vector3 _startRotation = new Vector3(Random.Range(-180,180), Random.Range(-180, 180), Random.Range(-180, 180));
        transform.localEulerAngles = _startRotation;
    }
    public void RollDice()
    {
        rb.constraints = RigidbodyConstraints.None;

        // G�n�rer un torque al�atoire puissant dans toutes les directions
        Vector3 randomTorque = Random.onUnitSphere * speedTorque;

        // Appliquer une force de lancer dans une direction al�atoire mais avec un minimum de contr�le
        Vector3 throwDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f), // Toujours vers le haut pour �viter un lancer trop plat
            Random.Range(-1f, 1f)
        ).normalized * speedTorque;

        // Appliquer les forces et rotations
        rb.AddForce(throwDirection, ForceMode.Impulse);
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        // D�marrer les ralentissements apr�s un court instant
        StartCoroutine(AddAngularDrag(dragMinus));
        StartCoroutine(StopDice(0));
    }
    IEnumerator AddAngularDrag(int angularDrag)
    {
        yield return new WaitForSeconds(timeByDragMinus);
        rb.angularDrag = angularDrag;
        rb.drag = angularDrag;
        if(angularDrag < 50)
        {
            StartCoroutine(AddAngularDrag(angularDrag + angularDrag));
        }
    }
    IEnumerator StopDice(int count)
    {
        yield return new WaitForSeconds(0.75f);
        if(count == 2)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            boxCollider.enabled = false;
            GetFaceOrientation();
        }
        else
        {
            StartCoroutine(StopDice(count + 1));
        }
    }
    void GetFaceOrientation()
    {
        Transform _face = null;
        float maxDotProduct = float.MinValue;

        foreach (var face in dieFaces)
        {
            // Direction de la cam�ra vers la face
            Vector3 directionToCamera = mainCamera.transform.position - face.position;

            // Normal de la face (supposons que chaque face a une normale correctement d�finie)
            Vector3 faceNormal = face.up;  // Si la normale de la face est vers le haut, sinon adapte en fonction de l'orientation

            // Calcul du produit scalaire
            float dotProduct = Vector3.Dot(faceNormal, directionToCamera.normalized);

            // On garde la face avec le produit scalaire le plus �lev�
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                _face = face;
            }
        }

        int valueFace = int.Parse(_face.name);
        _lastResult = valueFace;

        
        // Affiche ou utilise la face qui est la plus orient�e vers la cam�ra
        if (_face != null)
        {
            Vector3 _targetRotation = targetRotation[valueFace - 1];
            transform.DOLocalMove(targetPosition, 1.0f);
            transform.DOLocalRotate(_targetRotation, 1.0f);
        }
        //StartCoroutine(waitBeforeReturn());
        StartCoroutine(waitDisplay());
    }
    IEnumerator waitBeforeReturn()
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(m_Dice.gameObject);
    }  
    IEnumerator waitDisplay()
    {
        yield return new WaitForSeconds(0.01f);
        _diceManager.DisplayResult();
    }
    #endregion

    //void OnDrawGizmos()
    //{
    //    foreach (var face in dieFaces)
    //    {
    //        // Position de l'objet vide
    //        Vector3 position = face.position;

    //        // Direction de l'axe "up" (la normale)
    //        Vector3 direction = face.up;  // L'axe "up" de l'objet repr�sente la normale

    //        // Dessiner le rayon depuis la position de l'objet dans la direction de la normale
    //        Debug.DrawRay(position, direction * rayLength, Color.green);
    //        Debug.DrawRay(position, -direction * rayLength, Color.red);

    //    }

    //    Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
    //    // R�cup�rer le maillage de l'objet
    //    Vector3[] vertices = mesh.vertices;  // Obtenir les vertices du maillage
    //    Vector3[] normals = mesh.normals;  // Obtenir les normales des vertices

    //    // Dessiner les normales pour chaque vertex
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        // Calculer la position mondiale du vertex
    //        Vector3 worldVertex = transform.TransformPoint(vertices[i]);

    //        // Calculer la direction de la normale (dans l'espace mondial)
    //        Vector3 worldNormal = transform.TransformDirection(normals[i]);

    //        // Dessiner un rayon pour la normale
    //        Gizmos.color = Color.green;  // Choisir la couleur de la normale
    //        Gizmos.DrawRay(worldVertex, worldNormal * normalLength);
    //    }
    //}
}
