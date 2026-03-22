using Sunday.Application.Abstract;

namespace Sunday.Application.Campaigns.Create;

public record CreateCampaignCommand(string Name, string BrandId) : ICommand<string>;
