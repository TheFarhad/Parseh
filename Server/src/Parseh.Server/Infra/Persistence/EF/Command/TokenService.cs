namespace Parseh.Server.Infra.Persistence.EF.Command;

using System.Text;
using System.Threading;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Framework;
using Core.Domain.Aggregates.User.Entity;
using Core.Contract.Infra.Persistence.Command;

internal static class TokenParameter
{
    internal const string RefreshTokenCookieKey = nameof(RefreshTokenCookieKey);
    internal const string Roles = nameof(Roles);
    internal const string Permissions = nameof(Permissions);
    internal const string UserName = nameof(UserName);
}

public record JwtOption
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SecretKey { get; init; } = String.Empty;
    public int TokenExpirationInMin { get; init; } = 20;
    public int RefreshTokenExpirationInDay { get; init; } = 7;

    public JwtOption() { }
}

public sealed class TokenService : ITokenService
{
    readonly ParsehCommandDbStore _context;
    readonly HttpContext _httpContext;
    readonly JwtOption _jwtOptions;

    public TokenService(ParsehCommandDbStore context, IOptionsMonitor<JwtOption> jwtOptions, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _jwtOptions = jwtOptions.CurrentValue;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<Response<TokenResponse>> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
    {
        Response<TokenResponse> result = default!;
        var (tokenExpire, refreshTokenExpire) = ExpirationDateTime();

        var accessToken = GenerateAccessToken(user, tokenExpire);

        var refreshToken = GenerateRefreshToken();
        user.AddRerereshToken(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // --- [ set refresh-token on cookie (httponly) ] --- \\
        SetRefreshTokenOnCookie(refreshToken.HashedToken, refreshTokenExpire);

        result = new TokenResponse(user.Code.Value.ToString(), accessToken, tokenExpire);
        return result;
    }

    public async Task<Response<TokenResponse>> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        Response<TokenResponse> result = default!;

        //TODO: اگر رفرش توکن منقضی نشده و یا باطل نشده
        // درخواست ارجاع داده شود
        // چون نیازی به صدور توکن جدید نیست

        var cookieRefreshToken = _httpContext.Request.Cookies[TokenParameter.RefreshTokenCookieKey] ?? "";
        // -- [ توکنی در کوکی وجود ندارد ] -- \\
        if (cookieRefreshToken.IsEmpty())
        {
            result = Error.Unknown("پیغام مناسب نوشته شود");
            return result;
        }


        var existRefreshToken = user.RefreshTokens
                                         .SingleOrDefault(_ => _.HashedToken.Equals(cookieRefreshToken));

        if (existRefreshToken is { })
        {
            // -- [ توکن قبلا یکبار ریووک شده ] -- \\
            if (existRefreshToken.IsRevoked)
            {
                // TODO: do somthings...
                // احتمال خطر
            }
            // -- [ توکن منقضی شده و کاربر باید مجددا لاگین کند ] -- \\
            else if (existRefreshToken.IsExpire)
            {
                result = Error.Unauthorized("پیغام مناسب نوشته شود");
                return result;
            }
        }
        // -- [ توکنی با این مقدار در دیتابیس وجود ندارد ] -- \\
        // -- [  کاربر باید مجددا لاگین کند ] -- \\
        else
        {
            result = Error.Unauthorized("پیغام مناسب نوشته شود");
            return result;
        }

        // -- [ توکن وجود دارد ولی منقضی یا باطل شده است ] -- \\
        var (tokenExpire, refreshTokenExpire) = ExpirationDateTime();

        var accessToken = GenerateAccessToken(user, tokenExpire);
        var refreshToken = GenerateRefreshToken();

        // -- [ revoke old refresh-token ] -- \\
        existRefreshToken.Revoked();
        existRefreshToken.TokenReplacedBy(refreshToken.HashedToken);

        // -- [ add new refresh-token to current user ] -- \\
        user.AddRerereshToken(refreshToken);

        SetRefreshTokenOnCookie(refreshToken.HashedToken, refreshTokenExpire);

        await _context.SaveChangesAsync();

        result = new TokenResponse(user.Code.Value.ToString(), accessToken, tokenExpire);
        return result;
    }

    public async Task Logout(User user, string refreshToken)
    {
        // -- [ Call in logout method in user-repository ] -- \\
        var rt = user
                    .RefreshTokens
                    .FirstOrDefault(x => x.HashedToken.Equals(refreshToken));

        if (rt is { })
        {
            rt.Revoked("", "Logout");
            await Task.Delay(1);
            //await _db.SaveChangesAsync();

            _httpContext.Response.Cookies.Delete(TokenParameter.RefreshTokenCookieKey);
        }
    }

    string GenerateAccessToken(User user, DateTime expiration)
    {
        SigningCredentials credential;

        // private key only for singing token
        var rsa = RsaKeyProvider.LoadPrivateKey("Security/Keys/Private/private.key");
        if (rsa is not null)
        {
            var rsaSecurityKey = new RsaSecurityKey(rsa);
            credential = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
        }
        else
        {
            var secret = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var key = new SymmetricSecurityKey(secret);
            credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = expiration,
            SigningCredentials = credential
        };

        List<Claim> claims = [];

        // TODO: رول ها و پرمیژن ها واکشی شوند.
        // اینجا بهتر است اینکار انجام شود
        // یا در هندلر مربوطه، که یکجا رول ها و پرمیژن های یوزر رو را فقط با یک کوئری واکشی کنیم؟؟

        // -- [ set role cliams ] -- \\
        // -- ex: { "Roles" : ["Admin", "GraphicDesigner", "Acountant"] -- \\
        // -- [ مقادیر رول ها به صورت آرایه ای از رشته ها ذخیره می شود ] -- \\
        var roles = user.Roles.Select(_ => _.Role);
        foreach (var role in roles)
            claims.Add(new(TokenParameter.Roles, role.Title));

        // set permission claims
        // -- ex: { "Permissions" : ["Read", "Write", "Update", "Remove"] -- \\
        // -- [ مقادیر پرمیژن ها به صورت آرایه ای از رشته ها ذخیره می شود ] -- \\
        foreach (var role in roles)
        {
            role
                .Permissions
                .Select(_ => _.Permission)
                .Select(_ => new Claim(TokenParameter.Permissions, _.Title))
                .ToList()
                .ForEach(claims.Add);
        }

        tokenDescriptor.Subject = new ClaimsIdentity
        (
            [
              new (TokenParameter.UserName, user.UserName),
              new (JwtRegisteredClaimNames.Sub, user.Code.Value.ToString()),
              //new (JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
              //new (JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
              new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              ..claims
            ]
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    RefreshToken GenerateRefreshToken()
    {
        var userAgent = _httpContext.Request.Headers.UserAgent.ToString();
        var ua = userAgent.IsEmpty() ? "Unknown" : userAgent;

        // -- [ آیپی آدرس همیشه وجود دارد حتی اگر از پروکسی استفاده شده باشد و آیپی جعلی باشد ] -- \\
        var ipAddress = _httpContext.Connection.RemoteIpAddress?.ToString();
        var ip = ipAddress.IsEmpty() ? "Unknown" : ipAddress;
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var result = RefreshToken.New(token, ip, ua, _jwtOptions.RefreshTokenExpirationInDay);
        return result;
    }

    void SetRefreshTokenOnCookie(string refreshToken, DateTime refreshToenExpiration)
    {
        _httpContext.Response.Cookies.Append(TokenParameter.RefreshTokenCookieKey, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToenExpiration,
            //Domain = default = "localhost" => برای استففاده از لوکال هاست، اصلا نیازی نیست نوشته شود
            /* اگر باربر با / باشد این کوکی برای همه مسیرهای وب سایت معتبر است و در صورت کال شدن هر ای پی آی، برای آن ارسال میشود */
            /* برای پرفورمنس بهتر، بهتر است که فقط برای ای پی آی مربوط به رفرش توکن ست شود */
            /* اینطوری این کوکی فقط برای ای پی آی رفرش ارسال می شود */
            Path = "/users/refreshtoken"
        });
    }

    (DateTime TokenExpiration, DateTime RefreshTokenExpiration) ExpirationDateTime()
    {
        var now = DateTime.UtcNow;
        var tokenExpire = now.AddMinutes(_jwtOptions.TokenExpirationInMin);
        var refreshTokenExpire = now.AddDays(_jwtOptions.RefreshTokenExpirationInDay);
        return (tokenExpire, refreshTokenExpire);
    }

    //(RefreshToken? token, bool isReplay) RotateRefreshTokenAsync(User user)
    //{
    //    var recievedRefreshToken = _httpContext.Request.Cookies[TokenParameter.RefreshTokenCookieKey] ?? "";
    //    // -- [ توکنی در کوکی وجود ندارد ] -- \\
    //    if (recievedRefreshToken.IsEmpty())
    //        return (null, false);

    //    var rt = user
    //                .RefreshTokens
    //                .SingleOrDefault(_ => _.HashedToken.Equals(recievedRefreshToken));

    //    // -- [ توکن معتبر نباشد ] -- \\
    //    if (rt is null || rt.IsRevoked || DateTime.UtcNow > rt.ExpireAt)
    //    {
    //        // -- [ توکن موجود باشد ولی ریووک شده باشد ] -- \\
    //        if (rt is { IsRevoked: true })
    //            return (null, true);

    //        else if (
    //                rt is null ||
    //                rt.RemoteIp != _httpContext.Connection.RemoteIpAddress?.ToString() ||
    //                rt.UserAgent != _httpContext.Request.Headers.UserAgent
    //                )
    //            return (null, false);

    //        else
    //            return (null, false);
    //    }

    //    // -- [ توکن موجود و معتبر است ] -- \\
    //    else
    //        return (GenerateRefreshToken(), false);
    //}

    //(string? token, bool isReplay) RotateRefreshTokenAsync0(User user)
    //{
    //    var recievedRefreshToken = _httpContext.Request.Cookies[TokenParameter.RefreshTokenCookieKey] ?? "";

    //    // -- [ توکنی در کوکی وجود ندارد ] -- \\
    //    if (recievedRefreshToken.IsEmpty())
    //        return (null, false);

    //    var rt = user
    //                .RefreshTokens
    //                .SingleOrDefault(_ => _.HashedToken.Equals(recievedRefreshToken));

    //    // -- [ توکن معتبر نباشد ] -- \\
    //    if (rt is null || rt.IsRevoked || DateTime.UtcNow > rt.ExpireAt)
    //    {
    //        // -- [ توکن موجود باشد ولی ریووک شده باشد ] -- \\
    //        if (rt is { IsRevoked: true })
    //            return (null, true);

    //        else if (
    //                rt is null ||
    //                rt.RemoteIp != _httpContext.Connection.RemoteIpAddress?.ToString() ||
    //                rt.UserAgent != _httpContext.Request.Headers.UserAgent
    //                )
    //            return (null, false);

    //        else
    //            return (null, false);
    //    }

    //    // -- [ توکن موجود و معتبر است ] -- \\
    //    else
    //        return (GenerateRefreshToken(), false);
    //}
}

internal static class RsaKeyProvider
{
    internal static RSA LoadPrivateKey(string path)
    {
        var result = RSA.Create();
        var keyText = File.ReadAllText(path);

        var fileInfo = new FileInfo(path);
        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == 0)
            throw new UnauthorizedAccessException("Private key should be read-only!");

        result.ImportFromPem(keyText);
        return result;
    }

    internal static RSA LoadPublicKey(string path)
    {
        var result = RSA.Create();
        result.ImportFromPem(File.ReadAllText(path));
        return result;
    }
}

#region Token Service


//    public class TokenService
//    {
//        public async Task<bool> RevokeAccessTokenAsync(string jti, DateTime expireAt)
//        {
//            _db.TokenBlacklists.Add(new TokenBlacklist { Jti = jti, ExpireAt = expireAt });
//            await _db.SaveChangesAsync();
//            return true;
//        }
//    }

#endregion


//        [Authorize]
//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            var jti = User.FindFirst("jti")?.Value;
//            var exp = DateTimeOffset.FromUnixTimeSeconds(
//                long.Parse(User.FindFirst("exp")!.Value)).UtcDateTime;

//            if (jti != null)
//                await _tokenSvc.RevokeAccessTokenAsync(jti, exp);

//            Response.Cookies.Delete("refresh_token");
//            return NoContent();
//        }



// public async Task<IActionResult> Refresh()
// {
//     var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
//     var ua = Request.Headers["User-Agent"].ToString();
//     var (newToken, isReplay) = await _tokenSvc.RotateRefreshTokenAsync(old, ip, ua);

//     if (isReplay) return Unauthorized("Token reuse detected");
// }


#region Token and Refresh-Token Tips

// Token:
// در آبجکت ارسالی مربوط به توکن، خود توکن، زمان انقضایش و همچنین مشخصه ای از کاربر را قرار میدهیم
// زمان انقضا برای این است که بتوانیم در سمت کلاینت، انقضا را چک کنیم و از ارسال درخواست اضافی به سرور جلوگیری کنیم
// بهترین و امن ترین روش ارسال رفرش توکن به کمک کوکی است
// بنابراین ما رفرش توکن را به همراه توکن ارسال نمیکنیم و آن را در کوکی قرار میدهیم
// این کار باعث جلوگیری از دسترسی جاوااسکریپ و دستکاری توسط آن می شود
// محافظت در برابر حملات xss
// در هر درخواست به صورت اتوماتیک ارسال می شود


// Refresh-Token:
// اگر رفرش توکن ارسالی در دیتابیس موجود نبود 
// یا کاربری با این مشخصه وجود نداشت 
// یا اینکه رفرش توکن منقضی شده و یا اینکه باطل شده بود
// یا اینکه اصلا رفرش  توکنی در کوکی نبود
// کاربر اتورایز نیست و باید محددا لاگین کند

// اگر مشکلی نبود، باید مجددا یک توکن و یک رفرش توکن جدید ساخته شود
// رفرش توکن قبلی باطل شود
// رفرش توکن جدید در دیتابیس ثبت شود
// آبجکت توکن برای کاربر ارسال شود
// رفرش توکن جدید در کوکی برای کاربر ارسال شود

// برای جلوگیری از افزایش رکوردهای جدول رفرش توکن، با استفاده از یک بکگراند سرویس، هر چند وقت یکبار، رفرش توکن های باطل و یا منقضی شده را حذف کن
// مثلا هر هفت روز یکبار


// بعد از لاگ اوت کردن باید
// revoke exist refresh-token (very important)
//  Response.Cookies.Delete("refresh_token key"); 
#endregion

//public static string CreateFingerprint(HttpContext http)
//{
//    var ip = http.Connection.RemoteIpAddress?.ToString() ?? "";
//    var ua = http.Request.Headers["User-Agent"].ToString();

//    var composite = $"{ip}|{ua}";
//    using var sha = SHA256.Create();
//    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(composite));
//    return Convert.ToHexString(hash); // NET 6+: یا BitConverter.ToString(...).Replace("-", "")
//}

//var fp = FingerprintHelper.CreateFingerprint(HttpContext);
//    var rt = new RefreshToken
//    {
//        Token = generatedToken,
//        DeviceFingerprint = fp,
//        // ...
//    };


//var incomingFp = FingerprintHelper.CreateFingerprint(HttpContext);
//    var storedRt = await _db.RefreshTokens
//        .FirstOrDefaultAsync(r => r.Token == refreshToken);

//if (storedRt == null || storedRt.IsRevoked)
//    return Unauthorized();

//if (storedRt.DeviceFingerprint != incomingFp)
//{
//    _logger.LogWarning("مشخصات دستگاه با Token ناسازگار است. IP/UA مشکوک.");
//    return Unauthorized("دستگاه یا مرورگر متفاوت از صادرکنندهٔ توکن است.");
//}

#region Token Service


//    public class TokenService
//    {
//        public async Task<bool> RevokeAccessTokenAsync(string jti, DateTime expireAt)
//        {
//            _db.TokenBlacklists.Add(new TokenBlacklist { Jti = jti, ExpireAt = expireAt });
//            await _db.SaveChangesAsync();
//            return true;
//        }
//    }

#endregion

//        [Authorize]
//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            var jti = User.FindFirst("jti")?.Value;
//            var exp = DateTimeOffset.FromUnixTimeSeconds(
//                long.Parse(User.FindFirst("exp")!.Value)).UtcDateTime;

//            if (jti != null)
//                await _tokenSvc.RevokeAccessTokenAsync(jti, exp);

//            Response.Cookies.Delete("refresh_token");
//            return NoContent();
//        }



// public async Task<IActionResult> Refresh()
// {
//     var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
//     var ua = Request.Headers["User-Agent"].ToString();
//     var (newToken, isReplay) = await _tokenSvc.RotateRefreshTokenAsync(old, ip, ua);

//     if (isReplay) return Unauthorized("Token reuse detected");
// }

#region Token and Refresh-Token Tips

// Token:
// در آبجکت ارسالی مربوط به توکن، خود توکن، زمان انقضایش و همچنین مشخصه ای از کاربر را قرار میدهیم
// زمان انقضا برای این است که بتوانیم در سمت کلاینت، انقضا را چک کنیم و از ارسال درخواست اضافی به سرور جلوگیری کنیم
// بهترین و امن ترین روش ارسال رفرش توکن به کمک کوکی است
// بنابراین ما رفرش توکن را به همراه توکن ارسال نمیکنیم و آن را در کوکی قرار میدهیم
// این کار باعث جلوگیری از دسترسی جاوااسکریپ و دستکاری توسط آن می شود
// محافظت در برابر حملات xss
// در هر درخواست به صورت اتوماتیک ارسال می شود


// Refresh-Token:
// اگر رفرش توکن ارسالی در دیتابیس موجود نبود 
// یا کاربری با این مشخصه وجود نداشت 
// یا اینکه رفرش توکن منقضی شده و یا اینکه باطل شده بود
// یا اینکه اصلا رفرش  توکنی در کوکی نبود
// کاربر اتورایز نیست و باید محددا لاگین کند

// اگر مشکلی نبود، باید مجددا یک توکن و یک رفرش توکن جدید ساخته شود
// رفرش توکن قبلی باطل شود
// رفرش توکن جدید در دیتابیس ثبت شود
// آبجکت توکن برای کاربر ارسال شود
// رفرش توکن جدید در کوکی برای کاربر ارسال شود

// برای جلوگیری از افزایش رکوردهای جدول رفرش توکن، با استفاده از یک بکگراند سرویس، هر چند وقت یکبار، رفرش توکن های باطل و یا منقضی شده را حذف کن
// مثلا هر هفت روز یکبار


// بعد از لاگ اوت کردن باید
// revoke exist refresh-token (very important)
//  Response.Cookies.Delete("refresh_token key"); 
#endregion

//public static string CreateFingerprint(HttpContext http)
//{
//    var ip = http.Connection.RemoteIpAddress?.ToString() ?? "";
//    var ua = http.Request.Headers["User-Agent"].ToString();

//    var composite = $"{ip}|{ua}";
//    using var sha = SHA256.Create();
//    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(composite));
//    return Convert.ToHexString(hash); // NET 6+: یا BitConverter.ToString(...).Replace("-", "")
//}

//var fp = FingerprintHelper.CreateFingerprint(HttpContext);
//    var rt = new RefreshToken
//    {
//        Token = generatedToken,
//        DeviceFingerprint = fp,
//        // ...
//    };


//var incomingFp = FingerprintHelper.CreateFingerprint(HttpContext);
//    var storedRt = await _db.RefreshTokens
//        .FirstOrDefaultAsync(r => r.Token == refreshToken);

//if (storedRt == null || storedRt.IsRevoked)
//    return Unauthorized();

//if (storedRt.DeviceFingerprint != incomingFp)
//{
//    _logger.LogWarning("مشخصات دستگاه با Token ناسازگار است. IP/UA مشکوک.");
//    return Unauthorized("دستگاه یا مرورگر متفاوت از صادرکنندهٔ توکن است.");
//}
