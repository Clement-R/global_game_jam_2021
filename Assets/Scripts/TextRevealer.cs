using System.Collections;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

using DG.Tweening;

public class TextRevealer : MonoBehaviour
{
    [UnityEngine.Header("Configuration")]
    public int numCharactersFade = 3;
    public float charsPerSecond = 30;
    public float smoothSeconds = 0.75f;
    [UnityEngine.Header("References")]
    public TextMeshProUGUI Text;
    public UnityEvent allRevealed = new UnityEvent();

    private string originalString;
    private int nRevealedCharacters;
    private bool isRevealing = false;

    public bool IsRevealing
    {
        get
        {
            return isRevealing;
        }
    }

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        originalString = Text.text;
        if (string.IsNullOrEmpty(originalString))
            RestartWithText(Text.text);
    }

    public void Hide()
    {
        Text.text = BuildPartiallyRevealedString(originalString, keyCharIndex: -1, minIndex : 0, maxIndex : 0, fadeLength : 1);
        Text.alpha = 0; // Gros fix crado
    }

    public void RestartWithText(string strText)
    {
        isRevealing = false;
        nRevealedCharacters = 0;
        Text.alpha = 1; // Gros fix crado
        originalString = strText;
        Text.text = BuildPartiallyRevealedString(originalString, keyCharIndex: -1, minIndex : 0, maxIndex : 0, fadeLength : 1);
    }

    public void SetText(string p_text)
    {
        originalString = p_text;
    }

    public void Restart()
    {
        isRevealing = false;
        nRevealedCharacters = 0;
        Text.alpha = 1; // Gros fix crado
        Text.text = BuildPartiallyRevealedString(originalString, keyCharIndex: -1, minIndex : 0, maxIndex : 0, fadeLength : 1);
        RevealNextParagraphAsync();
    }

    public void ShowEverythingWithoutAnimation()
    {
        StopAllCoroutines();

        Text.text = originalString;
        nRevealedCharacters = originalString.Length;
        Text.alpha = 1; // Gros fix crado
        isRevealing = false;

        allRevealed.Invoke();
    }

    public void ShowNextParagraphWithoutAnimation()
    {
        if (IsAllRevealed()) return;

        StopAllCoroutines();

        var paragraphEnd = GetNextParagraphEnd(nRevealedCharacters);
        Text.text = BuildPartiallyRevealedString(original: originalString,
            keyCharIndex: paragraphEnd,
            minIndex: nRevealedCharacters,
            maxIndex: paragraphEnd,
            fadeLength: 0);

        nRevealedCharacters = paragraphEnd + 1;
        while (nRevealedCharacters < originalString.Length && originalString[nRevealedCharacters] == '\n')
            nRevealedCharacters += 1;

        if (IsAllRevealed())
            allRevealed.Invoke();

        isRevealing = false;
    }

    public void RevealNextParagraphAsync()
    {
        StartCoroutine(RevealNextParagraph());
    }
    public IEnumerator RevealNextParagraph()
    {
        if (IsAllRevealed() || isRevealing) yield break;

        var paragraphEnd = GetNextParagraphEnd(nRevealedCharacters);
        if (paragraphEnd < 0) yield break;

        isRevealing = true;

        var keyChar = (float) (nRevealedCharacters - numCharactersFade);
        var keyCharEnd = paragraphEnd;
        var speed = 0f;
        var secondsElapsed = 0f;

        while (keyChar < keyCharEnd)
        {
            secondsElapsed += Time.deltaTime;
            if (secondsElapsed <= smoothSeconds)
                speed = Mathf.Lerp(0f, charsPerSecond, secondsElapsed / smoothSeconds);
            else
            {
                var secondsLeft = (keyCharEnd - keyChar) / charsPerSecond;
                if (secondsLeft < smoothSeconds)
                    speed = Mathf.Lerp(charsPerSecond, 0.1f * charsPerSecond, 1f - secondsLeft / smoothSeconds);
            }

            keyChar = Mathf.MoveTowards(keyChar, keyCharEnd, speed * Time.deltaTime);
            Text.text = BuildPartiallyRevealedString(original: originalString,
                keyCharIndex: keyChar,
                minIndex: nRevealedCharacters,
                maxIndex: paragraphEnd,
                fadeLength: numCharactersFade);

            yield return null;
        }

        nRevealedCharacters = paragraphEnd + 1;

        while (nRevealedCharacters < originalString.Length && originalString[nRevealedCharacters] == '\n')
            nRevealedCharacters += 1;

        if (IsAllRevealed())
            allRevealed.Invoke();

        isRevealing = false;
    }

    public bool IsAllRevealed()
    {
        return nRevealedCharacters >= originalString.Length;
    }

    private int GetNextParagraphEnd(int startingFrom)
    {
        var paragraphEnd = originalString.IndexOf('\n', startingFrom);
        if (paragraphEnd < 0 && startingFrom < originalString.Length) paragraphEnd = originalString.Length - 1;
        return paragraphEnd;
    }

    private string BuildPartiallyRevealedString(string original, float keyCharIndex, int minIndex, int maxIndex, int fadeLength)
    {
        Text.alpha = 1; // Gros fix crado
        var lastFullyVisibleChar = Mathf.Max(Mathf.CeilToInt(keyCharIndex), minIndex - 1);
        var firstFullyInvisibleChar = (int) Mathf.Min(keyCharIndex + fadeLength, maxIndex) + 1;

        var revealed = original.Substring(0, lastFullyVisibleChar + 1);
        var unrevealed = original.Substring(firstFullyInvisibleChar);

        var sb = new StringBuilder();
        sb.Append(revealed);

        for (var i = lastFullyVisibleChar + 1; i < firstFullyInvisibleChar; ++i)
        {
            var c = original[i];
            var originalColorRGB = ColorUtility.ToHtmlStringRGB(Text.color);
            var alpha = Mathf.RoundToInt(255 * (keyCharIndex - i) / (float) fadeLength);
            sb.AppendFormat("<color=#{0}{1:X2}>{2}</color>", originalColorRGB, (byte) alpha, c);
        }

        sb.AppendFormat("<color=#00000000>{0}</color>", unrevealed);
        return sb.ToString();
    }

}