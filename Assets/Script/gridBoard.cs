using System.Collections.Generic;
using UnityEngine;

public class gridBoard : MonoBehaviour
{
    [Header("Ukuran papan")]
    public int baris = 15;  // jumlah baris (misal 10)
    public int kolom = 15;   // jumlah kolom (misal 10)

    [Header("Layout posisi kotak")]
    public float ukuranKotak = 1f;          // jarak antar kotak
    public Vector3 origin = Vector3.zero;   // titik awal kotak pertama (pojok kiri bawah)

    [Header("Prefab kotak")]
    public GameObject prefabKotak;          // drag prefab gambar kotak ke sini di Inspector

    private List<GameObject> daftarKotak = new List<GameObject>(); // penyimpanan objek kotak di scene
    public List<Vector3> posisiKotak = new List<Vector3>();        // penyimpanan posisi tiap kotak (array posisi)


    void Start()
    {
        BuatGrid();
        GambarGrid();
    }

    /// <summary>
    /// Membuat daftar posisi semua kotak di papan.
    /// </summary>
    public void BuatGrid()
    {
        posisiKotak.Clear(); // bersihkan data lama

        for (int bar = 0; bar < baris; bar++)
        {
            for (int kol = 0; kol < kolom; kol++)
            {
                // hitung posisi kotak berdasarkan baris & kolom
                Vector3 pos = origin + new Vector3(kol * ukuranKotak, bar * ukuranKotak, 0f);
                posisiKotak.Add(pos);
            }
        }
    }

    /// <summary>
    /// Menggambar grid di scene menggunakan prefab.
    /// </summary>
    public void GambarGrid()
    {
        // Hapus kotak lama (jika sudah ada)
        foreach (var kotak in daftarKotak)
        {
            if (kotak != null)
            {
                Destroy(kotak);
            } 
        }
            
        daftarKotak.Clear();

        // Buat kotak baru di setiap posisi
        for (int i = 0; i < posisiKotak.Count; i++)
        {
            if (prefabKotak == null)
            {
                Debug.LogWarning("Prefab kotak belum diisi di Inspector!");
                return;
            }

            GameObject kotakBaru = Instantiate(prefabKotak, posisiKotak[i], Quaternion.identity, transform);
            kotakBaru.name = $"Kotak_" + i; // beri nama urutan 0, 1, 2, 3...
            daftarKotak.Add(kotakBaru);
        }
    }

    /// <summary>
    /// Mendapatkan posisi dunia untuk nomor kotak tertentu (0-based).
    /// </summary>
    public Vector3 GetPosisiKotak(int nomor)
    {
        nomor = Mathf.Clamp(nomor, 0, posisiKotak.Count);
        return posisiKotak[nomor];
    }
}