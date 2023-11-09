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
        
        /// <summary>
        /// The Id Sended was invalid.
        /// </summary>
        public const string InvalidID = "InvalidID";
        
        /// <summary>
        /// The Customer requested doesnt exists.
        /// </summary>
        public const string CustomerDoesntExists = "CustomerDontExists";
        

        #endregion
    }
}
