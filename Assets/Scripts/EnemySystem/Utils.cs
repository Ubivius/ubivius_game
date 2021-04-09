using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    private static readonly Vector3 Vector3zero = Vector3.zero;
    private static readonly Vector3 Vector3one = Vector3.one;

    public const int sortingOrderDefault = 5000;

    // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
    public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
    {
        return (int)(baseSortingOrder - position.y) + offset;
    }


    // Get Main Canvas Transform
    private static Transform cachedCanvasTransform;
    public static Transform GetCanvasTransform()
    {
        if (cachedCanvasTransform == null)
        {
            Canvas canvas = MonoBehaviour.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                cachedCanvasTransform = canvas.transform;
            }
        }
        return cachedCanvasTransform;
    }

    // Get Default Unity Font, used in text objects if no font given
    public static Font GetDefaultFont()
    {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }


    // Create a Sprite in the World, no parent
    public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
    {
        return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
    }

    // Create a Sprite in the World
    public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.color = color;
        return gameObject;
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    // Draw a UI Sprite
    public static RectTransform DrawSprite(Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
    {
        RectTransform rectTransform = DrawSprite(null, color, parent, pos, size, name);
        return rectTransform;
    }

    // Draw a UI Sprite
    public static RectTransform DrawSprite(Sprite sprite, Transform parent, Vector2 pos, Vector2 size, string name = null)
    {
        RectTransform rectTransform = DrawSprite(sprite, Color.white, parent, pos, size, name);
        return rectTransform;
    }

    // Draw a UI Sprite
    public static RectTransform DrawSprite(Sprite sprite, Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
    {
        // Setup icon
        if (name == null || name == "") name = "Sprite";
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        RectTransform goRectTransform = go.GetComponent<RectTransform>();
        goRectTransform.SetParent(parent, false);
        goRectTransform.sizeDelta = size;
        goRectTransform.anchoredPosition = pos;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;

        return goRectTransform;
    }

    public static Text DrawTextUI(string textString, Vector2 anchoredPosition, int fontSize, Font font)
    {
        return DrawTextUI(textString, GetCanvasTransform(), anchoredPosition, fontSize, font);
    }

    public static Text DrawTextUI(string textString, Transform parent, Vector2 anchoredPosition, int fontSize, Font font)
    {
        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(parent, false);
        Transform textGoTrans = textGo.transform;
        textGoTrans.SetParent(parent, false);
        textGoTrans.localPosition = Vector3zero;
        textGoTrans.localScale = Vector3one;

        RectTransform textGoRectTransform = textGo.GetComponent<RectTransform>();
        textGoRectTransform.sizeDelta = new Vector2(0, 0);
        textGoRectTransform.anchoredPosition = anchoredPosition;

        Text text = textGo.GetComponent<Text>();
        text.text = textString;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleLeft;
        if (font == null) font = GetDefaultFont();
        text.font = font;
        text.fontSize = fontSize;

        return text;
    }


    // Parse a float, return default if failed
    public static float Parse_Float(string txt, float _default)
    {
        float f;
        if (!float.TryParse(txt, out f))
        {
            f = _default;
        }
        return f;
    }

    // Parse a int, return default if failed
    public static int Parse_Int(string txt, int _default)
    {
        int i;
        if (!int.TryParse(txt, out i))
        {
            i = _default;
        }
        return i;
    }
    public static int Parse_Int(string txt)
    {
        return Parse_Int(txt, -1);
    }

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    // Generate random normalized direction
    public static Vector2 GetRandomDir()
    {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    public static Vector3 GetVectorFromAngle(int angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static Vector3 GetVectorFromAngleInt(int angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static int GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        int angle = Mathf.RoundToInt(n);

        return angle;
    }

    public static int GetAngleFromVector180(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        int angle = Mathf.RoundToInt(n);

        return angle;
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
    {
        return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
