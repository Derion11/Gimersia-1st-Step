using System.Collections;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("Referensi papan")]
    public gridBoard papan;      // drag object Board yang punya script gridPapan

    [Header("Posisi awal (0 = kotak pertama)")]
    public int nomorSaatIni = 0;

    public void StartGame()
    {
        // pastikan ada papan
        if (papan == null)
        {
            Debug.LogWarning("Pawn: field 'papan' belum di-assign. Drag object Board ke sini.");
            return;
        }

        // buat teks dirender di atas tile
        var mr = this.GetComponent<SpriteRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "Default"; // atau "UI" kalau kamu punya layer itu
            mr.sortingOrder = 101;           // lebih tinggi daripada kotak (biasanya 0)
        }

        // snap ke kotak awal
        SnapKeKotak();
    }

    /// <summary>
    /// Maju satu kotak (kalau belum sampai kotak terakhir).
    /// </summary>
    public void JalanSatuLangkah()
    {
        if (papan == null) return;

        int maxKotak = papan.posisiKotak.Count - 1;

        if (nomorSaatIni >= maxKotak)
        {
            Debug.Log("Pawn sudah di kotak terakhir.");
            return;
        }

        int target = nomorSaatIni + 1;

        nomorSaatIni = target;
        SnapKeKotak();
    }

    /// <summary>
    /// Lempar dadu 1–6, lalu jalan sesuai hasilnya.
    /// </summary>
    public void LemparDaduDanJalan()
    {
        if (papan == null || papan.SedangGerak) return;

        // angka acak antara 1 sampai 6 (inklusif)
        int langkah = Random.Range(1, 7);
        
        Debug.Log( "Hasil dadu: " + langkah);

        // panggil fungsi jalan
        JalanBeberapaLangkah(langkah);
    }

    /// <summary>
    /// Jalan maju/mundur 'jumlahLangkah' kotak, satu-per-satu, 0-based.
    /// Positive = forward, Negative = backward
    /// </summary>
    public void JalanBeberapaLangkah(int jumlahLangkah, bool fromCard = false)
    {
        if (papan == null || papan.SedangGerak || jumlahLangkah == 0) return;

        int targetIndex = nomorSaatIni + jumlahLangkah;
        
        // Clamp to valid range [0, LastIndex]
        targetIndex = Mathf.Clamp(targetIndex, 0, papan.LastIndex);
        
        StartCoroutine(GerakStepByStep(targetIndex, fromCard));
    }

    /// <summary>
    /// Korutin: melangkah satu kotak demi satu kotak sampai targetIndex (forward or backward).
    /// </summary>
    private IEnumerator GerakStepByStep(int targetIndex, bool fromCard = false)
    {
        papan.SedangGerak = true;

        // Handle both forward and backward movement
        while (nomorSaatIni != targetIndex)
        {
            // Determine direction: +1 for forward, -1 for backward
            int direction = targetIndex > nomorSaatIni ? 1 : -1;
            int berikutnya = nomorSaatIni + direction;
            
            Vector3 tujuan = papan.GetPosisiKotak(berikutnya);

            // Move to next position
            transform.position = tujuan;

            if (AudioManaging.Instance != null)
            {
                AudioManaging.Instance.PlaySFX("step");
            }

            // Wait before next step
            yield return new WaitForSeconds(0.5f);

            nomorSaatIni = berikutnya;
        }
        

        // === Cek ular & tangga setelah berhenti (0-based) ===
        int sebelum = nomorSaatIni;
        int sesudah = papan.ApplyUlarTangga(sebelum);

        if (sesudah != sebelum)
        {
            Vector3 dari = transform.position;
            Vector3 ke = papan.GetPosisiKotak(sesudah);

            float t2 = 0f;
            float dur2 = 0.25f; // animasi singkat meluncur
            while (t2 < 1f)
            {
                t2 += Time.deltaTime / dur2;
                transform.position = Vector3.Lerp(dari, ke, t2);
                yield return null;
            }

            nomorSaatIni = sesudah;
        }

        // === NEW: Cek random card setelah ular & tangga (but not if this movement was from a card) ===
        if (!fromCard && papan.cardManager != null)
        {
            RandomCard card = papan.cardManager.GetCardAtPosition(nomorSaatIni);
            if (card != null)
            {
                papan.SedangGerak = true;  // Keep game paused
                papan.cardManager.OnCardActivated(this, card);
                yield break;  // Stop here, UI will handle the rest
            }
        }

        papan.SedangGerak = false;

        if (papan.giliranPlayer == 0)
        {
            papan.giliranPlayer = 1;
        }
        else
        {
            papan.giliranPlayer = 0;
        }
    }

    /// <summary>
    /// Pindahkan bidak langsung ke nomor kotak saat ini.
    /// </summary>

    public void SnapKeKotak()
    {
        if (papan == null) return;

        Vector3 pos = papan.GetPosisiKotak(nomorSaatIni);

        transform.position = pos;
        transform.SetParent(papan.transform); // rapikan hierarchy
    }
}