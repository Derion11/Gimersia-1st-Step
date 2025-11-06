using System.Collections.Generic;
using UnityEngine;

public class gridPapan : MonoBehaviour
{
    [Header("Ukuran papan")]
    public int baris = 10;   // jumlah baris (misal 10)
    public int kolom = 10;   // jumlah kolom (misal 10)

    [Header("Layout posisi kotak")]
    public float ukuranKotak = 1f;          // jarak antar kotak
    public Vector3 origin = Vector3.zero;   // titik awal kotak pertama (pojok kiri bawah)

    [Header("Prefab kotak")]
    public GameObject prefabKotak;          // drag prefab gambar kotak ke sini di Inspector

    private List<GameObject> daftarKotak = new List<GameObject>(); // penyimpanan objek kotak di scene
    public List<Vector3> posisiKotak = new List<Vector3>();        // penyimpanan posisi tiap kotak (array posisi)

    [Tooltip("True = penomoran zigzag (baris genap kiri->kanan, baris ganjil kanan->kiri)")]
    public bool zigzag = true;

    [Tooltip("True = papan di sumbu XY (2D). False = papan XZ (3D top-down)")]
    public bool layout2D_XY = true;

    public Bidak player01;
    public Bidak player02;

    public bool SedangGerak;
    public int giliranPlayer;

    public List<Vector2Int> daftarUlar = new List<Vector2Int>();
    public List<Vector2Int> daftarTangga = new List<Vector2Int>();
    private readonly Dictionary<int, int> _petaTeleport = new Dictionary<int, int>();

    /// <summary>Jumlah total kotak (0-based terakhir = Count-1).</summary>
    public int LastIndex => Mathf.Max(0, posisiKotak.Count - 1);

    [Header("Garis Ular & Tangga")]
    public float garisOffset = 0.08f;   // supaya garis tidak ketimpa tile (XY: +Z, XZ: +Y)
    public float garisLebar = 0.05f;   // ketebalan garis
    public Material garisMaterial;      // opsional; jika kosong akan dibuat otomatis

    [Header("Nomor kotak (0-based, opsional)")]
    public bool tampilkanNomor = true;
    public float offsetNomor = 0.05f; // XY: geser +Z, XZ: geser +Y
    public int ukuranFont = 64;
    public Color warnaTeks = Color.grey;

    // simpan line renderer supaya bisa dibersihkan
    private readonly List<LineRenderer> _garisUlar = new List<LineRenderer>();
    private readonly List<LineRenderer> _garisTangga = new List<LineRenderer>();

    


    void Start()
    {
        BuatGrid();
        GambarGrid();

        if (player01 != null && player02 != null)
        {
            player01.SnapKeKotak();
            player02.SnapKeKotak();
        }

        listUlarDanTangga();
        BangunPetaUlarTangga();
        GambarGarisUlarTangga();
    }


    //fungsi untuk entry manual ular dan tangga

