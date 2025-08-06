namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

public class TokenBlacklist
{
    public int Id { get; set; }
    public string Jti { get; set; } = String.Empty;
    public DateTime ExpireAt { get; set; }
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



