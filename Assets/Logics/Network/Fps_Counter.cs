using System.Collections;
using TMPro;
using UnityEngine;

public class Fps_Counter : MonoBehaviour
{
    [SerializeField] private TMP_Text _StatText; // ��������� ���� ��� ����������� ����������
    [SerializeField] private float updateTickRate = 1f; // �������� ���������� ����������

    private int fps;
    private int drawCalls;
    private int visibleRenderers;
    private int instancedDrawCalls;

    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] skinnedRenderers;

    private void Start()
    {
        // �������� ��������� �� ������
        meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        skinnedRenderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);

        StartCoroutine(StatUpdate(new WaitForSecondsRealtime(updateTickRate)));
    }

    private IEnumerator StatUpdate(WaitForSecondsRealtime timer)
    {
        double refreshRate = Screen.currentResolution.refreshRateRatio.value;

        while (true)
        {
            // ����� ��������
            fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            drawCalls = 0;
            visibleRenderers = 0;
            instancedDrawCalls = 0;

            // ������� ������� ���������
            foreach (var renderer in meshRenderers)
            {
                if (renderer == null || !renderer.isVisible) continue;
                visibleRenderers++;

                if (renderer.sharedMaterial != null && renderer.sharedMaterial.enableInstancing)
                {
                    instancedDrawCalls++;
                }
                else
                {
                    drawCalls++;
                }
            }

            foreach (var renderer in skinnedRenderers)
            {
                if (renderer == null || !renderer.isVisible) continue;
                visibleRenderers++;
                drawCalls++; // SkinnedMeshRenderers ������ ���������� ��������� draw call
            }

            // ���������� ������
            _StatText.text = $"FPS: {fps} | Hz: {refreshRate}\n" +
                             $"Visible Renderers: {visibleRenderers}\n" +
                             $"Draw Calls (approx.): {drawCalls}\n" +
                             $"Instanced Draw Calls: {instancedDrawCalls}";

            yield return timer;
        }
    }
}
