using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace CardMerchantSystem.API.Auth.Services;

/// <summary>
/// Keycloak JWT token'ındaki realm_access.roles claim'lerini
/// standart ClaimTypes.Role claim'lerine dönüştürür.
/// Bu sayede mevcut [Authorize(Roles = "Admin")] ve Policy'ler
/// hiçbir değişiklik olmadan çalışmaya devam eder.
/// </summary>
public class KeycloakClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        // Zaten role claim varsa tekrar ekleme (çift transform koruması)
        if (identity.FindFirst(ClaimTypes.Role) != null)
            return Task.FromResult(principal);

        // Keycloak realm_access.roles → ClaimTypes.Role
        MapRealmRoles(identity);

        // Keycloak preferred_username → ClaimTypes.Name
        MapUsername(identity);

        // Keycloak sub → ClaimTypes.NameIdentifier
        MapNameIdentifier(identity);

        // Keycloak email → ClaimTypes.Email
        MapEmail(identity);

        return Task.FromResult(principal);
    }

    private static void MapRealmRoles(ClaimsIdentity identity)
    {
        // Yöntem 1: realm_access JSON claim'inden parse et
        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            try
            {
                using var doc = JsonDocument.Parse(realmAccessClaim.Value);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleName = role.GetString();
                        if (!string.IsNullOrEmpty(roleName) && !IsKeycloakDefaultRole(roleName))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Geçersiz JSON, sessizce atla
            }
            return;
        }

        // Yöntem 2: Flat "roles" claim (realm-export'taki custom mapper)
        var roleClaims = identity.FindAll("roles").ToList();
        foreach (var claim in roleClaims)
        {
            if (!IsKeycloakDefaultRole(claim.Value))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));
            }
        }
    }

    private static void MapUsername(ClaimsIdentity identity)
    {
        if (identity.FindFirst(ClaimTypes.Name) != null)
            return;

        var preferred = identity.FindFirst("preferred_username");
        if (preferred != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, preferred.Value));
        }
    }

    private static void MapNameIdentifier(ClaimsIdentity identity)
    {
        if (identity.FindFirst(ClaimTypes.NameIdentifier) != null)
            return;

        var sub = identity.FindFirst("sub");
        if (sub != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub.Value));
        }
    }

    private static void MapEmail(ClaimsIdentity identity)
    {
        if (identity.FindFirst(ClaimTypes.Email) != null)
            return;

        var email = identity.FindFirst("email");
        if (email != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, email.Value));
        }
    }

    /// <summary>
    /// Keycloak'ın default rollerini filtrele (bizim uygulama rolleri değil)
    /// </summary>
    private static bool IsKeycloakDefaultRole(string role)
    {
        return role is "offline_access" or "uma_authorization" or "default-roles-cardmerchant";
    }
}