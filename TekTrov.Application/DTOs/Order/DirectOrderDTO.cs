

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TekTrov.Application.DTOs.Order
{
    public class DirectOrderDTO
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }

        [DefaultValue("Your Fullname")]
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[A-Za-z ]{3,}$",
            ErrorMessage = "Full name must contain only letters and spaces")]
        public string FullName { get; set; } = string.Empty;


        [DefaultValue("Your PhoneNumber")]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;


        [DefaultValue("Your Address")]
        [Required(ErrorMessage = "Address is required")]
        [MinLength(5, ErrorMessage = "Address is too short")]
        public string AddressLine { get; set; } = string.Empty;

        [DefaultValue("Your City")]
        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "City must contain only letters")]
        public string City { get; set; } = string.Empty;

        [DefaultValue("Your State")]
        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "State must contain only letters")]
        public string State { get; set; } = string.Empty;


        [DefaultValue("Your Postalcode")]
        [Required(ErrorMessage = "Postal code is required")]
        [RegularExpression(@"^[0-9]{5,6}$",
            ErrorMessage = "Postal code must be 5 or 6 digits")]
        public string PostalCode { get; set; } = string.Empty;


        [DefaultValue("Your Country")]
        [Required(ErrorMessage = "Country is required")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Country must contain only letters")]
        public string Country { get; set; } = string.Empty;
    }
}
