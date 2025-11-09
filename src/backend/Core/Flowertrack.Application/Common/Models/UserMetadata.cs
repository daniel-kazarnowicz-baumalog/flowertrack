namespace Flowertrack.Application.Common.Models;

/// <summary>
/// User metadata stored in Supabase auth.users.raw_user_meta_data
/// </summary>
public class UserMetadata
{
    /// <summary>
    /// User's role (service_admin, service_technician, organization_admin, organization_operator)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Organization ID (nullable - only for organization users)
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Converts metadata to dictionary for Supabase API
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            { "role", Role },
            { "full_name", FullName }
        };

        if (OrganizationId.HasValue)
        {
            dict.Add("organization_id", OrganizationId.Value.ToString());
        }

        return dict;
    }

    /// <summary>
    /// Creates UserMetadata from Supabase user metadata dictionary
    /// </summary>
    public static UserMetadata FromDictionary(Dictionary<string, object>? metadata)
    {
        if (metadata == null)
        {
            return new UserMetadata();
        }

        var userMetadata = new UserMetadata();

        if (metadata.TryGetValue("role", out var role))
        {
            userMetadata.Role = role?.ToString() ?? string.Empty;
        }

        if (metadata.TryGetValue("full_name", out var fullName))
        {
            userMetadata.FullName = fullName?.ToString() ?? string.Empty;
        }

        if (metadata.TryGetValue("organization_id", out var orgId) && 
            !string.IsNullOrEmpty(orgId?.ToString()) &&
            Guid.TryParse(orgId.ToString(), out var parsedOrgId))
        {
            userMetadata.OrganizationId = parsedOrgId;
        }

        return userMetadata;
    }
}
