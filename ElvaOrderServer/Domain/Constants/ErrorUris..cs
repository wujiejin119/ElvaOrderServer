namespace ElvaOrderServer.Domain.Constants
{
    public static class ErrorUris
    {
        public const string BaseUri = "https://errors.elva.com/";
        
        public const string ValidationError = BaseUri + "validation-error";

        public const string DomainError = BaseUri + "domain-error";
        
        public const string InvalidQuantity = BaseUri + "invalid-quantity";
        public const string DuplicateProduct = BaseUri + "duplicate-product";
        public const string OrderNotFound = BaseUri + "order-not-found";

        public const string Unauthorized = BaseUri + "unauthorized";
        public const string Forbidden = BaseUri + "forbidden";
        
        public const string InternalServerError = BaseUri + "internal-server-error";
        public const string DatabaseError = BaseUri + "database-error";
    }
}