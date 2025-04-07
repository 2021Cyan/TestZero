using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    // Attributes
    public float ActivationDistance;
    protected GameObject _player;

    protected Color bluePortal = new Color32(0x00, 0xC7, 0xFF, 0xFF); // Hexadecimal color 00C7FF
    protected Color redPortal = new Color32(0xFF, 0x62, 0x00, 0xFF); // Hexadecimal color FF6200
    protected Color yellowPortal = new Color32(0xFF, 0xF7, 0x00, 0xFF); // Hexadecimal color FFF820

    protected Gradient gradientBlue = new Gradient();
    protected Gradient gradientRed = new Gradient();

    private void Awake()
    {
        gradientBlue.SetKeys(
            new GradientColorKey[] { new GradientColorKey(bluePortal, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        gradientRed.SetKeys(
            new GradientColorKey[] { new GradientColorKey(redPortal, 0.0f), new GradientColorKey(yellowPortal, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
    }

    // Setters
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    // Behaviour
    public bool PlayerIsNear(Vector3 selfPosition)
    {
        if (_player != null)
        {
            return Vector3.Distance(_player.transform.position, selfPosition) < ActivationDistance;
        }
        else
        {
            return false;
        }
    }
    
    protected IEnumerator WaitFor(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
