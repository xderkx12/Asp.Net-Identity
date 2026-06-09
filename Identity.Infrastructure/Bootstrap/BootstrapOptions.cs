namespace Identity.Infrastructure.Bootstrap;

public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    public string AdminRoleName { get; set; } = "admin";
    public string AdminLogin { get; set; } = "admin";
    public string AdminPassword { get; set; } = string.Empty;
}
