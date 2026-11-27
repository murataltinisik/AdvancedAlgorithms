# ChangeMaker – Kasa Para Üstü Hesaplayıcı

Bir market kasasını simüle eden C# konsol uygulaması. Kasiyer alışveriş tutarını ve müşterinin ödediği tutarı girer. Uygulama, kasada o anda bulunan banknot ve bozuk paraları kullanarak para üstünü **açgözlü (greedy) algoritma** ile hesaplar ve kasa stoğunu takip eder.

> Bu proje bir algoritma ödevi olarak geliştirilmiştir: *"Market Kasası Para Üstü"* (Ödev 1).

## Özellikler

- **Para üstü hesaplama (greedy):** Para üstünü en büyük küpürden başlayarak verir. Her küpürden kasada bulunan adet kadar kullanılabilir.
- **Stok takibi:** Para üstü olarak verilen küpürler kasadan düşülür, müşterinin ödediği para kasaya eklenir.
- **Kasa durumu:** Her küpürü adedi ve ara toplamıyla birlikte listeler, kasadaki toplam parayı gösterir.
- **Kuruş tabanlı tutarlar:** Kayan nokta hatalarını azaltmak için küpürler tamsayı kuruş olarak tutulur (ör. `20000` = 200 TL).

## Küpürler

| Küpür | Değer (kuruş) | Tür | Başlangıç stoğu |
|---|---|---|---|
| 200 TL | 20000 | Banknot | 5 |
| 100 TL | 10000 | Banknot | 5 |
| 50 TL | 5000 | Banknot | 5 |
| 20 TL | 2000 | Banknot | 5 |
| 10 TL | 1000 | Banknot | 5 |
| 5 TL | 500 | Banknot | 5 |
| 1 TL | 100 | Madeni | 10 |
| 50 kr | 50 | Madeni | 10 |
| 25 kr | 25 | Madeni | 10 |
| 10 kr | 10 | Madeni | 10 |
| 5 kr | 5 | Madeni | 10 |

## Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download) veya üzeri

## Kurulum ve Çalıştırma

```bash
git clone https://github.com/<kullanici-adin>/cash-register-change-calculator.git
cd cash-register-change-calculator
dotnet run
```

## Kullanım

Uygulama açıldığında ana menü gösterilir:

```text
============ Cash Register ============
1) Calculating Change
2) Show Case Status
0) Logout
=======================================
Please, Select:
```

| Seçenek | Açıklama |
|---|---|
| `1` | Alışveriş tutarı ve ödenen tutar girilir. Uygulama para üstünü hesaplar ve kasa stoğunu günceller. |
| `2` | Kasadaki güncel stoğu ve toplam parayı gösterir. |
| `0` | Kasadan çıkış yapar. |

### Örnek: para üstü hesaplama

```text
Please, Select: 1
Enter, Purchase Amount: 140
Enter, Amount Paid: 200
20000 krs -> added new +1 (New Stock: 6)
```

Bu örnekte para üstü 60 TL'dir. Para üstü kasadan verilir (50 TL × 1, 10 TL × 1) ve müşterinin verdiği 200 TL'lik banknot kasaya eklenir.

### Girdi kontrolleri

- Menü seçimi boş bırakılırsa `Cannot option empty or null!` mesajı gösterilir.
- Geçersiz bir menü seçimi yapılırsa `Please, enter valid option` mesajı gösterilir.
- Ödenen tutar alışveriş tutarından azsa `The amount paid must be greater than purchase amount!` mesajı gösterilir.

## Proje Yapısı

```text
CashRegisterChangeCalculater/
├── Models/
│   └── Banknote.cs      # Küpür (Type, kuruş cinsinden) ve kasadaki adedi (Quantity)
└── Program.cs           # Menü, para üstü hesaplama ve kasa durumu
```

## Algoritma

Para üstü **açgözlü (greedy)** yaklaşımla hesaplanır:

1. Para üstü kuruşa çevrilir: `(amountPaid - purchaseAmount) * 100`.
2. Kasada bulunan küpürler büyükten küçüğe sıralanır.
3. Her küpürden, kalan para üstünü ve kasadaki adedi aşmayacak şekilde mümkün olduğunca çok alınır.

Zaman karmaşıklığı `k` küpür için **O(k)**'dır (verilen parça sayısı hariç).

## Yapılacaklar

- [ ] Para üstü dökümünü ekrana yazdırmak (hangi küpürden kaç adet verildiği)
- [ ] Tam para üstü verilemediğinde hata göstermek ve stoğu değiştirmemek
- [ ] `0` seçeneğinin programı kapatmasını sağlamak
- [ ] Tutar girişlerini doğrulamak (sayı olmayan, negatif, 2'den fazla ondalık basamaklı girdiler)
- [ ] Tutarları `double` yerine `long` kuruş olarak tutmak
- [ ] Sınırlı stokla en az parçayı bulan **dinamik programlama** çözümünü eklemek
- [ ] Greedy ve DP sonuçlarını karşılaştırmak
- [ ] Birim testleri yazmak (`CashRegister.Tests`)

## Lisans

Bu proje MIT Lisansı ile lisanslanmıştır.
