using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    
    [Tooltip("0 = O objeto segue a câmera 100% (efeito de infinito). 1 = Fica travada no cenário.")]
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
        
        float moveX = deltaMovement.x * (1 - parallaxEffectMultiplier);
        
        transform.position += new Vector3(moveX, 0, 0);
        
        lastCameraPosition = cameraTransform.position;
    }
}