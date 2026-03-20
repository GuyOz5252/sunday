using Sunday.Application.Abstracts;

namespace Sunday.Application.Campaigns.Create;

public record CreateCampaignCommand(string Name, string BrandId) : ICommand<string>;
