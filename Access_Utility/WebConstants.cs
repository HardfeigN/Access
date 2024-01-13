namespace Access_Utility
{
    public static class WebConstants
    {
        public const string ProductImagePath = @"\images\product\";
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";
        public const string Success = "Success";
        public const string Error = "Error";
        public const string Color = "Color";
        public const string Size = "Size";
        public const string InquiryProcessed = "Processed";


        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusProcessing = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";
    }

    public enum CatalogSection
    {
        Index,
        NewArrivals,
        Search
    }
}
