using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.ViewModels
{

    public class RegisterModel
    {
        [Display(Name = "Adı Soyadı")]
        [Required(ErrorMessage = "Adı Soyadı Giriniz!")]
        public string FullName { get; set; }




        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }





        [Display(Name = "E-Posta Adresi")]
        [Required(ErrorMessage = "E-Posta Adresi Giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta Adresi Giriniz!")]
        public string Email { get; set; }



        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string Password { get; set; }



        [Display(Name = "Parola Tekrar")]
        [Required(ErrorMessage = "Parola Tekrar Giriniz!")]
        [Compare("Password", ErrorMessage = "Parola Tekrarı Tutarsızdır!")]
        public string PasswordConfirm { get; set; }


        [Display(Name = "Şehir")]
        [Required(ErrorMessage = "Şehir Giriniz!")]
        public string City { get; set; }
    }
}
