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
    public bool layout2D_XY = true; // true = XY plane (2D), false = XZ plane (3D)

    [Header("Prefab kotak")]
    public GameObject prefabKotak;          // drag prefab gambar kotak ke sini di Inspector

    private List<GameObject> daftarKotak = new List<GameObject>(); // penyimpanan objek kotak di scene
    public List<Vector3> posisiKotak = new List<Vector3>();        // penyimpanan posisi tiap kotak (array posisi)

    [Header("Referensi Pemain")]
    public Pawn player01;
    public Pawn player02;

    [Header("Ular & Tangga (0-based index)")]
    public List<Vector2Int> daftarUlar = new List<Vector2Int>();
    public List<Vector2Int> daftarTangga = new List<Vector2Int>();
    private readonly Dictionary<int, int> _petaTeleport = new Dictionary<int, int>();

    [Header("Garis Ular & Tangga")]
    public float garisOffset = 0.08f;   // supaya garis tidak ketimpa tile (XY: +Z, XZ: +Y)
    public float garisLebar = 0.05f;   // ketebalan garis
    public Material garisMaterial;      // opsional; jika kosong akan dibuat otomatis

    // simpan line renderer supaya bisa dibersihkan
    private readonly List<LineRenderer> _garisUlar = new List<LineRenderer>();
    private readonly List<LineRenderer> _garisTangga = new List<LineRenderer>();

    [Header("Random Card System")]
    public RandomCardManager cardManager;

    [Header("Dice Animation")]
    public DiceAnimator diceAnimator;

    [Header("Game End")]
    public GameEndUI gameEndUI;

    public bool SedangGerak;
    public int giliranPlayer;
    public bool gameEnded = false;
    public int LastIndex => posisiKotak.Count - 1;

    void Start()
    {
        listUlarDanTangga();
        BuatGrid();
        GambarGrid();
        BangunPetaUlarTangga();
        GambarGarisUlarTangga();
        
        // Spawn random cards after board is ready
        if (cardManager != null)
        {
            cardManager.SpawnRandomCards();
        }
        
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

    /// <summary>Bangun peta ular tangga (0-based). Ular harus menurun, tangga harus menaik.</summary>
    public void BangunPetaUlarTangga()
    {
        _petaTeleport.Clear();
        int last = posisiKotak.Count - 1;

        // ular: to < from
        foreach (var it in daftarUlar)
        {
            if (it.x >= 0 && it.x <= last && it.y >= 0 && it.y <= last && it.y < it.x)
                _petaTeleport[it.x] = it.y;
            else
                Debug.LogWarning($"[Ular] Di-skip: {it.x}->{it.y} (harus valid & turun)");
        }

        // tangga: to > from
        foreach (var it in daftarTangga)
        {
            if (it.x >= 0 && it.x <= last && it.y >= 0 && it.y <= last && it.y > it.x)
                _petaTeleport[it.x] = it.y;
            else
                Debug.LogWarning($"[Tangga] Di-skip: {it.x}->{it.y} (harus valid & naik)");
        }
    }

    /// <summary>Jika index ada di kepala ular / dasar tangga, kembalikan tujuan; jika tidak, kembalikan index asal.</summary>
    public int ApplyUlarTangga(int index0)
    {
        if (_petaTeleport.TryGetValue(index0, out int tujuan))
            return tujuan;
        return index0;
    }

    //fungsi untuk entry manual ular dan tangga

    void listUlarDanTangga()
    {
        Vector2Int newTangga;

        newTangga = new Vector2Int(1, 11);
        daftarTangga.Add(newTangga);

        newTangga = new Vector2Int(6, 40);
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

        newUlar = new Vector2Int(17, 4);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(23, 11);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(36, 24);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(67, 44);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(85, 67);
        daftarUlar.Add(newUlar);

        newUlar = new Vector2Int(98, 75);
        daftarUlar.Add(newUlar);

    }

    /// <summary>
    /// Mendapatkan posisi dunia untuk nomor kotak tertentu (0-based).
    /// </summary>
    public Vector3 GetPosisiKotak(int nomor)
    {
        nomor = Mathf.Clamp(nomor, 0, posisiKotak.Count - 1);
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
        foreach (var t in daftarTangga)
        {
            if (t.x < 0 || t.x > last || t.y < 0 || t.y > last || t.y <= t.x) continue;

            Vector3 a = OffsetForLine(GetPosisiKotak(t.x));
            Vector3 b = OffsetForLine(GetPosisiKotak(t.y));

            var lr = BuatLine($"Tangga_{t.x}_to_{t.y}", Color.green, new Color(0.2f, 1f, 0.2f));
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
            _garisTangga.Add(lr);
        }

        // Ular = merah
        foreach (var s in daftarUlar)
        {
            if (s.x < 0 || s.x > last || s.y < 0 || s.y > last || s.y >= s.x) continue;

            Vector3 a = OffsetForLine(GetPosisiKotak(s.x));
            Vector3 b = OffsetForLine(GetPosisiKotak(s.y));

            var lr = BuatLine($"Ular_{s.x}_to_{s.y}", new Color(1f, 0.2f, 0.2f), Color.red);
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
            _garisUlar.Add(lr);
        }
    }

    /// <summary>
    /// Ends the game and shows the winner UI
    /// </summary>
    /// <param name="winningPawn">The pawn that won the game</param>
    public void EndGame(Pawn winningPawn)
    {
        if (gameEnded) return; // Prevent multiple calls
        
        gameEnded = true;
        SedangGerak = true; // Lock the game
        
        // Determine which player won (1-based for display)
        int winnerNumber = 0;
        Sprite winnerSprite = null;
        
        if (winningPawn == player01)
        {
            winnerNumber = 1;
            var sr = player01.GetComponent<SpriteRenderer>();
            if (sr != null) winnerSprite = sr.sprite;
        }
        else if (winningPawn == player02)
        {
            winnerNumber = 2;
            var sr = player02.GetComponent<SpriteRenderer>();
            if (sr != null) winnerSprite = sr.sprite;
        }
        
        Debug.Log($"Game Over! Player {winnerNumber} wins!");
        
        // Show the end game UI
        if (gameEndUI != null)
        {
            gameEndUI.ShowWinner(winnerNumber, winnerSprite);
        }
        else
        {
            Debug.LogWarning("GameEndUI is not assigned in gridBoard!");
        }
    }
}