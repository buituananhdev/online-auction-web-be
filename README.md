script: 
dotnet ef migrations add SomeThing --project OnlineAuctionWeb.Domain --startup-project OnlineAuctionWeb.Api

dotnet ef database update --project OnlineAuctionWeb.Domain --startup-project OnlineAuctionWeb.Api

dotnet ef migrations remove --project OnlineAuctionWeb.Domain --startup-project OnlineAuctionWeb.Api
