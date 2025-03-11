using System.Collections;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Animator ghostAnimator;
    private Animator playerAnimator;
    private PlayerController playerController;
    private void Awake()
    {
        ghostAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerAnimator = playerController.GetComponent<Animator>();
            StartCoroutine(SetupGhost());
        }
    }

    private IEnumerator SetupGhost()
    {
        if (ghostAnimator == null || playerAnimator == null)
        {
            yield break;
        }

        AnimatorStateInfo playerStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        float playbackTime = playerStateInfo.normalizedTime % 1.0f;

        gameObject.SetActive(true);
        ghostAnimator.enabled = true;

        AnimatorControllerParameter[] parameters = playerAnimator.parameters;
        foreach (AnimatorControllerParameter param in parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                ghostAnimator.SetBool(param.name, playerAnimator.GetBool(param.name));
            }
            else if (param.type == AnimatorControllerParameterType.Float)
            {
                ghostAnimator.SetFloat(param.name, playerAnimator.GetFloat(param.name));
            }
            else if (param.type == AnimatorControllerParameterType.Int)
            {
                ghostAnimator.SetInteger(param.name, playerAnimator.GetInteger(param.name));
            }
        }

        ghostAnimator.Play(playerStateInfo.fullPathHash, 0, playbackTime);
        ghostAnimator.Update(0);
        ghostAnimator.Update(0);
        ghostAnimator.enabled = false;
        StartFade(0.5f);
        yield return null;
    }

    public void StartFade(float fadeDuration)
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float fadeDuration)
    {
        float timer = 0f;
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Color[] initialColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            initialColors[i] = spriteRenderers[i].color;
        }

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null)
                {
                    Color c = initialColors[i];
                    c.a = Mathf.Lerp(initialColors[i].a, 0, t);
                    spriteRenderers[i].color = c;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
