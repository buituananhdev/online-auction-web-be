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
        Available = 1,
        Sold = 2
    }

    public enum PaymentStatusEnum
    {
        Pending = 1,
        Paid = 2
    }
}
