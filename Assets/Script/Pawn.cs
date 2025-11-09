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

    void Update()
    {
        // contoh kontrol sederhana: tekan Spasi untuk maju 1 langkah
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JalanSatuLangkah();
        }
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