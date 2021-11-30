using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Örnek3
{
    public partial class Form1 : Form
    {

        //Kişi Rehberi Uygulaması
        //1. Adım: SSMS'e giderek bir DB oluşturmalıyız. DB Adı:KisiRahberiDB
        //2.Adım: Oluşturulan bu DB'de bir tane Contacts isimli tablo oluşturmalıyız.
        //2.1. Adım: Contacts tablosunda yer alacak sütunlar :
        //        ID(int)
        //        FirstName(nvarchar(50))
        //        LastName(nvarchar(50))
        //        PhoneNumber(nvarchar(24))
        //3.Adım: Projeye Entity Framwork  eklemeliyiz.(NuGet'ten)
        //4.Adım: DB First yaklaşımı ile oluşturduğumuz DB'nin bir modelini uygulamamıza eklemeliyiz.
        //5.Adım: Uygulamada, kişi listeleme, kişi ekleme, kişi arama, kişi güncelleme, kişi silme işlemleri için gerekli kodlamayı yapmalıyız.


        public Form1()
        {
            InitializeComponent();
        }




        KisiRehberDBEntities rehber;





        private void Form1_Load(object sender, EventArgs e)
        {
            rehber = new KisiRehberDBEntities();

            btnKisiAra.Enabled = rehber.Contacts.ToList().Count > 0 ? true : false; //rehberimdeki Contacts tablomun listesini say ve 0'dan büyükse kişiara butonumu aktif et, 0 ise aktif etme.

            btnGuncellemeYap.Enabled = false;   //Sadece ben istediğimde Update yapıcağım için bunu kapalı getir.

            KişileriDoldur(rehber.Contacts.ToList());   //Kişileri doldur metodumla rehberin contacts tablosunun tüm listesini metotla çağır dedik.

            //lstKisiListesi.DataSource = rehber.Contacts.ToList();

        }


        private void KişileriDoldur(List<Contact>liste) //KişileriDoldur metodum Listemi Contact tablosu tipinde bir liste olarak çalışsın
        {
            lstKisiListesi.Items.Clear();

            foreach (Contact item in liste)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = item.ID.ToString();
                lvi.SubItems.Add(item.FirstName);
                lvi.SubItems.Add(item.LastName);
                lvi.SubItems.Add(item.PhoneNumber);

                lvi.Tag = item;     //İtem(Contact tablomdaki her bir hücrenin değeri)'ın bütün bilgilerini Tag ile hafızada tut(gizle). Bunu güncelleme işleminde kullanacağımız için Tag'de hazır tutuyoruz. Güncellerken Tag'deki veriyi alıp hazırca kullanıcaz. Yoksa bu satırı yazmasam da Listem zaten program çalıştığında otomatik geliyor. Bu satırı sadece bilgilerimi hafızada tutsun diye yazdık.

                lstKisiListesi.Items.Add(lvi);
            }
        }






        private void btnRehbereEkle_Click(object sender, EventArgs e)
        {
            //Insert Into Contacts(FirstName, LastName, PhoneNumber) VALUES (txtKisiAdi.Text, txtKisiSoyadi.Text, txtKisiTelefonu.Text)



            if (txtKisiAdi.Text.Trim() == "" || txtKisiSoyadi.Text.Trim() == "" || txtKisiTelefonu.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                rehber.Contacts.Add(new Contact()
                {
                    FirstName = txtKisiAdi.Text,
                    LastName = txtKisiSoyadi.Text,
                    PhoneNumber = txtKisiTelefonu.Text
                });
            }
            rehber.SaveChanges();

            MessageBox.Show("Kişi başarıyla rehbere eklenmiştir.");
            KişileriDoldur(rehber.Contacts.ToList());

            Temizle(grpKisiEklemePaneli);

            btnKisiAra.Enabled = true;
        }


        private void Temizle(GroupBox grp)
        {
            foreach (Control item in grp.Controls)
            {
                if(item is TextBox)
                {
                    item.Text = "";
                }
            }
        }





        private void btnKisiAra_Click(object sender, EventArgs e)
        {
            if(txtAranacak.Text.Trim()=="")
            {
                MessageBox.Show("Aranacak metni boş bırakamazsınız!");
            }
            else
            {
                KişileriDoldur(rehber.Contacts.Where(x => x.FirstName.ToLower().StartsWith(txtAranacak.Text.ToLower()) || x.LastName.ToLower().StartsWith(txtAranacak.Text.ToLower())).ToList());   //İsim yada soyad'ın textlerindekileri küçük harf yap ve başlangıç değerlerinde arama yap.

                Temizle(grpKisiAramaPaneli);
            }
        }





        private void btnGuncellemeYap_Click(object sender, EventArgs e)
        {
            if(txtKisiAdi.Text.Trim()==""||txtKisiSoyadi.Text.Trim()==""||txtKisiTelefonu.Text.Trim()=="")
            {
                MessageBox.Show("Lütfen boş alan bırakmayınız.");
            }
            else
            {
                guncellenecek.FirstName = txtKisiAdi.Text;
                guncellenecek.LastName = txtKisiSoyadi.Text;
                guncellenecek.PhoneNumber = txtKisiTelefonu.Text;

                rehber.SaveChanges();

                KişileriDoldur(rehber.Contacts.ToList());
                Temizle(grpKisiEklemePaneli);
            }
        }


        Contact guncellenecek;
        private void tsmGuncelle_Click(object sender, EventArgs e)
        {

            if(lstKisiListesi.SelectedItems.Count<=0)
            {
                return;
            }
            guncellenecek = lstKisiListesi.SelectedItems[0].Tag as Contact;

            txtKisiAdi.Text = guncellenecek.FirstName;
            txtKisiSoyadi.Text = guncellenecek.LastName;
            txtKisiTelefonu.Text = guncellenecek.PhoneNumber;

            btnGuncellemeYap.Enabled = true;
            btnRehbereEkle.Enabled = false;
            btnKisiAra.Enabled = false;
        }



        private void tsmSil_Click(object sender, EventArgs e)
        {
            if (lstKisiListesi.SelectedItems.Count <= 0)    //istview'da seçili bişeyim yoksa silemesin
                return;
            Contact kisi = lstKisiListesi.SelectedItems[0].Tag as Contact;  //listede seçili olanların ilk sıradakini silicek(0. index'dekini).

            rehber.Contacts.Remove(kisi);
            rehber.SaveChanges();

            KişileriDoldur(rehber.Contacts.ToList());        
        }
    }
}
