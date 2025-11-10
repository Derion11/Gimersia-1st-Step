using System.Collections.Generic;
using UnityEngine;

public class gridBoard : MonoBehaviour
{
    [Header("Ukuran papan")]
    public int baris = 10;  // jumlah baris (misal 10)
    public int kolom = 10;   // jumlah kolom (misal 10)

    [Header("Layout posisi kotak")]
    public float ukuranKotak = 1f;          // jarak antar kotak
    public Vector3 origin = Vector3.zero;   // titik awal kotak pertama (pojok kiri bawah)

    [Header("Prefab kotak")]
    public GameObject prefabKotak;          // drag prefab gambar kotak ke sini di Inspector

    private List<GameObject> daftarKotak = new List<GameObject>(); // penyimpanan objek kotak di scene
    public List<Vector3> posisiKotak = new List<Vector3>();        // penyimpanan posisi tiap kotak (array posisi)

    [Header("Referensi Pemain")]
    public Pawn player01;
    public Pawn player02;

    public bool SedangGerak;
    public int giliranPlayer;

    void Start()
    {
        BuatGrid();
        GambarGrid();
        if (player01 != null && player02 != null)
        {
            player01.SnapKeKotak();
            player02.SnapKeKotak();
        }
    }

    void Update()
    {
        // contoh kontrol sederhana: tekan Spasi untuk maju 1 langkah
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player01 != null && player02 != null)
            {
                if (giliranPlayer == 0)
                    {
                    player01.LemparDaduDanJalan();
                    }
                else
                    {
                    player02.LemparDaduDanJalan();
                    }  
            }
            // JalanSatuLangkah();
        }
    }

    /// <summary>
    /// Membuat daftar posisi semua kotak di papan.
    /// </summary>
    public void BuatGrid()
    {
        posisiKotak.Clear();

        for (int r = 0; r < baris; r++)
        {
            // jika baris genap: isi dari kiri ke kanan
            if (r % 2 == 0)
            {
                for (int c = 0; c < kolom; c++)
                {
                    Vector3 pos = origin + new Vector3(c * ukuranKotak, r * ukuranKotak, 0f);
                    posisiKotak.Add(pos);
                }
            }
            // jika baris ganjil: isi dari kanan ke kiri
            else
            {
                for (int c = kolom - 1; c >= 0; c--)
                {
                    Vector3 pos = origin + new Vector3(c * ukuranKotak, r * ukuranKotak, 0f);
                    posisiKotak.Add(pos);
                }
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
            
            // Tambahkan gradient warna
            float t = (float)i / (posisiKotak.Count - 1); // nilai 0 sampai 1
            daftarKotak.Add(kotakBaru);
        }
    }

    /// <summary>
    /// Mendapatkan posisi dunia untuk nomor kotak tertentu (0-based).
    /// </summary>
    public Vector3 GetPosisiKotak(int nomor)
    {
        nomor = Mathf.Clamp(nomor, 0, posisiKotak.Count - 1);
        return posisiKotak[nomor];
    }
}