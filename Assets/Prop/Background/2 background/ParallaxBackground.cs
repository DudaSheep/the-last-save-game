using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    
    [Tooltip("0 = A Lua segue a câmera 100% (efeito de infinito). 1 = Fica travada no cenário.")]
    [Range(0f, 1f)]
    public float parallaxEffectMultiplier = 0f; 

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Calcula o quanto a câmera se moveu neste frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Se o multiplicador for 0, a lua se move EXATAMENTE junto com a câmera (efeito real)
        transform.position += new Vector3(deltaMovement.x * (1 - parallaxEffectMultiplier), deltaMovement.y * (1 - parallaxEffectMultiplier), 0);
        
        lastCameraPosition = cameraTransform.position;
    }
}