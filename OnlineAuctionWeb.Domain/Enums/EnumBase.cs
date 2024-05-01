namespace OnlineAuctionWeb.Domain.Enums
{
    public enum RoleEnum
    {
        Buyer = 1,
        Seller = 2,
        Admin = 3
    }

    public enum StatusEnum
    {
        Active = 1,
        Inactive = 2
    }

    public enum ConditionEnum
    {
        New = 1,
        OpenBox = 2,
        Used = 3
    }

    public enum ProductStatusEnum
    {
        InProgess = 1,
        Ended = 2,
        Canceled = 3,
        PendingPublish = 4
    }

    public enum WatchListTypeEnum
    {
        RecentlyViewed = 1,
        WatchList = 2,
        Bidding = 3,
    }


    public enum BidStatusEnum
    {
        InProgress = 1,
        Winning = 2,
        Lost = 3
    }

    public enum NotificationType
    {
        UpdatePrice = 1,
        Feedback = 2,
        AuctionEnd = 3
    }

    public enum VnpayResponseCode
    {
        // VNPAY phản hồi qua IPN và Return URL
        Success = 00,
        SuspiciousTransaction = 7,
        UnsuccessfulTransactionDueToCustomerNotRegistered = 9,
        UnsuccessfulTransactionDueToInvalidAuthentication = 10,
        UnsuccessfulTransactionDueToExpired = 11,
        LockedAccount = 12,
        IncorrectPassword = 13,
        TransactionCancelledByCustomer = 24,
        InsufficientBalance = 51,
        ExceededTransactionLimit = 65,
        PaymentBankUnderMaintenance = 75,
        IncorrectPaymentPassword = 79,
        OtherErrors = 99,

        // Tra cứu giao dịch (vnp_Command=querydr)
        InvalidMerchant = 2,
        InvalidDataFormat = 3,
        TransactionNotFound = 91,
        DuplicateRequest = 94,
        InvalidSignature = 97,

        // Gửi yêu cầu hoàn trả (vnp_Command=refund)
        RefundAmountExceedsOriginalAmount = 2,
        RefundDataFormatIncorrect = 3,
        PartialRefundNotAllowedAfterFullRefund = 4,
        PartialRefundOnly = 13,
        RefundTransactionNotFound = 91,
        InvalidRefundAmount = 93,
        RefundRejectedByVNPAY = 95,
        TimeoutException = 98,
        OtherRefundErrors = 99
    }

}