    void listUlarDanTangga()
    {
        Vector2Int newTangga;

        newTangga = new Vector2Int(1, 11);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(9, 40);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(65, 74);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(70, 81);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(43, 67);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(84, 97);
        daftarTangga.Add(newTangga);

        Vector2Int newUlar;

        newUlar = new Vector2Int(15, 4);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(23, 11);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(36, 24);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(67, 44);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(85, 67);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(99, 75);
        daftarUlar.Add(newUlar);

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
        }
    }

    /// <summary>
    /// Membuat daftar posisi semua kotak di papan.
    /// </summary>
    public void BuatGrid()
    {
        posisiKotak.Clear(); // bersihkan data lama

        
        for (int r = 0; r < baris; r++)
        {
            if (zigzag && (r % 2 == 1))
            {
                // baris ganjil: kanan -> kiri
                for (int c = kolom - 1; c >= 0; c--)
                {
                    Vector3 pos = layout2D_XY
                        ? origin + new Vector3(c * ukuranKotak, r * ukuranKotak, 0f)
                        : origin + new Vector3(c * ukuranKotak, 0f, r * ukuranKotak);

                    posisiKotak.Add(pos);
                }
            }
            else
            {
                // baris genap: kiri -> kanan
                for (int c = 0; c < kolom; c++)
                {
                    Vector3 pos = layout2D_XY
                        ? origin + new Vector3(c * ukuranKotak, r * ukuranKotak, 0f)
                        : origin + new Vector3(c * ukuranKotak, 0f, r * ukuranKotak);

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
            daftarKotak.Add(kotakBaru);

            if (tampilkanNomor)
            {
                var goTeks = new GameObject("Nomor");
                goTeks.transform.SetParent(kotakBaru.transform, false);

                goTeks.transform.localPosition = new Vector3(0f, 0f, offsetNomor); // majukan +Z


                var tm = goTeks.AddComponent<TextMesh>();
                tm.text = i.ToString();// tampilkan angka 0-based
                tm.fontSize = ukuranFont;
                tm.characterSize = 0.07f;
                tm.color = warnaTeks;
                tm.alignment = TextAlignment.Center;
                tm.anchor = TextAnchor.MiddleCenter;

                var mr = goTeks.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.sortingLayerName = "Default";
                    mr.sortingOrder = 100; // di atas tile
                }
            }
        }

        

    }


    /// <summary>Bangun peta ular & tangga (0-based). Ular harus menurun, tangga harus menaik.</summary>
    public void BangunPetaUlarTangga()
    {
        _petaTeleport.Clear();
        int last = posisiKotak.Count - 1;

        // ular: to < from
        foreach (var koordinatUlar in daftarUlar)
        {
            //pastikan ular harus valid dan turun
            if (koordinatUlar.x >= 0 && koordinatUlar.x <= last && koordinatUlar.y >= 0 && koordinatUlar.y <= last && koordinatUlar.y < koordinatUlar.x)
                _petaTeleport[koordinatUlar.x] = koordinatUlar.y;
            else
                Debug.LogWarning($"[Ular] Di-skip: {koordinatUlar.x}->{koordinatUlar.y} (harus valid & turun)");
        }

        // tangga: to > from
        foreach (var koordinatTangga in daftarTangga)
        {
            //pastikan tangganya valid
            if (koordinatTangga.x >= 0 && koordinatTangga.x <= last && koordinatTangga.y >= 0 && koordinatTangga.y <= last && koordinatTangga.y > koordinatTangga.x)
                _petaTeleport[koordinatTangga.x] = koordinatTangga.y;
            else
                Debug.LogWarning($"[Tangga] Di-skip: {koordinatTangga.x}->{koordinatTangga.y} (harus valid & naik)");
        }
    }

    /// <summary>Jika index ada di kepala ular / dasar tangga, kembalikan tujuan; jika tidak, kembalikan index asal.</summary>
    public int ApplyUlarTangga(int index0)
    {
        if (_petaTeleport.TryGetValue(index0, out int tujuan))
            return tujuan;
        return index0;
    }


    /// <summary>
    /// Mendapatkan posisi dunia untuk nomor kotak tertentu (0-based).
    /// </summary>
    public Vector3 GetPosisiKotak(int nomor)
    {
        nomor = Mathf.Clamp(nomor, 0, posisiKotak.Count);
        //Debug.Log("posisi kotak ke-" + nomor + " x: " + posisiKotak[nomor].x + " y: " + posisiKotak[nomor].y);
        return posisiKotak[nomor];
    }

    private Vector3 OffsetForLine(Vector3 p)
    {
        // XY: majukan +Z; XZ: naikkan +Y
        return layout2D_XY ? (p + new Vector3(0f, 0f, garisOffset))
                           : (p + new Vector3(0f, garisOffset, 0f));
    }

    private Material GetOrCreateLineMaterial()
    {
        if (garisMaterial != null) return garisMaterial;
        // pakai shader yang aman di 2D/3D tanpa urusan lighting
        var mat = new Material(Shader.Find("Sprites/Default"));
        return mat;
    }

    private LineRenderer BuatLine(string nama, Color cStart, Color cEnd)
    {
        var go = new GameObject(nama);
        go.transform.SetParent(transform, worldPositionStays: true);

        var lr = go.AddComponent<LineRenderer>();
        lr.material = GetOrCreateLineMaterial();
        lr.positionCount = 2;
        lr.startWidth = garisLebar;
        lr.endWidth = garisLebar;
        lr.useWorldSpace = true;

        // warna
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(cStart, 0f),
            new GradientColorKey(cEnd,   1f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
            }
        );
        lr.colorGradient = grad;

        // pastikan di atas tile
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 200;

        return lr;
    }

    public void GambarGarisUlarTangga()
    {
        // bersihkan lama
        foreach (var l in _garisUlar) if (l) Destroy(l.gameObject);
        foreach (var l in _garisTangga) if (l) Destroy(l.gameObject);
        _garisUlar.Clear();
        _garisTangga.Clear();

        int last = posisiKotak.Count - 1;

        // Tangga = hijau
        foreach (var tangga in daftarTangga)
        {
            //tangga harus valid (tidak diluar kotak, tidak minus dan tangga harus naik
            if (tangga.x < 0 || tangga.x > last || tangga.y < 0 || tangga.y > last || tangga.y <= tangga.x) continue;


            Vector3 awal = OffsetForLine(GetPosisiKotak(tangga.x));
            Vector3 akhir = OffsetForLine(GetPosisiKotak(tangga.y));

            var lr = BuatLine($"Tangga_{tangga.x}_to_{tangga.y}", Color.green, new Color(0.2f, 1f, 0.2f));
            lr.SetPosition(0, awal);
            lr.SetPosition(1, akhir);
            _garisTangga.Add(lr);
        }

        // Ular = merah
        foreach (var ular in daftarUlar)
        {
            //ular harus valid (tidak diluar kotak, tidak minus dan harus turun
            if (ular.x < 0 || ular.x > last || ular.y < 0 || ular.y > last || ular.y >= ular.x) continue;

            Vector3 a = OffsetForLine(GetPosisiKotak(ular.x));
            Vector3 b = OffsetForLine(GetPosisiKotak(ular.y));

            var lr = BuatLine($"Ular_{ular.x}_to_{ular.y}", new Color(1f, 0.2f, 0.2f), Color.red);
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
            _garisUlar.Add(lr);
        }
    }
}