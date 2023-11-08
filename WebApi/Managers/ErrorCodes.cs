namespace RocketStoreApi.Managers
{
    /// <summary>
    /// Defines constants that describe error codes.
    /// </summary>
    public static partial class ErrorCodes
    {
        #region Internal Constants

        /// <summary>
        /// The customer already exists.
        /// </summary>
        public const string CustomerAlreadyExists = "CustomerAlreadyExists";
        public const string InvalidID = "InvalidID";
        public const string CustomerDontExists = "CustomerDontExists";
        public const string CustomersNotFound = "CustomersNotFound";

        #endregion
    }
}
