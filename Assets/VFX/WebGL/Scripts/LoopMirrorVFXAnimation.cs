using UnityEngine;

public class LoopMirrorVFXAnimation : SpriteSheetVFXAnimation
{
    private int currentFrame = 0;
    private bool goingForward = true;
    private float timer = 0f;

    protected override void InitializeAnimation()
    {
        // Aquí no es necesario inicializar nada adicional en este caso.
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            UpdateFrame();
            timer = 0f;
        }
    }

    private void UpdateFrame()
    {
        if (goingForward)
        {
            currentFrame++;
            if (currentFrame >= sprites.Length)
            {
                currentFrame = sprites.Length - 2;
                goingForward = false;
            }
        }
        else
        {
            currentFrame--;
            if (currentFrame < 0)
            {
                currentFrame = 1;
                goingForward = true;
            }
        }

        spriteRenderer.sprite = sprites[currentFrame];
    }
}
