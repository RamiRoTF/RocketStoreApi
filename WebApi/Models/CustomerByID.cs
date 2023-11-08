using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RocketStoreApi.Models
{
    /// <summary>
    /// Defines a customer.
    /// </summary>
    public partial class CustomerByID
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayName("Id")]
        [JsonPropertyName("Id")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayName("Name")]
        [JsonPropertyName("name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [DisplayName("Email")]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the VAT Number.
        /// </summary>
        [RegularExpression("^[0-9]{9}$")]
        [DisplayName("VAT Number")]
        [JsonPropertyName("vatNumber")]
        public string VatNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [DisplayName("City")]
        [JsonPropertyName("city")]
        public string City
        {
            get;
            set;
        }

        #endregion
    }
}
