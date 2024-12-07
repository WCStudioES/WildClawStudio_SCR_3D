using UnityEngine;

public abstract class SpriteSheetVFXAnimation : MonoBehaviour
{
    public Texture2D spritesheet; // La spritesheet completa
    public int rows = 6;          // Número de filas de sprites en la spritesheet
    public int columns = 6;       // Número de columnas de sprites en la spritesheet
    public float fps = 60;        // Cuadros por segundo de la animación

    protected SpriteRenderer spriteRenderer; // Renderer del sprite
    protected Sprite[] sprites;             // Array para almacenar los sprites individuales
    protected float frameRate;              // Tiempo entre cuadros

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        frameRate = 1 / fps;

        // Dividir la spritesheet en sprites individuales
        sprites = GetSpritesFromSheet(spritesheet, rows, columns);

        InitializeAnimation();
    }

    protected abstract void InitializeAnimation();

    protected Sprite[] GetSpritesFromSheet(Texture2D sheet, int rows, int columns)
    {
        int totalSprites = rows * columns;
        Sprite[] sprites = new Sprite[totalSprites];
        int spriteWidth = sheet.width / columns;
        int spriteHeight = sheet.height / rows;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Rect rect = new Rect(x * spriteWidth, (rows - y - 1) * spriteHeight, spriteWidth, spriteHeight);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                sprites[y * columns + x] = Sprite.Create(sheet, rect, pivot);
            }
        }

        return sprites;
    }

    public virtual void Toggle(bool active)
    {
        gameObject.SetActive(active);
    }
}
