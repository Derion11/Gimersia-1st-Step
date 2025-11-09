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
            LemparDaduDanJalan();
            // JalanSatuLangkah();
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
    /// Jalan maju beberapa langkah, melewati setiap grid satu-per-satu.
    /// </summary>
    public void JalanBeberapaLangkah(int jumlahLangkah)
    {
        if (papan == null || jumlahLangkah <= 0 || papan.SedangGerak) return;

        int totalKotak = papan.posisiKotak.Count;
        int target = Mathf.Min(nomorSaatIni + jumlahLangkah, totalKotak);

        StartCoroutine(GerakStepByStep(target));
    }

    /// <summary>
    /// Korutin: melangkah satu kotak demi satu kotak sampai targetNomor.
    /// </summary>
    private IEnumerator GerakStepByStep(int targetNomor)
    {
        papan.SedangGerak = true;

        // Melangkah dari (nomorSaatIni+1) sampai targetNomor
        while (nomorSaatIni < targetNomor)
        {
            int berikutnya = nomorSaatIni + 1;
            Vector3 start = transform.position;
            Vector3 tujuan = papan.GetPosisiKotak(berikutnya);

            // tanpa animasi, tapi tetap satu-per-satu
            transform.position = tujuan;
            // beri 1 frame jeda biar terlihat “bertahap”
            yield return null;

            nomorSaatIni = berikutnya;
        }

        papan.SedangGerak = false;
    }
}