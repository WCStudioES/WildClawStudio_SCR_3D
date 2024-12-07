using UnityEngine;

public class SimpleVFXAnimation : SpriteSheetVFXAnimation
{
    private int currentFrame = 0;
    private float timer = 0f;

    protected override void InitializeAnimation()
    {
        // Reiniciar la animación cada vez que se cree el objeto
        currentFrame = 0;
        timer = 0f;
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
        if (currentFrame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[currentFrame];
            currentFrame++;
        }
        else
        {
            Destroy(gameObject); // Destruir el objeto al final de la animación
        }
    }
}
